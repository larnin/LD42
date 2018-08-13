using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossKilledMenuLogic : MonoBehaviour
{
    const string submitButton = "Submit";
    const string cancelButton = "Cancel";

    [SerializeField] float m_enableDelay;
    [SerializeField] AudioClip m_victoryClip;
    [SerializeField] AudioClip m_contineClip;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_active = false;

    private void Awake()
    {
        m_subscriberList.Add(new Event<BossKilledEvent>.Subscriber(onBossDie));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

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

    void onBossDie(BossKilledEvent e)
    {
        DOVirtual.DelayedCall(m_enableDelay, () =>
        {
            if (this == null)
                return;
            SoundSystem.instance.play(m_victoryClip, 0.8f);
            gameObject.SetActive(true);
            m_active = true;
        });
    }

    void onAction()
    {
        SoundSystem.instance.play(m_contineClip);
        var obj = GameObject.Find("Exit");
        if (obj != null)
            obj.transform.position = new Vector3(0, 0, 0);
    }
}
