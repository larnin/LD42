using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using NRand;

public class LootLogic : SerializedMonoBehaviour
{
    [SerializeField] ModifierBase m_modifier;
    [SerializeField] float m_rotationSpeed = 1;
    [SerializeField] float m_baseModifierValue = 1;
    [SerializeField] float m_minModifierLevel = 0.5f;
    [SerializeField] float m_maxModifierLevel = 2;

    public ModifierBase modifier { get { return m_modifier; } }

    private void Awake()
    {
        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();
        var d = new UniformFloatDistribution(m_minModifierLevel, m_maxModifierLevel);

        modifier.life *= (int)((float)modifier.life * m_baseModifierValue + d.Next(rand) * GameInfos.level);
        modifier.speed *= (int)((float)modifier.speed * m_baseModifierValue + d.Next(rand) * GameInfos.level);
        modifier.fireRate *= (int)((float)modifier.fireRate * m_baseModifierValue + d.Next(rand) * GameInfos.level);
        modifier.power *= (int)((float)modifier.power * m_baseModifierValue + d.Next(rand) * GameInfos.level);
    }

    private void Start()
    {
        LootManagerLogic.instance.add(this);
    }

    void Update()
    {
        float angle = transform.rotation.eulerAngles.z + m_rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnDestroy()
    {
        LootManagerLogic.instance.remove(this);
    }
}
