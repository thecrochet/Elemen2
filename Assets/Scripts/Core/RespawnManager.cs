using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Anything that changes during the level and must come back when the player respawns
// (pickups, breakable objects...) implements this and registers itself with the RespawnManager.
//
// Think of it like a video game "save":
//   - CommitState() = SAVE the current state (at the start of the level, and at each checkpoint)
//   - ResetObject() = LOAD the last saved state (at each respawn)
public interface IResettable
{
    void CommitState();
    void ResetObject();
}

// Handles death, respawn and checkpoints for the whole level. One per scene.
//
// Design (pillar 2): the level restarts IN PLACE. No scene reload, nothing is created or
// destroyed, so the player is running again in a fraction of a second.
public class RespawnManager : MonoBehaviour
{
    // other scripts (Hazard, Checkpoint, pickups...) find the manager through this
    public static RespawnManager Instance { get; private set; }

    // ------------------------------------------------------------------
    // SETTINGS
    // ------------------------------------------------------------------
    public CC_ELEMENTO Player;

    // Where the player starts the level, and which way he faces after EVERY respawn.
    // Put the player on this spot in the scene too.
    public Transform SpawnPoint;

    // Time between death and running again. The slider stops at 0.5 on purpose:
    // that is the maximum allowed by the pillars.
    [Range(0f, 0.5f)] public float RespawnDelay = 0.25f;

    // If the player falls below this height, he dies (set it under the lowest platform)
    public float KillY = -30f;
    // Same for the top of the level (a character with flipped gravity falls UP). Set it above your highest ceiling.
    public float KillTopY = 100f;

    // Optional: a ParticleSystem placed in the scene (Play On Awake OFF, Looping OFF).
    // We move and replay the same one every death, so nothing is created while playing.
    public ParticleSystem DeathEffect;

    // Optional: instant manual respawn (for example the R key). Great for testing.
    public InputActionReference RestartAction;

    // ------------------------------------------------------------------
    // INFO FOR THE UI (later): shows the player his progress
    // ------------------------------------------------------------------
    public int Attempts { get; private set; }

    // time since the last respawn
    public float RunTime => Time.time - _runStartTime;

    // ------------------------------------------------------------------
    // PRIVATE VARIABLES
    // ------------------------------------------------------------------
    // created once with room for 64 objects, so adding to it won't allocate while playing
    private readonly List<IResettable> _resettables = new List<IResettable>(64);

    // the "save": where we come back and with which power
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;
    private PowerUpType _checkpointPower;
    private Checkpoint _activeCheckpoint;

    private bool _isDead;
    private bool _locked;   // true once the level is complete
    private float _respawnTime;
    private float _runStartTime;

    // Awake runs before every Start, so the Instance is ready when the pickups register
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        Attempts = 1;
        _runStartTime = Time.time;

        // the first "save" is the start of the level
        _spawnPosition = SpawnPoint.position;
        _spawnRotation = SpawnPoint.rotation;
        _checkpointPower = Player.StartingPower;
    }

    // Objects register themselves in their Start. At that moment they are still in their
    // starting state, so we save it right away (this works whatever the Start order is).
    // Called by LevelManager when the level is complete: no more deaths, respawns or checkpoints
    // until the next scene loads.
    public void Lock()
    {
        _locked = true;
    }

    public void Register(IResettable resettable)
    {
        _resettables.Add(resettable);
        resettable.CommitState();
    }

    public void Unregister(IResettable resettable)
    {
        _resettables.Remove(resettable);
    }

    private void Update()
    {
        // level complete: nothing can reset the run anymore
        if (_locked) return;

        // manual respawn = instant, even while dead
        if (RestartAction != null && RestartAction.action.WasPressedThisFrame())
        {
            Respawn();
            return;
        }

        if (_isDead)
        {
            // the short delay is over: back to the last checkpoint
            if (Time.time >= _respawnTime)
                Respawn();
            return;
        }

        // fell out of the level
        if (Player.transform.position.y < KillY || Player.transform.position.y > KillTopY)
            KillPlayer();
    }

    // Called by Checkpoint when the player touches it.
    // Returns true only if it is a NEW checkpoint.
    public bool SetCheckpoint(Checkpoint checkpoint)
    {
        if (_isDead || _locked || checkpoint == _activeCheckpoint) return false;

        _activeCheckpoint = checkpoint;

        // SAVE: where to come back, which power we have, and the state of every object
        _spawnPosition = checkpoint.Position;
        _checkpointPower = Player.CurrentPower;

        for (int i = 0; i < _resettables.Count; i++)
            _resettables[i].CommitState();

        return true;
    }

    // Called by hazards (and by the fall check above)
    public void KillPlayer()
    {
        // already dead: a second hazard touching us in the same moment must not count twice
        if (_isDead || _locked) return;

        _isDead = true;
        _respawnTime = Time.time + RespawnDelay;

        Player.Die();

        if (DeathEffect != null)
        {
            DeathEffect.transform.position = Player.transform.position;
            DeathEffect.Play();
        }
    }

    private void Respawn()
    {
        _isDead = false;

        // 1) the player goes back to the last checkpoint (or the start), with the power he had there
        Player.Respawn(_spawnPosition, _spawnRotation, _checkpointPower);

        // 2) LOAD: every object goes back to its last saved state (a plain for loop = no garbage)
        for (int i = 0; i < _resettables.Count; i++)
            _resettables[i].ResetObject();

        // 3) new attempt, the timer restarts
        Attempts++;
        _runStartTime = Time.time;
    }
}