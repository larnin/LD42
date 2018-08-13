using UnityEngine;
using System.Collections;

public class SoundSystem : MonoBehaviour
{
    static SoundSystem m_instance;
    public static SoundSystem instance { get { return m_instance; } }

    AudioSource[] m_sources;

    private void Awake()
    {
        if(m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_sources = transform.Find("Sounds").GetComponentsInChildren<AudioSource>();
    }
    
    public void play(AudioClip clip, float volume = 0.5f)
    {
        foreach(var s in m_sources)
        {
            if(!s.isPlaying)
            {
                s.clip = clip;
                s.volume = volume;
                s.Play();
                return;
            }
        }
    }
}
