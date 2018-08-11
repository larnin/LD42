using UnityEngine;
using System.Collections;

public class PlayerShipControlerLogic : MonoBehaviour
{
    const string horizontalAxis = "Horizontal";
    const string verticalAxis = "Vertical";
    const string joysticFireXAxis = "JoyFireX";
    const string joysticFireYAxis = "JoyFireY";

    [SerializeField] float m_speedMultiplier;
    [SerializeField] float m_acceleration = 5;
    [SerializeField] float m_deadZone = 0.1f;
    [SerializeField] float m_rotSpeed = 100;

    ShipLogic m_ship;
    ParticleSystem m_particleSystem;

    Vector2 m_speed;
    float m_orientation;

    private void Awake()
    {
        m_ship = GetComponent<ShipLogic>();
        m_particleSystem = GetComponentInChildren<ParticleSystem>();
        m_particleSystem.Stop();
    }
    
    void Update()
    {
        if (GameInfos.paused)
            return;

        Vector2 dir = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));

        if (dir.sqrMagnitude < m_deadZone * m_deadZone)
            dir = Vector2.zero;

        Vector2 targetSpeed = m_ship.speed * m_speedMultiplier * dir;
        float acceleration = m_acceleration * m_speedMultiplier;

        if(targetSpeed.x > m_speed.x)
        {
            m_speed.x += acceleration * Time.deltaTime;
            if (m_speed.x > targetSpeed.x)
                m_speed.x = targetSpeed.x;
        }
        if(targetSpeed.x < m_speed.x)
        {
            m_speed.x -= acceleration * Time.deltaTime;
            if (m_speed.x < targetSpeed.x)
                m_speed.x = targetSpeed.x;
        }

        if (targetSpeed.y > m_speed.y)
        {
            m_speed.y += acceleration * Time.deltaTime;
            if (m_speed.y > targetSpeed.y)
                m_speed.y = targetSpeed.y;
        }
        if (targetSpeed.y < m_speed.y)
        {
            m_speed.y -= acceleration * Time.deltaTime;
            if (m_speed.y < targetSpeed.y)
                m_speed.y = targetSpeed.y;
        }

        var dSpeed = m_speed * Time.deltaTime;

        transform.position = transform.position + new Vector3(dSpeed.x, dSpeed.y, 0);

        Vector2 fireDir = new Vector2(Input.GetAxisRaw(joysticFireXAxis), -Input.GetAxisRaw(joysticFireYAxis));
        if (fireDir.sqrMagnitude < m_deadZone * m_deadZone)
            fireDir = Vector2.zero;

        var targetAngle = m_orientation;
        if (fireDir.sqrMagnitude > 0)
            targetAngle = Vector2.SignedAngle(new Vector2(1, 0), fireDir);
        else if(dir.sqrMagnitude > 0)
            targetAngle = Vector2.SignedAngle(new Vector2(1, 0), dir);

        var offsetAngle = targetAngle - m_orientation;
        while (offsetAngle < -180)
            offsetAngle += 360;
        while (offsetAngle > 180)
            offsetAngle -= 360;

        if (offsetAngle < 0)
            m_orientation -= Mathf.Min(m_rotSpeed * Time.deltaTime, -offsetAngle);
        else if (offsetAngle > 0)
            m_orientation += Mathf.Min(m_rotSpeed * Time.deltaTime, offsetAngle);

        while (m_orientation < -180)
            m_orientation += 360;
        while (m_orientation > 180)
            m_orientation -= 360;

        transform.rotation = Quaternion.Euler(0, 0, m_orientation);

        if (dir.sqrMagnitude < 0.1 * 0.1)
        {
            if(m_particleSystem.isEmitting)
                m_particleSystem.Stop();
        }
        else if(!m_particleSystem.isEmitting)
            m_particleSystem.Play();

        m_ship.fire = fireDir.sqrMagnitude > 0.1 * 0.1;
    }
}
