using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MuteButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI TextMesh;

    private bool _isMuted = false;

    private void Awake()
    {
        Button.onClick.AddListener(ToggleMute);
    }

    public void ToggleMute()
    {
        _isMuted = !_isMuted;
        TextMesh.text = _isMuted ? "Unmute audio" : "Mute audio";
        AudioListener.volume = _isMuted ? 0 : 1;
    }
}
