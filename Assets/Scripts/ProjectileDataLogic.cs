using UnityEngine;
using System.Collections;

public class ProjectileDataLogic : MonoBehaviour
{
    public GameObject sender;
    public int power;
    public float life;
    public float speed;

    private void Update()
    {
        if (GameInfos.paused)
            return;

        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

        transform.position = transform.position + speed * transform.right * Time.deltaTime;
    }
}
