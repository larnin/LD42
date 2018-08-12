using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ProjectileDataLogic : MonoBehaviour
{
    [SerializeField] GameObject m_deathPrefab;
    public GameObject sender;
    public int power;
    public float life;
    public float speed;

    protected virtual void Update()
    {
        if (GameInfos.paused)
            return;

        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

        transform.position = transform.position + speed * transform.right * Time.deltaTime;
    }

    private void OnDestroy()
    {
        var deathPrefab = m_deathPrefab;
        var pos = transform.position;
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (deathPrefab == null)
                return;

            var obj = Instantiate(deathPrefab);
            obj.transform.position = pos;
        });
    }
}
