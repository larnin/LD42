using UnityEngine;
using System.Collections;

public class WeaponPulsed : WeaponBase
{
    [SerializeField] GameObject m_projectile;
    [SerializeField] float m_offset = 0.2f;
    [SerializeField] float m_projectileCount = 2;
    [SerializeField] float m_baseFireRate = 0.5f;
    [SerializeField] float m_pulseFireRate = 0.1f;
    [SerializeField] float m_powerMultiplier = 1;
    [SerializeField] float m_baseSpeed = 1;
    [SerializeField] float m_rateSpeed = 0.25f;
    [SerializeField] float m_life = 5;
    [SerializeField] Color m_color = Color.white;
    
    float m_fireRate;
    float m_longFireRate;
    float m_power;
    float m_delayToNextProjectile;
    int m_weaponCount;
    bool m_enabled;
    int m_bulletCount;

    public override void update(ShipLogic ship)
    {
        if (!m_enabled)
            return;

        m_delayToNextProjectile -= Time.deltaTime;

        if (ship.fire && m_delayToNextProjectile <= 0)
        {
            if(m_bulletCount >= m_weaponCount)
            {
                m_delayToNextProjectile = m_longFireRate;
                m_bulletCount = 0;
            }
            else 
                m_delayToNextProjectile = m_fireRate;

            float startOffset = -m_offset * (m_projectileCount-1) / 2;
            for(int i = 0; i < m_projectileCount; i++)
            {
                fire(m_projectile, ship.gameObject, new Vector3(0, startOffset + m_offset * i, 1), 0, (int)m_power, m_baseSpeed + m_rateSpeed * ship.fireRate, m_life, m_color);
            }
            
            m_bulletCount++;
        }
    }

    public override void updateStats(ShipLogic ship)
    {
        m_fireRate = 1 / (m_baseFireRate * ship.fireRate);
        m_longFireRate = 1 / (m_pulseFireRate * ship.fireRate);
        m_power = ship.power * m_powerMultiplier;


        int index = indexOfTypeOf(this, ship.modifiers);
        int count = countItemOfType<WeaponPulsed>(ship.modifiers);

        m_enabled = index == 0;
        m_weaponCount = count;

        m_delayToNextProjectile = 0;
    }
}
