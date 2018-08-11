using UnityEngine;
using System.Collections;

public class AIShipControlerLogic : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerProjectile(collision.GetComponent<ProjectileDataLogic>());
    }

    void onTriggerProjectile(ProjectileDataLogic p)
    {
        if (p == null)
            return;

    }
}
