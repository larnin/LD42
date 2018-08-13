using NRand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WeaponStraight : WeaponBase
{
    [SerializeField] GameObject m_projectile;
    [SerializeField] float m_baseFireRate = 0.1f;
    [SerializeField] float m_powerMultiplier = 2;
    [SerializeField] float m_baseSpeed = 1;
    [SerializeField] float m_rateSpeed = 0.25f;
    [SerializeField] float m_life = 5;
    [SerializeField] Color m_color = Color.white;
    
    float m_fireRate;
    float m_power;
    float m_delayToNextProjectile;
    bool m_enabled;

    public override void update(ShipLogic ship)
    {
        if (!m_enabled)
            return;

        m_delayToNextProjectile -= Time.deltaTime;

        if (ship.fire && m_delayToNextProjectile <= 0)
        {
            m_delayToNextProjectile = m_fireRate;

            SoundSystem.instance.play(new BernoulliDistribution().Next(new StaticRandomGenerator<DefaultRandomGenerator>()) ? m_shootClip : m_shootClip2, 0.03f);
            fire(m_projectile, ship.gameObject, new Vector3(0, 0, 1), 0, (int)m_power, m_baseSpeed + m_rateSpeed * ship.fireRate, m_life, m_color);
        }
    }

    public override void updateStats(ShipLogic ship)
    {
        int count = countItemOfType<WeaponStraight>(ship.modifiers);
        m_fireRate = 1 / (m_baseFireRate * ship.fireRate * count);
        m_power = ship.power * m_powerMultiplier;

        int index = indexOfTypeOf(this, ship.modifiers);
        m_enabled = index == 0;

        m_delayToNextProjectile = 0;
    }
}
