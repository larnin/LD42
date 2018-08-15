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
    [SerializeField] GameObject m_bossPrefab;
    [SerializeField] int m_bossLevel = 9;

    private void Awake()
    {
        if((GameInfos.level + 1) % m_bossLevel == 0)
        {
            spawnBoss();
            return;
        }

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
            m.life = (int)(e.levelLife * GameInfos.level);
            m.speed = (int)(e.levelSpeed * GameInfos.level);
            m.fireRate = (int)(e.levelFireRate * GameInfos.level);
            m.power = (int)(e.levelPower * GameInfos.level);

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

    void spawnBoss()
    {
        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();
        for (int i = 0; i < GameInfos.bossKillCount + 1; i++)
        {
            Vector2 pos = Vector2.zero;
            for (int j = 0; j < 10; j++)
            {
                pos = new UniformVector2SquareDistribution(-m_spawnRadius, m_spawnRadius, -m_spawnRadius, m_spawnRadius).Next(rand);
                if (pos.sqrMagnitude < m_dontSpawnRadius * m_dontSpawnRadius)
                    continue;
                break;
            }

            var mob = Instantiate(m_bossPrefab);
            mob.transform.position = new Vector3(pos.x, pos.y, -1);
        }

        var exit = Instantiate(m_exitPrefab);
        exit.transform.position = new Vector3(0, 200, -1);
        exit.name = m_exitPrefab.name;
    }
}
