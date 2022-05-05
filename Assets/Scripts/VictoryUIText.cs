using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryUIText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private TextMeshProUGUI _pushedText;
    [SerializeField] private TextMeshProUGUI _beenPushedText;
    [SerializeField] private TextMeshProUGUI _deathsText;

    [SerializeField] private Button _playAgain;
    [SerializeField] private Button _quit;

    private Dictionary<PlayerColors, string> _colors = new Dictionary<PlayerColors, string>() { 
        { PlayerColors.Red, "#FF0000" },
        { PlayerColors.LimeGreen,  "#32CD32"},
        { PlayerColors.Blue,  "#0000CD"},
        { PlayerColors.Yellow, "#FFFF00" },
        { PlayerColors.Orange,  "#FF4500"},
        { PlayerColors.Pink, "#FFC0CB"},
        { PlayerColors.Cyan, "#00FFFF" },
        { PlayerColors.SaddleBrown, "#8B4513" }
    };

    private void Awake()
    {

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        _playAgain.onClick.AddListener(OnPlay);
        _quit.onClick.AddListener(OnQuit);
        SetVictoryText(VictoryVolume.PlayerData.PlayerID);



        SetStats(players);
    }

    private void SetVictoryText(int playerIndex)
    {
        _winnerText.text = $"<{_colors[(PlayerColors)playerIndex]}>Player {playerIndex + 1} Wins!</color>";
    }

    private void OnPlay()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnQuit()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void SetStats (PlayerController[] players)
    {
        PlayerData mostPushed = null;
        PlayerData beenPushed = null;
        PlayerData mostDeaths = null;

        foreach (PlayerController player in players)
        {
            if (mostPushed == null)
            {
                mostPushed = player.PlayerData;
                beenPushed = player.PlayerData;
                mostDeaths = player.PlayerData;
            }
            else
            {
                if (mostPushed.HasPushedCount < player.PlayerData.HasPushedCount)
                {
                    mostPushed = player.PlayerData;
                }
                if (beenPushed.BeenPushedCount < player.PlayerData.BeenPushedCount)
                {
                    beenPushed = player.PlayerData;
                }
                if (mostDeaths.DeathCount < player.PlayerData.DeathCount)
                {
                    mostDeaths = player.PlayerData;
                }
            }
        }

        _pushedText.text = $"Biggest Asshole: <{_colors[(PlayerColors) mostPushed.PlayerID]}>Player {mostPushed.PlayerID + 1}</color>";
        _beenPushedText.text = $"Punching Bag: <{_colors[(PlayerColors)beenPushed.PlayerID]}>Player {beenPushed.PlayerID + 1}</color>";
        _deathsText.text = $"Idiot: <{_colors[(PlayerColors)mostDeaths.PlayerID]}>Player {mostDeaths.PlayerID + 1}</color>";

    }
}
