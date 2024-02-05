using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private Image emoteIcon;

    public enum EmoteType
    {
        Happy,
        Neutral,
        Angry,
        Hungry,
        Dead,
        Scared,
    }

    [SerializeField] private Sprite happyEmote;
    [SerializeField] private Sprite neutralEmote;
    [SerializeField] private Sprite angryEmote;
    [SerializeField] private Sprite hungryEmote;
    [SerializeField] private Sprite deadEmote;
    [SerializeField] private Sprite scaredEmote;

    private void Awake()
    {
        Transform canvasTransform = transform.Find("Canvas");
        if(canvasTransform != null)
        {
            emoteIcon = canvasTransform.Find("Icon_Emote").GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Chatbubble: CANVAS NOT FOUND!");
        }
    }

    public void playEmote(EmoteType type)
    {
        StartCoroutine(emote(type));
    }

    private Sprite GetEmoteSprite(EmoteType type)
    {
        switch (type)
        {
            default:
            case EmoteType.Happy: return happyEmote;
            case EmoteType.Neutral: return neutralEmote;
            case EmoteType.Angry: return angryEmote;
            case EmoteType.Hungry: return hungryEmote;
            case EmoteType.Dead: return deadEmote;
            case EmoteType.Scared: return scaredEmote;
        }
    }

    IEnumerator emote(EmoteType type)
    {
        emoteIcon.sprite = GetEmoteSprite(type);
        Color iconColor = emoteIcon.color; // Get the color as a variable
        iconColor.a = 1; // Modify the alpha of the variable
        emoteIcon.color = iconColor; // Assign the modified color back to emoteIcon.color
        float alpha = 1f;
        yield return new WaitForSeconds(5);

        while (alpha >= 0f)
        {
            alpha -= 0.005f;
            
            iconColor.a = alpha; // Modify the alpha of the variable
            emoteIcon.color = iconColor; // Assign the modified color back to emoteIcon.color
            yield return null;
        }
        yield return null;
    }
}
