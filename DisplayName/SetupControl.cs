using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupControl : MonoBehaviour
{
    public enableComponents[] components;
    public CustomNetworkManager customNetworkManager;
    
    // Start is called before the first frame update
    void Start()
    {
        components = FindObjectsOfType<enableComponents>();
        foreach (enableComponents component in components)
        {
            component.enabled = true;
        }

        customNetworkManager = FindObjectOfType<CustomNetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Logout()
    {
        customNetworkManager.Logout();
        
        SoundManager.instance.PlayBGM();
    }
}
