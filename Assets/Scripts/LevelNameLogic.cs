using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class LevelNameLogic : MonoBehaviour
{
    [SerializeField] Color m_color;
    [SerializeField] float m_duration;

    SpriteTextLogic m_text;

    private void Start()
    {
        m_text = GetComponent<SpriteTextLogic>();
        m_text.setText("stage " + (GameInfos.level + 1), m_color);
        DOVirtual.DelayedCall(m_duration, () => gameObject.SetActive(false));
    }
}
