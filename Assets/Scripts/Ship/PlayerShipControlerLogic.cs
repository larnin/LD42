using UnityEngine;
using System.Collections;

public class PlayerShipControlerLogic : MonoBehaviour
{
    const string horizontalAxis = "Horizontal";
    const string verticalAxis = "Vertical";
    const string joysticFireXAxis = "JoyFireX";
    const string joysticFireYAxis = "JoyFireY";
    const string mouseXAxis = "MouseX";
    const string mouseYAxis = "MouseY";
    const string mouseButton = "MouseFire";

    [SerializeField] int m_baseSpeed;
    [SerializeField] float m_speedMultiplier;
    [SerializeField] float m_acceleration = 5;
    [SerializeField] float m_deadZone = 0.1f;
    [SerializeField] float m_rotSpeed = 100;
    [SerializeField] float m_invincibilityDuration = 1.0f;
    [SerializeField] float m_invincibilityBlinkSpeed = 5;
    [SerializeField] AudioClip m_takingDamageClip;
    [SerializeField] AudioClip m_deathClip;
    [SerializeField] GameObject m_contactPrefab;
    [SerializeField] float m_lifeLossRadius;
    [SerializeField] float m_lifeLossTime;
    [SerializeField] float m_lifeLossShakeTime;
    [SerializeField] float m_lifeLossShakePower;
    [SerializeField] Color m_lifeLossColor;
    [SerializeField] float m_deathRadius;
    [SerializeField] float m_deathTime;
    [SerializeField] float m_deathShakeTime;
    [SerializeField] float m_deathShakePower;
    [SerializeField] Color m_deathColor;
    [SerializeField] float m_cursorSpeed;
    [SerializeField] float m_maxCursorDistance;
    [SerializeField] float m_cursorHideDelay;

    ShipLogic m_ship;
    ParticleSystem m_particleSystem;
    SpriteRenderer m_renderer;
    Transform m_cursor;

    Vector2 m_speed;
    float m_orientation;
    float m_invincibilityTime;
    float m_timeFromLastCursorMove;
    Vector2 m_cursorOffset;

    private void Awake()
    {
        m_ship = GetComponent<ShipLogic>();
        m_particleSystem = GetComponentInChildren<ParticleSystem>();
        m_particleSystem.Stop();
        m_renderer = GetComponent<SpriteRenderer>();
        m_cursor = transform.Find("Cursor");
        m_timeFromLastCursorMove = m_cursorHideDelay;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        if(GameInfos.modifiers != null)
        {
            m_ship.modifiers = GameInfos.modifiers;
            m_ship.updateModifierStats();
            m_ship.life = GameInfos.life;
            if (m_ship.life > m_ship.maxLife)
                m_ship.life = m_ship.maxLife;
        }
        Event<UpdateUIEvent>.Broadcast(new UpdateUIEvent(m_ship));
    }

    void Update()
    {
        if (GameInfos.paused)
            return;

        Vector2 dir = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));

        if (dir.sqrMagnitude < m_deadZone * m_deadZone)
            dir = Vector2.zero;

        Vector2 targetSpeed = (m_ship.speed + m_baseSpeed) * m_speedMultiplier * dir;
        float acceleration = (m_ship.speed + m_baseSpeed ) * m_acceleration;

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
        {
            m_timeFromLastCursorMove = m_cursorHideDelay;
            targetAngle = Vector2.SignedAngle(new Vector2(1, 0), fireDir);
        }
        else if(m_timeFromLastCursorMove < m_cursorHideDelay)
        {
            targetAngle = Vector2.SignedAngle(new Vector2(1, 0), m_cursorOffset);
        }
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

        m_ship.fire = fireDir.sqrMagnitude > 0.1 * 0.1 || Input.GetButton(mouseButton);

        m_invincibilityTime -= Time.deltaTime;
        Color c = m_renderer.color;
        if (m_invincibilityTime > 0)
        {
            m_renderer.color = new Color(c.r, c.g, c.b, Mathf.Sin(m_invincibilityTime * m_invincibilityBlinkSpeed) / 3 + 0.66f);
        }
        else m_renderer.color = new Color(c.r, c.g, c.b);

        updateMouseCursor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onProjectileCollide(collision.GetComponent<ProjectileDataLogic>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onShipCollide(collision.gameObject.GetComponent<AIShipControlerLogic>());
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        m_speed = Vector2.zero;
    }

    void onProjectileCollide(ProjectileDataLogic p)
    {
        if (p == null || p.sender == gameObject)
            return;

        int power = p.power;

        Destroy(p.gameObject);

        damage(p.power);
    }

    void onShipCollide(AIShipControlerLogic s)
    {
        if (s == null)
            return;

        if (m_invincibilityTime > 0)
            return;

        s.kill();
        
        damage(1);
    }

    void damage(int power)
    {
        if (m_invincibilityTime > 0)
            return;

        m_ship.life -= power;
        Event<UpdateUIEvent>.Broadcast(new UpdateUIEvent(m_ship));

        m_invincibilityTime = m_invincibilityDuration;

        if (m_ship.life <= 0)
            ondeath();
        else
        {
            SoundSystem.instance.play(m_takingDamageClip, 0.8f);
            instanciateDmg(false);
        }
    }

    void ondeath()
    {
        SoundSystem.instance.play(m_deathClip, 0.8f);
        Event<DieEvent>.Broadcast(new DieEvent());
        instanciateDmg(true);
        Destroy(gameObject);
    }

    void instanciateDmg(bool death)
    {
        var obj = Instantiate(m_contactPrefab);
        obj.transform.position = transform.position;
        var comp = obj.GetComponent<LifeLossExplosionLogic>();
        var renderer = obj.GetComponent<SpriteRenderer>();
        if(death)
        {
            comp.speed = m_deathRadius;
            comp.duration = m_deathTime;
            renderer.color = m_deathColor;
            Event<PlayCameraEffectEvent>.Broadcast(new PlayCameraEffectEvent(CameraEffectType.Shake, m_deathShakePower, m_deathShakeTime));
        }
        else
        {
            comp.speed = m_lifeLossRadius;
            comp.duration = m_lifeLossTime;
            renderer.color = m_lifeLossColor;
            Event<PlayCameraEffectEvent>.Broadcast(new PlayCameraEffectEvent(CameraEffectType.Shake, m_lifeLossShakePower, m_lifeLossShakeTime));
        }
    }

    void updateMouseCursor()
    {
        m_timeFromLastCursorMove += Time.deltaTime;
        m_cursor.gameObject.SetActive(m_timeFromLastCursorMove <= m_cursorHideDelay);

        Vector2 offset = new Vector2(Input.GetAxisRaw(mouseXAxis), Input.GetAxisRaw(mouseYAxis));

        if(Mathf.Abs(offset.x) > 0 || Mathf.Abs(offset.y) > 0 || Input.GetButton(mouseButton))
            m_timeFromLastCursorMove = 0;

        offset *= m_cursorSpeed;
        m_cursorOffset += offset;
        float offsetLenght = m_cursorOffset.magnitude;
        if (offsetLenght > m_maxCursorDistance)
            m_cursorOffset *= m_maxCursorDistance / offsetLenght;

        m_cursor.transform.position = transform.position + new Vector3(m_cursorOffset.x, m_cursorOffset.y, -0.1f);
    }
}
