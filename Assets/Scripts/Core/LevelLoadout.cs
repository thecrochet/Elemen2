using UnityEngine;

// How the level treats the powers (Progression Spec). Put ONE in the level scene (on the same object as the LevelManager is fine).
//
//   Play mode : you can use a power only if you OWN it (boss) AND this level ALLOWS it.
//   Free mode : you can use every power you own, whatever the level allows. No best time, no stars are saved.
//
// No LevelLoadout in a scene = every power is allowed (the level plays exactly as before).
// Gate powers (an element gate for a power you do not have) are not blocked by this list: the level designer placed the gate.
[DisallowMultipleComponent]
[DefaultExecutionOrder(-200)]   // runs before the character's Awake
public class LevelLoadout : MonoBehaviour
{
    public enum LevelMode
    {
        Play,   // ranked: Owned AND Allowed
        Free    // practice: everything you own, nothing is saved
    }

    public static LevelLoadout Instance { get; private set; }

    public CC_ELEMENTO Player;

    [Header("Era loadout")]
    // Tick the powers this level lets you use.
    // Tutorial: none. World 1: Fire. World 2: Fire + Earth. World 3: Fire + Earth + Air. World 4: all four.
    public PowerMask AllowedPowers = PowerMask.Fire | PowerMask.Air | PowerMask.Earth | PowerMask.Water;

    public LevelMode Mode = LevelMode.Play;

    [Header("Debug (until the hub exists)")]
    // Treat every power as earned (the same as having beaten every boss)
    public bool DebugUnlockAll = false;

    public bool IsFreeMode => Mode == LevelMode.Free;

    private void Awake()
    {
        Instance = this;

        if (Player == null)
            Player = FindFirstObjectByType<CC_ELEMENTO>();

        if (Player != null)
            Player.SetLoadout(AllowedPowers, IsFreeMode, DebugUnlockAll);
        else
            Debug.LogWarning("LevelLoadout: no CC_ELEMENTO found.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
