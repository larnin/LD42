using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ShipLogic : SerializedMonoBehaviour
{
    [SerializeField] int m_baseLife = 3;
    [SerializeField] int m_baseSpeed = 3;
    [SerializeField] int m_baseFireRate = 3;
    [SerializeField] int m_basePower = 3;
    [SerializeField] GameObject m_deathPrefab;

    public bool fire { get; set; }

    int m_maxLife = 0;
    int m_life = 0;
    int m_speed = 0;
    int m_fireRate = 0;
    int m_power = 0;

    public int maxLife { get { return m_maxLife; } }
    public int life { get { return m_life; } set { m_life = value; } }
    public int speed { get { return m_speed; } }
    public int fireRate { get { return m_fireRate; } }
    public int power { get { return m_power; } }

    public List<ModifierBase> modifiers = new List<ModifierBase>();

    public void updateModifierStats()
    {
        int oldMaxLife = m_maxLife;

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

        int addedLife = life - oldMaxLife;
        if (addedLife > 0)
            m_life += addedLife;
        m_maxLife = life;
        if (m_life > m_maxLife)
            m_life = m_maxLife;

        m_speed = speed;
        m_fireRate = fireRate;
        m_power = power;

        foreach (var m in modifiers)
            m.updateStats(this);
    }

    private void Awake()
    {
        m_life = 1000000;
        updateModifierStats();
    }

    private void Update()
    {
        if (GameInfos.paused)
            return;

        foreach (var m in modifiers)
            m.update(this);
    }

    private void OnDestroy()
    {
        var deathPrefab = m_deathPrefab;
        var pos = transform.position;
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (deathPrefab == null)
                return;

            var obj = Instantiate(deathPrefab);
            obj.transform.position = pos;
        });
    }
}
