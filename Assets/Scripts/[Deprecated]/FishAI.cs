using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishAI : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float rotationSpeed = 5f;
    public float raycastDistance = 2f;

    [Header("StatReadout")]
    [SerializeField] TMP_Text FishFoodCounter;
    public float fishFoodEaten = 0;

    //these are the layers that the fish can see
    public LayerMask obstacleLayer;
    public LayerMask foodLayer;

    void Update()
    {
        Swim();
        FishFoodCounter.text = fishFoodEaten.ToString();
    }

    // If the fish touched food
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Food"))
        {
            Eat();
            Destroy(other.gameObject);
        }
    }

    // METHODS
    void Swim()
    {
        // Draw debug raycast in Scene window
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);

        RaycastHit hit;

        //RAYCAST OBSTACLE
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, obstacleLayer))
        {
            // If an obstacle is detected, turn around
            TurnAround();

            return;  
        }

        //RAYCAST FOOD
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, foodLayer))
        {
            // If food is detected, follow the food
            swimSpeed = Mathf.Lerp(swimSpeed,10f,0.5f);
            MoveTowardsFood(hit.point);

            return;
        }

        //NO TARGET FOUND
        else
        {
            swimSpeed = 2f;
            //move forward by swimspeed
            transform.Translate(Vector3.forward * swimSpeed * Time.deltaTime);

            //rotate by rotation speed at a random range
            transform.Rotate(Vector3.up, Random.Range(-rotationSpeed, rotationSpeed));
        }
    }

    void TurnAround()
    {
        transform.Rotate(Vector3.up, 180f);
    }

    void MoveTowardsFood(Vector3 foodPosition)
    {
        // Calculate direction to the food
        Vector3 directionToFood = foodPosition - transform.position;

        // Rotate towards the food
        Quaternion rotationToFood = Quaternion.LookRotation(directionToFood);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToFood, rotationSpeed * Time.deltaTime);

        // Move forward towards the food
        transform.Translate(Vector3.forward * swimSpeed * Time.deltaTime);

    }

    void Eat()
    {
        fishFoodEaten++;
    }

}
