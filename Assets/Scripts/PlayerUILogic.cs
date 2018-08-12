using UnityEngine;
using System.Collections;

public class PlayerUILogic : MonoBehaviour
{
    [SerializeField] Color m_lifeColor;
    [SerializeField] Color m_speedColor;
    [SerializeField] Color m_fireRateColor;
    [SerializeField] Color m_powerColor;
    [SerializeField] float m_itemSpacing = 1.5f;
    [SerializeField] float m_lifeSpacing = 0.5f;
    [SerializeField] Sprite m_lifeItemSprite;
    [SerializeField] Sprite m_noItemSprite;

    SubscriberList m_subscriberList = new SubscriberList();

    Transform m_lifeBase;
    Transform m_itemsBase;
    SpriteTextLogic m_life;
    SpriteTextLogic m_speed;
    SpriteTextLogic m_fireRate;
    SpriteTextLogic m_power;

    private void Awake()
    {
        m_subscriberList.Add(new Event<UpdateUIEvent>.Subscriber(onUiUpdate));
        m_subscriberList.Subscribe();

        m_lifeBase = transform.Find("LifeBase");
        m_itemsBase = transform.Find("ModifiersBase");
        m_life = transform.Find("StatLife").GetComponent<SpriteTextLogic>();
        m_speed = transform.Find("StatSpeed").GetComponent<SpriteTextLogic>();
        m_fireRate = transform.Find("StatFireRate").GetComponent<SpriteTextLogic>();
        m_power = transform.Find("StatPower").GetComponent<SpriteTextLogic>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void onUiUpdate(UpdateUIEvent e)
    {
        for (int i = m_lifeBase.childCount - 1; i >= 0; i++)
            Destroy(m_lifeBase.GetChild(i).gameObject);
        for (int i = m_itemsBase.childCount - 1; i >= 0; i++)
            Destroy(m_itemsBase.GetChild(i).gameObject);

        m_life.setText("A " + e.ship.maxLife, m_lifeColor);
        m_speed.setText("B " + e.ship.speed, m_speedColor);
        m_fireRate.setText("C " + e.ship.fireRate, m_fireRateColor);
        m_power.setText("D " + e.ship.power, m_powerColor);

        for(int i = 0; i < e.ship.maxLife; i++)
        {
            Color c = m_lifeColor;
            if(i >= e.ship.life)
            {
                c.r /= 2;
                c.g /= 2;
                c.b /= 2;
            }

            var obj = new GameObject("Life" + i);
            obj.transform.parent = m_lifeBase;
            obj.transform.localPosition = new Vector3(i * m_lifeSpacing, 0, 0);
            var comp = obj.AddComponent<SpriteRenderer>();
            comp.sprite = m_lifeItemSprite;
            comp.color = c;
        }

        for (int i = 0; i < e.ship.modifiers.Count || i < GameInfos.playerModifierCount; i++)
        {
            var obj = new GameObject("Mod" + i);
            obj.transform.parent = m_itemsBase;
            obj.transform.localPosition = new Vector3(i * m_itemSpacing, 0, 0);
            var comp = obj.AddComponent<SpriteRenderer>();
            if (i >= e.ship.modifiers.Count)
                comp.sprite = m_noItemSprite;
            else comp.sprite = e.ship.modifiers[i].m_image;
        }
    }
}
