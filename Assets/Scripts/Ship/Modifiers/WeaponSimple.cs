using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NRand;

public class WeaponSimple : WeaponBase
{
    [SerializeField] GameObject m_projectile;
    [SerializeField] float m_rotationPerLevel = 10;
    [SerializeField] float m_baseFireRate = 0.5f;
    [SerializeField] float m_powerMultiplier = 1;
    [SerializeField] float m_baseSpeed = 1;
    [SerializeField] float m_rateSpeed = 0.25f;
    [SerializeField] float m_life = 5;
    [SerializeField] Color m_color = Color.white;

    float m_rotation;
    float m_fireRate;
    float m_power;
    float m_delayToNextProjectile;
    

    public override void update(ShipLogic ship)
    {
        m_delayToNextProjectile -= Time.deltaTime;

        if(ship.fire && m_delayToNextProjectile <= 0)
        {
            m_delayToNextProjectile = m_fireRate;

            var rot = new UniformFloatDistribution(-m_rotation, m_rotation).Next(new StaticRandomGenerator<DefaultRandomGenerator>());

            SoundSystem.instance.play(new BernoulliDistribution().Next(new StaticRandomGenerator<DefaultRandomGenerator>()) ? m_shootClip : m_shootClip2, 0.03f);
            fire(m_projectile, ship.gameObject, new Vector3(0, 0, 1), rot, (int)m_power, m_baseSpeed + m_rateSpeed * ship.fireRate, m_life, m_color);
        }
    }

    public override void updateStats(ShipLogic ship)
    {

        m_fireRate = 1 / (m_baseFireRate * ship.fireRate);
        m_power = ship.power * m_powerMultiplier;

        int count = countItemOfType<WeaponSimple>(ship.modifiers);
        m_rotation = count * m_rotationPerLevel;

        m_delayToNextProjectile = 0;
    }
}
