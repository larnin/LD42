using UnityEngine;
using System.Collections;

public class LootPickerLogic : MonoBehaviour
{
    const string lootButton = "Submit";

    [SerializeField] GameObject m_player;
    [SerializeField] float m_lootRadius = 5;
    [SerializeField] Vector3 m_buttonOffset;

    GameObject m_button;

    private void Awake()
    {
        m_button = transform.Find("Button").gameObject;
        m_button.SetActive(false);
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
            //do something
        }
    }
}
