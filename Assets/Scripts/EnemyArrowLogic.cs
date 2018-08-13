using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemyArrowLogic : MonoBehaviour
{
    [SerializeField] Color m_targetEnemyColor;
    [SerializeField] Color m_targetExitColor;

    SpriteRenderer m_renderer;
    Transform m_target;

    private void Awake()
    {
        m_renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        var target = EnemyList.getNearest(transform.position);

        if (target != null)
        {
            m_target = target.transform;
            m_renderer.color = m_targetEnemyColor;
        }
        else if (m_target == null)
        {
            m_target = GameObject.Find("Exit").transform;
            m_renderer.color = m_targetExitColor;
        }

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos = new Vector2(m_target.transform.position.x, m_target.transform.position.y);
        float angle = Vector2.SignedAngle(new Vector2(1, 0), targetPos - pos);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
