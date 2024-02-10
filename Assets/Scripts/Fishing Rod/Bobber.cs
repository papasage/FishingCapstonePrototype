using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    LineRenderer lineRenderer;
    GameObject hook;
    GameObject firePoint;

    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        hook = GameObject.Find("Hook");
        firePoint = GameObject.Find("FirePoint");
    }
    private void Update()
    {
        if (hook != null && firePoint != null)
        {
            SetLineRendererPositions();
        }
        else
        {
            lineRenderer = GetComponent<LineRenderer>();
            hook = GameObject.Find("Hook");
            firePoint = GameObject.Find("FirePoint");
        }
        
    }

    void SetLineRendererPositions()
    {
        // Set the positions of the line renderer to the positions of the start and end objects
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, firePoint.transform.position);
        lineRenderer.SetPosition(1, this.transform.position);
        lineRenderer.SetPosition(2, hook.transform.position);
    }
}
