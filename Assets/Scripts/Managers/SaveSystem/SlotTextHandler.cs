using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class SlotTextHandler : MonoBehaviour
{
    [SerializeField] private GameObject noSaveText;
    [SerializeField] private GameObject yesSaveText;
    [SerializeField] private LocalizeStringEvent difficultyText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text lastPlayedText;
    public void UpdateSlot(SaveData data)
    {
        //if no file found
        if (data == null)
        {
            yesSaveText.SetActive(false);
            noSaveText.SetActive(true);
            return;
        }
        yesSaveText.SetActive(true);
        noSaveText.SetActive(false);

        // Update the difficulty text based on the loaded save data
        difficultyText.StringReference.TableReference = "UI";
        string key = data.difficulty switch
        {
            1 => "EASY DIFFICULTY",
            2 => "MEDIUM DIFFICULTY",
            3 => "HARD DIFFICULTY",
            _ => null
        };
        if (key != null)
        {
            difficultyText.StringReference.TableEntryReference = key;
            difficultyText.RefreshString();
        }
        else
        {
            Debug.LogWarning($"Unknown difficulty level {data.difficulty} in save slot.");
        }

        //Update time played text
        playTimeText.text = Format(data.playTime);

        //Update last played text
        DateTime lastPlayed = new(data.lastPlayed); //conversion: long -> DateTime
        lastPlayedText.text = lastPlayed.ToShortDateString();
    }
    private static string Format(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return string.Format
        (
            "{0:D3}:{1:D2}:{2:D2}",
            (int)time.TotalHours,
            time.Minutes,
            time.Seconds
        );
    }
}
