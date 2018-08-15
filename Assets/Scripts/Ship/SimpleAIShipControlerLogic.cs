using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public  class SimpleAIShipControlerLogic : AIShipControlerLogic
{
    [SerializeField] float m_detectionRadius;
    [SerializeField] float m_baseMoveSpeed;
    [SerializeField] float m_baseRotationSpeed;

    Transform m_target;
    float m_agroTime;

    protected override void onStart()
    {
        m_target = GameObject.FindObjectOfType<PlayerShipControlerLogic>().transform;
        m_ship.fire = true;
    }

    protected override void onUpdate()
    {
        m_agroTime -= Time.deltaTime;
        float rot = transform.rotation.eulerAngles.z;
        rot += m_baseRotationSpeed * m_ship.speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rot);

        if (m_target == null)
            return;
        if((m_target.transform.position - transform.position).sqrMagnitude < m_detectionRadius * m_detectionRadius || m_agroTime > 0)
        {
            var dir = (m_target.transform.position - transform.position).normalized;
            dir *= m_baseMoveSpeed * m_ship.speed * Time.deltaTime;
            transform.position += dir;
        }
    }

    protected override void onDamage()
    {
        m_agroTime = m_damageAgroTime;
    }
}
