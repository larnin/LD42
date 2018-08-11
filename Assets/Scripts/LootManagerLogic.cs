using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NRand;

public class LootManagerLogic : MonoBehaviour
{
    static LootManagerLogic m_instance;
    public static LootManagerLogic instance { get { return m_instance; } }

    [Serializable]
    public class LootInfo
    {
        public float fixedWeight;
        public float levelWeight;
        public GameObject loot;
    }

    [SerializeField] float m_noLootProbability;
    [SerializeField] List<LootInfo> m_loots;

    void Awake()
    {
        m_instance = this;
    }

    public GameObject getRandomLoot()
    {
        List<float> weights = new List<float>();
        weights.Add(m_noLootProbability);
        foreach (var l in m_loots)
            weights.Add(l.fixedWeight + l.levelWeight * GameInfos.level);
        var d = new DiscreteDistribution(weights).Next(new StaticRandomGenerator<DefaultRandomGenerator>());

        if (d == 0)
            return null; //no loot
        return m_loots[d - 1].loot;
    }
}
