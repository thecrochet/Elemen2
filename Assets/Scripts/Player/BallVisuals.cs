using UnityEngine;
using UnityEngine.Serialization;

// Makes the sphere LOOK like it rolls and changes its material depending on the power.
//
// The physics are still done by the Kinematic Character Motor (a capsule that slides).
// This script only rotates the VISUAL sphere, so it never touches the collision.
//
// Put it on the same object as CC_ELEMENTO.
public class BallVisuals : MonoBehaviour
{
    // ------------------------------------------------------------------
    // REFERENCES
    // ------------------------------------------------------------------
    public CC_ELEMENTO Player;

    // The sphere mesh. It must be a CHILD of the character and have NO collider on it.
    public Transform BallMesh;
    public Renderer BallRenderer;

    // ------------------------------------------------------------------
    // ROLLING SETTINGS
    // ------------------------------------------------------------------
    // Radius of the sphere mesh (a Unity sphere with scale 1 has a radius of 0.5)
    public float BallRadius = 0.5f;

    // 1 = the texture exactly follows the ground. Higher = spins faster (cartoon feel)
    public float RollSpeedMultiplier = 1f;

    // ------------------------------------------------------------------
    // MATERIALS (one per power)
    // ------------------------------------------------------------------
    public Material NoPowerMaterial;
    // (FormerlySerializedAs keeps the materials you already assigned in the Inspector)
    [FormerlySerializedAs("DashMaterial")] public Material FireMaterial;
    [FormerlySerializedAs("DoubleJumpMaterial")] public Material AirMaterial;
    [FormerlySerializedAs("GroundPoundMaterial")] public Material EarthMaterial;
    [FormerlySerializedAs("WallJumpMaterial")] public Material WaterMaterial;

    // ------------------------------------------------------------------
    // OPTIONAL: a parachute object (child of the character) shown while gliding
    // ------------------------------------------------------------------
    public GameObject ParachuteVisual;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    private Vector3 _lastPosition;
    private Quaternion _rollRotation;
    private PowerUpType _shownPower;
    private bool _wasDead;

    private void Start()
    {
        _lastPosition = transform.position;
        _rollRotation = BallMesh.rotation;

        _shownPower = Player.CurrentPower;
        ApplyMaterial(_shownPower);
    }

    // LateUpdate runs after the motor moved the character, so we use the final position
    private void LateUpdate()
    {
        // dead = the ball is hidden (the respawn is instant, so there is nothing to wait for)
        if (Player.CurrentState == CharacterState.Dead)
        {
            if (BallRenderer.enabled)
                BallRenderer.enabled = false;

            if (ParachuteVisual != null && ParachuteVisual.activeSelf)
                ParachuteVisual.SetActive(false);

            _wasDead = true;
            return;
        }

        // we just respawned: show the ball again, and the teleport must not count as rolling
        if (_wasDead)
        {
            _wasDead = false;
            BallRenderer.enabled = true;
            _lastPosition = transform.position;

            UpdateMaterial();
            return;
        }

        UpdateRolling();
        UpdateMaterial();
        UpdateParachute();
    }

    // show the parachute only while the character glides
    private void UpdateParachute()
    {
        if (ParachuteVisual == null) return;

        bool show = Player.IsGliding;

        if (ParachuteVisual.activeSelf != show)
            ParachuteVisual.SetActive(show);
    }

    // ------------------------------------------------------------------
    // ROLLING
    // ------------------------------------------------------------------
    private void UpdateRolling()
    {
        // how far did we move since last frame?
        Vector3 moved = transform.position - _lastPosition;
        _lastPosition = transform.position;

        // a teleport (respawn) is not a roll
        if (moved.magnitude > 5f) return;

        // only the movement along the ground makes the ball roll (jumping up/down doesn't)
        // the character's own up (it is upside down while the gravity is flipped, so the ball rolls the right way)
        Vector3 up = transform.up;
        Vector3 flatMove = Vector3.ProjectOnPlane(moved, up);
        float distance = flatMove.magnitude;

        if (distance > 0.0001f)
        {
            // A ball turns by (distance / radius) radians. That is exactly why it looks like it
            // rolls without slipping.
            float angle = (distance / BallRadius) * Mathf.Rad2Deg * RollSpeedMultiplier;

            // The ball turns around the axis that is perpendicular to the move direction and up.
            // Moving right (+X) gives the axis -Z, so the top of the ball goes forward.
            Vector3 rollAxis = Vector3.Cross(up, flatMove.normalized);

            // add this small turn on top of the rotation we already have (world space)
            _rollRotation = Quaternion.AngleAxis(angle, rollAxis) * _rollRotation;
        }

        // We set the WORLD rotation so the character's own turning (UpdateRotation) doesn't spin the ball
        BallMesh.rotation = _rollRotation;
    }

    // ------------------------------------------------------------------
    // MATERIAL
    // ------------------------------------------------------------------
    private void UpdateMaterial()
    {
        // only change the material when the power changed
        if (Player.CurrentPower == _shownPower) return;

        _shownPower = Player.CurrentPower;
        ApplyMaterial(_shownPower);
    }

    private void ApplyMaterial(PowerUpType power)
    {
        Material material = NoPowerMaterial;

        switch (power)
        {
            case PowerUpType.Fire:
                material = FireMaterial;
                break;
            case PowerUpType.Air:
                material = AirMaterial;
                break;
            case PowerUpType.Earth:
                material = EarthMaterial;
                break;
            case PowerUpType.Water:
                material = WaterMaterial;
                break;
        }

        if (material != null)
            BallRenderer.material = material;
    }
}