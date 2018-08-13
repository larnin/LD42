using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DashAIShipControler : AIShipControlerLogic
{
    enum State
    {
        Idle,
        Targeting,
        Dashing,
    }

    [SerializeField] float m_detectionRadius = 5;
    [SerializeField] float m_baseRotationSpeed;
    [SerializeField] float m_targetedRotationSpeed;
    [SerializeField] float m_baseTargetingSpeed;
    [SerializeField] float m_targetingFixedDelay;
    [SerializeField] float m_baseDelayBetweenDash;
    [SerializeField] float m_baseDelayAfterDashing;
    [SerializeField] float m_dashRange;
    [SerializeField] float m_dashSpeed;
    [SerializeField] int m_level;

    Transform m_target;
    State m_state = State.Idle;
    bool m_targeted = false;

    float m_coldown = 0;

    ParticleSystem m_targetingParticle;

    float m_targetingTime;

    float m_dashCount;
    float m_durationToNextDash;

    protected override void onStart()
    {
        m_target = GameObject.FindObjectOfType<PlayerShipControlerLogic>().transform;
        m_targetingParticle = transform.Find("TargetParticle").GetComponentInChildren<ParticleSystem>();
        m_targetingParticle.Stop();
        m_ship.fire = false;
    }

    protected override void onUpdate()
    {
        if (m_state == State.Idle)
            onIdle();
        else if (m_state == State.Dashing)
            onDash();
        else if (m_state == State.Targeting)
            onTargeting();
    }

    void onIdle()
    {
        m_coldown -= Time.deltaTime;
        
        if(m_targeted)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = new Vector2(m_target.transform.position.x, m_target.transform.position.y);
            float angle = Vector2.SignedAngle(new Vector2(1, 0), targetPos - pos) - Vector2.SignedAngle(new Vector2(1, 0), transform.right);
            float dAngle = (angle > 0 ? 1 : -1) * Time.deltaTime * m_targetedRotationSpeed * m_ship.speed;
            if (Mathf.Abs(dAngle) > Mathf.Abs(angle))
            {
                m_state = State.Targeting;
                m_targetingTime = m_targetingFixedDelay + 1 / (m_baseTargetingSpeed * m_ship.speed);
                m_targetingParticle.Play();
                dAngle = angle;
            }
            float currentAngle = transform.rotation.eulerAngles.z + dAngle;
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
        else if(m_coldown < 0 && (m_target.transform.position - transform.position).sqrMagnitude < m_detectionRadius * m_detectionRadius)
            m_targeted = true;
        else
        {
            m_targeted = false;
            float rot = transform.rotation.eulerAngles.z;
            rot += m_baseRotationSpeed * m_ship.speed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, rot);
        }
    }

    void onTargeting()
    {
        m_targetingTime -= Time.deltaTime;
        if (m_targetingTime < m_targetingFixedDelay && m_targetingParticle.isEmitting)
            m_targetingParticle.Stop();

        if (m_targetingTime > 1 / (m_baseDelayBetweenDash * m_ship.speed))
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = new Vector2(m_target.transform.position.x, m_target.transform.position.y);
            float angle = Vector2.SignedAngle(new Vector2(1, 0), targetPos - pos);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if(m_targetingTime < 0)
        {
            m_state = State.Dashing;
            m_dashCount = 0;
            m_durationToNextDash = 0;
        }
    }

    void onDash()
    {
        m_durationToNextDash -= Time.deltaTime;

        m_ship.fire = m_durationToNextDash > 0 && m_durationToNextDash < 1 / (m_dashSpeed * m_ship.speed);

        if(m_durationToNextDash < 0)
        {
            if(m_dashCount >= m_level)
            {
                m_state = State.Idle;
                m_coldown = 1 / (m_baseDelayAfterDashing * m_ship.speed);
                m_targeted = false;
                return;
            }

            m_dashCount++;
            float dashTime = 1 / (m_dashSpeed * m_ship.speed);
            m_durationToNextDash = dashTime + 1 / (m_baseDelayBetweenDash * m_ship.speed);

            transform.DOMove(transform.position + transform.right * m_dashRange, dashTime).SetEase(Ease.Linear);

            DOVirtual.DelayedCall(dashTime, () =>
            {
                if (this == null)
                    return;
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                Vector2 targetPos = new Vector2(m_target.transform.position.x, m_target.transform.position.y);
                float angle = Vector2.SignedAngle(new Vector2(1, 0), targetPos - pos);
                transform.DORotate(new Vector3(0, 0, angle), 0.2f);
            });
        }
        
    }
}
