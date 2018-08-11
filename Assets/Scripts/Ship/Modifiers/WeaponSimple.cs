﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

            fire(m_projectile, ship.gameObject, new Vector3(0, 0, 1), m_rotation, (int)m_power, m_baseSpeed + m_rateSpeed * ship.fireRate, m_life, m_color);
        }
    }

    public override void updateStats(ShipLogic ship)
    {
        m_fireRate = 1 / (m_baseFireRate * ship.fireRate);
        m_power = ship.power * m_powerMultiplier;


        int index = indexOfTypeOf(this, ship.modifiers);
        int count = countItemOfType<WeaponSimple>(ship.modifiers);

        m_rotation = -(count - 1) * m_rotationPerLevel / 2 + m_rotationPerLevel * index;

        m_delayToNextProjectile = 0;
    }
}
