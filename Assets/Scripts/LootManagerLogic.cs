using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NRand;

public class LootManagerLogic : MonoBehaviour
{
    static LootManagerLogic m_instance;
    public static LootManagerLogic instance { get { return m_instance; } }

    List<LootLogic> m_lootsInstance = new List<LootLogic>();
    public void add(LootLogic loot)
    {
        if(this != null)
            m_lootsInstance.Add(loot);
    }

    public void remove(LootLogic loot)
    {
        if(this != null)
            m_lootsInstance.Remove(loot);
    }

    public LootLogic getNearestLootInRadius(Vector3 pos, float radius)
    {
        LootLogic best = null;
        float bestDist = float.MaxValue;

        foreach (var l in m_lootsInstance)
        {
            float d = (pos - l.transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                best = l;
                bestDist = d;
            }
        }
        if (bestDist <= radius * radius)
            return best;
        return null;
    }

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
