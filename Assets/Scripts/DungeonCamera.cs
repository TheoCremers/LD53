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

    private Vector2 _lastPos;
    private Vector3 _dragPos;

    private bool _cameraDrag;

    private InputManager _input;

    private bool _followTarget;

    public MimicGuy MimicGuy;

    private Bounds _camWorldBounds = new Bounds(Vector3.zero, new Vector3(10, 10, 0));

    // Start is called before the first frame update
    void Start()
    {
        _input = InputManager.i;

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
        if (_followTarget) 
        {
            CameraRef.transform.position = new Vector3(MimicGuy.transform.position.x, MimicGuy.transform.position.y, CameraRef.transform.position.z);
        }
        else if (_freeCam) 
        {
            var mousePos = _input.MousePosition.ReadValue<Vector2>();
            if (_input.RightClickAction.triggered)
            {
                _dragPos = CameraRef.ScreenToWorldPoint(mousePos);
                _cameraDrag = true;
            }
            if (_input.RightClickAction.IsPressed() && _cameraDrag)
            {   
                if (_lastPos != mousePos)
                {
                    var diff = _dragPos - CameraRef.ScreenToWorldPoint(mousePos);
                    var newPos = CameraRef.transform.position + diff;        
                    newPos.x = Mathf.Clamp(newPos.x, _camWorldBounds.min.x, _camWorldBounds.max.x);
                    newPos.y = Mathf.Clamp(newPos.y, _camWorldBounds.min.y, _camWorldBounds.max.y);
                    CameraRef.transform.position = newPos;        
                    _lastPos = mousePos;
                }
            } 
            else
            {
                _cameraDrag = false;
            }  
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
        _followTarget = true;
    }

    private void InitiateOverlordTurn()
    {
        // Zoom out
        StartCoroutine(ZoomCoroutine(1.5f, 3));

        // Allow camera movement within bounds
        _freeCam = true;
        _followTarget = false;
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
