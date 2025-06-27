using Photon.Pun;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class TimeUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private float gameDuration = 120f;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject rankPanel;

    private double startTime;

    private void Start()
    {
        Manager.Instance.observer.OnGameEnd += GameOverUI;

        // 마스터가 시작 시간 설정
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonHashtable hash = new PhotonHashtable();
            hash.Add("StartTime", PhotonNetwork.Time);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];

            double elapsedTime = PhotonNetwork.Time - startTime;
            double remainingTime = gameDuration - elapsedTime;

            if (remainingTime <= 0)
            {
                timeText.text = "00:00";
                Manager.Instance.observer.EndGame();
                //GameOverUI();
                enabled = false;
                return;
            }

            int minutes = Mathf.FloorToInt((float)remainingTime / 60f);
            int seconds = Mathf.FloorToInt((float)remainingTime % 60f);

            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void GameOverUI()
    {
        rankPanel.SetActive(true);
    }
}