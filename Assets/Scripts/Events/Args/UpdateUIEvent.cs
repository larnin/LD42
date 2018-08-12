using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UpdateUIEvent : EventArgs
{
    public UpdateUIEvent(ShipLogic _ship)
    {
        ship = _ship;
    }

    public ShipLogic ship;
}