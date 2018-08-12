using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class ShipLogic : SerializedMonoBehaviour
{
    [SerializeField] int m_baseLife = 3;
    [SerializeField] int m_baseSpeed = 3;
    [SerializeField] int m_baseFireRate = 3;
    [SerializeField] int m_basePower = 3;

    public bool fire { get; set; }

    int m_maxLife;
    int m_life;
    int m_speed;
    int m_fireRate;
    int m_power;

    public int maxLife { get { return m_maxLife; } }
    public int life { get { return m_life; } set { m_life = value; } }
    public int speed { get { return m_speed; } }
    public int fireRate { get { return m_fireRate; } }
    public int power { get { return m_power; } }

    public List<ModifierBase> modifiers = new List<ModifierBase>();

    public void updateModifierStats()
    {
        int life = m_baseLife;
        int speed = m_baseSpeed;
        int fireRate = m_baseFireRate;
        int power = m_basePower;

        foreach (var m in modifiers)
        {
            life += m.life;
            speed += m.speed;
            fireRate += m.fireRate;
            power += m.power;
        }

        m_maxLife = life;
        if (m_life > m_maxLife)
            m_life = m_maxLife;

        m_speed = speed;
        m_fireRate = fireRate;
        m_power = power;

        foreach (var m in modifiers)
            m.updateStats(this);
    }

    private void Start()
    {
        m_life = int.MaxValue;
        updateModifierStats();
    }

    private void Update()
    {
        foreach (var m in modifiers)
            m.update(this);
    }
}
