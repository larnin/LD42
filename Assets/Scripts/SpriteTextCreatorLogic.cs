using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class SpriteTextCreatorLogic : SerializedMonoBehaviour
{
    static SpriteTextCreatorLogic m_instance = null;
    public static SpriteTextCreatorLogic instance { get { return m_instance; } }

    [SerializeField] Dictionary<char, Sprite> m_letters;
    [SerializeField] float m_spacing;

    private void Awake()
    {
        m_instance = this;
    }

    public GameObject create(string text, TextAlignment alignment, Vector3 pos, Color color)
    {
        var obj = new GameObject("Text");
        float origin = 0;
        if (alignment == TextAlignment.Center)
            origin = -m_spacing * text.Length / 2;
        else if (alignment == TextAlignment.Right)
            origin = -m_spacing * text.Length;

        for(int i = 0 ; i < text.Length; i++)
        {
            if (!m_letters.ContainsKey(text[i]))
                continue;

            float x = origin + i * m_spacing;
            var letter = new GameObject(i.ToString());
            var renderer = letter.AddComponent<SpriteRenderer>();
            renderer.sprite = m_letters[text[i]];
            renderer.color = color;

            letter.transform.parent = obj.transform;
            letter.transform.localPosition = new Vector3(x, 0, 0);
        }
        obj.transform.position = pos;
        return obj;
    }
}
