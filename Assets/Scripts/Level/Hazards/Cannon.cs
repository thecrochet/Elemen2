using UnityEngine;

// A cannon that shoots a projectile on a timer. The projectile kills on touch (it carries the Hazard script).
//
// STRUCTURE (the menu GameObject > Elemento > Hazards > Cannon builds it):
//   Cannon           this script + a solid Box Collider
//    +- Body, Barrel pictures, no collider
//    +- Muzzle       empty object at the tip of the barrel: the shots start here
//    +- Projectile   the TEMPLATE: a Sphere Collider [trigger] + Hazard + a picture child.
//                    The script makes copies of it when the game starts (a few, reused forever: nothing is created while playing)
//    +- ShootEffect  Particle System (Play On Awake OFF, Looping OFF), plays at each shot (optional)
//
// AIM: turn the whole Cannon object. "Local Direction" (1,0,0) means "along the red arrow of the Cannon".
//
// FAIRNESS: every shot is calculated from the LEVEL CLOCK, not from its own timer:
//   - it only flies while the level clock runs
//   - after a death every shot is exactly where it was at your last checkpoint
// So every retry shows exactly the same pattern. Do not put the cannon on a moving platform.
[DisallowMultipleComponent]
public class Cannon : MonoBehaviour, IResettable
{
    [Header("Shots")]
    // The template projectile (a child of the cannon). It must have a trigger collider and the Hazard script.
    public GameObject Projectile;

    // Where the shots start (an empty child at the tip of the barrel). Empty = the cannon's own position.
    public Transform Muzzle;

    // The direction of the shots, in the cannon's own space (1,0,0 = along its red arrow)
    public Vector3 LocalDirection = Vector3.right;

    // Units per second
    public float Speed = 8f;

    // Seconds between two shots
    public float Interval = 2f;

    // Seconds before the first shot (put two cannons out of step with this)
    public float Delay = 0f;

    // How far a shot flies before it disappears (world units)
    public float Range = 14f;

    [Header("Look (optional)")]
    public ParticleSystem ShootEffect;

    private Transform[] _slots;
    private Rigidbody[] _bodies;
    private bool[] _live;

    private float _time;            // the cannon's own clock
    private float _committedTime;   // the "save"
    private int _lastShot = -1;     // number of the last shot that was fired (for the shoot effect)

    private void Start()
    {
        if (Projectile == null)
        {
            Debug.LogWarning("Cannon: no Projectile assigned.", this);
            enabled = false;
            return;
        }

        float interval = Mathf.Max(Interval, 0.1f);
        float lifetime = Range / Mathf.Max(Speed, 0.01f);

        // enough projectiles so that every shot that is still flying has its own
        int count = Mathf.CeilToInt(lifetime / interval) + 1;

        _slots = new Transform[count];
        _bodies = new Rigidbody[count];
        _live = new bool[count];

        for (int i = 0; i < count; i++)
        {
            GameObject copy = i == 0 ? Projectile : Instantiate(Projectile, Projectile.transform.parent);
            copy.name = Projectile.name + "_" + i;

            // a kinematic body moves only when WE tell it to (no gravity, no pushing)
            if (!copy.TryGetComponent(out Rigidbody body))
                body = copy.AddComponent<Rigidbody>();

            body.isKinematic = true;
            body.useGravity = false;

            _slots[i] = copy.transform;
            _bodies[i] = body;

            copy.SetActive(false);
        }

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Register(this);

        _lastShot = ShotNumber();
        UpdateProjectiles(true);
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.Unregister(this);
    }

    private void FixedUpdate()
    {
        // only runs while the level clock runs (or after the finish)
        LevelManager level = LevelManager.Instance;

        if (level == null || level.IsClockRunning || level.IsFinished)
            _time += Time.fixedDeltaTime;

        int shot = ShotNumber();

        if (shot > _lastShot)
        {
            _lastShot = shot;

            if (ShootEffect != null)
                ShootEffect.Play(true);
        }

        UpdateProjectiles(false);
    }

    // The number of the latest shot that was fired (-1 = none yet). Shot 0 is fired at "Delay".
    private int ShotNumber()
    {
        if (_time < Delay) return -1;

        return Mathf.FloorToInt((_time - Delay) / Mathf.Max(Interval, 0.1f));
    }

    // Puts every projectile where it must be at this moment. The position only depends on the clock.
    private void UpdateProjectiles(bool teleport)
    {
        float interval = Mathf.Max(Interval, 0.1f);
        float speed = Mathf.Max(Speed, 0.01f);
        float lifetime = Range / speed;

        int k = ShotNumber();
        int count = _slots.Length;

        Vector3 direction = transform.TransformDirection(LocalDirection).normalized;
        Vector3 origin = (Muzzle != null ? Muzzle : transform).position;

        for (int j = 0; j < count; j++)
        {
            bool live = false;
            float age = 0f;

            if (k >= 0)
            {
                // each projectile handles every "count"-th shot: this is the latest shot that belongs to projectile j
                int mine = k - (((k - j) % count) + count) % count;

                if (mine >= 0)
                {
                    age = _time - (Delay + mine * interval);
                    live = age >= 0f && age <= lifetime;
                }
            }

            GameObject go = _slots[j].gameObject;

            if (live)
            {
                Vector3 position = origin + direction * speed * age;

                if (!_live[j])
                {
                    // a new shot starts: put it at its place first, then show it
                    _slots[j].position = position;
                    go.SetActive(true);
                    _live[j] = true;
                }

                if (teleport)
                    _slots[j].position = position;
                else
                    _bodies[j].MovePosition(position);
            }
            else if (_live[j])
            {
                go.SetActive(false);
                _live[j] = false;
            }
        }
    }

    // SAVE (start of the level and at each checkpoint)
    public void CommitState()
    {
        _committedTime = _time;
    }

    // LOAD (at each respawn)
    public void ResetObject()
    {
        _time = _committedTime;
        _lastShot = ShotNumber();

        if (_slots != null)
            UpdateProjectiles(true);
    }

#if UNITY_EDITOR
    // draws the path of the shots in the Scene view
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = (Muzzle != null ? Muzzle : transform).position;
        Vector3 direction = transform.TransformDirection(LocalDirection).normalized;

        Gizmos.color = new Color(1f, 0.3f, 0.5f, 0.8f);
        Gizmos.DrawLine(origin, origin + direction * Range);
        Gizmos.DrawWireSphere(origin + direction * Range, 0.2f);
    }
#endif
}
