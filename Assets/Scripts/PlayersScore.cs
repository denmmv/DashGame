using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayersScore : MonoBehaviour
{
    [SerializeField] private data _data;
    public GameObject TabletPrefab;
    public List<GameObject> Heroes = new List<GameObject>();
    public List<int> Score = new List<int>();
    public List<HeroScore> Tablets = new List<HeroScore>();
    private int _pointsToWin;
    public NetworkRoomManager NRM;
    public GameObject EndPanel;
    public TMP_Text EndText;
    private Coroutine EndGameCoroutine;
    private float _restartCooldown;

    private void Start()
    {
        _restartCooldown = _data.RestartCooldown;
        _pointsToWin = _data.PointsToWin;
    }
    public int AddHero(GameObject hero, int score)
    {       
        Heroes.Add(hero);
        Score.Add(0);
        Tablets.Add(Instantiate(TabletPrefab, CanvasUI.instance.Panel).GetComponent<HeroScore>());
        Tablets[Tablets.Count - 1].NameText.text = "Player " + Tablets.Count.ToString();
        int buffer = Score[Heroes.Count - 1];
        Tablets[Tablets.Count - 1].ScoreText.text = buffer.ToString();
        return Heroes.Count-1;       
    }  
    public void AddPoint(int id)
    {
        Score[id]++;
        int buffer = Score[id];
        Tablets[id].ScoreText.text = buffer.ToString();
        if (buffer == _pointsToWin)
        {
            EndGameShow(id);
        }
    }
    public void EndGameShow(int id)
    {
        id++;
        EndText.text = "Player " + id + " win!";
        EndPanel.SetActive(true);
        EndGameCoroutine = StartCoroutine(endGameCoroutine());
    }
    IEnumerator endGameCoroutine()
    {
        yield return new WaitForSeconds(_restartCooldown);
        EndGame();
    }
    public void EndGame()
    {
        foreach(GameObject hero in Heroes)
        {
            Destroy(hero);
        }
        foreach(HeroScore tablet in Tablets)
        {
            Destroy(tablet.gameObject);
        }
        Heroes.Clear();
        Score.Clear();
        Tablets.Clear();
        EndPanel.SetActive(false);
        NRM.ServerChangeScene("MainScene");
        NRM.Reset();
    }
}
