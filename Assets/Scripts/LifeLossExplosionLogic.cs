using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LifeLossExplosionLogic : MonoBehaviour
{
    [SerializeField] Ease m_ease;
    [SerializeField] Ease m_colorEase;

    public float duration;
    public float speed;

    SpriteRenderer m_renderer;
    float m_currentTime = 0;
    
    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        m_currentTime += Time.deltaTime;
        if (m_currentTime > duration)
            Destroy(gameObject);

        float s = DOVirtual.EasedValue(0, 1, m_currentTime / duration, m_ease) * speed;
        float a = DOVirtual.EasedValue(0, 1, m_currentTime / duration, m_colorEase);

        transform.localScale = new Vector3(s, s, s);
        Color c = m_renderer.color;
        c.a = 1-a;
        m_renderer.color = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var projectile = collision.GetComponent<ProjectileDataLogic>();
        if (projectile == null)
            return;
        if (projectile.sender == null || projectile.sender.GetComponent<AIShipControlerLogic>() != null)
            Destroy(collision.gameObject);
    }
}
