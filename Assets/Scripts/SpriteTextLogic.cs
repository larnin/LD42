using UnityEngine;
using System.Collections;

public class SpriteTextLogic : MonoBehaviour
{
    GameObject m_text = null;

    public void setText(string text)
    {
        if (m_text != null)
            Destroy(m_text);

        m_text = SpriteTextCreatorLogic.instance.create(text, TextAlignment.Center, Vector3.zero, Color.red);
        m_text.transform.parent = transform;
        m_text.transform.localPosition = Vector3.zero;
    }

    private void Start()
    {
        setText("ABCD123456789");
    }
}
