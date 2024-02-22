using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    // I called this class LineManager, but it became more of a "Line Manager". I would change the name, but I dont want to break things. 

    LineRenderer lineRenderer;
    GameObject hook;
    GameObject firePoint;

    public GameObject hookedFish;

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
        
        if (hookedFish != null)
        {
            lineRenderer.SetPosition(2, hookedFish.transform.position);
        }
    }
}
