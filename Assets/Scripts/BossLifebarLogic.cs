using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossLifebarLogic : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_spriteLife;
    [SerializeField] int m_activeLevel;
    [SerializeField] float m_hideTime = 2;
    int m_totalLife;
    
    float m_initialWidth;

    bool m_lifeAtZero = false;

    void Start()
    {
        if (m_activeLevel != GameInfos.level)
        {
            gameObject.SetActive(false);
            return;
        }
        
        m_totalLife = EnemyList.getSumLife();
        m_initialWidth = m_spriteLife.size.x;
    }
    
    void Update()
    {
        if (m_lifeAtZero)
            return;

        float height = m_spriteLife.size.y;

        int life = EnemyList.getSumLife();
        m_spriteLife.size = new Vector2(life / (float)m_totalLife * m_initialWidth, height);

        if(life <= 0)
        {
            m_lifeAtZero = true;

            Event<BossKilledEvent>.Broadcast(new BossKilledEvent());
            DOVirtual.DelayedCall(m_hideTime, () =>
            {
                if (this == null)
                    return;
                gameObject.SetActive(false);
            });
        }
    }
}
