using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Fish", menuName = "Fish")]
public class Fish : ScriptableObject
{
    [Header("Artwork")]
    public GameObject mesh;                         // The artwork object that will be spawned to represent the fish

    [Header("Ability Stats")]
    public bool isLure = false;          // Applied by special lure scriptable objects. Lures are boids so that they can be seen and hunted, but they don't swim/school/hunt/escape
    public float swimSpeed = 26.3f;                 // This is the speed they move forward with ApplySwimBehavior();
    public float swimHuntSpeed = 35f;               // This is the speed they move forward with ApplySwimBehavior(); while hunting
    public float swimEscapeSpeed = 33f;             // This is the speed they move forward with ApplySwimBehavior() while escaping
    public float deviateRange = 10f;                // When deviating, the rotation is a random rotation between a negative and positive of this value. 
    public float deviateChance = 5f;                // Chance to deviate out of 100
    public float perceptionRadius = 5.5f;           // If another boid enters this range, it becomes a neighbor
    public float avoidanceRadius= 7f;               // The distance it takes to react to an obstacle TODO:Check the neighbor list for threats and react when they get in this range
    public float avoidanceSeconds = 1.5f;           // The amount of time in seconds it takes to complete an avoidance

    [Header("Food Stats")]
    public float foodScore = 1f;                    // Rename this to Predator Score. The Higher the number, the larger the fish they can eat
    public float foodScoreMax = 4f;                 // This is the maximum foodScore that the fish can hit before they begin to die of old age.
    public float scoreDifferenceThreshold = 0.0f;   // this is the amount that your foodScore has to be greater by in order to eat another fish.
    public float minSizeMultiplier = 0.5f;          // The minimum possible factor for size generation
    public float maxSizeMultiplier = 1.5f;          // The maximum possible factor for size generation
    public float hungerWeight = 1f;                 // TODO: The factor for how aggressively the fish will move towards prey. x1 is default movement speed.
    public float hungryInSeconds = 30f;             // The amount of time in seconds it takes to become hungry
    public float biteRange = 2.5f;                  // Range a fish must be to eat
    public float decompositionTime = 5f;                // Variable for how long it takes to start decomposition

    [Header("Schooling Behavior Stats")]
    public float cohesionWeight = 0.89f;            // This fish's desire to stay in the center of its neighbors 1 = 100%
    public float separationWeight = 1.5f;           // This fish's desire to have space from its neighbors 1 = 100%
    public float alignmentWeight = 0.47f;           // This fish's desire to travel with its neighbors 1 = 100%
    public float alignmentSpeed = 0.82f;            // How hard will the boid make alignment adjustments. The actual aligning occurs in the calculation method for reasons.

    [Header("Fish Facts")]
    public string maidenName;
    public string favoriteSong;
    public int luckyNumber;

    [Header("Starting Modifier")]
    public bool isSchoolingOnSpawn;
}
