using UnityEngine;
using System.Collections;

public class SpriteTextLogic : MonoBehaviour
{
    GameObject m_text = null;

    public void setText(string text, Color color)
    {
        if (m_text != null)
            Destroy(m_text);

        m_text = SpriteTextCreatorLogic.instance.create(text, TextAlignment.Center, Vector3.zero, color);
        m_text.transform.parent = transform;
        m_text.transform.localPosition = Vector3.zero;
    }

    private void Start()
    {

    }
}
