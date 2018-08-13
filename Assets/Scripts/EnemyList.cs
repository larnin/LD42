using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class EnemyList
{
    static List<AIShipControlerLogic> m_enemies = new List<AIShipControlerLogic>();

    public static void add(AIShipControlerLogic e)
    {
        m_enemies.Add(e);
    }

    public static void remove(AIShipControlerLogic e)
    {
        m_enemies.Remove(e);
    }

    public static AIShipControlerLogic getNearest(Vector3 pos)
    {
        float bestDist = float.MaxValue;
        AIShipControlerLogic best = null;

        foreach(var e in m_enemies)
        {
            var d = (pos - e.transform.position).sqrMagnitude;
            if(d < bestDist)
            {
                bestDist = d;
                best = e;
            }
        }

        return best;
    }
}
