using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WeaponEnemySimpleShape : WeaponBase
{
    [SerializeField] GameObject m_projectile;
    [SerializeField] float m_baseFireRate = 0.5f;
    [SerializeField] float m_baseFireRateSlow = 0.1f;
    [SerializeField] float m_powerMultiplier = 1;
    [SerializeField] float m_baseSpeed = 1;
    [SerializeField] float m_rateSpeed = 0.25f;
    [SerializeField] float m_life = 5;
    [SerializeField] Color m_color = Color.white;
    [SerializeField] float m_weaponLevel = 1;

    float m_rotation;
    float m_fireRate;
    float m_fireRateSlow;
    float m_power;
    float m_delayToNextProjectile;
    int m_projectileCount;

    public override void update(ShipLogic ship)
    {
        m_delayToNextProjectile -= Time.deltaTime;

        if (/*ship.fire &&*/ m_delayToNextProjectile <= 0)
        {
            m_projectileCount++;
            if (m_projectileCount >= m_weaponLevel)
            {
                m_delayToNextProjectile = m_fireRateSlow;
                m_projectileCount = 0;
            }
            else m_delayToNextProjectile = m_fireRate;

            Vector3 offset = Quaternion.Euler(0, 0, m_rotation) * new Vector3(0.6f, 0, 1);

            fire(m_projectile, ship.gameObject, offset, m_rotation, (int)m_power, m_baseSpeed + m_rateSpeed * ship.fireRate, m_life, m_color);
        }
    }

    public override void updateStats(ShipLogic ship)
    {
        m_fireRate = 1 / (m_baseFireRate * ship.fireRate);
        m_fireRateSlow = 1 / (m_baseFireRateSlow * ship.fireRate);
        m_power = ship.power * m_powerMultiplier;


        int index = indexOfTypeOf(this, ship.modifiers);
        int count = countItemOfType<WeaponEnemySimpleShape>(ship.modifiers);

        m_rotation = index * 360 / count + 90;

        m_delayToNextProjectile = 0;
    }
}
