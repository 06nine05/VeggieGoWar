using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public enum Action { attack, defense, skill, item }

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool isReady;

    public bool isLocal;

    [SerializeField] private GameObject buttonList;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defenseButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button itemButton;

    [SerializeField] private int skillCount = 3;
    [SerializeField] private int itemCount = 3;
    
    private PlayerStat stat;
    private Action action;
   
    // Start is called before the first frame update
    private void Start()
    {
        stat = GetComponent<PlayerStat>();

        if (!isLocalPlayer)
        {
            attackButton.interactable = false;
            defenseButton.interactable = false;
            skillButton.interactable = false;
            itemButton.interactable = false;
        }

        else
        {
            isLocal = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActionAttack()
    {
        if (hasAuthority)
        {
            CmdActionAttack();
        }
    }

    [Command]
    private void CmdActionAttack()
    {
        RpcActionAttack();
    }

    [ClientRpc]
    private void RpcActionAttack()
    {
        action = Action.attack;
        isReady = true;
        ChangeReady(true);
        HideButton();
        GameManager.instance.CheckIfAllReady(); 
    }
    
    public void ActionDefense()
    {
        if (hasAuthority)
        {
            CmdActionDefense();
        }
    }

    [Command]
    private void CmdActionDefense()
    {
        RpcActionDefense();
    }

    [ClientRpc]
    private void RpcActionDefense()
    {
        action = Action.defense;
        isReady = true;
        ChangeReady(true);
        HideButton();
        GameManager.instance.CheckIfAllReady();
    }
    
    public void ActionSkill()
    {
        if (hasAuthority)
        {
            CmdActionSkill();
        }
    }
    
    [Command]
    private void CmdActionSkill()
    {
        RpcActionSkill();
    }

    [ClientRpc]
    private void RpcActionSkill()
    {
        action = Action.skill;
        skillCount--;
        isReady = true;
        ChangeReady(true);
        HideButton();

        if (skillCount <= 0)
        {
            skillButton.interactable = false;
        }
        
        GameManager.instance.CheckIfAllReady();
    }

    public void ActionItem()
    {
        if(hasAuthority)
        {
            CmdActionItem();
        }
    }
    
    [Command]
    private void CmdActionItem()
    {
        RpcActionItem();
    }

    [ClientRpc]
    private void RpcActionItem()
    {
        action = Action.item;
        itemCount--;
        isReady = true;
        ChangeReady(true);
        HideButton();
        
        if (itemCount <= 0)
        {
            itemButton.interactable = false;
        }
        
        GameManager.instance.CheckIfAllReady();
    }

    public PlayerStat GetStat()
    {
        return stat;
    }

    public Action GetAction()
    {
        return action;
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        this.isReady = newValue;
        if (!this.isReady)
        {
            buttonList.SetActive(true);
        }
    }

    private void ChangeReady(bool newValue)
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady(newValue);
        }
    }

    private void HideButton()
    {
        buttonList.SetActive(false);
    }
    
    public void Lose()
    {
        GameManager.instance.GetText().text = "YOU LOSE";
            
        SoundManager.instance.PlayLose();
        
        GameManager.instance.GetPanel().gameObject.SetActive(true);
    }

    [Command]
    private void CmdSetPlayerReady(bool newValue)
    {
        this.PlayerReadyUpdate(this.isReady, newValue);
    }
}
