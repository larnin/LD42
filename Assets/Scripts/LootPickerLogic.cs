using UnityEngine;
using System.Collections;

public class LootPickerLogic : MonoBehaviour
{
    const string lootButton = "Submit";

    [SerializeField] GameObject m_player;
    [SerializeField] float m_lootRadius = 5;
    [SerializeField] Vector3 m_buttonOffset;

    GameObject m_button;
    ShipLogic m_ship;

    private void Awake()
    {
        m_button = transform.Find("Button").gameObject;
        m_button.SetActive(false);
    }

    private void Start()
    {
        m_ship = m_player.GetComponent<ShipLogic>();
    }

    void Update()
    {
        if (GameInfos.paused)
            return;

        var loot = LootManagerLogic.instance.getNearestLootInRadius(m_player.transform.position, m_lootRadius);

        if (loot == null)
        {
            m_button.SetActive(false);
            return;
        }

        m_button.SetActive(true);
        m_button.transform.position = loot.transform.position + m_buttonOffset;

        if(Input.GetButtonDown(lootButton))
        {
            if (m_ship.modifiers.Count < GameInfos.playerModifierCount)
            {
                m_ship.modifiers.Add(loot.modifier);
                m_ship.updateModifierStats();
                Event<UpdateUIEvent>.Broadcast(new UpdateUIEvent(m_ship));
            }
            else Event<CrushEvent>.Broadcast(new CrushEvent(loot.modifier, m_ship));

            Destroy(loot.gameObject);
        }
    }
}
