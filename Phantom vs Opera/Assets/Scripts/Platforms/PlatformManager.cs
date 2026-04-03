using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;

    [Header("Movement")]
    public float platformSpeed = 5f;
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