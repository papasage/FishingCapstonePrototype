using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool flipHorizontal = false;
    void Update()
    {
        if (flipHorizontal)
        {
            transform.LookAt(-Camera.main.transform.position, Vector3.up);
        }
        
        if (!flipHorizontal)
        {
            transform.LookAt(Camera.main.transform.position, Vector3.up);
        }
    }
}

