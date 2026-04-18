using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;

    private List<MusicPlatform> activePlatforms = new List<MusicPlatform>();

    /// <summary>Fired once per platform right before it is destroyed at the despawn line (still valid).</summary>
    public static event System.Action<MusicPlatform> OnPlatformDespawning;

    [Header("Settings")]
    public float platformSpeed = 5f;
    public float platformLengthMultiplier = 2f; // makes length of platforms bigger - base from MIDI is a bit short
    public bool isPaused = false;

    [Header("Despawn")]
    public Transform despawnPoint; // platforms get destroyed when X <= this

    private float _travelTime; // time between spawn and when the platform reaches the player

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (despawnPoint == null ) // if you forget to assign despawn point, find one (hopfully)
        {
            despawnPoint = GameObject.Find("PlatformDespawnPoint").transform;
        }

        // find distance between spawner and player's x position
        float spawnerX = GameObject.Find("PlatformSpawnPoint").transform.position.x;
        float playerX = GameObject.Find("Player").transform.position.x;
        if (spawnerX == null || playerX == null)
        {
            Debug.LogError("Player or PlatformSpawner's X position not found. PlatformManager can not calculate distance.");
        }

        float distance = spawnerX - playerX;
        _travelTime = distance / platformSpeed;

        Debug.Log("It will take " + _travelTime + " seconds to reach the player.");

    }

    private void Update()
    {
        HandlePlatformDespawn();
    }

    public void Pause(bool shouldPause)
    {
        foreach (var platform in activePlatforms)
        {
            platform.Pause(shouldPause);
        }

        isPaused = shouldPause;
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
                OnPlatformDespawning?.Invoke(platform);
                Destroy(platform.gameObject);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    public float TravelTime => _travelTime;
}
