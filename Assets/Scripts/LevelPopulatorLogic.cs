using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;
using System;

public class LevelPopulatorLogic : MonoBehaviour
{
    [Serializable]
    class EnemyInfo
    {
        public float baseWeight;
        public float levelWeight;
        public GameObject enemyPrefab;
        public float levelLife;
        public float levelSpeed;
        public float levelFireRate;
        public float levelPower;
    }

    [SerializeField] List<EnemyInfo> m_enemy;
    [SerializeField] int m_enemyCountBase = 5;
    [SerializeField] float m_minEnemyCountPerLevel = 1;
    [SerializeField] float m_maxEnemyCountPerLevel = 2;
    [SerializeField] float m_spawnRadius;
    [SerializeField] float m_dontSpawnRadius;
    [SerializeField] GameObject m_exitPrefab;

    private void Awake()
    {
        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();

        int nb = m_enemyCountBase + new UniformIntDistribution((int)(m_minEnemyCountPerLevel * GameInfos.level), (int)(m_maxEnemyCountPerLevel * GameInfos.level) + 1).Next(rand);

        for(int i = 0; i < nb; i++)
        {
            Vector2 pos = Vector2.zero;
            for(int j = 0; j < 10; j++)
            {
                pos = new UniformVector2SquareDistribution(-m_spawnRadius, m_spawnRadius, -m_spawnRadius, m_spawnRadius).Next(rand);
                if (pos.sqrMagnitude < m_dontSpawnRadius * m_dontSpawnRadius)
                    continue;
                break;
            }

            List<float> weights = new List<float>();
            foreach (var en in m_enemy)
                weights.Add(en.baseWeight + en.levelWeight * GameInfos.level);

            var index = new DiscreteDistribution(weights).Next(rand);
            var e = m_enemy[index];
            var mob = Instantiate(e.enemyPrefab);
            mob.transform.position = new Vector3(pos.x, pos.y, -1);

            Modifier m = new Modifier();
            m.life = (int)e.levelLife;
            m.speed = (int)e.levelSpeed;
            m.fireRate = (int)e.levelFireRate;
            m.power = (int)e.levelPower;

            var s = mob.GetComponent<ShipLogic>();
            s.modifiers.Add(m);
            s.updateModifierStats();
        }

        spawnExit();
    }

    void spawnExit()
    {
        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();

        Vector2 pos = Vector2.zero;
        for (int i = 0; i < 10; i++)
        {
            pos = new UniformVector2SquareDistribution(-m_spawnRadius, m_spawnRadius, -m_spawnRadius, m_spawnRadius).Next(rand);
            if (pos.sqrMagnitude < m_dontSpawnRadius * m_dontSpawnRadius)
                continue;
            break;
        }

        var mob = Instantiate(m_exitPrefab);
        mob.transform.position = new Vector3(pos.x, pos.y, -1);
        mob.name = m_exitPrefab.name;
    }
}
