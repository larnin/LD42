using UnityEngine;
using System.Collections;

public class LifebarLogic : MonoBehaviour
{
    [SerializeField] float m_barFullSize = 2;
    [SerializeField] float m_showTime = 2;

    SpriteRenderer m_sprite;
    float m_time;
    Vector3 m_offset;

    private void Awake()
    {
        m_sprite = GetComponent<SpriteRenderer>();
        m_sprite.size = new Vector2(m_barFullSize, m_sprite.size.y);
        m_sprite.enabled = false;
        if (transform.parent != null)
            m_offset = transform.position - transform.parent.position;
    }

    public void show(int life, int maxLife)
    {
        float value = (float)life / maxLife;
        m_sprite.size = new Vector2(m_barFullSize * value, m_sprite.size.y);
        m_sprite.enabled = true;
        m_time = m_showTime;
    }

    private void LateUpdate()
    {
        m_time -= Time.deltaTime;
        if (m_sprite.enabled && m_time < 0)
            m_sprite.enabled = false;

        if(transform.parent != null)
        {
            transform.position = transform.parent.position + m_offset;
            transform.rotation = Quaternion.identity;
        }
    }
}
