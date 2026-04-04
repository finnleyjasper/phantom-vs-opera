using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;

    [Header("Settings")]
    public float platformSpeed = 5f;
    public float platformLengthMultiplier = 2f; // makes length of platforms bigger - base from MIDI is a bit short
    [Tooltip("Time between when the platform is spawned and when its note is heard in the music")] public float spawnDelay;
    // this allows us to spawn the platform off-screen and have it reach the FOV in time with the music
    public bool isPaused = false;

    [Header("Despawn")]
    public Transform despawnPoint; // platforms get destroyed when X <= this

    private List<MusicPlatform> activePlatforms = new List<MusicPlatform>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandlePlatformDespawn();
    }

    public float GetSpeed()
    {
        return isPaused ? 0f : platformSpeed;
    }

    public void RegisterPlatform(MusicPlatform platform)
    {
        activePlatforms.Add(platform);
    }

    public void UnregisterPlatform(MusicPlatform platform)
    {
        activePlatforms.Remove(platform);
    }

    private void HandlePlatformDespawn()
    {
        if (despawnPoint == null) return;

        // Iterate in reverse so we can remove while looping
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            MusicPlatform platform = activePlatforms[i];
            if (platform.transform.position.x <= despawnPoint.position.x)
            {
                Destroy(platform.gameObject);
                activePlatforms.RemoveAt(i);
            }
        }
    }
}
