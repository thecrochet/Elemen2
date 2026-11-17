using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

// Lives on the Cinemachine camera (the zones add it by themselves the first time they are needed).
// It remembers the normal framing, keeps the list of zones the player is in, and eases the camera
// toward the right values. Not used directly: CameraZone talks to it.
public class CameraZoneBlender : MonoBehaviour
{
    // the blender of the current level (CameraZone uses it when the player respawns)
    public static CameraZoneBlender Current { get; private set; }

    private CinemachinePositionComposer _composer;

    // the framing you set on the Position Composer in the Inspector
    private float _defaultDistance;
    private Vector3 _defaultOffset;

    // the zones the player is inside, the last one in the list wins
    private readonly List<CameraZone> _zones = new List<CameraZone>(4);

    // where we are going, and how fast
    private float _targetDistance;
    private Vector3 _targetOffset;
    private float _blendTime = 0.8f;

    // the speeds SmoothDamp needs to remember
    private float _distanceVelocity;
    private Vector3 _offsetVelocity;

    // Finds the blender on this camera, or adds it
    public static CameraZoneBlender Get(CinemachineCamera cinemachineCamera)
    {
        if (cinemachineCamera.TryGetComponent(out CameraZoneBlender blender))
            return blender;

        if (!cinemachineCamera.TryGetComponent(out CinemachinePositionComposer composer))
        {
            Debug.LogWarning("CameraZone: the Cinemachine camera has no Position Composer, so the zone can't change the framing.");
            return null;
        }

        blender = cinemachineCamera.gameObject.AddComponent<CameraZoneBlender>();
        blender.Setup(composer);
        return blender;
    }

    private void Setup(CinemachinePositionComposer composer)
    {
        _composer = composer;
        _defaultDistance = composer.CameraDistance;
        _defaultOffset = composer.TargetOffset;

        _targetDistance = _defaultDistance;
        _targetOffset = _defaultOffset;

        Current = this;

        // nothing to ease yet: the Update only runs when we are going somewhere
        enabled = false;
    }

    private void OnDestroy()
    {
        if (Current == this)
            Current = null;
    }

    public void Enter(CameraZone zone)
    {
        // the zone entered last goes to the end of the list
        _zones.Remove(zone);
        _zones.Add(zone);
        Retarget();
    }

    public void Exit(CameraZone zone)
    {
        _zones.Remove(zone);
        Retarget();
    }

    // Back to the normal framing at once (used after a death, so the retry stays instant)
    public void ClearAll()
    {
        _zones.Clear();

        _targetDistance = _defaultDistance;
        _targetOffset = _defaultOffset;
        _distanceVelocity = 0f;
        _offsetVelocity = Vector3.zero;

        _composer.CameraDistance = _defaultDistance;
        _composer.TargetOffset = _defaultOffset;
        enabled = false;
    }

    // Decides where the camera must go: the last zone, or the normal framing if there is none
    private void Retarget()
    {
        _targetDistance = _defaultDistance;
        _targetOffset = _defaultOffset;

        if (_zones.Count > 0)
        {
            CameraZone zone = _zones[_zones.Count - 1];

            if (zone.ChangeDistance)
                _targetDistance = zone.CameraDistance;

            if (zone.ChangeOffset)
                _targetOffset = zone.TargetOffset;

            _blendTime = zone.BlendTime;
        }

        // when we go BACK to normal we use the blend time of the zone we left, which is still in _blendTime
        enabled = true;
    }

    // Update (not LateUpdate): Cinemachine reads the values in LateUpdate, so it always sees the newest ones
    private void Update()
    {
        _composer.CameraDistance = Mathf.SmoothDamp(_composer.CameraDistance, _targetDistance, ref _distanceVelocity, _blendTime);
        _composer.TargetOffset = Vector3.SmoothDamp(_composer.TargetOffset, _targetOffset, ref _offsetVelocity, _blendTime);

        // arrived: stop updating until the next zone
        if (Mathf.Abs(_composer.CameraDistance - _targetDistance) < 0.001f
            && (_composer.TargetOffset - _targetOffset).sqrMagnitude < 0.000001f)
        {
            _composer.CameraDistance = _targetDistance;
            _composer.TargetOffset = _targetOffset;
            enabled = false;
        }
    }
}
