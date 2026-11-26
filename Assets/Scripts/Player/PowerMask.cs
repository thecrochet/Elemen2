using UnityEngine;

// The powers you OWN for good (earned from the bosses), as a set of ticks.
//
// Each tick is the bit of the element's number in PowerUpType (Fire 0, Air 1, Earth 2, Water 3, Aether 5),
// so this list follows PowerUpType: when you add an element at the END of PowerUpType, add it here too.
[System.Flags]
public enum PowerMask
{
    None = 0,
    Fire = 1 << 0,
    Air = 1 << 1,
    Earth = 1 << 2,
    Water = 1 << 3,
    Aether = 1 << 5
}

public static class PowerMaskUtil
{
    // the bit of one element (None gives 0: the base ball is never "owned")
    public static int Bit(PowerUpType power)
    {
        return power == PowerUpType.None ? 0 : 1 << (int)power;
    }
}

// Reads and writes the owned powers on this computer (PlayerPrefs, key "Unlocked_Powers").
//
//   PowerSave.Unlock(PowerUpType.Fire);     // call this when a boss is beaten
//   PowerSave.Has(PowerUpType.Fire);
//   PowerSave.Clear();                      // start a new game (or use the menu Elemento > Clear Saved Powers)
public static class PowerSave
{
    private const string Key = "Unlocked_Powers";

    private static int _mask;
    private static bool _loaded;

    public static int Mask
    {
        get
        {
            if (!_loaded)
            {
                _mask = PlayerPrefs.GetInt(Key, 0);
                _loaded = true;
            }
            return _mask;
        }
    }

    public static bool Has(PowerUpType power)
    {
        return (Mask & PowerMaskUtil.Bit(power)) != 0;
    }

    public static void Unlock(PowerUpType power)
    {
        int bit = PowerMaskUtil.Bit(power);
        if (bit == 0 || (Mask & bit) != 0) return;

        _mask = Mask | bit;
        PlayerPrefs.SetInt(Key, _mask);
        PlayerPrefs.Save();
    }

    // Takes one power away again (for testing: tick and untick a power in the Inspector)
    public static void Lock(PowerUpType power)
    {
        int bit = PowerMaskUtil.Bit(power);
        if (bit == 0 || (Mask & bit) == 0) return;

        _mask = Mask & ~bit;
        PlayerPrefs.SetInt(Key, _mask);
        PlayerPrefs.Save();
    }

    // Owned or not, in one call
    public static void Set(PowerUpType power, bool owned)
    {
        if (owned)
            Unlock(power);
        else
            Lock(power);
    }

    // All four elements at once (the same as having beaten every boss)
    public static void UnlockAll()
    {
        _mask = PowerMaskUtil.Bit(PowerUpType.Fire) | PowerMaskUtil.Bit(PowerUpType.Air)
              | PowerMaskUtil.Bit(PowerUpType.Earth) | PowerMaskUtil.Bit(PowerUpType.Water);
        _loaded = true;
        PlayerPrefs.SetInt(Key, _mask);
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        _mask = 0;
        _loaded = true;
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }

    // Unity can keep static values between Play sessions when "Enter Play Mode Options" is on: start fresh
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _loaded = false;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Elemento/Clear Saved Powers")]
    private static void ClearFromMenu()
    {
        Clear();
        Debug.Log("Elemento: saved powers cleared.");
    }
#endif
}
