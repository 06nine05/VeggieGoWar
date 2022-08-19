using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    private PlayerStat stat;
    
    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public PlayerStat GetStat()
    {
        return stat;
    }
}
