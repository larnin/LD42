using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayCameraEffectEvent : EventArgs
{
    public PlayCameraEffectEvent(CameraEffectType _effectType, float _power, float _time)
    {
        effectType = _effectType;
        power = _power;
        time = _time;
    }

    public CameraEffectType effectType;
    public float power;
    public float time;
}

