using UnityEngine;
using System.Collections;
using System;

[Serializable]
public abstract class ModifierBase
{
    public int life;
    public int speed;
    public int fireRate;
    public int power;

    public virtual void update(ShipLogic ship) { }
    public virtual void updateStats(ShipLogic ship) { }
}

public class Modifier : ModifierBase
{

}

