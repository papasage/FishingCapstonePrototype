using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishDiaryMenu : MonoBehaviour
{
    public GameObject journalEntryPrefab;
    public Transform fishListParent;

    //this is a list of all the entries we populate in the diary. We clear them when the diary is closed.
    private List<GameObject> instantiatedEntries = new List<GameObject>();

    public ScrollRect scrollRect;
    public float scrollSpeed = 10f;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void OnEnable()
    {
        PopulateFishDiary();
    }
    private void OnDisable()
    {
        ClearFishDiary();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("LeftStickY") > ControllerInputManager.instance.joystickDeadzone || Input.GetAxisRaw("LeftStickY") < -ControllerInputManager.instance.joystickDeadzone) ;
        {
            float verticalInput = Input.GetAxisRaw("LeftStickY");
            float scrollDistance = verticalInput * scrollSpeed * Time.deltaTime;

            // Apply scroll
            Vector2 newPosition = scrollRect.content.anchoredPosition;
            newPosition.y += scrollDistance;
            scrollRect.content.anchoredPosition = newPosition;
        }

    }

    void PopulateFishDiary()
    {
        //grab the FishDataManager, load the fish data .json file, and make a list of what you find
        BoidBehaviorData[] caughtFish = FishDataManager.instance.LoadFishData();

        //for every boid in the list...
        foreach(BoidBehaviorData fish in caughtFish)
        {
            // make a new entry on the UI
            GameObject fishEntry = Instantiate(journalEntryPrefab, fishListParent);

            // Add the instantiated entry to the list
            instantiatedEntries.Add(fishEntry); 

            // call your method to fill the data on that entry
            PopulateFishEntry(fishEntry,fish);
        }
    }

    void PopulateFishEntry(GameObject entry, BoidBehaviorData fish)
    {
        // Assuming consistent nesting structure, access children directly
        Transform entryTransform = entry.transform;

        // Access breed text component
        TMP_Text breedText = entryTransform.Find("data/breed label/breed").GetComponent<TMP_Text>();
        // Access size text component
        TMP_Text sizeText = entryTransform.Find("data/size label/size").GetComponent<TMP_Text>();
        // Access level text component
        TMP_Text levelText = entryTransform.Find("data/level label/level").GetComponent<TMP_Text>();
        // Access combo text component
        TMP_Text comboText = entryTransform.Find("data/combo label/combo").GetComponent<TMP_Text>();
        // Access time text component
        //TMP_Text timeText = entryTransform.Find("data/time label/time").GetComponent<TMP_Text>();
        // Access sketch image component
        Image sketch = entryTransform.Find("sketch").GetComponent<Image>();

        breedText.text = fish.breed;
        sizeText.text = "x" + fish.size.ToString("F2");
        levelText.text = fish.level.ToString("F0");
        comboText.text = fish.combo.ToString();
        //timeText.text = fish.timeCaught.ToString();
        sketch.sprite = fish.sketch;
    }

    void ClearFishDiary()
    {
        foreach (GameObject entry in instantiatedEntries)
        {
            Destroy(entry); // Destroy each instantiated entry
        }
        instantiatedEntries.Clear(); // Clear the list
    }
}
