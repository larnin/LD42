using UnityEngine;
using System.Collections;

public class AIShipControlerLogic : MonoBehaviour
{
    [SerializeField] AudioClip m_deathClip;
    [SerializeField] AudioClip m_damageClip;
    [SerializeField] protected float m_damageAgroTime = 2;

    protected ShipLogic m_ship;
    protected LifebarLogic m_lifebar;
    bool m_killed = false;

    private void Awake()
    {
        m_ship = GetComponent<ShipLogic>();
        m_lifebar = GetComponentInChildren<LifebarLogic>();

        onAwake();

        EnemyList.add(this);
    }
    protected virtual void onAwake() { }

    private void Start()
    {
        onStart();
    }
    protected virtual void onStart() { }

    void Update()
    {
        if (GameInfos.paused)
            return;

        onUpdate();
    }
    protected virtual void onUpdate() { }

    private void OnDestroy()
    {
        EnemyList.remove(this);
        onDestroy();
    }
    protected virtual void onDestroy() { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerProjectile(collision.GetComponent<ProjectileDataLogic>());
    }

    void onTriggerProjectile(ProjectileDataLogic p)
    {
        if (p == null || this == null || p.sender == null || p.sender.GetComponent<PlayerShipControlerLogic>() == null)
            return;

        m_ship.life -= p.power;
        m_lifebar.show(m_ship.life, m_ship.maxLife);

        Destroy(p.gameObject);

        if (m_ship.life <= 0)
            onKill(true);
        else
        {
            SoundSystem.instance.play(m_damageClip, 0.15f);
            onDamage();
        }
    }
    protected virtual void onDamage() { }

    public void kill()
    {
        onKill(false);
    }

    void onKill(bool dropLoot)
    {
        if (m_killed)
            return;
        m_killed = true;

        if (dropLoot)
        {
            var loot = LootManagerLogic.instance.getRandomLoot();
            if (loot != null)
                Instantiate(loot, transform.position, Quaternion.identity);

            GameInfos.killCount++;
        }

        SoundSystem.instance.play(m_deathClip, 0.2f);

        Destroy(gameObject);
    }
}
