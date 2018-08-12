using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MainMenuLogic : SerializedMonoBehaviour
{
    const string verticalAxis = "Vertical";
    const string submitButton = "Submit";
    const string cancelButton = "Cancel";
    const string gameName = "Game";

    [SerializeField] List<Transform> m_buttons;
    [SerializeField] Transform m_cursor;
    [SerializeField] GameObject m_howToScreen;
    [SerializeField] GameObject m_projectilePrefab;
    [SerializeField] Color m_projectileColor;
    [SerializeField] float m_projectileSpeed;
    [SerializeField] float m_moveDuration = 0.5f;
    [SerializeField] float m_selectScale = 1.5f;
    [SerializeField] float m_notSelectedScale = 2.0f;
    [SerializeField] float m_delayAction = 1.0f;
    [SerializeField] List<ModifierBase> m_startModifiers;

    int m_pos = -1;
    float m_oldAxis;
    bool m_selected = false;
    int m_selectedPos;
    bool m_howToShown = false;

    private void Start()
    {
        move(0);
        m_howToScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown(cancelButton))
            onCancel();

        if (m_howToShown)
            return;

        var axis = Input.GetAxisRaw(verticalAxis);
        if(Mathf.Abs(axis) > 0.5f && Mathf.Abs(m_oldAxis) < 0.5f)
            move(m_pos + (axis > 0 ? -1 : 1));
        if (Input.GetButtonDown(submitButton))
            onSubmit();
        
        m_oldAxis = axis;
    }

    void move(int newPos)
    {
        if (newPos < 0)
            newPos = 0;
        if (newPos >= m_buttons.Count)
            newPos = m_buttons.Count - 1;
        if (newPos == m_pos)
            return;

        if (m_pos >= 0 && m_pos < m_buttons.Count)
            m_buttons[m_pos].DOScale(m_notSelectedScale, m_moveDuration).SetEase(Ease.Linear); ;
        m_buttons[newPos].DOScale(m_selectScale, m_moveDuration).SetEase(Ease.Linear);

        m_pos = newPos;

        m_cursor.DOMoveY(m_buttons[m_pos].position.y, m_moveDuration).SetEase(Ease.Linear); ;
    }

    void onSubmit()
    {
        fire(m_projectilePrefab, m_cursor.gameObject, 0, m_projectileSpeed, 100, m_projectileColor);

        if (m_selected)
            return;
        m_selected = true;
        m_selectedPos = m_pos;

        DOVirtual.DelayedCall(m_delayAction, () =>
        {
            if (m_selectedPos == 0)
                onPlay();
            if (m_selectedPos == 1)
                onHardcore();
            if (m_selectedPos == 2)
                onHowTo();
            if (m_selectedPos == 3)
                onQuit();
        });

    }

    void onCancel()
    {
        if (m_howToShown)
        {
            m_howToScreen.SetActive(false);
            m_howToShown = false;
            m_selected = false;
        }
    }

    public void fire(GameObject projectile, GameObject sender, float rot, float speed, float life, Color color)
    {
        var obj = GameObject.Instantiate(projectile);

        var pos = sender.transform.position;
        pos.z += 0.1f;
        obj.transform.position = pos;

        var senderRot = sender.transform.rotation.eulerAngles.z;
        obj.transform.rotation = Quaternion.Euler(0, 0, senderRot + rot);

        var comp = obj.GetComponent<ProjectileDataLogic>();
        if (comp != null)
        {
            comp.sender = sender;
            comp.speed = speed;
            comp.life = life;
        }
        obj.GetComponentInChildren<Renderer>().material.color = color;
    }

    void onPlay()
    {
        GameInfos.clear();
        GameInfos.modifiers = m_startModifiers;
        GameInfos.hardmode = false;
        SceneSystem.changeScene(gameName);
    }

    void onHardcore()
    {
        GameInfos.clear();
        GameInfos.modifiers = m_startModifiers;
        GameInfos.hardmode = true;
        SceneSystem.changeScene(gameName);
    }

    void onHowTo()
    {
        m_howToShown = true;
        m_howToScreen.SetActive(true);
    }

    void onQuit()
    {
        Application.Quit();
    }
}
