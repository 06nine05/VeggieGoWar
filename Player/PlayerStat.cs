using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using TMPro;

public enum Type { player, enemy }
public class PlayerStat : NetworkBehaviour
{
    [Header("Setting")] 
    [SerializeField] private Type type;
    [SerializeField] private int maxHealth = 100;
    [SyncVar]
    [SerializeField] private int atk = 30;
    [SerializeField] private AudioSource audioSource;
    
    public Flash flash;
    
    [SerializeField] private bool isGuard = false;
    [SyncVar]
    [SerializeField] private int currentHealth;

    public bool isDead = false;

    public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);

    public event HealthChangedDelegate EventHealthChanged;

    private void Update()
    {
        if (!isDead)
        {
            if (currentHealth <= 0)
            {
                StartCoroutine(destroy());
            }
        }
    }
    
    #region Server
    [ClientRpc]
    private void RpcHealthChanged(int currentHealth, int maxHealth)
    {
        EventHealthChanged?.Invoke(currentHealth,maxHealth);
    }
    
    private void SetHealth(int value)
    {
        if (value < currentHealth && flash != null)
        {
            flash.FlashColor(Color.red);
        }
        
        else if (value > currentHealth && flash != null)
        {
            flash.FlashColor(Color.green);
        }

        currentHealth = value;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        EventHealthChanged?.Invoke(currentHealth,maxHealth);
        RpcHealthChanged(currentHealth,maxHealth);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetHealth(int value)
    {
        SetHealth(value);
    }

    public override void OnStartServer() => SetHealth(maxHealth);

    [Command(requiresAuthority = false)]
    private void CmdDealDamage(int damage)
    {
        RpcDealDamage(damage);
    }
    
    [ClientRpc]
    private void RpcDealDamage(int damage) => CmdSetHealth(Mathf.Max(currentHealth - damage, 0));

    #endregion

    private void TakeHit(int damage)
    {
        this.CmdDealDamage(damage);
    }

    public void Attack(PlayerStat target)
    {
        if (currentHealth <= 0) return;
        
        if (!target.isGuard)
        {
            target.TakeHit(atk);
            
            SoundManager.instance.Play(audioSource, SoundManager.Sound.Attack);
        }

        else
        {
            target.SetGuard(false);
        }
    }

    public void SkillAttack(PlayerStat target)
    {
        if (!target.isGuard)
        {
            target.TakeHit(atk*2);
            
            SoundManager.instance.Play(audioSource, SoundManager.Sound.Attack);
        }

        else
        {
            target.SetGuard(false);
        }
    }
    
    public void Heal()
    {
        TakeHit(-50);
        
        SoundManager.instance.Play(audioSource, SoundManager.Sound.Heal);
    }

    public void SetGuard(bool value)
    {
        if (value)
        {
            SoundManager.instance.Play(audioSource, SoundManager.Sound.Shield);
            flash.FlashColor(Color.blue);
        }

        else
        {
            SoundManager.instance.Play(audioSource, SoundManager.Sound.ShieldBreak);
            flash.FlashColor(Color.magenta);
        }

        isGuard = value;
    }

    public void SetAtk(int value)
    {
        CmdSetAtk(value);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetAtk(int value)
    {
        RpcSetAtk(value);
    }

    [ClientRpc]
    private void RpcSetAtk(int value)
    {
        atk = value;
    }

    IEnumerator destroy()
    {
        isDead = true;
        
        yield return new WaitForSeconds(1);

        if (type == Type.enemy)
        {
            GameManager.instance.GetText().text = "YOU WIN";
            
            SoundManager.instance.PlayWin();
            
            GameManager.instance.GetPanel().gameObject.SetActive(true);
        }

        else
        {
            GameManager.instance.Lose();
        }
        
        gameObject.SetActive(false);
    }
}