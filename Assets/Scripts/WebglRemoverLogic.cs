using UnityEngine;
using System.Collections;

public class WebglRemoverLogic : MonoBehaviour
{
    void Awake()
    {
#if UNITY_WEBGL
        Destroy(gameObject);
#endif
    }
}
