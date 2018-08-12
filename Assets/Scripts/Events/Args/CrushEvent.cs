using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CrushEvent : EventArgs
{
    public CrushEvent(ModifierBase _modifier, ShipLogic _ship)
    {
        modifier = _modifier;
        ship = _ship;
    }

    public ModifierBase modifier;
    public ShipLogic ship;
}
