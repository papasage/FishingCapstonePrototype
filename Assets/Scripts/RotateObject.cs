using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public bool rotateEnabled = false;  // Public boolean variable to enable/disable rotation
    public float rotationSpeed = 5f;    // Rotation speed

    // Update is called once per frame
    void Update()
    {
        if (rotateEnabled)
        {
            RotateGameObject();
        }
    }

    void RotateGameObject()
    {
        // Get the current rotation of the object
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Update the rotation based on the rotation speed
        float newRotation = currentRotation.y + (rotationSpeed * Time.deltaTime);

        // Apply the new rotation to the object
        transform.rotation = Quaternion.Euler(currentRotation.x, newRotation, currentRotation.z);
    }
}
