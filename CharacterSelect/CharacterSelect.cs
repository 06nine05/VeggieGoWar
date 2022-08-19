using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CharacterSelect : NetworkBehaviour
{
    public static CharacterSelect Instance;
    
    [SerializeField] private GameObject CharacterSelectDisplayer = default;
    [SerializeField] private Transform characterPreviewParent = default;
    [SerializeField] private TMP_Text characterNameText = default;
    [SerializeField] private Character[] characters = default;

    private int currentCharacterIndex = 0;
    private List<GameObject> characterInstances = new List<GameObject>();

    private PlayerObjectController playerObject;
    public GameObject leftSelectUI, rightSelectUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Start()
    {
        if (characterPreviewParent.childCount == 0)
        {
            foreach (var character in characters)
            {
                GameObject characterInstance =
                    Instantiate(character.CharacterPrefab, characterPreviewParent);
                characterInstance.SetActive(false);
                characterInstances.Add(characterInstance);
            }
        }
        
        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
        
        CharacterSelectDisplayer.SetActive(true);

        playerObject = NetworkClient.localPlayer.GetComponent<PlayerObjectController>();

    }

    private void Update()
    {
        
    }

    public void CreateSelectedCharacter(int index, enableComponents characterComponent)
    {
        GameObject characterInstance = Instantiate(characters[index].GameplayCharacterPrefab, Vector3.zero,
            Quaternion.identity, characterComponent.PlayerModel.transform);
    }

    public void OnReadyToPlay(bool isReady)
    {
        leftSelectUI.SetActive(!isReady);
        rightSelectUI.SetActive(!isReady);
    }
    
    public void RightSelect()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characterInstances.Count;
        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;

        playerObject.CmdUpdatePlayerCharacter(currentCharacterIndex);
    }

    public void LeftSelect()
    {
        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex += characterInstances.Count;
        }
        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;

        playerObject.CmdUpdatePlayerCharacter(currentCharacterIndex);
    }
}
