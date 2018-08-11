using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public abstract class Weapon
{
    public abstract void update(ShipLogic ship);
    public abstract void updateStats(ShipLogic ship);
}
