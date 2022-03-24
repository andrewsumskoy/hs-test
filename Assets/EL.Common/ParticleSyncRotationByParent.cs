using UnityEngine;

namespace EL.Common
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSyncRotationByParent : MonoBehaviour
    {
        private ParticleSystem.Particle[] _particles;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        }

        private void LateUpdate()
        {
            var rotation = transform.rotation.eulerAngles;
            var main = _particleSystem.main;
            main.startRotationY = rotation.y;
            var count = _particleSystem.GetParticles(_particles);
            for (var i = 0; i < count; i++)
                _particles[i].rotation = rotation.y;
            _particleSystem.SetParticles(_particles, count);
        }
    }
}