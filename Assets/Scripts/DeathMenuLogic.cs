using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathMenuLogic : MonoBehaviour
{
    const string acceptButton = "Submit";

    [SerializeField] SpriteTextLogic m_ennemyLabel;
    [SerializeField] SpriteTextLogic m_levelLabel;
    [SerializeField] float m_deathTime = 2;
    [SerializeField] AudioClip m_deathClip;
    [SerializeField] AudioClip m_continueClip;

    SubscriberList m_subscriberList = new SubscriberList();

    void Start()
    {
        m_subscriberList.Add(new Event<DieEvent>.Subscriber(onDeath));
        m_subscriberList.Subscribe();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown(acceptButton))
        {
            GameInfos.paused = true;
            SoundSystem.instance.play(m_continueClip);
            SceneSystem.changeScene("Main");
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void onDeath(DieEvent e)
    {
        DOVirtual.DelayedCall(m_deathTime, () =>
        {
            SoundSystem.instance.play(m_deathClip, 0.8f);
            gameObject.SetActive(true);
            GameInfos.paused = true;
            m_ennemyLabel.setText(GameInfos.killCount.ToString(), Color.white);
            m_levelLabel.setText((GameInfos.level + 1).ToString(), Color.white);
        });
    }
}
