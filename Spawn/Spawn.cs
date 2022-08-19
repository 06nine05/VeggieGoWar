using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Chat;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;
    [SerializeField] private Transform bossSpawn;
    [SerializeField] private GameObject boss;
    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = spawns[i].position;
        }

        GameObject newBoss = Instantiate(boss, bossSpawn.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
