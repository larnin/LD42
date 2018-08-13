using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class WeaponBase : ModifierBase
{
    [SerializeField] protected AudioClip m_shootClip;
    [SerializeField] protected AudioClip m_shootClip2;

    public void fire(GameObject projectile, GameObject sender, Vector3 offset, float rot, int power, float speed, float life, Color color)
    {
        var obj = GameObject.Instantiate(projectile);

        var pos = sender.transform.position;
        pos += sender.transform.TransformDirection(offset);
        obj.transform.position = pos;

        var senderRot = sender.transform.rotation.eulerAngles.z;
        obj.transform.rotation = Quaternion.Euler(0, 0, senderRot + rot);

        var comp = obj.GetComponent<ProjectileDataLogic>();
        if (comp != null)
        {
            comp.sender = sender;
            comp.power = power;
            comp.speed = speed;
            comp.life = life;
        }
        obj.GetComponentInChildren<Renderer>().material.color = color;
    }

    static protected int indexOf(ModifierBase item, List<ModifierBase> modifiers)
    {
        return modifiers.IndexOf(item);
    }

    static protected int countItemOfType<T>(List<ModifierBase> modifiers) where T : ModifierBase
    {
        int count = 0;
        foreach(var m in modifiers)
        {
            if (m is T)
                count++;
        }
        return count;
    }

    static protected int indexOfTypeOf<T>(T item, List<ModifierBase> modifiers) where T : ModifierBase
    {
        int count = 0;
        foreach (var m in modifiers)
        {
            if (m is T)
            {
                if (m == item)
                    return count;
                count++;
            }
        }
        return -1;
    }
}