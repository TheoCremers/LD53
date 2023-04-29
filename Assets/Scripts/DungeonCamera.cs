using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
    #region EventChannels

    // for example: StartOverlordTurnChannel.RaiseEvent();
    public VoidEventChannel StartOverlordTurnChannel;

    public VoidEventChannel StartMimicTurnChannel;

    #endregion

    public Camera CameraRef;

    private bool _freeCam;

    // Start is called before the first frame update
    void Start()
    {
        // Example zoom in at start
        //CameraRef.orthographicSize = 3;
        //InitiateMimicTurn();

        // Example zoom out at start
        CameraRef.orthographicSize = 1;
        InitiateOverlordTurn();
    }

    void Awake()
    {
        StartOverlordTurnChannel.OnEventRaised += InitiateOverlordTurn;
        StartMimicTurnChannel.OnEventRaised += InitiateMimicTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if (_freeCam) 
        {

        }
    }

    private void InitiateMimicTurn()
    {
        // Lock camera movement
        _freeCam = false;
        // Snap to Mimic
        // Zoom in
        StartCoroutine(ZoomCoroutine(1.5f, 1));

        // Follow Mimic around
    }

    private void InitiateOverlordTurn()
    {
        // Zoom out
        StartCoroutine(ZoomCoroutine(1.5f, 3));

        // Allow camera movement within bounds
        _freeCam = true;
    }

    private IEnumerator ZoomCoroutine(float duration, float targetSize)
    {
        float initialSize = CameraRef.orthographicSize;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            CameraRef.orthographicSize = Mathf.Lerp(initialSize, targetSize, Mathf.SmoothStep(0, 1.0f, timer / duration));
            yield return null;
        }
    }
}
