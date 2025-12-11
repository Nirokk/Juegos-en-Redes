using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerscore;
    private void Start()
    {
        StartCoroutine(FetchScores());
    }
    public IEnumerator FetchScores()
    {
        yield return new WaitForSeconds(1f);
        LootLockerSDKManager.GetScoreList("mostkills", 10, 0, (response) =>
        {
            if (response.success)
            {
                string names = "Names\n";
                string scores = "Scores\n";
                foreach (var score in response.items)
                {
                    names += score.player.name + "\n";
                    scores += score.score + "\n";
                }
                playerNames.text = names;
                playerscore.text = scores;
            }
            else
            {
                Debug.Log("Error getting scores");
            }
        });
    }
}
