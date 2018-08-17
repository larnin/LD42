using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PauseMenuLogic : MonoBehaviour
{
    const string selectButton = "Submit";
    const string cancelButton = "Cancel";
    const string pauseButton = "Pause";
    const string verticalAxis = "Vertical";

    [SerializeField] List<Transform> m_items;
    [SerializeField] GameObject m_screen;
    [SerializeField] float m_selectedScale = 2.5f;
    [SerializeField] float m_notSelectedScale = 1.5f;
    [SerializeField] float m_moveDuration = 0.2f;
    [SerializeField] AudioClip m_moveClip;
    [SerializeField] AudioClip m_selectClip;
    [SerializeField] AudioClip m_cancelClip;

    bool m_active = false;
    int m_pos = -1;
    float m_oldAxis;
    
    void Start()
    {
        m_screen.SetActive(false);
        foreach (var button in m_items)
        {
            var comp = button.GetComponent<ButtonLogic>();
            if (comp != null)
            {
                comp.onClickFunction = () => onClick(button);
                comp.onEnterFunction = () => onHover(button);
            }
        }

        DOVirtual.DelayedCall(0.01f, () => m_items.RemoveAll(x => { return x == null; }));
    }

    void onHover(Transform t)
    {
        int index = m_items.IndexOf(t);
        if (index >= 0)
            move(index);
    }

    void onClick(Transform t)
    {
        onHover(t);
        onSubmit();
    }

    void LateUpdate()
    {
        if(!m_active && Input.GetButtonDown(pauseButton) && !GameInfos.paused)
        {
            activate();
            return;
        }

        if (!m_active)
            return;

        if(Input.GetButtonDown(cancelButton))
        {
            SoundSystem.instance.play(m_cancelClip);
            onContinue();
            return;
        }

        var axis = Input.GetAxisRaw(verticalAxis);
        if (Mathf.Abs(axis) > 0.5f && Mathf.Abs(m_oldAxis) < 0.5f)
            move(m_pos + (axis > 0 ? -1 : 1));
        if (Input.GetButtonDown(selectButton))
            onSubmit();

        m_oldAxis = axis;
    }

    void activate()
    {
        m_active = true;
        GameInfos.paused = true;
        GameInfos.pauseMenu = true;
        m_screen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        move(0);
    }

    void onSubmit()
    {
        SoundSystem.instance.play(m_selectClip);
        if (m_pos == 0)
            onContinue();
        if (m_pos == 1)
            onMain();
        if (m_pos == 2)
            onQuit();
    }

    void move(int newPos)
    {
        if (newPos < 0)
            newPos = 0;
        if (newPos >= m_items.Count)
            newPos = m_items.Count - 1;
        if (newPos == m_pos)
            return;

        if (m_pos >= 0 && m_pos < m_items.Count)
        {
            SoundSystem.instance.play(m_moveClip);
            m_items[m_pos].DOScale(m_notSelectedScale, m_moveDuration).SetEase(Ease.Linear); ;
        }
        m_items[newPos].DOScale(m_selectedScale, m_moveDuration).SetEase(Ease.Linear);

        m_pos = newPos;
    }

    void onContinue()
    {
        m_active = false;
        GameInfos.paused = false;
        GameInfos.pauseMenu = false;
        m_screen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void onMain()
    {
        SceneSystem.changeScene("Main");
    }

    void onQuit()
    {
        Application.Quit();
    }
}
