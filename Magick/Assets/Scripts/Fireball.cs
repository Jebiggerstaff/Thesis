using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    float lifeTimer=1f;

    public float radius = 5f;
    public float power = 100f;

    void Awake()
    {
        Vector3 explosionPos = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && rb.gameObject.layer != 8)
            {
                rb.AddExplosionForce(power, explosionPos, radius, 1f, ForceMode.Impulse);          
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, radius);
    }

    void Update()
    {
        if(lifeTimer <= 0f)
        {
            Object.Destroy(gameObject);
        }
        lifeTimer -= Time.deltaTime;
    }
}
