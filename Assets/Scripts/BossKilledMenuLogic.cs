using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossKilledMenuLogic : MonoBehaviour
{
    const string submitButton = "Submit";
    const string cancelButton = "Cancel";

    [SerializeField] float m_enableDelay;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_active = false;

    void Start()
    {
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (!m_active || GameInfos.pauseMenu)
            return;

        GameInfos.paused = true;

        if (Input.GetButtonDown(submitButton) || Input.GetButtonDown(cancelButton))
            onAction();
    }

    void onBossDie()
    {
        DOVirtual.DelayedCall(m_enableDelay, () =>
        {
            if (this == null)
                return;
            gameObject.SetActive(true);
            m_active = true;
        });
    }

    void onAction()
    {
        var obj = GameObject.Find("Exit");
        if (obj != null)
            obj.transform.position = new Vector3(0, 0, 0);
    }
}
