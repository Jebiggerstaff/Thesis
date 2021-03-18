using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float radius = 5f;
    public float power = 100f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, radius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
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
            Object.Destroy(gameObject);
        }
    }
}
