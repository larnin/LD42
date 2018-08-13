using UnityEngine;
using System.Collections;
using NRand;

public class BossAIShipControler : AIShipControlerLogic
{
    [SerializeField] BossAIShipControler m_parentNode;
    [SerializeField] float m_baseSpeed;
    [SerializeField] float m_minSpeed;
    [SerializeField] float m_baseAcceleration;
    [SerializeField] float m_tailDistance;
    [SerializeField] float m_randomRadius = 40;
    [SerializeField] float m_validRadius = 5;
    [SerializeField] float m_delayNextPoint = 5;

    SpriteRenderer m_headRenderer;
    SpriteRenderer m_tailRenderer;

    Transform m_target;

    Vector2 m_speed;
    Vector3 m_targetPosition;
    bool m_wasRandomPoint = false;
    float m_timeToNextPoint = 0;

    public BossAIShipControler nextSegment;

    StaticRandomGenerator<DefaultRandomGenerator> m_rand = new StaticRandomGenerator<DefaultRandomGenerator>();
    UniformVector2SquareDistribution m_d;

    protected override void onStart()
    {
        m_target = GameObject.FindObjectOfType<PlayerShipControlerLogic>().transform;

        m_headRenderer = transform.Find("Head").GetComponent<SpriteRenderer>();
        m_tailRenderer = transform.Find("Tail").GetComponent<SpriteRenderer>();
        m_d = new UniformVector2SquareDistribution(-m_randomRadius, m_randomRadius, -m_randomRadius, m_randomRadius);

        if (m_parentNode != null)
            m_parentNode.nextSegment = this;
    }

    protected override void onUpdate()
    {
        m_ship.fire = m_parentNode != null;

        if (m_parentNode == null)
            execHead();
        else execTail();
    }

    void execHead()
    {
        m_headRenderer.gameObject.SetActive(true);
        m_tailRenderer.gameObject.SetActive(false);

        m_timeToNextPoint -= Time.deltaTime;
        if (m_timeToNextPoint < 0)
            selectNextPoint();

        if ((transform.position - m_targetPosition).sqrMagnitude < m_validRadius * m_validRadius)
            selectNextPoint();

        var dir = (m_targetPosition - transform.position).normalized * Time.deltaTime * m_baseAcceleration;

        m_speed += new Vector2(dir.x, dir.y);
        float speed = m_speed.magnitude;
        if(speed > m_baseSpeed)
        {
            m_speed /= speed;
            m_speed *= m_baseSpeed;
        }
        if(speed < m_minSpeed && speed > 0)
        {
            m_speed /= speed;
            m_speed *= m_minSpeed;
        }
        var speedDt = m_speed * Time.deltaTime;

        transform.position += new Vector3(speedDt.x, speedDt.y, 0);
        float angle = Vector2.SignedAngle(new Vector2(1, 0), m_speed);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (nextSegment == null)
            kill();
    }

    void execTail()
    {
        m_headRenderer.gameObject.SetActive(false);
        m_tailRenderer.gameObject.SetActive(true);

        var dir = m_parentNode.transform.position - transform.position ;
        float d = dir.magnitude;
        dir /= d;

        if (d < m_tailDistance)
            return;
        

        float angle = Vector2.SignedAngle(new Vector2(1, 0), new Vector2(dir.x, dir.y));
        transform.rotation = Quaternion.Euler(0, 0, angle);
        d -= m_tailDistance;
        transform.position += dir * d;
    }

    void selectNextPoint()
    {
        if(m_wasRandomPoint)
        {
            m_targetPosition = m_target.transform.position;
        }
        else
        {
            var pos = m_d.Next(m_rand);
            m_targetPosition = new Vector3(pos.x, pos.y, 0);
        }

        m_wasRandomPoint = !m_wasRandomPoint;
        m_timeToNextPoint = m_delayNextPoint;
    }
}
