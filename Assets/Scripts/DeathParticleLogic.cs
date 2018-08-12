using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DeathParticleLogic : MonoBehaviour
{
    [SerializeField] float m_duration = 2;
    [SerializeField] float m_stopParticleTime = 0.2f;

    ParticleSystem m_particle;
    float m_timer;

    private void Start()
    {
        m_particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > m_stopParticleTime && m_particle.isEmitting)
            m_particle.Stop();
        if (m_timer > m_duration)
            Destroy(gameObject);
    }
}
