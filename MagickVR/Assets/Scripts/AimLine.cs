using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    public GameObject RightHand;

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, RightHand.transform.position);
        lineRenderer.SetPosition(1, RightHand.transform.forward * 20 + RightHand.transform.position);
    }
}
