using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CrusherLogic : MonoBehaviour
{
    const string acceptButton = "Submit";
    const string cancelButton = "Cancel";
    const string horizontalAxis = "Horizontal";

    class ObjectInfo
    {
        public Transform transform;
        public SpriteRenderer sprite;
        public SpriteTextLogic line1;
        public SpriteTextLogic line2;
        public SpriteTextLogic line3;
    }

    [SerializeField] GameObject m_modifierPrefab;
    [SerializeField] Color m_lifeColor;
    [SerializeField] Color m_speedColor;
    [SerializeField] Color m_fireRateColor;
    [SerializeField] Color m_powerColor;
    [SerializeField] float m_itemSpacing = 1;
    [SerializeField] float m_crusherTransitionTime;
    [SerializeField] float m_crushTime = 2;
    [SerializeField] float m_particlePlayTime = 0.2f;
    [SerializeField] AudioClip m_moveClip;
    [SerializeField] AudioClip m_cancelClip;
    [SerializeField] AudioClip m_crushClip;

    SubscriberList m_subscriverList = new SubscriberList();
    bool m_active = false;

    ObjectInfo m_crusher = new ObjectInfo();
    List<ObjectInfo> m_objects = new List<ObjectInfo>();
    SpriteRenderer m_background;
    Transform m_objectsBase;
    ParticleSystem m_particles;

    ModifierBase m_modifier;
    ShipLogic m_ship;

    int m_crusherPosition;

    float m_oldValue;

    private void Awake()
    {
        m_subscriverList.Add(new Event<CrushEvent>.Subscriber(onCrush));
        m_subscriverList.Subscribe();

        m_crusher.transform = transform.Find("ObjectCrusher");
        m_crusher.sprite = m_crusher.transform.Find("Object").GetComponent<SpriteRenderer>();
        m_crusher.line1 = m_crusher.transform.Find("Line1").GetComponent<SpriteTextLogic>();
        m_crusher.line2 = m_crusher.transform.Find("Line2").GetComponent<SpriteTextLogic>();
        m_crusher.line3 = m_crusher.transform.Find("Line3").GetComponent<SpriteTextLogic>();
        m_background = GetComponent<SpriteRenderer>();
        m_objectsBase = transform.Find("InventoryBase");
        m_particles = GetComponentInChildren<ParticleSystem>();
        m_particles.Stop();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        m_subscriverList.Unsubscribe();
    }

    void onCrush(CrushEvent e)
    {
        foreach (var o in m_objects)
            Destroy(o.transform.gameObject);
        m_objects.Clear();

        m_modifier = e.modifier;
        m_ship = e.ship;

        setInfos(m_crusher, e.modifier);
        
        foreach(var m in e.ship.modifiers)
        {
            var obj = Instantiate(m_modifierPrefab);
            ObjectInfo infos = new ObjectInfo();
            infos.transform = obj.transform;
            infos.sprite = obj.GetComponent<SpriteRenderer>();
            infos.line1 = obj.transform.Find("Line1").GetComponent<SpriteTextLogic>();
            infos.line2 = obj.transform.Find("Line2").GetComponent<SpriteTextLogic>();
            infos.line3 = obj.transform.Find("Line3").GetComponent<SpriteTextLogic>();

            setInfos(infos, m);

            infos.transform.parent = m_objectsBase;
            infos.transform.localPosition = new Vector3(m_objects.Count * m_itemSpacing - (e.ship.modifiers.Count - 1) * m_itemSpacing / 2, 0, 0);

            m_objects.Add(infos);
        }

        setCruherIndex(m_objects.Count / 2 - 1);

        m_background.size = new Vector2((m_objects.Count + 1) * m_itemSpacing, m_background.size.y);

        gameObject.SetActive(true);
        m_active = true;
    }

    private void Update()
    {
        if (!m_active || GameInfos.pauseMenu)
            return;

        GameInfos.paused = true;

        if (Input.GetButtonDown(acceptButton))
            onAccept();
        if (Input.GetButtonDown(cancelButton))
            onCancel();

        float value = Input.GetAxisRaw(horizontalAxis);
        if (Mathf.Abs(m_oldValue) < 0.5f && Mathf.Abs(value) > 0.5f)
            setCruherIndex(m_crusherPosition + (value > 0 ? 1 : -1));
        m_oldValue = value;
    }

    void setInfos(ObjectInfo infos, ModifierBase modifier)
    {
        infos.sprite.sprite = modifier.m_image;

        List<string> texts = new List<string>();
        List<Color> colors = new List<Color>();

        if (modifier.life != 0)
        {
            texts.Add("A " + modifier.life.ToString());
            colors.Add(m_lifeColor);
        }
        if (modifier.speed != 0)
        {
            texts.Add("B " + modifier.speed.ToString());
            colors.Add(m_speedColor);
        }
        if (modifier.fireRate != 0)
        {
            texts.Add("C " + modifier.fireRate.ToString());
            colors.Add(m_fireRateColor);
        }
        if (modifier.power != 0)
        {
            texts.Add("D " + modifier.power.ToString());
            colors.Add(m_powerColor);
        }

        if (texts.Count > 0)
            infos.line1.setText(texts[0], colors[0]);
        else infos.line1.setText("", Color.white);
        if (texts.Count > 1)
            infos.line2.setText(texts[1], colors[1]);
        else infos.line2.setText("", Color.white);
        if (texts.Count > 2)
            infos.line3.setText(texts[2], colors[2]);
        else infos.line3.setText("", Color.white);
    }

    void setCruherIndex(int index)
    {
        if(m_objects.Count == 1)
        {
            float target = m_objects[0].transform.localPosition.x;
            m_crusher.transform.localPosition = new Vector3(target, m_crusher.transform.localPosition.y, m_crusher.transform.localPosition.z);
            return;
        }
        if (index < 0)
            index = 0;
        if (index >= m_objects.Count - 1)
            index = m_objects.Count - 2;

        if (index == m_crusherPosition)
            return;

        SoundSystem.instance.play(m_moveClip);

        float targetX = (m_objects[index].transform.localPosition.x + m_objects[index+1].transform.localPosition.x)/2;
        m_crusher.transform.DOLocalMoveX(targetX, m_crusherTransitionTime).SetEase(Ease.Linear);

        m_crusherPosition = index;
    }

    void onAccept()
    {
        if(m_ship.modifiers.Count == 1)
            m_ship.modifiers[0] = m_modifier;
        else
        {
            m_ship.modifiers[m_crusherPosition] = m_modifier;
            m_ship.modifiers.RemoveAt(m_crusherPosition + 1);
            GameInfos.playerModifierCount--;
        }

        m_ship.updateModifierStats();
        Event<UpdateUIEvent>.Broadcast(new UpdateUIEvent(m_ship));

        var size = m_background.size;
        onCrush(new CrushEvent(new Modifier(), m_ship));
        m_background.size = size;
        m_active = false;

        SoundSystem.instance.play(m_crushClip);

        Event<PlayCameraEffectEvent>.Broadcast(new PlayCameraEffectEvent(CameraEffectType.Shake, 1, 0.5f));

        m_particles.Play();
        DOVirtual.DelayedCall(m_particlePlayTime, () => m_particles.Stop());

        DOVirtual.DelayedCall(m_crushTime, () => onCancel(false));
    }

    void onCancel(bool cancel = true)
    {
        if (cancel)
            SoundSystem.instance.play(m_cancelClip);
        gameObject.SetActive(false);
        GameInfos.paused = false;
    }
}
