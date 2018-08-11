using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class LootLogic : SerializedMonoBehaviour
{
    [SerializeField] ModifierBase m_modifier;
    [SerializeField] float m_rotationSpeed = 1;

    public ModifierBase modifier { get { return m_modifier; } }
    
    void Update()
    {
        float angle = transform.rotation.eulerAngles.z + m_rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
