using UnityEngine;
using System.Collections;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField] float m_timeScale;
    [SerializeField] float m_power;

    float m_z;

    private void Awake()
    {
        m_z = transform.position.z;    
    }

    void Update()
    {
        float t = Time.time * m_timeScale;
        transform.position = new Vector3(Mathf.Cos(5 * t) + 0.5f * Mathf.Sin(3 * t) - 0.3f * Mathf.Cos(7 * t), Mathf.Cos(4 * t) + 0.6f * Mathf.Sin(5 * t) - Mathf.Sin(9 * t), m_z) * m_power;
    }
}
