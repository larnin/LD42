using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class FadeLogic : MonoBehaviour
{
    [SerializeField] float m_fadeTime = 1.0f;

    SpriteRenderer m_renderer;
    SubscriberList m_subscriberList = new SubscriberList();

    static FadeLogic m_instance = null;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<ShowLoadingScreenEvent>.Subscriber(onFade));
        m_subscriberList.Subscribe();
        m_renderer = GetComponent<SpriteRenderer>();

        m_renderer.color = new Color(0, 0, 0, 0);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void onFade(ShowLoadingScreenEvent e)
    {
        if (e.start)
            m_renderer.DOColor(Color.black, m_fadeTime);
        else m_renderer.DOColor(new Color(0, 0, 0, 0), m_fadeTime);

    }
}
