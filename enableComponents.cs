using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class enableComponents : NetworkBehaviour
{
    public Transform target;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(true);

        DisplayUIControl displayUIControl = GetComponent<DisplayUIControl>();
        displayUIControl.SetPlayerValues();
    }
}