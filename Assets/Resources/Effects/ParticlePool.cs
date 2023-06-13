using UnityEngine;
using System.Collections;

public class ParticlePool : MonoBehaviour
{
    public string particlePath = "Effects/SmokeParticle";
    public Transform emissionPoint;
    public int poolSize = 100;
    public float particleSize = 1f;
    [SerializeField]
    private ParticleSystem[] particlePool;
    private int currentIndex = 0;
    [SerializeField]
    Vector3 offset = new Vector3(0f, 0.4f, 2.4f);
    [SerializeField]
    float minSize = 6;
    [SerializeField]
    float maxSize = 7;
    public float fadeDuration = 8f;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        particlePool = new ParticleSystem[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem newParticle = Resources.Load<ParticleSystem>(particlePath);
            newParticle = Instantiate(newParticle, Vector3.zero, Quaternion.identity);
            newParticle.gameObject.SetActive(false);
            particlePool[i] = newParticle;
        }
    }

    public void EnableNextParticle(float _size)
    {
        _size /= 30;

        ParticleSystem currentParticle = particlePool[currentIndex];
        currentParticle.gameObject.SetActive(true);
        currentParticle.Stop();

        float size = Random.Range(_size / minSize, _size / maxSize);
        ParticleSystem.MainModule mainModule = currentParticle.main;
        mainModule.startSize = size;

        Vector3 newPosition = transform.position;
        newPosition.x += Random.Range(-0.1f, 0.1f);
        Quaternion newRotation = transform.rotation * Quaternion.Euler(0, 180, 0);

        Vector3 worldOffset = newRotation * offset;

        newPosition += worldOffset;

        currentParticle.Emit(1);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[currentParticle.main.maxParticles];
        int numParticles = currentParticle.GetParticles(particles);

        particles[numParticles - 1].position = newPosition;
        particles[numParticles - 1].rotation3D = newRotation.eulerAngles;

        currentParticle.SetParticles(particles, numParticles);

        currentParticle.Play();

        currentIndex++;
        if (currentIndex >= poolSize)
        {
            currentIndex = 0;
        }
    }


}