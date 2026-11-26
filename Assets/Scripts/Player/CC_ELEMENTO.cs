using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using KinematicCharacterController;

// The states the character can be in (same idea as the "Multiple movement states" + "Charging state"
// sections of the walkthrough)
public enum CharacterState
{
    Default,
    Dashing,
    GroundPound,
    Dead,
    Finished,
    Held        // hovering in a Dash Hold zone (appended last, keep the order)
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CC_ELEMENTO : MonoBehaviour, ICharacterController
{
    // ------------------------------------------------------------------
    // INPUTS AND CAMERA
    // ------------------------------------------------------------------
    public InputActionReference MoveAction;
    public InputActionReference JumpAction;
    public InputActionReference DashAction;
    public InputActionReference GroundPoundAction;
    public InputActionReference RunAction;           // optional: hold to run (only used if UseRun is on)
    public InputActionReference GravityFlipAction;   // used by the Vapor power (can share the Dash button)
    // optional: a button you HOLD to glide (can share the Dash / Flip button, you only have one element at a time).
    // Empty = the glide uses the jump button instead (release it once, then hold it while falling)
    public InputActionReference GlideAction;
    public CinemachineCamera PlayerCamera;

    // true  = 2.5D side-scroller (left/right only, ignores the camera direction)
    // false = movement relative to the camera (your original 3D version)
    public bool IsSideScroller = true;

    // ------------------------------------------------------------------
    // MOVEMENT SETTINGS
    // ------------------------------------------------------------------
    // How fast you can move on the ground
    public float MaxGroundSpeed = 11f;
    // How fast you can move in the air (same as ground = full air control, Super Meat Boy style)
    public float MaxAirSpeed = 11f;

    // How fast the velocity changes on the ground (units per second, high = snappy)
    public float GroundAcceleration = 120f;
    // How fast the velocity slows down when no input is pressed on the ground
    public float GroundDeceleration = 150f;
    // How fast the velocity changes in the air
    public float AirAcceleration = 80f;

    // How quickly the character turns
    public float RotationRate = 25f;

    // How strong the character falls (high gravity = tight jumps)
    public Vector3 Gravity = new Vector3(0, -70, 0);

    // ------------------------------------------------------------------
    // RUN SETTINGS (optional, Super Meat Boy style)
    // ------------------------------------------------------------------
    // OFF = the character works exactly like before. ON = holding the Run button gives a higher top speed.
    public bool UseRun = false;
    // Top speed while running (MaxGroundSpeed above is the walking speed)
    public float RunSpeed = 16f;
    // How fast the speed builds up while running (lower = more momentum, higher = snappier)
    public float RunAcceleration = 45f;

    // ------------------------------------------------------------------
    // JUMP SETTINGS
    // ------------------------------------------------------------------
    // How much upward velocity the jump adds
    public float JumpStrength = 21f;

    // Gravity is multiplied by this while falling (falling feels heavier than rising)
    public float FallGravityMultiplier = 1.4f;
    // Gravity is multiplied by this when the jump button is released early (short hop)
    public float JumpCutMultiplier = 2.5f;
    // Limit of the falling speed
    public float MaxFallSpeed = 28f;

    // Time after leaving the ground where you can still jump
    // (= JumpPostGroundingGraceTime in the walkthrough)
    public float CoyoteTime = 0.1f;
    // Time before landing where a jump press is remembered
    // (= JumpPreGroundingGraceTime in the walkthrough)
    public float JumpBufferTime = 0.1f;

    private bool hasDoubleJump;   // set by SetPower()

    // ------------------------------------------------------------------
    // WALL JUMP SETTINGS (Super Meat Boy style)
    // ------------------------------------------------------------------
    // false until the player picks up the power-up
    private bool hasWallJump;   // set by SetPower()

    // How long you stay glued to the wall (no falling) when you first touch it
    public float WallStickTime = 0.25f;
    // How fast you slide down the wall after the stick time is over
    public float WallSlideSpeed = 4f;

    // The push away from the wall (horizontal) and up (vertical) when wall jumping
    public float WallJumpHorizontalSpeed = 12f;
    public float WallJumpVerticalSpeed = 20f;

    // Short time after a wall jump where the air control is locked,
    // so the player can't instantly steer back into the wall
    public float WallJumpControlLockTime = 0.15f;

    // OnMovementHit is only called while we push into the wall, so we remember the contact
    // for a few moments. This also lets the player press jump a tiny bit late.
    public float WallContactGraceTime = 0.1f;

    // ------------------------------------------------------------------
    // DASH SETTINGS
    // ------------------------------------------------------------------
    // false until the player picks up the power-up
    private bool hasDash;   // set by SetPower()
    public float DashSpeed = 28f;
    // distance of the dash = DashSpeed x DashDuration
    public float DashDuration = 0.15f;
    // small delay before the dash can come back
    public float DashCooldown = 0.25f;

    // ------------------------------------------------------------------
    // BLOCK MOMENTUM - breaking blocks makes the dash / the slam stronger
    // ------------------------------------------------------------------
    // Every breakable block (DestructibleObject) the dash BREAKS adds to the dash: a bit of speed and a bit of TIME.
    // A long row of blocks therefore carries you all the way through it (the dash reach is extended).
    // Only a block that really breaks counts (a hit on a tough block that is not broken yet does not).
    [Header("Block Momentum")]
    [Tooltip("Speed added to the dash for every block it breaks (units/second)")]
    public float DashBreakSpeedBonus = 3f;

    [Tooltip("Time added to the dash for every block it breaks (seconds). This is what extends the reach.")]
    public float DashBreakTimeBonus = 0.045f;

    [Tooltip("The most extra time a dash can gain in total (the cap on the reach)")]
    public float DashMaxExtraTime = 0.6f;

    [Tooltip("The dash never gets faster than this (keep it under ~40 so the player cannot tunnel through walls)")]
    public float DashMaxSpeed = 40f;

    [Tooltip("After a dash that broke blocks you keep this part of the dash speed (0 = off, 0.6 = 60%). Never below the run speed, never above Boost Max Speed.")]
    [Range(0f, 1f)] public float DashKeepSpeedFraction = 0.6f;

    [Tooltip("Speed added to the ground pound for every block it falls through (units/second)")]
    public float GroundPoundBreakSpeedBonus = 3f;

    [Tooltip("The ground pound never gets faster than this (keep it under ~60)")]
    public float GroundPoundMaxSpeed = 60f;

    // ------------------------------------------------------------------
    // BOOST (a DashZone in Boost mode, a "dash ring") - works without any power
    // ------------------------------------------------------------------
    // After the boost you keep its speed this long (seconds), as long as you keep pushing the same way.
    public float BoostMomentumTime = 1.2f;

    // SONIC STYLE: after a boost, as long as you keep pushing the way it threw you, the speed limit keeps growing.
    // How much the limit grows per second (0 = off, you just keep the exit speed).
    public float BoostAcceleration = 12f;

    // The limit never grows beyond this (the run speed is 16, a dash is 28)
    public float BoostMaxSpeed = 30f;

    // On = the momentum does not run out while you keep pushing the same way (it ends a moment after you let go or turn)
    public bool BoostKeepWhilePushing = true;

    // ------------------------------------------------------------------
    // SLOPE CLIMB (Sonic style): run up a slope with boost speed and keep going UP the wall at its top
    // ------------------------------------------------------------------
    // Master switch (off = the wall just stops you, like before)
    public bool SlopeClimb = true;

    // On = only works while the boost momentum is alive. Off (default) = only SPEED counts, like Sonic.
    public bool ClimbNeedsBoost = false;

    // On (default) = the run must come OFF A SLOPE. Running straight into a flat wall does not climb.
    public bool ClimbNeedsSlope = true;

    // A floor counts as a slope when it is tilted at least this many degrees (the 45 degree slope = 45)
    public float ClimbSlopeMinAngle = 20f;

    // How long (seconds) after leaving a slope the wall still counts as 'reached from a slope'
    public float ClimbSlopeMemory = 0.3f;

    // You need at least this speed when you hit the wall (walking 11, running 16, boost exit speed 18)
    public float ClimbMinSpeed = 17f;

    // How much of your speed turns into upward speed (0.9 = 90 %)
    public float ClimbSpeedConversion = 0.9f;

    // The upward speed never starts above this
    public float ClimbMaxSpeed = 22f;

    // How fast the upward speed fades, per second. Height climbed = speed x speed / (2 x this), so 16 -> about 4.3 units
    public float ClimbDeceleration = 30f;

    // The climb never lasts longer than this (seconds)
    public float ClimbMaxTime = 0.8f;

    // Small push into the wall so we keep touching it while we go up (units per second)
    public float ClimbWallPress = 2f;

    // The wall must touch us at least this often (seconds). When it stops, we reached the top
    public float ClimbContactGrace = 0.08f;

    // At the top of the wall: push over the edge (horizontal units per second), so we land on top
    public float ClimbLedgeHop = 6f;

    // After a climb, a new one cannot start for this long (seconds)
    public float ClimbCooldown = 0.25f;

    // ------------------------------------------------------------------
    // ONE-WAY PLATFORMS
    // ------------------------------------------------------------------
    // Press DOWN + JUMP while standing on a one-way platform to drop through it.
    // For this long (seconds) one-way platforms do not exist for us. It only has to be long enough to leave the top.
    public float DropThroughTime = 0.2f;

    // ------------------------------------------------------------------
    // GROUND POUND SETTINGS
    // ------------------------------------------------------------------
    // false until the player picks up the power-up
    private bool hasGroundPound;   // set by SetPower()
    // Short pause in the air before slamming down (like Mario)
    public float GroundPoundWindup = 0.12f;
    // How fast the character slams down
    public float GroundPoundSpeed = 40f;
    // Size of the shockwave when landing (hits destructible objects around the feet)
    public float GroundPoundRadius = 2.5f;
    // Which layers the shockwave looks at
    public LayerMask DestructibleLayers = ~0;

    // Small bounce after landing (Mario style). Apex = strength² / (2 x gravity), so 12 is about 1 unit.
    // A jump pressed right at the landing beats it and gives a full jump instead.
    public float GroundPoundBounceStrength = 12f;

    // ------------------------------------------------------------------
    // GLIDE SETTINGS (parachute, Peach style)
    // ------------------------------------------------------------------
    // false until the player picks up the power-up
    private bool hasGlide;   // set by SetPower()
    // Maximum falling speed while the parachute is open
    public float GlideFallSpeed = 3f;
    // How hard the parachute brakes the fall (gravity while falling is about 98, so 160 slows you down)
    public float GlideBrake = 160f;
    // true  = release the jump button once in the air, then press it again while falling (keeps normal jumps tight)
    // false = holding jump through the whole jump also glides
    public bool GlideNeedsNewPress = true;

    // ------------------------------------------------------------------
    // VAPOR SETTINGS (gravity flip, Metal Storm style)
    // ------------------------------------------------------------------
    // false until the player picks up the power-up
    private bool hasGravityFlip;   // set by SetPower()
    // Speed given toward the new "floor" at the moment of the flip (so it feels instant, not floaty)
    public float FlipLaunchSpeed = 10f;
    // Short delay before the next flip
    public float FlipCooldown = 0.15f;
    // A flip pressed this many seconds BEFORE it becomes possible (just before an orb, just before landing) still happens
    public float FlipBufferTime = 0.12f;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    public CharacterState CurrentState = CharacterState.Default;

    // read by BallVisuals (parachute) and the debug overlay
    public bool IsGliding { get; private set; }
    public bool IsGravityFlipped => _gravityFlipped;

    // gravity is turned around while the Vapor power has flipped us
    private Vector3 CurrentGravity => _gravityFlipped ? -Gravity : Gravity;

    // inputs
    private Vector3 _moveInput;
    private Vector3 _lookInput;

    // jump
    private float _jumpBufferTimer;
    private float _coyoteTimer;
    private bool _jumpConsumed;
    private bool _doubleJumpConsumed;
    private int _extraAirJumps;       // from an Air recharge orb: extra jumps in the air until we land (the triple jump)
    private int _extraFlips;          // from a Water recharge orb: extra gravity flips until we land
    private bool _jumpedThisFrame;

    // wall jump
    private Vector3 _wallNormal;          // points away from the wall, towards the player
    private float _wallContactTimer;
    private float _wallStickTimer;
    private float _wallJumpLockTimer;

    // dash
    private bool _dashRequested;
    private bool _dashUsed;
    private Vector3 _dashDirection;
    private Vector3 _dashVelocity;
    private float _dashTimeRemaining;
    private float _dashSpeedNow;            // the dash speed, grows with every block broken
    private float _dashExtraTime;           // the time gained from broken blocks (capped)
    private int _dashBlocksBroken;          // blocks broken by this dash
    private float _slamSpeed;               // the slam speed, grows with every block fallen through
    private float _dashCooldownTimer;

    // boost (see BoostLaunch)
    private bool _boostRequested;
    private bool _boostActive;              // the current "dash" is a boost
    private Vector3 _boostDirection;
    private float _boostSpeed;
    private float _boostTime;
    private float _boostExitSpeed;
    // DASH ZONE: Lane (forces the dash direction while inside) and Charge (extra dashes until landing)
    private DashZone _laneZone;
    private Vector3 _laneDirection;
    private int _dashCharges;
    private float _superDashMultiplier = 1f;   // > 1 = the next dash is longer (from a Super Dash zone)

    private float _boostMomentumTimer;      // while > 0 the speed limit is raised to _boostCap
    private float _boostCap;

    // slope climb (see HandleClimbHit)
    private bool _climbing;
    private Vector3 _climbWallNormal;       // points away from the wall, towards the player
    private float _climbSpeed;
    private float _climbTimeLeft;
    private float _climbContactTimer;
    private float _climbCooldownTimer;
    private Vector3 _preMoveVelocity;       // the velocity we asked for this step, before the wall stopped us

    private float _slopeContactTimer;      // > 0 = we were on a slope a moment ago

    // read by BallVisuals / debug
    public bool IsClimbing => _climbing;

    // dash hold zone (the barrel)
    private DashZone _holdZone;
    private DashZone _holdLastZone;         // the zone we were just thrown from: it cannot catch us again until we leave it
    private bool _holdRequested;
    private float _holdTimer;

    // one-way platform drop-through: > 0 while one-way platforms are ignored
    private float _dropThroughTimer;

    // ground pound
    private bool _groundPoundRequested;
    private float _groundPoundWindupRemaining;
    private bool _bounceRequested;   // set on landing, applied in UpdateVelocity
    private bool _isBouncing;        // true while going up from the bounce

    // sound
    private float _airTime;            // how long we have been in the air (a landing sound needs a real fall)

    // Jump, Land, DoubleJump and GroundPoundImpact appear this many units ABOVE the pivot (0 = at the pivot).
    // Raise it if the effect looks too low under the ball, lower it if it floats.
    [Header("Effects")]
    public float EffectFeetHeight = 0.15f;

    // The run trail only shows when the ball is at least this fraction of the run speed (0.85 = near top speed)
    [Range(0.5f, 1f)] public float RunTrailSpeedFraction = 0.85f;

    // where the feet effects appear: just above the pivot, along the ball's up direction (follows a gravity flip)
    private Vector3 FeetPosition => _kinematicMotor.TransientPosition + _kinematicMotor.TransientRotation * Vector3.up * EffectFeetHeight;

    // where the effects appear: the middle of the ball (the pivot is at the feet)
    private Vector3 BallCenter => _kinematicMotor.TransientPosition + _kinematicMotor.TransientRotation * _capsule.center;

    // bounce pad
    private float _padLaunchSpeed;     // set by LaunchFromPad, applied in UpdateVelocity (0 = nothing waiting)
    private Collider _lastSlamGround;      // the broken block the slam just went through (see PostGroundingUpdate)
    private float _recentSlamTimer;    // >0 for a moment after a ground pound landing

    // ice
    private Collider _lastGroundCollider;   // the ground we checked last (so we only look again when it changes)
    private IceSurface _currentIce;           // the ice under our feet (null = normal ground)

    // run
    private bool _isRunning;

    // glide
    private bool _glideReady;      // the jump button was released in the air, so the next press can open the parachute
    private bool _glideLatched;    // true while gliding: the speed limit is frozen (pressing/releasing run does nothing)
    private float _glideSpeedCap;  // the horizontal speed limit chosen when the parachute opened

    // gravity flip (vapor)
    private bool _flipRequested;
    private bool _gravityFlipped;
    private bool _flipUsed;       // one flip per trip, given back when we land
    private bool _justFlipped;
    private float _flipCooldownTimer;
    private float _flipBufferTimer;
    private CapsuleCollider _capsule;

    // the Motor of the character
    private KinematicCharacterMotor _kinematicMotor;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // connect the motor from the KCC asset to this game object
        _kinematicMotor = GetComponent<KinematicCharacterMotor>();
        _kinematicMotor.CharacterController = this;
        _capsule = GetComponent<CapsuleCollider>();

        // start with the power chosen in the Inspector (None = no power)
        _awakeDone = true;
        SetPower(StartingPower);
    }

    // Update runs every frame, so this is where we catch the button presses.
    // We only REMEMBER them here. The velocity is changed later, in UpdateVelocity,
    // because the walkthrough says all velocity must be handled there.
    private void Update()
    {
        // while the game is paused (PauseMenu sets Time.timeScale to 0) the buttons are ignored,
        // so a press made in the menu can't wait there and become a jump when the game resumes
        if (Time.timeScale == 0f) return;

        if (JumpAction.action.WasPressedThisFrame())
            _jumpBufferTimer = JumpBufferTime;
        else
            _jumpBufferTimer -= Time.deltaTime;

        if (DashAction != null && DashAction.action.WasPressedThisFrame())
            _dashRequested = true;

        if (GroundPoundAction != null && GroundPoundAction.action.WasPressedThisFrame())
            _groundPoundRequested = true;

        // the flip press is remembered for a moment (like the jump), so pressing a hair early still works
        if (GravityFlipAction != null && GravityFlipAction.action.WasPressedThisFrame())
            _flipBufferTimer = FlipBufferTime;
        else
            _flipBufferTimer -= Time.deltaTime;

        if (_flipBufferTimer > 0f)
            _flipRequested = true;
    }

    // ------------------------------------------------------------------
    // ICharacterController CALLBACKS
    // ------------------------------------------------------------------
    public void BeforeCharacterUpdate(float deltaTime)
    {
        // read the player input first
        UpdateInputs();

        // check if we should start a dash or a ground pound
        HandleHoldRequest();
        HandleHold(deltaTime);
        HandleDashInput();
        HandleBoostRequest();
        HandleGroundPoundInput();
        HandleFlipInput();
    }

    private void UpdateInputs()
    {
        // read the 2D move action from the new input system
        Vector2 moveActionInput = MoveAction.action.ReadValue<Vector2>();

        if (IsSideScroller)
        {
            // side-scroller: only left / right (x axis), nothing on z
            _moveInput = new Vector3(moveActionInput.x, 0, 0);
            _moveInput = Vector3.ClampMagnitude(_moveInput, 1);
        }
        else
        {
            // 3D: turn the 2D values into 3D (y is 0 because we don't move up/down with the stick)
            _moveInput = new Vector3(moveActionInput.x, 0, moveActionInput.y);
            _moveInput = Vector3.ClampMagnitude(_moveInput, 1);

            // move relative to the camera direction
            _moveInput = GetCameraPlanarRotation(PlayerCamera.State.GetFinalOrientation()) * _moveInput;
        }

        // save the move direction as the look direction
        _lookInput = _moveInput;

        // is the run button held? (always false while UseRun is off, so nothing changes)
        _isRunning = UseRun && RunAction != null && RunAction.action.IsPressed();
    }

    // logic to get the camera direction flattened on the character's plane
    private Quaternion GetCameraPlanarRotation(Quaternion cameraRotation)
    {
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, _kinematicMotor.CharacterUp).normalized;

        if (cameraPlanarDirection.sqrMagnitude == 0)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, _kinematicMotor.CharacterUp).normalized;
        }

        return Quaternion.LookRotation(cameraPlanarDirection, _kinematicMotor.CharacterUp);
    }

    // this rotates the character
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // no turning while dead or finished
        if (CurrentState == CharacterState.Dead || CurrentState == CharacterState.Finished || _lookInput == Vector3.zero) return;

        Vector3 targetLookDirection = Vector3.Slerp(_kinematicMotor.CharacterForward, _lookInput, deltaTime * RotationRate);

        currentRotation = Quaternion.LookRotation(targetLookDirection, _kinematicMotor.CharacterUp);
    }

    // "What should my velocity be right now?"
    // "ref" means we change the motor's own velocity value, not a copy of it
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // AirMovement turns this on again if the parachute is open this frame
        IsGliding = false;

        switch (CurrentState)
        {
            case CharacterState.Default:
                {
                    // remember that we are on a slope (the climb needs it)
                    if (_kinematicMotor.GroundingStatus.IsStableOnGround
                        && Vector3.Angle(_kinematicMotor.GroundingStatus.GroundNormal, _kinematicMotor.CharacterUp) >= ClimbSlopeMinAngle)
                    {
                        _slopeContactTimer = ClimbSlopeMemory;
                    }

                    // climbing a wall: its own movement, no ground, no air, no normal jump
                    if (_climbing)
                    {
                        ClimbMovement(ref currentVelocity, deltaTime);
                        break;
                    }

                    // is the character standing on the ground?
                    if (_kinematicMotor.GroundingStatus.IsStableOnGround)
                    {
                        GroundMovement(ref currentVelocity, deltaTime);
                    }
                    // or in the air?
                    else
                    {
                        AirMovement(ref currentVelocity, deltaTime);
                    }

                    // check if a jump should happen
                    HandleJumping(ref currentVelocity);

                    // the small bounce after a ground pound (a jump wins over it)
                    HandleBounce(ref currentVelocity);

                    // a bounce pad launch (beats the small bounce)
                    HandlePadLaunch(ref currentVelocity);

                    // remember what we wanted, the wall hit that follows needs the speed from BEFORE the stop
                    _preMoveVelocity = currentVelocity;
                    break;
                }

            case CharacterState.Held:
                {
                    // hovering in a Dash Hold zone: no movement of its own (HandleHold places the ball)
                    currentVelocity = Vector3.zero;
                    break;
                }

            case CharacterState.Dashing:
                {
                    // constant velocity, no gravity while dashing (same as the Charging state)
                    currentVelocity = _dashVelocity;
                    break;
                }

            case CharacterState.GroundPound:
                {
                    if (_groundPoundWindupRemaining > 0f)
                    {
                        // 1) windup: hang in the air for a moment
                        currentVelocity = Vector3.zero;
                    }
                    else
                    {
                        // 2) slam: straight down, fast
                        currentVelocity = -_kinematicMotor.CharacterUp * _slamSpeed;
                    }
                    break;
                }

            case CharacterState.Dead:
                {
                    // dead = no movement at all
                    currentVelocity = Vector3.zero;
                    break;
                }

            case CharacterState.Finished:
                {
                    // During the exit animation a script moves the ball (see MoveBallCenterTo), so the motor adds nothing
                    if (_scriptedMotion)
                    {
                        currentVelocity = Vector3.zero;
                        break;
                    }

                    // The level is complete: slide to a stop. Gravity still works, so a finish
                    // touched in mid-air ends with the character landing instead of floating.
                    Vector3 up = _kinematicMotor.CharacterUp;
                    Vector3 vertical = Vector3.Project(currentVelocity, up);
                    Vector3 horizontal = currentVelocity - vertical;

                    horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, GroundDeceleration * deltaTime);

                    if (_kinematicMotor.GroundingStatus.IsStableOnGround)
                        vertical = Vector3.zero;
                    else
                        vertical += CurrentGravity * deltaTime;

                    currentVelocity = horizontal + vertical;
                    break;
                }
        }
    }

    // "Called after the character moved": timers, resets and state changes live here
    public void AfterCharacterUpdate(float deltaTime)
    {
        // timers that always count down, whatever the state
        _dashCooldownTimer -= deltaTime;
        _boostMomentumTimer -= deltaTime;
        _climbCooldownTimer -= deltaTime;
        _slopeContactTimer -= deltaTime;

        // SONIC STYLE: pushing the way the boost threw us raises the speed limit (and keeps the momentum alive)
        if (_boostMomentumTimer > 0f && BoostAcceleration > 0f && _moveInput.sqrMagnitude > 0.01f
            && Vector3.Dot(_moveInput.normalized, _boostDirection) > 0.7f)
        {
            _boostCap = Mathf.MoveTowards(_boostCap, BoostMaxSpeed, BoostAcceleration * deltaTime);

            if (BoostKeepWhilePushing)
                _boostMomentumTimer += deltaTime;
        }

        _dropThroughTimer -= deltaTime;
        _wallJumpLockTimer -= deltaTime;
        _wallContactTimer -= deltaTime;
        _flipCooldownTimer -= deltaTime;
        _recentSlamTimer -= deltaTime;

        // SOUND: a landing after a real fall (not a tiny bump)
        if (_kinematicMotor.GroundingStatus.IsStableOnGround)
        {
            if (_airTime > 0.15f && CurrentState == CharacterState.Default)
            {
                Sfx.Play(SfxType.Land);
                Vfx.Play(VfxType.Land, FeetPosition, _kinematicMotor.TransientRotation);
            }

            _airTime = 0f;
        }
        else
        {
            _airTime += deltaTime;
        }

        // the stick time only runs while we really touch the wall
        if (_wallContactTimer > 0f)
            _wallStickTimer -= deltaTime;

        switch (CurrentState)
        {
            case CharacterState.Default:
                {
                    if (_kinematicMotor.GroundingStatus.IsStableOnGround && !_jumpedThisFrame && !_justFlipped)
                    {
                        // touching the ground: give back the jumps and refresh the coyote timer
                        _jumpConsumed = false;
                        _doubleJumpConsumed = false;
                        _coyoteTimer = CoyoteTime;
                        _isBouncing = false;
                        _flipUsed = false;   // a new flip is possible after landing
                        _extraAirJumps = 0;  // recharge orb extras only last until we land
                        _extraFlips = 0;

                        // give back the dash too (after the cooldown)
                        if (_dashCooldownTimer <= 0f)
                            _dashUsed = false;

                        // extra dashes and the super dash only last until you land
                        _dashCharges = 0;
                        _superDashMultiplier = 1f;
                    }
                    else
                    {
                        // in the air: the coyote time runs out
                        _coyoteTimer -= deltaTime;
                    }
                    break;
                }

            case CharacterState.Dashing:
                {
                    _dashTimeRemaining -= deltaTime;

                    // dash time is over, go back to the default state
                    if (_dashTimeRemaining <= 0f)
                        TransitionToState(CharacterState.Default);
                    break;
                }

            case CharacterState.GroundPound:
                {
                    // count down the windup
                    if (_groundPoundWindupRemaining > 0f)
                        _groundPoundWindupRemaining -= deltaTime;
                    break;
                }
        }

        _justFlipped = false;
    }

    // ------------------------------------------------------------------
    // MOVEMENT
    // ------------------------------------------------------------------
    // Looks at the ground under the feet, but only when it is a NEW ground (so it costs almost nothing)
    private void UpdateGroundSurface()
    {
        Collider ground = _kinematicMotor.GroundingStatus.GroundCollider;

        if (ground == _lastGroundCollider) return;

        _lastGroundCollider = ground;
        _currentIce = ground != null ? ground.GetComponentInParent<IceSurface>() : null;
    }

    // logic for the ground movement
    private void GroundMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        // holding the run button (only if UseRun is on) = higher top speed, slower build-up
        float maxSpeed = _isRunning ? RunSpeed : MaxGroundSpeed;

        // after a boost: the speed limit stays high for a moment
        if (_boostMomentumTimer > 0f)
            maxSpeed = Mathf.Max(maxSpeed, _boostCap);
        float pushAccel = _isRunning ? RunAcceleration : GroundAcceleration;
        float stopAccel = GroundDeceleration;

        // ICE: a slippery ground lowers the grip (slow to start, slow to stop, slow to turn around)
        UpdateGroundSurface();

        if (_currentIce != null)
        {
            pushAccel = Mathf.Min(pushAccel, _currentIce.Acceleration);
            stopAccel = Mathf.Min(stopAccel, _currentIce.Deceleration);
        }

        Vector3 targetVelocity = _moveInput * maxSpeed;

        // accelerate when pushing, decelerate (faster) when releasing
        float accel = _moveInput.sqrMagnitude > 0.01f ? pushAccel : stopAccel;

        // on the ground the glide is closed again (it needs a new press in the next jump)
        _glideReady = false;
        _glideLatched = false;

        // the air loops only exist in the air: turn them off when we touch the ground
        // (SetLoop does nothing if the loop is already off, so this costs nothing)
        Vfx.SetLoop(VfxType.GlideStream, false);
        Vfx.SetLoop(VfxType.WallSlide, false);

        // MoveTowards changes the velocity by a fixed amount per second (snappier than Lerp)
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * deltaTime);

        // RUN TRAIL: a light trace behind us, only when we run AND are really fast (so it is earned, not constant)
        bool trailOn = _isRunning && currentVelocity.magnitude >= RunSpeed * RunTrailSpeedFraction;
        Vfx.SetLoop(VfxType.RunTrail, trailOn);
    }

    // logic for the air movement
    private void AirMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        // no run trail in the air
        Vfx.SetLoop(VfxType.RunTrail, false);

        // split the velocity in a vertical part (up/down) and a horizontal part
        Vector3 up = _kinematicMotor.CharacterUp;
        Vector3 vertical = Vector3.Project(currentVelocity, up);
        Vector3 horizontal = currentVelocity - vertical;

        // air control on the horizontal part only
        // (locked for a very short time right after a wall jump)
        if (_moveInput.sqrMagnitude > 0.01f && _wallJumpLockTimer <= 0f)
        {
            // (running keeps the run speed in the air too, but while gliding the speed limit is frozen)
            float airMaxSpeed = _glideLatched ? _glideSpeedCap : (_isRunning ? RunSpeed : MaxAirSpeed);

            // after a boost: the speed limit stays high for a moment
            if (_boostMomentumTimer > 0f)
                airMaxSpeed = Mathf.Max(airMaxSpeed, _boostCap);
            Vector3 targetVelocity = _moveInput * airMaxSpeed;
            horizontal = Vector3.MoveTowards(horizontal, targetVelocity, AirAcceleration * deltaTime);
        }

        // choose how strong the gravity is this frame
        bool falling = Vector3.Dot(vertical, up) < 0f;
        bool jumpHeld = JumpAction.action.IsPressed();

        float gravityMultiplier = 1f;
        if (falling)
        {
            gravityMultiplier = FallGravityMultiplier;   // heavier when falling
            _isBouncing = false;                         // the bounce is over
        }
        else if (!jumpHeld && !_isBouncing)
        {
            // jump released early = short hop (not during the bounce, it always has the same height)
            gravityMultiplier = JumpCutMultiplier;
        }

        // gravity only applies in the air, not on the ground
        vertical += CurrentGravity * gravityMultiplier * deltaTime;

        // GLIDE (parachute): hold the glide button while falling and the fall speed is capped
        if (hasGlide)
        {
            // with a GlideAction button the glide works at any moment of the fall (even on the first jump);
            // without one it uses the jump button, which must be released once in the air first
            bool ownButton = GlideAction != null;
            bool glideHeld = ownButton ? GlideAction.action.IsPressed() : jumpHeld;

            if (!jumpHeld)
                _glideReady = true;

            float fallSpeed = -Vector3.Dot(vertical, up);

            if (glideHeld && fallSpeed > 0f && (ownButton || _glideReady || !GlideNeedsNewPress))
            {
                IsGliding = true;

                // freeze the horizontal speed limit at the moment the parachute opens
                if (!_glideLatched)
                {
                    _glideLatched = true;
                    Sfx.Play(SfxType.GlideOpen);
                    Vfx.PlayTinted(VfxType.GlideOpen, BallCenter, Quaternion.identity, PowerUpType.Air);
                    Vfx.SetLoop(VfxType.GlideStream, true, PowerUpType.Air);
                    _glideSpeedCap = _isRunning ? RunSpeed : MaxAirSpeed;
                }

                // slow down smoothly towards the glide speed (not an instant stop)
                if (fallSpeed > GlideFallSpeed)
                    vertical = -up * Mathf.MoveTowards(fallSpeed, GlideFallSpeed, GlideBrake * deltaTime);
            }
            else
            {
                _glideLatched = false;   // the parachute is closed
                Vfx.SetLoop(VfxType.GlideStream, false);
            }
        }

        // wall slide: we touch a wall AND push towards it AND we are falling
        // (-_wallNormal points INTO the wall)
        bool pushingIntoWall = Vector3.Dot(_moveInput, -_wallNormal) > 0.1f;
        bool onWall = hasWallJump && _wallContactTimer > 0f && pushingIntoWall;

        // sparks while we slide down the wall
        Vfx.SetLoop(VfxType.WallSlide, onWall && Vector3.Dot(vertical, up) < 0f, PowerUpType.Water);

        if (onWall && Vector3.Dot(vertical, up) < 0f)
        {
            // first we stick (no falling at all), then we slide down slowly
            float maxWallFallSpeed = _wallStickTimer > 0f ? 0f : WallSlideSpeed;

            if (Vector3.Dot(vertical, up) < -maxWallFallSpeed)
                vertical = -up * maxWallFallSpeed;
        }

        // limit the falling speed
        if (Vector3.Dot(vertical, up) < -MaxFallSpeed)
            vertical = -up * MaxFallSpeed;

        currentVelocity = horizontal + vertical;
    }

    // ------------------------------------------------------------------
    // JUMPING
    // ------------------------------------------------------------------
    private void HandleJumping(ref Vector3 currentVelocity)
    {
        _jumpedThisFrame = false;

        // no jump was pressed (or the buffer ran out)
        if (_jumpBufferTimer <= 0f)
            return;

        bool isGrounded = _kinematicMotor.GroundingStatus.IsStableOnGround;

        // 0) DOWN + JUMP on a one-way platform = drop through it (no jump)
        if (isGrounded && IsHoldingDown && TryStartDropThrough())
            return;

        // 1) normal jump: on the ground, or just left it (coyote time)
        if (!_jumpConsumed && _coyoteTimer > 0f)
        {
            ApplyJump(ref currentVelocity);
            Sfx.Play(SfxType.Jump);
            Vfx.Play(VfxType.Jump, FeetPosition, _kinematicMotor.TransientRotation);

            _jumpConsumed = true;
            _coyoteTimer = 0f;
        }
        // 2) wall jump: in the air, touching a wall
        else if (hasWallJump && !isGrounded && _wallContactTimer > 0f)
        {
            ApplyWallJump(ref currentVelocity);
        }
        // 3) double jump: in the air, and the double jump is not used yet
        //    (an Air recharge orb adds extra jumps on top: double jump -> triple jump)
        else if (hasDoubleJump && (!_doubleJumpConsumed || _extraAirJumps > 0) && _coyoteTimer <= 0f)
        {
            ApplyJump(ref currentVelocity);
            Sfx.Play(SfxType.DoubleJump);
            Vfx.PlayTinted(VfxType.DoubleJump, FeetPosition, _kinematicMotor.TransientRotation, PowerUpType.Air);
            RaiseMove(PowerUpType.Air, MorphDoubleJumpTime);

            // the normal double jump is spent first, then the extra ones from the orb
            if (!_doubleJumpConsumed)
                _doubleJumpConsumed = true;
            else
                _extraAirJumps--;
        }
    }

    // ------------------------------------------------------------------
    // DASH HOLD ZONE (the barrel): the ball hovers in the zone, Jump throws it along the arrows
    // ------------------------------------------------------------------
    // Called by the zone when the ball enters it
    public void EnterHold(DashZone zone)
    {
        if (zone == null || zone == _holdLastZone) return;
        if (CurrentState != CharacterState.Default && CurrentState != CharacterState.Dashing) return;

        _holdZone = zone;
        _holdRequested = true;
    }

    // Called by the zone when the ball leaves it: it can catch us again after that
    public void ExitHoldZone(DashZone zone)
    {
        if (_holdLastZone == zone)
            _holdLastZone = null;
    }

    private void HandleHoldRequest()
    {
        if (!_holdRequested) return;
        _holdRequested = false;

        if (_holdZone == null) return;
        if (CurrentState != CharacterState.Default && CurrentState != CharacterState.Dashing) return;

        // caught in the middle of a dash or a boost: finish it cleanly first
        if (CurrentState == CharacterState.Dashing)
            TransitionToState(CharacterState.Default);

        TransitionToState(CharacterState.Held);
    }

    private void HandleHold(float deltaTime)
    {
        if (CurrentState != CharacterState.Held) return;

        // the zone is gone (switched off): just let go
        if (_holdZone == null || !_holdZone.isActiveAndEnabled)
        {
            _holdZone = null;
            TransitionToState(CharacterState.Default);
            return;
        }

        _holdTimer += deltaTime;

        // hover in the middle of the zone (it follows the zone when the zone moves)
        Vector3 bob = _kinematicMotor.CharacterUp * (Mathf.Sin(_holdTimer * 6f) * 0.08f);
        Vector3 pivotToCenter = BallCenter - _kinematicMotor.TransientPosition;
        _kinematicMotor.SetPosition(_holdZone.transform.position + bob - pivotToCenter);
        _kinematicMotor.BaseVelocity = Vector3.zero;

        bool timeUp = _holdZone.HoldMaxTime > 0f && _holdTimer >= _holdZone.HoldMaxTime;

        if (_jumpBufferTimer > 0f || timeUp)
            ReleaseHold();
    }

    // Throws the ball along the zone's arrows (same as a boost, so the dash is given back and the speed carries on)
    private void ReleaseHold()
    {
        DashZone zone = _holdZone;

        _holdZone = null;
        _holdLastZone = zone;
        _jumpBufferTimer = 0f;

        TransitionToState(CharacterState.Default);
        BoostLaunch(zone.Direction, zone.HoldLaunchSpeed, zone.HoldLaunchTime, zone.HoldExitSpeed);
        HandleBoostRequest();
        Sfx.Play(SfxType.DashBoost);
    }

    // ------------------------------------------------------------------
    // SLOPE CLIMB
    // ------------------------------------------------------------------
    // Called for every wall hit. Starts the climb, or keeps it alive while we touch the wall.
    private void HandleClimbHit(Vector3 hitNormal)
    {
        if (!SlopeClimb) return;

        Vector3 up = _kinematicMotor.CharacterUp;

        // only a vertical wall counts (the 45 degree slope itself is stable ground, so it is not a wall)
        if (Mathf.Abs(Vector3.Dot(hitNormal, up)) >= 0.1f) return;

        if (_climbing)
        {
            _climbContactTimer = ClimbContactGrace;
            return;
        }

        if (_climbCooldownTimer > 0f) return;
        if (ClimbNeedsBoost && _boostMomentumTimer <= 0f) return;
        if (ClimbNeedsSlope && _slopeContactTimer <= 0f) return;

        Vector3 wallNormal = Vector3.ProjectOnPlane(hitNormal, up).normalized;
        float speed = _preMoveVelocity.magnitude;

        if (speed < ClimbMinSpeed) return;

        // we must run INTO the wall (not along it), and the player must still push towards it
        if (Vector3.Dot(_preMoveVelocity.normalized, -wallNormal) < 0.5f) return;
        if (Vector3.Dot(_moveInput, -wallNormal) <= 0.1f) return;

        _climbing = true;
        _climbWallNormal = wallNormal;
        _climbSpeed = Mathf.Min(speed * ClimbSpeedConversion, ClimbMaxSpeed);
        _climbTimeLeft = ClimbMaxTime;
        _climbContactTimer = ClimbContactGrace;

        // the boost is used up by the climb, and no free ground jump in the air afterwards
        _boostMomentumTimer = 0f;
        _coyoteTimer = 0f;
    }

    // Velocity while climbing: straight up the wall, fading out.
    private void ClimbMovement(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 up = _kinematicMotor.CharacterUp;

        _climbTimeLeft -= deltaTime;
        _climbContactTimer -= deltaTime;
        _climbSpeed = Mathf.MoveTowards(_climbSpeed, 0f, ClimbDeceleration * deltaTime);

        // JUMP: kick off the wall (needs no power, same push as the wall jump)
        if (_jumpBufferTimer > 0f)
        {
            Sfx.Play(SfxType.WallJump);
            Vfx.PlayTinted(VfxType.WallJump, BallCenter - _climbWallNormal * 0.5f, Quaternion.identity, PowerUpType.Water);

            _kinematicMotor.ForceUnground();
            currentVelocity = (_climbWallNormal * WallJumpHorizontalSpeed) + (up * WallJumpVerticalSpeed);
            _wallJumpLockTimer = WallJumpControlLockTime;
            _jumpedThisFrame = true;
            _jumpBufferTimer = 0f;
            EndClimb();
            return;
        }

        // the wall is gone = we reached the top: hop over the edge
        if (_climbContactTimer <= 0f)
        {
            currentVelocity = (up * Mathf.Max(_climbSpeed, ClimbLedgeHop)) + (-_climbWallNormal * ClimbLedgeHop);
            EndClimb();
            return;
        }

        // out of time, out of speed, or the player stopped pushing: let go (gravity takes over)
        bool pushing = Vector3.Dot(_moveInput, -_climbWallNormal) > 0.1f;

        if (_climbTimeLeft <= 0f || _climbSpeed < 0.5f || !pushing)
        {
            currentVelocity = up * _climbSpeed;
            EndClimb();
            return;
        }

        _kinematicMotor.ForceUnground();
        currentVelocity = (up * _climbSpeed) + (-_climbWallNormal * ClimbWallPress);
        Vfx.SetLoop(VfxType.WallSlide, true, PowerUpType.Water);
    }

    private void EndClimb()
    {
        _climbing = false;
        _climbCooldownTimer = ClimbCooldown;
        Vfx.SetLoop(VfxType.WallSlide, false);
    }

    // the actual jump push, used by both jumps
    private void ApplyJump(ref Vector3 currentVelocity)
    {
        // tell the motor not to snap back to the ground (required to leave the ground)
        _kinematicMotor.ForceUnground();

        // remove the old vertical velocity and add a fresh jump, so the jump always has the same height
        currentVelocity += (_kinematicMotor.CharacterUp * JumpStrength)
            - Vector3.Project(currentVelocity, _kinematicMotor.CharacterUp);

        _jumpedThisFrame = true;

        // the buffered press is used up
        _jumpBufferTimer = 0f;
    }

    // the wall jump: we REPLACE the velocity (away from the wall + up)
    private void ApplyWallJump(ref Vector3 currentVelocity)
    {
        Sfx.Play(SfxType.WallJump);
        Vfx.PlayTinted(VfxType.WallJump, BallCenter - _wallNormal * 0.5f, Quaternion.identity, PowerUpType.Water);
        RaiseMove(PowerUpType.Water, MorphWallJumpTime);

        _kinematicMotor.ForceUnground();

        currentVelocity = (_wallNormal * WallJumpHorizontalSpeed)
            + (_kinematicMotor.CharacterUp * WallJumpVerticalSpeed);

        // lock the air control for a very short time
        _wallJumpLockTimer = WallJumpControlLockTime;

        // we left the wall
        _wallContactTimer = 0f;
        _wallStickTimer = 0f;

        _jumpedThisFrame = true;
        _jumpBufferTimer = 0f;
    }

    // ------------------------------------------------------------------
    // DASH (same pattern as the Charging state in the walkthrough)
    // ------------------------------------------------------------------
    private void HandleDashInput()
    {
        if (_dashRequested && hasDash && (!_dashUsed || _dashCharges > 0)
            && _dashCooldownTimer <= 0f
            && CurrentState == CharacterState.Default)
        {
            TransitionToState(CharacterState.Dashing);
        }

        // the request is used (or ignored), reset it
        _dashRequested = false;
    }

    // ------------------------------------------------------------------
    // BOOST (the dash ring)
    // ------------------------------------------------------------------
    // Called by DashZone (Boost mode) when the ball enters it.
    //   direction = where to go (world space), speed = units per second during the boost,
    //   time = seconds of the boost, exitSpeed = the speed you keep when it ends.
    // It works like a dash (no gravity, constant speed), needs no power, and gives the dash back.
    public void BoostLaunch(Vector3 direction, float speed, float time, float exitSpeed)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        _boostDirection = direction.normalized;
        _boostSpeed = speed;
        _boostTime = Mathf.Max(time, 0.05f);
        _boostExitSpeed = exitSpeed;
        _boostRequested = true;
    }

    // ------------------------------------------------------------------
    // DASH ZONE: LANE and CHARGE
    // ------------------------------------------------------------------
    // Lane: while the ball is inside the zone, every dash goes along the zone's direction (the dash still needs the Fire power).
    public void EnterLane(DashZone zone, Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;
        _laneZone = zone;
        _laneDirection = direction.normalized;
    }

    // Only the zone that set the lane can end it (two touching zones do not cancel each other)
    public void ExitLane(DashZone zone)
    {
        if (_laneZone == zone)
            _laneZone = null;
    }

    // Charge: gives extra dashes (on top of the normal one) until the ball lands or dies. Needs the Fire power to be used.
    public void AddDashCharge(int count)
    {
        if (!hasDash) return;   // only the Fire power can use extra dashes
        _dashCharges = Mathf.Max(_dashCharges, Mathf.Max(count, 0));
    }

    // Super Dash: the NEXT dash goes "multiplier" times further (2 = double). Needs the Fire power.
    public void AddSuperDash(float multiplier)
    {
        if (!hasDash) return;
        _superDashMultiplier = Mathf.Max(_superDashMultiplier, multiplier);
    }

    // How many "loaded" dashes the ball holds (extra dashes + a super dash). Used for the pulse.
    public int DashCharges
    {
        get { return _dashCharges + (_superDashMultiplier > 1f ? 1 : 0); }
    }

    // ------------------------------------------------------------------
    // RECHARGE ORB (called by RechargeOrb)
    // ------------------------------------------------------------------
    // Gives ONE extra use of the ability of "power", until the ball lands (or dies):
    //   Fire  = one extra dash
    //   Air   = one extra jump in the air (double jump becomes a triple jump)
    //   Water = one extra gravity flip (flip, then flip back in mid air)
    // Returns true if the orb was used. False = the ball doesn't have that power right now,
    // or can't use it (dead, finished): the orb then stays where it is.
    public bool GiveRecharge(PowerUpType power)
    {
        if (CurrentState == CharacterState.Dead || CurrentState == CharacterState.Finished) return false;
        if (!HasAbilityOf(power)) return false;

        switch (power)
        {
            case PowerUpType.Fire:
                if (!hasDash) return false;
                AddDashCharge(1);
                _dashCooldownTimer = 0f;   // the extra dash is ready right away
                return true;

            case PowerUpType.Air:
                if (!hasDoubleJump) return false;
                _extraAirJumps = 1;
                return true;

            case PowerUpType.Water:
                if (!hasGravityFlip) return false;
                _extraFlips = 1;
                _flipCooldownTimer = 0f;   // the extra flip is ready right away
                return true;
        }

        return false;
    }

    // True while a Lane zone forces the dash direction
    public bool InLane
    {
        get { return _laneZone != null; }
    }

    private void HandleBoostRequest()
    {
        if (!_boostRequested) return;
        _boostRequested = false;

        if (CurrentState != CharacterState.Default && CurrentState != CharacterState.Dashing) return;

        // a boost during a dash: finish the first one cleanly, then start the new one
        if (CurrentState == CharacterState.Dashing)
            TransitionToState(CharacterState.Default);

        _boostActive = true;
        TransitionToState(CharacterState.Dashing);
    }

    // True while the player holds DOWN (with JUMP it starts a drop through a one-way platform)
    public bool IsHoldingDown
    {
        get
        {
            if (MoveAction == null) return false;

            float y = MoveAction.action.ReadValue<Vector2>().y;
            return _gravityFlipped ? y > 0.7f : y < -0.7f;
        }
    }

    // If the ground under us is a one-way platform that allows it, start the drop. Returns true if the drop started.
    private bool TryStartDropThrough()
    {
        Collider ground = _kinematicMotor.GroundingStatus.GroundCollider;

        if (ground == null || !ground.TryGetComponent(out OneWayPlatform oneWay) || !oneWay.AllowDropThrough)
            return false;

        _dropThroughTimer = DropThroughTime;

        // leave the ground, and use up the jump press (no free jump in the air)
        _kinematicMotor.ForceUnground();
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        return true;
    }

    // The place of the feet = the lowest point of the ball's capsule (NOT the pivot: the capsule sits about 0.5 above the pivot)
    public Vector3 FeetWorldPosition
    {
        get { return _kinematicMotor.TransientPosition + _kinematicMotor.CharacterTransformToCapsuleBottom; }
    }

    // ------------------------------------------------------------------
    // GROUND POUND
    // ------------------------------------------------------------------
    private void HandleGroundPoundInput()
    {
        // only possible in the air
        if (_groundPoundRequested && hasGroundPound
            && CurrentState == CharacterState.Default
            && !_kinematicMotor.GroundingStatus.IsStableOnGround)
        {
            TransitionToState(CharacterState.GroundPound);
        }

        _groundPoundRequested = false;
    }

    // The small bounce after a ground pound landing
    private void HandleBounce(ref Vector3 currentVelocity)
    {
        if (!_bounceRequested) return;
        _bounceRequested = false;

        // a jump pressed right at the landing wins: a full jump instead of the small bounce
        if (_jumpedThisFrame) return;

        // required to leave the ground (same as a normal jump)
        _kinematicMotor.ForceUnground();

        // replace the vertical velocity so the bounce always has the same height
        currentVelocity += (_kinematicMotor.CharacterUp * GroundPoundBounceStrength)
            - Vector3.Project(currentVelocity, _kinematicMotor.CharacterUp);

        _isBouncing = true;

        // the bounce counts as our jump: no free extra jump in the air
        _jumpConsumed = true;
        _coyoteTimer = 0f;

        // so AfterCharacterUpdate doesn't see us "on the ground" this frame
        _jumpedThisFrame = true;
    }

    // ------------------------------------------------------------------
    // BOUNCE PAD (called by BouncePad)
    // ------------------------------------------------------------------
    // The pad calls this when we touch it. A ground pound landing gives the BIG launch.
    // Returns true if the pad launched us (the pad can play its effect).
    // True if the last pad launch came from a ground pound (a BouncePad uses it to pick the big effect)
    public bool LastPadLaunchWasGroundPound { get; private set; }

    public bool LaunchFromPad(float strength, float groundPoundStrength)
    {
        if (CurrentState != CharacterState.Default && CurrentState != CharacterState.GroundPound) return false;

        bool slamming = CurrentState == CharacterState.GroundPound && _groundPoundWindupRemaining <= 0f;

        // still in the windup of the ground pound: the pad ignores us
        if (CurrentState == CharacterState.GroundPound && !slamming) return false;

        if (slamming)
        {
            // we hit the pad while slamming: shockwave + back to the default state
            GroundPoundImpact();
            TransitionToState(CharacterState.Default);
        }
        else if (_recentSlamTimer > 0f)
        {
            // the landing was processed a moment before the pad noticed us: it still counts as a slam
            slamming = true;
        }

        LastPadLaunchWasGroundPound = slamming;
        _padLaunchSpeed = slamming ? groundPoundStrength : strength;
        _recentSlamTimer = 0f;
        return true;
    }

    // Like LaunchFromPad, but ONLY a ground pound counts: a normal landing does nothing and
    // returns false (used by the Earth hover pad, which you stand on until you slam it).
    public bool LaunchFromPadSlamOnly(float groundPoundStrength)
    {
        bool inSlam = CurrentState == CharacterState.GroundPound && _groundPoundWindupRemaining <= 0f;
        bool justSlammed = CurrentState == CharacterState.Default && _recentSlamTimer > 0f;

        if (!inSlam && !justSlammed) return false;

        return LaunchFromPad(0f, groundPoundStrength);
    }

    private void HandlePadLaunch(ref Vector3 currentVelocity)
    {
        if (_padLaunchSpeed <= 0f) return;

        float speed = _padLaunchSpeed;
        _padLaunchSpeed = 0f;

        _kinematicMotor.ForceUnground();

        // replace the vertical velocity, so the launch always has the same height
        currentVelocity += (_kinematicMotor.CharacterUp * speed)
            - Vector3.Project(currentVelocity, _kinematicMotor.CharacterUp);

        // like the small bounce: no jump cut, and no free ground jump in the air
        // (the double jump of Air is still available)
        _isBouncing = true;
        _jumpConsumed = true;
        _doubleJumpConsumed = false;
        _coyoteTimer = 0f;
        _jumpBufferTimer = 0f;
        _wallContactTimer = 0f;

        _jumpedThisFrame = true;
    }

    // These two are created ONCE and reused on every landing, so the shockwave makes no garbage
    private readonly Collider[] _impactHits = new Collider[32];
    private readonly List<DestructibleObject> _impactTargets = new List<DestructibleObject>(16);

    // The shockwave when we land: breaks every destructible object around the feet
    private void GroundPoundImpact()
    {
        Sfx.Play(SfxType.GroundPoundImpact);
        _airTime = 0f;   // the impact has its own sound: no extra landing sound
        Vfx.PlayTinted(VfxType.GroundPoundImpact, FeetPosition, _kinematicMotor.TransientRotation, PowerUpType.Earth);

        // NonAlloc = fills our existing array instead of creating a new one
        int count = Physics.OverlapSphereNonAlloc(transform.position, GroundPoundRadius,
            _impactHits, DestructibleLayers, QueryTriggerInteraction.Ignore);

        // an object can have several colliders, but it must only be hit once
        _impactTargets.Clear();

        for (int i = 0; i < count; i++)
        {
            DestructibleObject destructible = _impactHits[i].GetComponentInParent<DestructibleObject>();

            if (destructible != null && !_impactTargets.Contains(destructible))
            {
                _impactTargets.Add(destructible);
                destructible.TakeHit(HitSource.GroundPound, -_kinematicMotor.CharacterUp);
            }
        }
    }

    // ------------------------------------------------------------------
    // GRAVITY FLIP (Vapor power)
    // ------------------------------------------------------------------
    private void HandleFlipInput()
    {
        if (_flipRequested && hasGravityFlip && (!_flipUsed || _extraFlips > 0)
            && _flipCooldownTimer <= 0f
            && CurrentState == CharacterState.Default)
        {
            FlipGravity();
        }

        // the request is used (or ignored), reset it
        _flipRequested = false;
    }

    // Turns the character upside down. Everything in this script uses CharacterUp, so once the
    // character is rotated, jumping, landing, wall jumping... all work on the ceiling too.
    private void FlipGravity()
    {
        // the first flip of a trip is free; a second one costs the extra flip from a Water orb
        if (_flipUsed && _extraFlips > 0)
            _extraFlips--;

        Quaternion oldRotation = _kinematicMotor.TransientRotation;

        // 1) rotate 180 degrees around the FORWARD axis (so the character keeps facing the same way)
        Quaternion newRotation = Quaternion.AngleAxis(180f, _kinematicMotor.CharacterForward) * oldRotation;

        // 2) the pivot is at the feet but the capsule is above it, so turning would move the capsule.
        //    We move the pivot to keep the capsule exactly where it was.
        Vector3 capsuleCenter = _capsule.center;
        Vector3 newPosition = _kinematicMotor.TransientPosition
                              + (oldRotation * capsuleCenter) - (newRotation * capsuleCenter);

        _kinematicMotor.SetRotation(newRotation);
        _kinematicMotor.SetPosition(newPosition);

        _gravityFlipped = !_gravityFlipped;

        // 3) give a push towards the new floor (keeps the sideways speed, replaces the vertical speed)
        Vector3 newUp = newRotation * Vector3.up;
        Vector3 sideways = Vector3.ProjectOnPlane(_kinematicMotor.BaseVelocity, newUp);
        _kinematicMotor.BaseVelocity = sideways - newUp * FlipLaunchSpeed;

        // 4) we are in the air now: no snapping back to the old floor, no free jump
        _kinematicMotor.ForceUnground();
        _coyoteTimer = 0f;
        _isBouncing = false;
        _wallContactTimer = 0f;
        _wallStickTimer = 0f;

        _flipUsed = true;
        _flipCooldownTimer = FlipCooldown;
        _flipBufferTimer = 0f;
        _justFlipped = true;

        Sfx.Play(SfxType.GravityFlip);
        Vfx.PlayTinted(VfxType.GravityFlip, BallCenter, Quaternion.identity, PowerUpType.Water);
        RaiseMove(PowerUpType.Water, MorphFlipTime);
    }

    // ------------------------------------------------------------------
    // STATE MACHINE
    // ------------------------------------------------------------------
    private void TransitionToState(CharacterState newState)
    {
        CharacterState oldState = CurrentState;

        OnStateExit(oldState, newState);
        CurrentState = newState;
        OnStateEnter(newState, oldState);

        // a new state starts clean
        _jumpedThisFrame = false;
        _climbing = false;
    }

    private void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch (state)
        {
            case CharacterState.Held:
                {
                    _holdTimer = 0f;
                    _kinematicMotor.ForceUnground();
                    _kinematicMotor.BaseVelocity = Vector3.zero;
                    _jumpBufferTimer = 0f;
                    _dashRequested = false;
                    Vfx.SetLoop(VfxType.DashTrail, false);
                    Sfx.Play(SfxType.ChargeGain);
                    break;
                }

            case CharacterState.Dashing:
                {
                    if (_boostActive)
                    {
                        // a boost: the ring decides the direction and the speed, and the dash is given back
                        _dashDirection = _boostDirection;
                        _dashVelocity = _boostDirection * _boostSpeed;
                        _dashTimeRemaining = _boostTime;
                        _dashUsed = false;
                        _kinematicMotor.ForceUnground();
                    }
                    else
                    {
                        // dash where the player pushes, or where the character faces if no input
                        _dashDirection = _moveInput.sqrMagnitude > 0.01f
                            ? _moveInput.normalized
                            : _kinematicMotor.CharacterForward;

                        // inside a Lane zone the direction is the lane's, whatever the input
                        if (_laneZone != null)
                            _dashDirection = _laneDirection;

                        // the normal dash was already used: this one costs a charge
                        if (_dashUsed && _dashCharges > 0)
                            _dashCharges--;

                        // cache the dash velocity (like the charging velocity)
                        _dashSpeedNow = DashSpeed;
                        _dashVelocity = _dashDirection * _dashSpeedNow;
                        _dashTimeRemaining = DashDuration;
                        _dashExtraTime = 0f;
                        _dashBlocksBroken = 0;

                        // a super dash: the speed stays the same, it just lasts longer (so it goes further), once
                        if (_superDashMultiplier > 1f)
                        {
                            _dashTimeRemaining = DashDuration * _superDashMultiplier;
                            _superDashMultiplier = 1f;
                        }

                        _dashUsed = true;
                    }

                    Sfx.Play(SfxType.Dash);
                    Vfx.PlayTinted(VfxType.DashStart, BallCenter, Quaternion.identity, PowerUpType.Fire);
                    Vfx.SetLoop(VfxType.DashTrail, true, PowerUpType.Fire);
                    Vfx.SetLoop(VfxType.RunTrail, false);

                    // the air loops stop while dashing (AirMovement does not run during a dash)
                    Vfx.SetLoop(VfxType.GlideStream, false);
                    Vfx.SetLoop(VfxType.WallSlide, false);
                    break;
                }

            case CharacterState.GroundPound:
                {
                    _groundPoundWindupRemaining = GroundPoundWindup;
                    _slamSpeed = GroundPoundSpeed;
                    _lastSlamGround = null;
                    Sfx.Play(SfxType.GroundPoundStart);
                    Vfx.PlayTinted(VfxType.GroundPoundStart, BallCenter, Quaternion.identity, PowerUpType.Earth);

                    // the air loops stop during the slam
                    Vfx.SetLoop(VfxType.GlideStream, false);
                    Vfx.SetLoop(VfxType.WallSlide, false);

                    // forget the wall, we are not sliding anymore
                    _wallContactTimer = 0f;
                    break;
                }

            case CharacterState.Dead:
                {
                    // turn the capsule off so nothing can touch us again while dead
                    // (same idea as the NoClip state of the walkthrough)
                    _kinematicMotor.SetCapsuleCollisionsActivation(false);
                    _kinematicMotor.BaseVelocity = Vector3.zero;
                    break;
                }
        }
    }

    private void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch (state)
        {
            case CharacterState.Dashing:
                {
                    if (_boostActive)
                    {
                        // a boost keeps its speed, and the speed limit stays raised for a moment
                        _kinematicMotor.BaseVelocity = _dashDirection * _boostExitSpeed;
                        _boostCap = _boostExitSpeed;
                        _boostMomentumTimer = BoostMomentumTime;
                        _boostActive = false;
                    }
                    else
                    {
                        // keep a little momentum instead of stopping dead
                        float exitSpeed = MaxGroundSpeed;

                        // BLOCK MOMENTUM: a dash that broke blocks keeps part of its speed (the boost momentum
                        // system holds the speed limit up for a moment, as long as you keep pushing)
                        if (_dashBlocksBroken > 0 && DashKeepSpeedFraction > 0f)
                        {
                            float kept = Mathf.Clamp(_dashSpeedNow * DashKeepSpeedFraction, MaxGroundSpeed, Mathf.Max(MaxGroundSpeed, BoostMaxSpeed));

                            if (kept > MaxGroundSpeed)
                            {
                                exitSpeed = kept;
                                _boostCap = kept;
                                _boostMomentumTimer = BoostMomentumTime;
                            }
                        }

                        _kinematicMotor.BaseVelocity = _dashDirection * exitSpeed;
                        _dashBlocksBroken = 0;
                    }

                    _dashCooldownTimer = DashCooldown;
                    Vfx.SetLoop(VfxType.DashTrail, false);
                    break;
                }

            case CharacterState.GroundPound:
                {
                    // remove the fast downward velocity so we don't keep it after landing
                    _kinematicMotor.BaseVelocity = Vector3.zero;
                    break;
                }

            case CharacterState.Dead:
                {
                    // the capsule is back on
                    _kinematicMotor.SetCapsuleCollisionsActivation(true);
                    break;
                }
        }
    }

    // ------------------------------------------------------------------
    // DEATH AND RESPAWN (called by RespawnManager)
    // ------------------------------------------------------------------
    // The character stops and can't be touched anymore (BallVisuals hides the ball)
    public void Die()
    {
        if (CurrentState == CharacterState.Dead) return;

        TransitionToState(CharacterState.Dead);
        Sfx.Play(SfxType.Death);
        Vfx.StopLoops();
        Vfx.PlayTinted(VfxType.Death, BallCenter, Quaternion.identity, CurrentPower);
    }

    // The level is complete (called by LevelManager): the character stops and ignores the controls.
    public void Finish()
    {
        if (CurrentState == CharacterState.Dead || CurrentState == CharacterState.Finished) return;

        TransitionToState(CharacterState.Finished);
        Sfx.Play(SfxType.LevelFinish);
    }

    // ------------------------------------------------------------------
    // EXIT ANIMATION (called by LevelExitSequence after Finish)
    // ------------------------------------------------------------------
    // true while a script moves the ball (the motor must not push it around)
    private bool _scriptedMotion;

    // where the middle of the ball is right now (world position)
    public Vector3 BallCenterPosition => BallCenter;

    // Starts the scripted move: the ball passes through everything and no longer feels gravity
    public void BeginScriptedExit()
    {
        _scriptedMotion = true;
        _kinematicMotor.SetCapsuleCollisionsActivation(false);
        _kinematicMotor.BaseVelocity = Vector3.zero;
    }

    // Puts the MIDDLE of the ball at a world position (SetPosition is the right way to move a KCC character by script)
    public void MoveBallCenterTo(Vector3 center)
    {
        if (!_scriptedMotion) return;

        Vector3 pivotToCenter = BallCenter - _kinematicMotor.TransientPosition;
        _kinematicMotor.SetPosition(center - pivotToCenter);
    }

    // Puts the character back at the last checkpoint (or the level start), ready to run, in ONE call.
    // No scene reload: that is what keeps the retry instant (pillar 2).
    public void Respawn(Vector3 position, Quaternion rotation, PowerUpType power)
    {
        // 1) teleport. SetPosition is the right way to teleport a KCC character
        //    (the walkthrough says so too: never move the transform directly).
        Vector3 positionBefore = transform.position;

        _kinematicMotor.SetPosition(position);
        _kinematicMotor.SetRotation(rotation);

        // tell Cinemachine we teleported, so the camera jumps with us instead of gliding across the level
        if (PlayerCamera != null)
            PlayerCamera.OnTargetObjectWarped(transform, position - positionBefore);

        // 2) back to the default state (this also turns the capsule back on)
        _scriptedMotion = false;
        _kinematicMotor.SetCapsuleCollisionsActivation(true);
        TransitionToState(CharacterState.Default);

        // 3) stop all movement
        _kinematicMotor.BaseVelocity = Vector3.zero;

        // 4) forget everything that was happening, so the new try starts clean
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _jumpConsumed = false;
        _doubleJumpConsumed = false;
        _extraAirJumps = 0;
        _extraFlips = 0;
        _jumpedThisFrame = false;

        _wallContactTimer = 0f;
        _wallStickTimer = 0f;
        _wallJumpLockTimer = 0f;

        _dashRequested = false;
        _dashUsed = false;
        _dashCooldownTimer = 0f;
        _dashVelocity = Vector3.zero;
        _dashSpeedNow = 0f;
        _dashExtraTime = 0f;
        _dashBlocksBroken = 0;
        _slamSpeed = GroundPoundSpeed;

        _boostRequested = false;
        _boostActive = false;
        _boostMomentumTimer = 0f;
        _climbing = false;
        _climbCooldownTimer = 0f;
        _slopeContactTimer = 0f;
        _holdZone = null;
        _holdLastZone = null;
        _holdRequested = false;
        _laneZone = null;
        _dashCharges = 0;
        _superDashMultiplier = 1f;
        _dropThroughTimer = 0f;

        _groundPoundRequested = false;
        _groundPoundWindupRemaining = 0f;
        _bounceRequested = false;
        _isBouncing = false;

        _glideReady = false;
        _glideLatched = false;
        IsGliding = false;

        _padLaunchSpeed = 0f;
        _recentSlamTimer = 0f;
        _airTime = 0f;

        // gravity is back to normal (the rotation was already reset by SetRotation above)
        _gravityFlipped = false;
        _flipRequested = false;
        _flipUsed = false;
        _justFlipped = false;
        _flipCooldownTimer = 0f;
        _flipBufferTimer = 0f;

        // 5) the power we had when we reached the checkpoint
        SetPower(power);

        Sfx.Play(SfxType.Respawn);
        Vfx.PlayTinted(VfxType.Respawn, BallCenter, Quaternion.identity, power);
    }

    // ------------------------------------------------------------------
    // POWERS
    // ------------------------------------------------------------------
    // Two sources of powers:
    //   1) OWNED powers = earned from a boss, kept forever (saved), or ticked here for testing.
    //   2) The GATE power = the last element gate crossed that you did NOT own. It lasts until the end
    //      of the level (or until you die back to a checkpoint that was before the gate).
    //
    // Choose the base look you start the level with here (None = the gold Base Spark, Aether = the dark tutorial ball).
    public PowerUpType StartingPower = PowerUpType.None;

    [Header("Owned powers")]
    // Read the powers earned from the bosses (saved on this computer under "Unlocked_Powers")
    public bool UseSavedPowers = true;

    // CHEAT: tick the powers you want to own right now, without any boss
    public PowerMask CheatOwnedPowers = PowerMask.None;

    [Header("Morph time (how long the ball wears the skin of a short move)")]
    public float MorphDoubleJumpTime = 0.30f;
    public float MorphWallJumpTime = 0.30f;
    public float MorphFlipTime = 0.35f;

    // The look of the ball (its "base skin") AND the gate power. BallVisuals and the checkpoint read this.
    public PowerUpType CurrentPower { get; private set; } = PowerUpType.None;

    // A short move happened (double jump, wall jump, flip): BallVisuals morphs the ball for "seconds".
    // Long moves (dash, ground pound, glide) are read by BallVisuals from the state, so they are not sent here.
    public event System.Action<PowerUpType, float> MoveStarted;

    private void RaiseMove(PowerUpType element, float seconds)
    {
        if (MoveStarted != null)
            MoveStarted(element, seconds);
    }

    // The level's loadout (set by LevelLoadout). No LevelLoadout in the scene = every power allowed.
    private int _allowedMask = int.MaxValue;
    private bool _freeMode;
    private bool _unlockAll;
    private bool _awakeDone;

    public void SetLoadout(PowerMask allowed, bool freeMode, bool unlockAll)
    {
        _allowedMask = (int)allowed;
        _freeMode = freeMode;
        _unlockAll = unlockAll;

        // if the character is already running, the abilities are recomputed right away
        if (_awakeDone)
            SetPower(CurrentPower);
    }

    // Recomputes what the ball can do RIGHT NOW from the powers it owns (used by the Inspector buttons while you play,
    // so ticking a power in the Inspector works at once, without a restart or a new gate).
    public void RefreshPowers()
    {
        if (_awakeDone)
            SetPower(CurrentPower);
    }

#if UNITY_EDITOR
    // Ticking "Use Saved Powers" or a cheat power in the Inspector during Play mode counts at once
    private void OnValidate()
    {
        if (Application.isPlaying)
            RefreshPowers();
    }
#endif

    // true if this power is earned (boss, or the cheat ticks, or unlock-all) AND the level allows it.
    // Free mode ignores the level's list: everything you earned works.
    public bool OwnsPower(PowerUpType power)
    {
        int bit = PowerMaskUtil.Bit(power);
        if (bit == 0) return false;

        int unlocked = (int)CheatOwnedPowers;

        if (UseSavedPowers)
            unlocked |= PowerSave.Mask;

        if (_unlockAll)
            unlocked = int.MaxValue;

        if (!_freeMode)
            unlocked &= _allowedMask;

        return (unlocked & bit) != 0;
    }

    // the ability exists if we own it, or if the gate gave it to us
    private bool HasAbilityOf(PowerUpType power)
    {
        return CurrentPower == power || OwnsPower(power);
    }

    // Called by ElementGate (a new power), by Respawn (the checkpoint skin) and at the start.
    public void SetPower(PowerUpType newPower)
    {
        CurrentPower = newPower;

        // Each element gives its own powers:
        //   Fire = dash | Air = double jump + glide | Earth = ground pound | Water = wall jump + vapor (gravity flip)
        hasDash = HasAbilityOf(PowerUpType.Fire);
        if (!hasDash)
        {
            _dashCharges = 0;   // losing the power cancels the extra dashes
            _superDashMultiplier = 1f;
        }
        hasDoubleJump = HasAbilityOf(PowerUpType.Air);
        hasGlide = hasDoubleJump;
        if (!hasDoubleJump)
            _extraAirJumps = 0;   // losing the power cancels the extra air jumps

        hasGroundPound = HasAbilityOf(PowerUpType.Earth);
        hasWallJump = HasAbilityOf(PowerUpType.Water);
        hasGravityFlip = hasWallJump;
        if (!hasGravityFlip)
            _extraFlips = 0;      // losing the power cancels the extra flips

        // Aether and None have no ability yet. When Aether gets one, add a line here.

        // losing the Vapor power while upside down: put the character back on the floor
        if (_gravityFlipped && !hasGravityFlip)
            FlipGravity();

        // the ambient particles follow the BASE skin
        Vfx.SetPower(newPower);
    }

    // ------------------------------------------------------------------
    // MOTOR CALLBACKS
    // ------------------------------------------------------------------
    public bool IsColliderValidForCollisions(Collider coll)
    {
        // a one-way platform only exists for us when we are above it (and not dropping through)
        if (coll.TryGetComponent(out OneWayPlatform oneWay))
            return oneWay.IsSolidFor(FeetWorldPosition, _dropThroughTimer > 0f);

        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    // Called when the character hits something while moving.
    // (This is where the walkthrough detects walls for the wall jump.)
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        switch (CurrentState)
        {
            case CharacterState.Default:
                {
                    // slope climb: a fast run into a vertical wall (works on the ground too)
                    HandleClimbHit(hitNormal);

                    // a wall = in the air, not a stable surface, and the surface is vertical
                    // (its normal is almost perpendicular to "up")
                    bool isWall = !_kinematicMotor.GroundingStatus.IsStableOnGround
                                  && !hitStabilityReport.IsStable
                                  && Mathf.Abs(Vector3.Dot(hitNormal, _kinematicMotor.CharacterUp)) < 0.1f;

                    if (hasWallJump && isWall)
                    {
                        // a NEW wall contact: start the stick time
                        if (_wallContactTimer <= 0f)
                            _wallStickTimer = WallStickTime;

                        _wallNormal = Vector3.ProjectOnPlane(hitNormal, _kinematicMotor.CharacterUp).normalized;
                        _wallContactTimer = WallContactGraceTime;
                    }
                    break;
                }

            case CharacterState.Dashing:
                {
                    // the dash breaks destructible objects
                    DestructibleObject destructible = hitCollider.GetComponentInParent<DestructibleObject>();

                    if (destructible != null)
                    {
                        bool wasBroken = destructible.IsBroken;
                        destructible.TakeHit(HitSource.Dash, _dashDirection);

                        // BLOCK MOMENTUM: a block that broke because of us makes the dash faster and longer
                        // (not for a boost: a boost's speed and time belong to its ring)
                        if (!_boostActive && !wasBroken && destructible.IsBroken)
                        {
                            _dashBlocksBroken++;

                            _dashSpeedNow = Mathf.Min(DashMaxSpeed, _dashSpeedNow + DashBreakSpeedBonus);
                            _dashVelocity = _dashDirection * _dashSpeedNow;

                            float extra = Mathf.Min(DashBreakTimeBonus, DashMaxExtraTime - _dashExtraTime);
                            if (extra > 0f)
                            {
                                _dashExtraTime += extra;
                                _dashTimeRemaining += extra;
                            }
                        }
                    }
                    break;
                }
        }
    }

    // Called right after the motor checked the ground (the walkthrough uses it to detect landing)
    public void PostGroundingUpdate(float deltaTime)
    {
        // we were slamming down and now we touch stable ground = IMPACT
        if (CurrentState == CharacterState.GroundPound
            && _groundPoundWindupRemaining <= 0f
            && _kinematicMotor.GroundingStatus.IsStableOnGround)
        {
            // the block we landed on (so we can see if the shockwave broke it)
            Collider groundCollider = _kinematicMotor.GroundingStatus.GroundCollider;

            // this block was already handled (PostGroundingUpdate can run twice in one motor step)
            if (groundCollider != null && groundCollider == _lastSlamGround) return;

            GroundPoundImpact();

            // MARIO SLAM: if the block under us broke, the slam does NOT stop: we fall through
            // and hit the next one (a stack of breakable blocks is destroyed in one go)
            DestructibleObject groundBlock = groundCollider != null ? groundCollider.GetComponentInParent<DestructibleObject>() : null;

            if (groundBlock != null && groundBlock.IsBroken)
            {
                // BLOCK MOMENTUM: every block we fall through makes the slam faster
                _slamSpeed = Mathf.Min(GroundPoundMaxSpeed, _slamSpeed + GroundPoundBreakSpeedBonus);

                _lastSlamGround = groundCollider;
                _kinematicMotor.ForceUnground();
                return;
            }

            _lastSlamGround = null;
            TransitionToState(CharacterState.Default);

            // Landing gives the jumps back (the Default case of AfterCharacterUpdate
            // didn't run during the ground pound). This lets a jump pressed at the landing work.
            _jumpConsumed = false;
            _doubleJumpConsumed = false;
            _extraAirJumps = 0;
            _extraFlips = 0;
            _coyoteTimer = CoyoteTime;

            // and the bounce starts in the next UpdateVelocity
            _bounceRequested = true;

            // remember the slam for a moment (a bounce pad may notice us a few milliseconds later)
            _recentSlamTimer = 0.2f;
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }
}
