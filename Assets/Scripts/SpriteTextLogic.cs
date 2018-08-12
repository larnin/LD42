using UnityEngine;
using System.Collections;

public class SpriteTextLogic : MonoBehaviour
{
    [SerializeField] string m_text;
    [SerializeField] TextAlignment m_alignement = TextAlignment.Center;
    [SerializeField] Color m_color;
    GameObject m_value = null;

    public void setText(string text, Color color)
    {
        if (m_value != null)
            Destroy(m_value);

        m_value = SpriteTextCreatorLogic.instance.create(text, m_alignement, Vector3.zero, color);
        m_value.transform.parent = transform;
        m_value.transform.localPosition = Vector3.zero;
        m_value.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        if (m_text.Length > 0 && m_value == null)
        {
            m_value = SpriteTextCreatorLogic.instance.create(m_text, m_alignement, Vector3.zero, m_color);
            m_value.transform.parent = transform;
            m_value.transform.localPosition = Vector3.zero;
            m_value.transform.localScale = Vector3.one;
        }
    }
}
