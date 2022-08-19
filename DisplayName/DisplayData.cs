using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Speaker {idle, use, mute}

public class DisplayData : MonoBehaviour
{
    public RawImage steamImage;
    public TextMeshProUGUI textName;
    public CanvasGroup canvasGroup;

    public Texture SteamImage
    {
        set
        {
            steamImage.texture = value;
        }
    }

    public string TextName
    {
        set
        {
            textName.text = value;
        }
    }
}
