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

            GameInfos.bossKillCount++;
            if(GameInfos.bossKillCount > 1)
            {
                onAction(false);
                return;
            }

            SoundSystem.instance.play(m_victoryClip, 0.8f);
            gameObject.SetActive(true);
            m_active = true;
        });
    }

    void onAction(bool playSound = true)
    {
        if(playSound)
            SoundSystem.instance.play(m_contineClip);
        GameInfos.paused = false;
        gameObject.SetActive(false);
        var obj = GameObject.Find("Exit");
        if (obj != null)
            obj.transform.position = new Vector3(0, 0, 0);
    }
}
