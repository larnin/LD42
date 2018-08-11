using UnityEngine;
using System.Collections;

public class CameraLogic : MonoBehaviour
{
    [SerializeField] GameObject m_target;
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_speedPower = 1.2f;
    [SerializeField] float m_maxCapedSpeed = 10.0f;
    
    void LateUpdate()
    {
        var targetPos = m_target.transform.position;
        var pos = transform.position;
        pos.z = targetPos.z;

        var dir = targetPos - pos;
        var dirNorm = dir.magnitude;
        if (dirNorm < 0.01f)
            return;

        var speed = Mathf.Min(Mathf.Pow(dirNorm * m_speed, m_speedPower), m_maxCapedSpeed) * Time.deltaTime;
        if (speed > dirNorm)
            speed = dirNorm;
        dir *= speed / dirNorm;
        transform.position += dir;
    }
}
