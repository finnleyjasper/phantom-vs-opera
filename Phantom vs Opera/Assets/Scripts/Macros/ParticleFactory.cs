using UnityEngine;
using System.Collections.Generic;

public class ParticleFactory : MonoBehaviour
{
    [HideInInspector] public static ParticleFactory Instance;
    [SerializeField] public List<ParticleSystem> particleSystems; // particle system prefabs

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void CreateParticleSystem(string name, Vector3 position)
    {
        ParticleSystem newParticleSystem = null;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (particleSystem.name == name)
            {
                newParticleSystem = particleSystem;
            }
        }

        if (newParticleSystem == null)
        {
            Debug.LogWarning("ParticleFactory could not find particle system with name: " + name);
            return;
        }

        newParticleSystem = Instantiate(newParticleSystem, position, Quaternion.identity);
        newParticleSystem.Play();

        // make sure all particlesystem prefabs have Play on Awake --> true
        // make sure all particlesystem prefabs have stop action --> destroy
    }
}
