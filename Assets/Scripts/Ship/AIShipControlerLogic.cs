using UnityEngine;
using System.Collections;

public class AIShipControlerLogic : MonoBehaviour
{
    ShipLogic m_ship;
    LifebarLogic m_lifebar;


    private void Awake()
    {
        m_ship = GetComponent<ShipLogic>();
        m_lifebar = GetComponentInChildren<LifebarLogic>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerProjectile(collision.GetComponent<ProjectileDataLogic>());
    }

    void onTriggerProjectile(ProjectileDataLogic p)
    {
        if (p == null || p.sender.GetComponent<PlayerShipControlerLogic>() == null)
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

        Destroy(gameObject);
    }
}
