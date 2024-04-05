using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static BoidBehavior;

[System.Serializable]

public class BoidBehavior : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////////
    //REFERENCES
    ////////////////////////////////////////////////////////////////////
    [Header("Scriptable Object")]
    public Fish fish;                    // The current fish brain assigned to this boid (scriptable object) if left blank, it will pick its own inside of SelectBoid();
    public Fish[] allFish;               // A list of all the fish brains this prefab can pull from when spawning or shuffling.

    [Header("Layers")]
    public LayerMask obstacleLayer;      // This is what the fish sees as an obstacle to turn around

    [Header("Required References")]
    private Rigidbody rb;                // The Rigidbody component of this boid is initialized on Start().
    private ChatBubble chatBubble;       // Fish Emote Prefab
    private List<BoidBehavior> boids;      // This List is used to log every boid in the scene. Initialized on Start().
    private List<BoidBehavior> neighbors;  // This List is used determine what boids are closest on the "boid" List. Used in ApplySchoolingBehavior()
    private GameObject currentMesh;      // When we instantiate a mesh to the fish, THAT INSTANCE will be stored here.
    private float currentSpeed;          // Storing the current movement speed to use in ApplySwimBehavior();
    private Vector3 predatorDistance;
    private ConfigurableJoint hook;
    private FishingRod fishingRod;
    private int minnowCount = 0;

    //I DONT USE THESE ANYMORE. CAN WE DELETE?
    //private TMP_Text label;              // Fish name tag
    //private LineManager bobber;
    //private float waterLevel;
    //private Vector3 foodTarget;          // This is where we are storing the current prey target, if there is one

    [Header("Bool States")]
    public bool isHungry = false;        // Trigger for ApplyHuntingBehavior(). Set by the COROUTUNE EncroachingHunger()
    public bool isElderly = false;
    public bool foundFood = false;       // When using ApplyHuntingHehavior(), the fish is using DetectFood(). If a neighbor is found with a lower foodScore, this is the trigger to start the hunt.
    public bool isDead = false;          // A dead fish is a fish with no scriptable object. Currently called by other attacking fish when they Eat()
    public bool isBeingHunted = false;   // Trigger for Panic. Set by the predator fish's ApplyHuntingBehavior().
    public bool isHooked = false;        // Trigger for ApplyHookedBehavior().
    public bool isHookSet = false;      // This is checked before the SetHook coroutine is fired. If it is set, dont try to set it again.

    [Header("Bool Behaviors")]
    public bool isSchooling = true;      // Trigger for ApplySchoolingBehavior(). Currently this is always true
    public bool isDeviating = false;     // Decides when a fish will make random micro movements for realstic wandering.
    public bool isFightingBack = false;
    public bool isPanicking = false;     // This is checked before the Panic coroutine is fired. If it is panicking, dont try to set it again.

    public delegate void OnDeath();
    public static OnDeath onDeath;

    //
    //THE FOLLOWING REFERENCES ARE FILLED IN BY THE SCRIPTABLE OBJECT INSIDE OF BecomeBoid()
    //

    [Header("Artwork")]
    public GameObject mesh;             // This is the data for the mesh we want to become. Filled by a scriptable object.
    
    [Header("Ability Stats")]
    public bool isLure;          // Applied by special lure scriptable objects. Lures are boids so that they can be seen and hunted, but they don't swim/school/hunt/escape
    public float swimSpeed;             // This is the speed they move forward with ApplySwimBehavior()
    public float swimHuntSpeed;         // This is the speed they move forward with ApplySwimBehavior() while hunting
    public float swimEscapeSpeed;       // This is the speed they move forward with ApplySwimBehavior() while escaping
    public float deviateRange;          // When deviating, the rotation is a random rotation between a negative and positive of this value. 
    public float deviateChance;          // Chance to deviate out of 100
    public float perceptionRadius;      // If another boid enters this range, it becomes a neighbor
    public float avoidanceRadius;       // The distance it takes to react to an obstacle TODO:Check the neighbor list for threats and react when they get in this range
    public float avoidanceSeconds;      // The amount of time in seconds it takes to complete an avoidance

    [Header("Food Stats")]
    public float foodScore;             // Food chain placement. Fish can only eat fish with lower foodScores.
    public float foodScoreMax;          // This is the maximum foodScore that the fish can hit before they begin to die of old age.
    public float scoreDifferenceThreshold; // this is the amount that your foodScore has to be greater by in order to eat another fish.
    public float sizeMultiplier;        // Genetic lottery. Randomly rolled on birth from a range set by that fish's breed (in scriptable object)
    public float hungerWeight;          // UNUSED: Potentially increases the aggression/speed of the fish over time as they get hungry.
    public float hungryInSeconds;       // The amount of time in seconds it takes to become hungry
    public float biteRange;             // Range a fish must be to Eat() another fish when hunting. 
    public float decompositionTime;     // Variable for how long it takes to start decomposition
    public BoidBehavior target;
    public float comboMeter = 1;        //this will increase when a fish eats a hooked fish.

    [Header("Schooling Behavior Stats")]
    public float cohesionWeight;        // Stat for prioritising cohesion. The desire to be in the center of your neighbors. 1 = 100%
    public float separationWeight;      // Stat for prioritising seperation. The desire to need space from your neighbors. 1 = 100%
    public float alignmentWeight;       // Stat for prioritising alignment. The desire to face the same direction as your neighbors. 1 = 100%
    public float alignmentSpeed;        // How hard will the boid make alignment adjustments. The actual aligning occurs in the calculation method for reasons.

    [Header("Fish Facts")]               // This is just silly scriptable object test data (for now)
    public string maidenName;           // UNUSED
    public string favoriteSong;         // UNUSED
    public int luckyNumber;             // UNUSED
    public Sprite sketch;               // This is the Image of the fish

    ////////////////////////////////////////////////////////////////////
    //RUNTIME
    ////////////////////////////////////////////////////////////////////
    #region Runtime
    void Awake()
    {
        //initialize the list of all boids in the pond
        GetAllBoids();
        //Choose a fish from the array of scriptable object fish if you dont already have one assigned
        if (fish == null)
        {
            if (!isLure)
            {
                if (minnowCount >= 10)
                {
                    //Debug.Log("Minnow Count Maxed: Minnow Blocked");
                    SelectBoid(Random.Range(1, allFish.Length));
                }
                else
                {
                    //Debug.Log("Minnow Count Not Reached: Minnow Unlocked");
                    SelectBoid(Random.Range(0, allFish.Length));
                }
            }
        }

        //initialize variables based on Scriptable Object input
        BecomeBoid();
        
        
        //Start the hunger clock
        
        if (!isLure && maidenName != "Kavkiin Minnow")
        {
            StartCoroutine(EncroachingHunger());
        }

    }

    private void OnEnable()
    {
        //FishSpawner.onSpawn += GetAllBoids;
        BoidBehavior.onDeath += GetAllBoids;
    }
    
    void FixedUpdate()
    {
        if (!isDead && !isLure)
        {
            ApplySwimBehavior();
            currentSpeed = swimSpeed;

            if (!isHooked)
            {
                ApplyUprightBehavior();
                ApplyObstacleAvoidanceBehavior();

                //Fill the neighbor list using this GetNeighbors method
                neighbors = GetNeighborsWithinPerceptionRange();

                if (isSchooling)
                {
                    ApplySchoolingBehavior();
                }

                if (isHungry)
                {
                    ApplyHuntingBehavior();
                }

                if (isBeingHunted && predatorDistance.magnitude < perceptionRadius)
                {
                    if (!isPanicking)
                    {
                        StartCoroutine(PanicCoroutine());
                    }
                }

                if (neighbors == null || neighbors.Count <= 0 && !isHungry)
                {
                    ApplyDeviateBehavior(deviateChance);
                }
            }

            if (isHooked)
            {
                ApplyHookedBehavior();
            }

        }

        //DEBUG: Refresh the Boid list
        if (Input.GetKeyDown("b"))
        {
            GetAllBoids();
        }

        //DEBUG: Shuffle all Boids and refresh Lists
        if (Input.GetKeyDown("s"))
        {
            ShuffleBoid();
        }

        //DEBUG: Kill all Boids and refresh Lists.
        if (Input.GetKeyDown("k"))
        {
            Die();
        }
        
        if (Input.GetKeyDown("d"))
        {
            StartCoroutine(DeviateCoroutine());
        }

        if (Input.GetKeyDown("j"))
        {
            //SaveFish();
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Neighbor Perception Sphere Debug Visuals
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);

        //Obstacle Reaction Sphere Debug Visuals
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
    #endregion
    ////////////////////////////////////////////////////////////////////
    // INITIALIZING METHODS
    ////////////////////////////////////////////////////////////////////
    void SelectBoid(int fishNumber)
    {
        isDead = false;

        //Spawn the updatedFishNumber
        fish = allFish[fishNumber];
    }
    void BecomeBoid()
    {
        // Find your chatbubble
        chatBubble = transform.Find("ChatBubble").GetComponent<ChatBubble>();
        // Find your rigidbody
        rb = GetComponent<Rigidbody>();
        // ensure we are not using gravity, because we only enable it for a dead fish

            rb.useGravity = false;
        
        
        // Find the name tag object on the prefab
        //label = GetComponentInChildren<TMP_Text>();

        //Clear out any current mesh being held from a previous scriptable object.
        if (currentMesh != null)
        {
            Destroy(currentMesh);
        }

        //take the scriptable object data and update this boid's values.
        if (fish != null)
        {
            // ART
            mesh = fish.mesh;

            //FIND WATER LEVEL


            //ABILITY STATS
            isLure = fish.isLure;
            swimSpeed = fish.swimSpeed;
            swimHuntSpeed = fish.swimHuntSpeed;
            swimEscapeSpeed = fish.swimEscapeSpeed;
            deviateRange = fish.deviateRange;
            deviateChance = fish.deviateChance;
            perceptionRadius = fish.perceptionRadius;
            avoidanceRadius = fish.avoidanceRadius;
            avoidanceSeconds = fish.avoidanceSeconds;

            //FOOD STATS
            sizeMultiplier = Random.Range(fish.minSizeMultiplier,fish.maxSizeMultiplier);
            if (!isLure)
            {
                foodScore = fish.foodScore * sizeMultiplier;
            }
            else foodScore = fish.foodScore;
            foodScoreMax = fish.foodScoreMax;
            scoreDifferenceThreshold = fish.scoreDifferenceThreshold;
            hungerWeight = fish.hungerWeight;
            hungryInSeconds = fish.hungryInSeconds;
            biteRange = fish.biteRange;
            decompositionTime = fish.decompositionTime;

            //BOID BEHAVIORS
            cohesionWeight = fish.cohesionWeight;
            separationWeight = fish.separationWeight;
            alignmentWeight = fish.alignmentWeight;
            alignmentSpeed = fish.alignmentSpeed;
            isSchooling = fish.isSchoolingOnSpawn;

            //FISH FACTS
            maidenName = fish.maidenName;
            favoriteSong = fish.favoriteSong;
            luckyNumber = fish.luckyNumber;
            sketch = fish.sketch;

            //Instantiate the mesh and store the instance for later deleting.
            currentMesh = Instantiate(mesh,transform);
            SetSize(sizeMultiplier);

            //Fill in the name tag on the prefab
            //label.text = maidenName;
            //label.text = foodScore.ToString();

            if (isLure)
            {
                comboMeter = 0;
            }
        }

        //If there is no scriptable object found, enable gravity. A fish skeleton will sink to the bottom.
        if (fish == null)
        {
            isDead = true;
            rb.useGravity = true;
            //label.text = "Dead";
            foodScore = -1f;

            //ABILITY STATS
            swimSpeed = 0f;
            perceptionRadius = 0f;
            avoidanceRadius = 0f;
            avoidanceSeconds = 0f;

            //BOID BEHAVIORS
            cohesionWeight = 0f;
            separationWeight = 0f;
            alignmentWeight = 0f;
            alignmentSpeed = 0f;

            //FISH FACTS
            maidenName = "Boner";
            favoriteSong = "Dem Bones - Alice in Chains";
            luckyNumber = 13;
        }

    }
    void SetSize(float size)
    {
        currentMesh.transform.localScale = new Vector3(size, size, size);
    }
    void GetAllBoids()
    {    
        boids = new List<BoidBehavior>(GameObject.FindObjectsOfType<BoidBehavior>());

        minnowCount = 0;

        foreach (BoidBehavior otherBoid in boids)
        {
            if (otherBoid.maidenName == "Kavkiin Minnow")
            {
                minnowCount++;
            }
        }
        //Debug.Log("Boids Initialized! Found Boids: " + boids.Count + "Found Minnows: " + minnowCount);
    }
    void ShuffleBoid()
    {
        //Choose a fish from the array of scriptable object fish
        SelectBoid(Random.Range(0, allFish.Length));
        //initialize variables based on Scriptable Object input
        BecomeBoid();
        //initialize the list of all boids in the pond
        GetAllBoids();
    }
    ////////////////////////////////////////////////////////////////////
    // BEHAVIOR METHODS
    ////////////////////////////////////////////////////////////////////
    void ApplySwimBehavior()
    {
        Vector3 normalForward = transform.forward * currentSpeed;
        rb.velocity += normalForward * Time.deltaTime;
    }
    void ApplyDeviateBehavior(float chance)
    {
        int chanceRoll = Random.Range(1,100);

        if (chanceRoll < chance && !isDeviating)
        {
            StartCoroutine(DeviateCoroutine());
        }
    }
    void ApplyObstacleAvoidanceBehavior()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceRadius, obstacleLayer))
        {
            StartCoroutine(TurnAroundCoroutine(hit.normal));
            return;
        }
    }
    void ApplySchoolingBehavior()
    {
        //Calculate the Boid Behavior vectors using these methods, while inputing the neighbor List data
        Vector3 cohesionVector = CalculateCohesion(neighbors);
        Vector3 separationVector = CalculateSeparation(neighbors);
        Vector3 alignmentVector = CalculateAlignment(neighbors);

        //Combine Boid Behaviors into a final steering vector, with adjusted weights for variability
        Vector3 finalSteering = cohesionVector * cohesionWeight + separationVector * separationWeight + alignmentVector * alignmentWeight;

        //MOVE the rigidBody with the final steering vector by the speed of time
        rb.velocity += finalSteering * Time.deltaTime;

    }
    void ApplyHuntingBehavior()
    {
        if (!foundFood)
        {
            target = DetectFood(neighbors);
        }

        if (target != null)
        {
            currentSpeed = swimHuntSpeed;
            transform.LookAt(target.transform.position);
            target.isBeingHunted = true;

            Vector3 toTarget = transform.position - target.transform.position;

            target.predatorDistance = toTarget;

            if (toTarget.magnitude <= biteRange)
            {
                Eat(target);
                target = null;
                
            }

            return;
        }
    }
    void ApplyUprightBehavior()
    {
        //Tick time
        float timeCount = 0f;
        timeCount += Time.deltaTime;

        //Calculate the target rotation
        Quaternion startRotation = transform.rotation;
        Vector3 rotationVector = new Vector3(startRotation.x, startRotation.y, 0);
        Quaternion targetRotation = startRotation * Quaternion.Euler(rotationVector);

        //Apply the rotation
        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeCount / 0.5f);
        
    }
    void ApplyHookedBehavior()
    {
        if (!isHookSet)
        {
            SetTheHook();
        }
        
        if (fishingRod != null)
        {
            //move away from rod
            transform.LookAt(-fishingRod.transform.position);
            
            //Deviate TIMES TEN!
            ApplyDeviateBehavior(deviateChance * 10f);

            Debug.DrawLine(transform.position, transform.position + transform.forward * 2.0f, Color.green);
            Debug.DrawLine(transform.position, fishingRod.transform.position - transform.position * 2.0f, Color.magenta);

            if (!ControllerInputManager.instance.isReeling)
            {
                StartCoroutine(TugTheLineCoroutine());
            }

            if (fishingRod.isReeled)
            {
                Land(this);
            }
        }
    }
    ////////////////////////////////////////////////////////////////////
    // FISHING ROD METHODS
    ////////////////////////////////////////////////////////////////////
    void SetTheHook()
    {
        if (!isLure)
        {
            chatBubble.playEmote(ChatBubble.EmoteType.Hooked);
            foundFood = false;
            isHungry = false;
            StopCoroutine(EncroachingAge());
            StopCoroutine(EncroachingHunger());
            StartCoroutine(PanicCoroutine());
        }

        isHookSet = true;
        hook = GameObject.Find("Hook").GetComponent<ConfigurableJoint>();
        transform.position = hook.transform.position;
        hook.connectedBody = rb;

        hook.angularXMotion = ConfigurableJointMotion.Free;
        hook.angularYMotion = ConfigurableJointMotion.Free;
        hook.angularZMotion = ConfigurableJointMotion.Free;

        fishingRod = GameObject.Find("FishingRod").GetComponent<FishingRod>();
        fishingRod.hookHasFish = true;
        fishingRod.hookedFish = this.gameObject;
        fishingRod.Bite();
    }
    public void Unhook()
    {
        isHooked = false;
        isHookSet = false;

        //Fish escaped, end the fight
        StopCoroutine(TugTheLineCoroutine());
        isFightingBack = false;

        if (hook.connectedBody != null)
        {
            hook.connectedBody = null;
        }
        
        if (fishingRod != null)
        {
            fishingRod.hookHasFish = false;
            fishingRod.hookedFish = null;
        }

        //The escaped fish unlocks hidden potential! Max food score up. (Exploitable??)
        StopCoroutine(EncroachingHunger());
        foodScoreMax++;
        StopCoroutine(EncroachingAge());
        isElderly = false;
        StartCoroutine(EncroachingHunger());
        comboMeter = 1;

    }
    void Land(BoidBehavior caught)
    {
        AudioManager.instance.FishOut();
        fishingRod.Catch(caught);
        fishingRod.hookHasFish = false;
        Destroy(this.gameObject);
    }
    ////////////////////////////////////////////////////////////////////
    // VERB METHODS
    ////////////////////////////////////////////////////////////////////
    void Eat(BoidBehavior boid)
    {
        chatBubble.playEmote(ChatBubble.EmoteType.Happy);

        isHungry = false;
        foundFood = false;
        StartCoroutine(EncroachingHunger());
        
        if (!boid.isLure && !boid.isHooked)
        {
            foodScore++;
            sizeMultiplier += .5f;
            SetSize(sizeMultiplier);
        }

        if (boid.isHooked)
        {
            isHookSet = false;
            isHooked = true;
            
            comboMeter += boid.comboMeter;
            AudioManager.instance.Combo(comboMeter - 1f);
            Debug.Log("COMBO! x" + comboMeter);
        }

        boid.Die();

        if (foodScore >= foodScoreMax)
        {
            StartCoroutine(EncroachingAge());
        }
    }
    void Die()
    {
        if (isLure)
        {
            Destroy(this.gameObject);
        }

        isHooked = false;
        isHookSet = false;
        fish = null;
        isDead = true;
        onDeath(); //Call out the death event
        chatBubble.playEmote(ChatBubble.EmoteType.Dead);
        BecomeBoid();
        GetAllBoids();
        StartCoroutine(Decompose());
    }
    void StimulateAppetite(float chance)
    {
        Debug.Log("Bait Spotted: Rolling For Appetite");
        int chanceRoll = Random.Range(1, 100);

        if (chanceRoll < chance && maidenName != "Kavkiin Minnow")
        {
            Debug.Log("Roll Success: Now Hungry");
            isHungry = true;
        }
    }
    ////////////////////////////////////////////////////////////////////
    // PERCEPTION METHODS
    ////////////////////////////////////////////////////////////////////
    List<BoidBehavior> GetNeighborsWithinPerceptionRange()
    {
        //Refresh the neighbors List
        neighbors = new List<BoidBehavior>();

        // Iterate through all boids in the scene
        foreach (BoidBehavior otherBoid in boids)
        {
            // if the otherBoid is THIS, skip it in the loops
            if (otherBoid == this)
            {
                continue;
            }

            if (otherBoid != null)
            {
                //Find the distance between THIS gameObject and the otherBoid
                float distance = Vector3.Distance(transform.position, otherBoid.transform.position);

                //If this distance is close enough to be percieved, add it to the neighbors list!
                if (distance < perceptionRadius)
                {
                    neighbors.Add(otherBoid);

                    if (otherBoid.isLure || otherBoid.isHooked)
                    {
                        StimulateAppetite(90f);
                    }
                }
            }
        }

        //Once the loop has been made for all of the otherBoids, then return the compiled list
        return neighbors;
    }
    BoidBehavior DetectFood(List<BoidBehavior> neighbors)
    {
        if (neighbors.Count == 0)
        {
            foundFood = false;
            return null;
        }

        BoidBehavior target = null;

        foreach (BoidBehavior neighbor in neighbors)
        {
            if (neighbor.isLure || neighbor.isHooked)
            {
                //Debug.Log("A Level " + foodScore + " " + maidenName + " FISH SAW BAIT");

                if (neighbor.foodScore < (foodScore - scoreDifferenceThreshold) && neighbor.foodScore > 0)
                {
                    //Debug.Log("FISH WANTS BAIT");
                    foundFood = true;
                    target = neighbor;
                    Debug.DrawLine(transform.position, target.transform.position, Color.red);
                    break;
                }

            }
            //Check if the food score is lower than yours, but not below zero (dead)
            if (neighbor.foodScore < (foodScore - scoreDifferenceThreshold) && neighbor.foodScore > 0)
            {
                foundFood = true;
                target = neighbor;
                Debug.DrawLine(transform.position, target.transform.position, Color.red);
                break;
            }
            else foundFood = false;
        }
        return target;
    }
    Vector3 CalculateCohesion(List<BoidBehavior> neighbors)
    {
        //If there are no neighbors in your area, you cannot Cohere
        if (neighbors.Count == 0)
        {
            return Vector3.zero;
        }

        //init average position
        Vector3 averagePosition = Vector3.zero;

        //Add up the transforms of every neighbor in your area
        foreach (BoidBehavior neighbor in neighbors)
        {
            averagePosition += neighbor.transform.position;
        }

        //then divide them by the amount of neighbors that you have
        averagePosition /= neighbors.Count;

        //Debug.DrawLine(transform.position, averagePosition, Color.green);

        //lastly, find the difference between the average neighbor position, and THIS boid's position. Normalize it, and return.
        return (averagePosition - transform.position).normalized;
    }
    Vector3 CalculateSeparation(List<BoidBehavior> neighbors)
    {
        //If there are no neighbors in your area, you cannot Separate
        if (neighbors.Count == 0)
        {
            return Vector3.zero;
        }

        //init separation vector
        Vector3 separationVector = Vector3.zero;

        //Calculate a distance vector between THIS object and each neighbor on the List
        foreach (BoidBehavior neighbor in neighbors)
        {
            //Debug.DrawLine(transform.position, neighbor.transform.position, Color.green);
            Vector3 toNeighbor = transform.position - neighbor.transform.position;

            //Here we are dividing the vector by it's magnitude to the closer neighbors weigh more.
            //if the distance vector is low, then the result of the division will be high. 
            //then we are adding it to the total seperationVector
            separationVector += toNeighbor.normalized / toNeighbor.magnitude;
        }

        //The total seperationVector is averaged
        separationVector /= neighbors.Count;

        //normalize and return the result
        return separationVector.normalized;
    }
    Vector3 CalculateAlignment(List<BoidBehavior> neighbors)
    {
        //If there are no neighbors in your area, you cannot Align
        if (neighbors.Count == 0)
        {
            return Vector3.zero;
        }

        //init average direction
        Vector3 averageDirection = Vector3.zero;

        //Add together the relative forward vectors of every neighbor in the List
        foreach (BoidBehavior neighbor in neighbors)
        {
            averageDirection += neighbor.transform.forward;
        }

        //average the total
        averageDirection /= neighbors.Count;

        // Debug line to visualize the average direction
       // Debug.DrawLine(transform.position, transform.position + averageDirection, Color.magenta);

        //MANUALLY TURNING THE TRANSFORM
        // Calculate the rotation needed to face the average direction
        Quaternion targetRotation = Quaternion.LookRotation(averageDirection);
        // Smoothly interpolate towards the target rotation (you can adjust the speed)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * alignmentSpeed);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * alignmentSpeed);
        //by subtracting THIS boids forward vector, we can know how far it must turn to reach the average direction
        return (averageDirection - transform.forward).normalized;
        //return averageDirection.normalized;
    }
    

    /*
    void SaveFish()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(Application.dataPath + "savedFish.json", json);
    }
    */

    ////////////////////////////////////////////////////////////////////
    //COROUTINES
    ////////////////////////////////////////////////////////////////////
    IEnumerator TurnAroundCoroutine(Vector3 obstacleNormal)
    {
        float elapsedTime = 0f;


        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(obstacleNormal);

        while (elapsedTime < avoidanceSeconds)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / avoidanceSeconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is set
        
        transform.rotation = targetRotation;
    }

    IEnumerator DeviateCoroutine()
    {
        isDeviating = true;
        float elapsedTime = 0f;

        Quaternion startRotation = transform.rotation;
        Vector3 rotationVector = new Vector3(0, Random.Range(-deviateRange,deviateRange) ,0 );
        Quaternion targetRotation = startRotation * Quaternion.Euler(rotationVector);

        while (elapsedTime < .5f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDeviating = false;
    }

    IEnumerator PanicCoroutine()
    {
        isPanicking = true;
        chatBubble.playEmote(ChatBubble.EmoteType.Scared);
        
        
        currentSpeed = swimEscapeSpeed;
        isHungry = false;
        foundFood = false;
  


        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isPanicking = false;
        currentSpeed = swimSpeed;
        StartCoroutine(EncroachingHunger());

        
    }
    IEnumerator TugTheLineCoroutine()
    {
        while (!isFightingBack)
        {
            isFightingBack = true;

            //TESTING VARIABLE SCHOOLING BEHAVIOR: Stop Schooling
            isSchooling = false;

            yield return new WaitForSeconds(0.1f);
            
            if(fishingRod != null) //did we lose the rod in the last .1 second check
            {
                if (fishingRod.rodToBobberString.maxDistance < fishingRod.rodToBobberStringSlack)
                {
                    fishingRod.rodToBobberString.maxDistance += (0.05f * foodScore);
                    Debug.Log("Tugging The Line!");
                }

                if (fishingRod.rodToBobberString.maxDistance >= fishingRod.rodToBobberStringSlack)
                {
                    fishingRod.bobberToHookLineHealth -= 3f; // HIT 3 TIMES IF THE DISTANCE IS MAXED OUT!
                    Debug.Log("MAX TUG! HIT x3");
                }

                if (fishingRod.RTBLineSnapped || fishingRod.BTHLineSnapped)
                {
                    isFightingBack = false;
                    //TESTING VARIABLE SCHOOLING BEHAVIOR: Return to default schooling behavior
                    if (fish != null)
                    {
                        isSchooling = fish.isSchoolingOnSpawn;
                    }
                    break;
                }

                //TESTING VARIABLE SCHOOLING BEHAVIOR: Return to default schooling behavior
                if (fish != null)
                {
                    isSchooling = fish.isSchoolingOnSpawn;
                }
                isFightingBack = false;
            }
        }
    }
    IEnumerator EncroachingHunger()
    {
        float elapsedTime = 0f;

        while (elapsedTime < (hungryInSeconds * sizeMultiplier))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isDead && maidenName != "Kavkiin Minnow" && !isHooked) 
        {
            isHungry = true;
            chatBubble.playEmote(ChatBubble.EmoteType.Hungry);
        }

    }
    IEnumerator EncroachingAge()
    {
        chatBubble.playEmote(ChatBubble.EmoteType.Old);
        isElderly = true;
        isHungry = false;

        float elapsedTime = 0f;

        while (elapsedTime < 70f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isDead && !isHooked)
        {
            Die();
        }

    }
    IEnumerator Decompose()
    {
        float scale = 1f;
        yield return new WaitForSeconds(decompositionTime);

        while (scale >= 0f)
        {
            scale -= 0.005f;
            
            //We are not using SetScale() here because that only effects the currentMesh. We want to shrink the whole prefab.

            transform.localScale -= new Vector3(0.005f, 0.005f, 0.005f);

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
