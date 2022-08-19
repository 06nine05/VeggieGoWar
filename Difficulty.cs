using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Difficulty : NetworkBehaviour
{
    [SyncVar(hook = nameof(DifficultyUpdate))]
    [SerializeField] private bool isHard = false;

    [SerializeField] private TextMeshProUGUI difficultyText;
    
    // Start is called before the first frame update
    void Start()
    {
        isHard = false;
        difficultyText.text = "Normal";
        difficultyText.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDifficulty()
    {
        CmdChangeDifficulty();
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeDifficulty()
    {
        DifficultyUpdate(isHard, !isHard);
    }

    private void DifficultyUpdate(bool oldValue, bool newValue)
    {
        isHard = newValue;

        if (isHard)
        {
            difficultyText.text = "Hard";
            difficultyText.color = Color.red;
        }

        else
        {
            difficultyText.text = "Normal";
            difficultyText.color = Color.yellow;
        }
    }

    public bool GetIsHard()
    {
        return isHard;
    }
}
