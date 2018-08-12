using UnityEngine;
using System.Collections;

public class AIShipControlerLogic : MonoBehaviour
{
    protected ShipLogic m_ship;
    protected LifebarLogic m_lifebar;


    private void Awake()
    {
        m_ship = GetComponent<ShipLogic>();
        m_lifebar = GetComponentInChildren<LifebarLogic>();

        onAwake();
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
            onKill();
    }

    void onKill()
    {
        var loot = LootManagerLogic.instance.getRandomLoot();
        if(loot != null)
            Instantiate(loot, transform.position, Quaternion.identity);

        GameInfos.killCount++;

        Destroy(gameObject);
    }
}
