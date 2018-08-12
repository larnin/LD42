using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using NRand;

public enum CameraEffectType
{
    None,
    Shake,
}

public class CameraLogic : SerializedMonoBehaviour
{
    [SerializeField] GameObject m_target;
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_speedPower = 1.2f;
    [SerializeField] float m_maxCapedSpeed = 10.0f;
    [SerializeField] Dictionary<CameraEffectType, BaseCameraEffect> m_effects;

    CameraEffectType m_currentEffect;
    float m_effectTime;
    float m_effectPower;

    Vector3 pos;
    float vOffset;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Start()
    {
        pos = transform.position;
        vOffset = m_target.transform.position.z - transform.position.z;
    }

    void LateUpdate()
    {
        var targetPos = m_target.transform.position;
        targetPos.z -= vOffset;

        var dir = targetPos - pos;
        var dirNorm = dir.magnitude;
        if (dirNorm > 0.01f)
        {

            var speed = Mathf.Min(Mathf.Pow(dirNorm * m_speed, m_speedPower), m_maxCapedSpeed) * Time.deltaTime;
            if (speed > dirNorm)
                speed = dirNorm;
            dir *= speed / dirNorm;
            pos = pos + dir;
        }
        transform.position = pos + m_effects[m_currentEffect].pos(m_effectPower);

        m_effectTime -= Time.deltaTime;
        if (m_effectTime < 0 && m_currentEffect != CameraEffectType.None)
            m_currentEffect = CameraEffectType.None;
    }

    void onPlayEffect(PlayCameraEffectEvent e)
    {
        m_currentEffect = e.effectType;
        m_effectPower = e.power;
        m_effectTime = e.time;
    }

    abstract class BaseCameraEffect
    {
        public abstract Vector3 pos(float power);
        public abstract Quaternion rot(float power);
    }

    class NoEffect : BaseCameraEffect
    {
        public override Vector3 pos(float power)
        {
            return Vector3.zero;
        }

        public override Quaternion rot(float power)
        {
            return Quaternion.identity;
        }
    }

    class ShakeCameraEffect : BaseCameraEffect
    {
        public override Vector3 pos(float power)
        {
            return new UniformVector3SphereDistribution(power).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
        }

        public override Quaternion rot(float power)
        {
            return Quaternion.identity;
        }
    }
}
