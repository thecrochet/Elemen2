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
    Finished
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
    private float _dashCooldownTimer;

    // ground pound
    private bool _groundPoundRequested;
    private float _groundPoundWindupRemaining;
    private bool _bounceRequested;   // set on landing, applied in UpdateVelocity
    private bool _isBouncing;        // true while going up from the bounce

    // sound
    private float _airTime;            // how long we have been in the air (a landing sound needs a real fall)

    // where the effects appear: the middle of the ball (the pivot is at the feet)
    private Vector3 BallCenter => _kinematicMotor.TransientPosition + _kinematicMotor.TransientRotation * _capsule.center;

    // bounce pad
    private float _padLaunchSpeed;     // set by LaunchFromPad, applied in UpdateVelocity (0 = nothing waiting)
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

        if (GravityFlipAction != null && GravityFlipAction.action.WasPressedThisFrame())
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
        HandleDashInput();
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
                        currentVelocity = -_kinematicMotor.CharacterUp * GroundPoundSpeed;
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
                Vfx.Play(VfxType.Land, _kinematicMotor.TransientPosition, _kinematicMotor.TransientRotation);
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

                        // give back the dash too (after the cooldown)
                        if (_dashCooldownTimer <= 0f)
                            _dashUsed = false;
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

        // MoveTowards changes the velocity by a fixed amount per second (snappier than Lerp)
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * deltaTime);
    }

    // logic for the air movement
    private void AirMovement(ref Vector3 currentVelocity, float deltaTime)
    {
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
                    Vfx.PlayTinted(VfxType.GlideOpen, BallCenter, Quaternion.identity, CurrentPower);
                    Vfx.SetLoop(VfxType.GlideStream, true);
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
        Vfx.SetLoop(VfxType.WallSlide, onWall && Vector3.Dot(vertical, up) < 0f);

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

        // 1) normal jump: on the ground, or just left it (coyote time)
        if (!_jumpConsumed && _coyoteTimer > 0f)
        {
            ApplyJump(ref currentVelocity);
            Sfx.Play(SfxType.Jump);
            Vfx.Play(VfxType.Jump, _kinematicMotor.TransientPosition, _kinematicMotor.TransientRotation);

            _jumpConsumed = true;
            _coyoteTimer = 0f;
        }
        // 2) wall jump: in the air, touching a wall
        else if (hasWallJump && !isGrounded && _wallContactTimer > 0f)
        {
            ApplyWallJump(ref currentVelocity);
        }
        // 3) double jump: in the air, and the double jump is not used yet
        else if (hasDoubleJump && !_doubleJumpConsumed && _coyoteTimer <= 0f)
        {
            ApplyJump(ref currentVelocity);
            Sfx.Play(SfxType.DoubleJump);
            Vfx.PlayTinted(VfxType.DoubleJump, _kinematicMotor.TransientPosition, _kinematicMotor.TransientRotation, CurrentPower);

            _doubleJumpConsumed = true;
        }
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
        Vfx.PlayTinted(VfxType.WallJump, BallCenter - _wallNormal * 0.5f, Quaternion.identity, CurrentPower);

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
        if (_dashRequested && hasDash && !_dashUsed
            && _dashCooldownTimer <= 0f
            && CurrentState == CharacterState.Default)
        {
            TransitionToState(CharacterState.Dashing);
        }

        // the request is used (or ignored), reset it
        _dashRequested = false;
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

        _padLaunchSpeed = slamming ? groundPoundStrength : strength;
        _recentSlamTimer = 0f;
        return true;
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
        Vfx.PlayTinted(VfxType.GroundPoundImpact, _kinematicMotor.TransientPosition, _kinematicMotor.TransientRotation, CurrentPower);

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
                destructible.TakeHit(HitSource.GroundPound);
            }
        }
    }

    // ------------------------------------------------------------------
    // GRAVITY FLIP (Vapor power)
    // ------------------------------------------------------------------
    private void HandleFlipInput()
    {
        if (_flipRequested && hasGravityFlip && !_flipUsed
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
        _justFlipped = true;

        Sfx.Play(SfxType.GravityFlip);
        Vfx.PlayTinted(VfxType.GravityFlip, BallCenter, Quaternion.identity, CurrentPower);
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
    }

    private void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch (state)
        {
            case CharacterState.Dashing:
                {
                    // dash where the player pushes, or where the character faces if no input
                    _dashDirection = _moveInput.sqrMagnitude > 0.01f
                        ? _moveInput.normalized
                        : _kinematicMotor.CharacterForward;

                    // cache the dash velocity (like the charging velocity)
                    _dashVelocity = _dashDirection * DashSpeed;
                    _dashTimeRemaining = DashDuration;
                    _dashUsed = true;
                    Sfx.Play(SfxType.Dash);
                    Vfx.PlayTinted(VfxType.DashStart, BallCenter, Quaternion.identity, CurrentPower);
                    Vfx.SetLoop(VfxType.DashTrail, true);
                    break;
                }

            case CharacterState.GroundPound:
                {
                    _groundPoundWindupRemaining = GroundPoundWindup;
                    Sfx.Play(SfxType.GroundPoundStart);
                    Vfx.PlayTinted(VfxType.GroundPoundStart, BallCenter, Quaternion.identity, CurrentPower);

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
                    // keep a little momentum instead of stopping dead
                    _kinematicMotor.BaseVelocity = _dashDirection * MaxGroundSpeed;
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
        TransitionToState(CharacterState.Default);

        // 3) stop all movement
        _kinematicMotor.BaseVelocity = Vector3.zero;

        // 4) forget everything that was happening, so the new try starts clean
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _jumpConsumed = false;
        _doubleJumpConsumed = false;
        _jumpedThisFrame = false;

        _wallContactTimer = 0f;
        _wallStickTimer = 0f;
        _wallJumpLockTimer = 0f;

        _dashRequested = false;
        _dashUsed = false;
        _dashCooldownTimer = 0f;
        _dashVelocity = Vector3.zero;

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

        // 5) the power we had when we reached the checkpoint
        SetPower(power);

        Sfx.Play(SfxType.Respawn);
        Vfx.PlayTinted(VfxType.Respawn, BallCenter, Quaternion.identity, power);
    }

    // ------------------------------------------------------------------
    // POWER-UPS (called by PowerUpPickup)
    // ------------------------------------------------------------------
    // Only ONE element at a time: picking a new one replaces the old one.
    // Choose the power you start with here (None = no power). Handy for testing.
    public PowerUpType StartingPower = PowerUpType.None;

    // the power the player has right now (BallVisuals reads this to pick the material)
    public PowerUpType CurrentPower { get; private set; } = PowerUpType.None;

    public void SetPower(PowerUpType newPower)
    {
        CurrentPower = newPower;

        // only the chosen power is true, all the others are false
        // Each element gives its own powers:
        //   Fire = dash | Air = double jump + glide | Earth = ground pound | Water = wall jump + vapor (gravity flip)
        hasDash = newPower == PowerUpType.Fire;
        hasDoubleJump = newPower == PowerUpType.Air;
        hasGlide = newPower == PowerUpType.Air;
        hasGroundPound = newPower == PowerUpType.Earth;
        hasWallJump = newPower == PowerUpType.Water;
        hasGravityFlip = newPower == PowerUpType.Water;

        // losing the Vapor power while upside down: put the character back on the floor
        if (_gravityFlipped && !hasGravityFlip)
            FlipGravity();

        // the right ambient particles for this element
        Vfx.SetPower(newPower);
    }

    // ------------------------------------------------------------------
    // MOTOR CALLBACKS
    // ------------------------------------------------------------------
    public bool IsColliderValidForCollisions(Collider coll)
    {
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
                        destructible.TakeHit(HitSource.Dash);
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
            GroundPoundImpact();
            TransitionToState(CharacterState.Default);

            // Landing gives the jumps back (the Default case of AfterCharacterUpdate
            // didn't run during the ground pound). This lets a jump pressed at the landing work.
            _jumpConsumed = false;
            _doubleJumpConsumed = false;
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