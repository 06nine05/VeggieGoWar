using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.NetworkRoom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameManager() { instance = this; }
    
    [SerializeField] private Transform[] spawns;
    [SerializeField] private Transform bossSpawn;
    [SerializeField] private GameObject enemy;
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button GoButton;
    
    private GameObject[] players;
    private List<Player> playersdata;
    private GameObject spawnedEnemy;
    [SerializeField] private Enemy enemydata;

    // Start is called before the first frame update
    private void Start()
    {
        playersdata = new List<Player>();
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            playersdata.Add(player.GetComponent<Player>());
            player.GetComponent<Player>().GetStat().flash = player.GetComponentInChildren<Flash>();
        }
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = spawns[i].position;
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (Player playerdata in playersdata)
        {
            if (playerdata.isReady)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (!AllReady) return;
        
        Debug.Log("Active");
        
        StartCoroutine(action());
    }

    public RectTransform GetPanel()
    {
        return panel;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }

    public void Go()
    {
        StartCoroutine(action());
        GoButton.interactable = false;
    }

    public void Lose()
    {
        foreach (var player in playersdata)
        {
            player.Lose();
        }
    }
    
    IEnumerator action()
    {
        foreach (var player in playersdata)
        {
            switch (player.GetAction())
            {
                case Action.attack:
                    if (player.isLocal)
                    {
                        player.GetStat().Attack(enemydata.GetStat());
                    }
                    break;
                
                case Action.defense:
                    if (player.isLocal)
                    {
                        player.GetStat().SetGuard(true);
                    }
                    break;
                
                case Action.skill:
                    if (player.isLocal)
                    {
                        player.GetStat().SkillAttack(enemydata.GetStat());
                    }
                    break;
                
                case Action.item:
                    if (player.isLocal)
                    {
                        player.GetStat().Heal();
                    }
                    break;
            }
        }
        
        yield return new WaitForSeconds(1);
        
        int index = Random.Range(0, playersdata.Count);
            
        enemydata.GetStat().Attack(playersdata[index].GetStat());

        yield return new WaitForSeconds(1);

        foreach (Player player in playersdata)
        {
            player.isReady = false;
        }
    }
    
}
