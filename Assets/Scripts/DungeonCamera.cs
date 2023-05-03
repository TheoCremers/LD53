using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DungeonCamera : MonoBehaviour
{
    #region EventChannels

    // for example: StartOverlordTurnChannel.RaiseEvent();
    public VoidEventChannel StartOverlordTurnChannel;

    public VoidEventChannel StartIdleTurnChannel;
    public VoidEventChannel StartMimicTurnChannel;

    #endregion

    public Camera CameraRef;

    private bool _freeCam;

    private Vector2 _lastPos;
    private Vector3 _dragPos;

    private bool _cameraDrag;

    private InputManager _input;

    private bool _followTarget;
    private bool _lockedOnTarget = false;

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
        CameraRef.orthographicSize = 2;
        //InitiateIdleTurn();
    }

    void Awake()
    {
        StartOverlordTurnChannel.OnEventRaised += InitiateOverlordTurn;
        StartIdleTurnChannel.OnEventRaised += InitiateIdleTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if (_followTarget) 
        {
            var targetPosition = new Vector3(MimicGuy.transform.position.x, MimicGuy.transform.position.y + 0.13f, transform.position.z);
            //CameraRef.transform.position = targetPosition;
            if (_lockedOnTarget)
            {
                transform.position = targetPosition;
            }
            else
            {
                Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, 5f * Time.deltaTime);
                transform.position = smoothPosition;
                if ((smoothPosition - targetPosition).magnitude < 0.005f)
                {
                    _lockedOnTarget = true;
                }
            }
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
                    var newPos = transform.position + diff;        
                    newPos.x = Mathf.Clamp(newPos.x, _camWorldBounds.min.x, _camWorldBounds.max.x);
                    newPos.y = Mathf.Clamp(newPos.y, _camWorldBounds.min.y, _camWorldBounds.max.y);
                    transform.position = newPos;        
                    _lastPos = mousePos;
                }
            } 
            else
            {
                _cameraDrag = false;
            }  
        }
    }

    private void InitiateIdleTurn()
    {
        if (_followTarget == false) 
        {
            //StartCoroutine(ZoomAndMoveCoroutine(1.2f, 1f, new Vector3(MimicGuy.transform.position.x, MimicGuy.transform.position.y+0.13f, transform.position.z)));
            DOTween.Kill(CameraRef);
            CameraRef.DOOrthoSize(1f, 1.2f);
        }

        // Lock camera movement
        _freeCam = false;
        // Snap to Mimic
        // Zoom in
        //StartCoroutine(ZoomCoroutine(1.5f, 0.75f));

        // Follow Mimic around
        _followTarget = true;
        _lockedOnTarget = false;
    }



    private void InitiateOverlordTurn()
    {
        // Zoom out
        //StartCoroutine(ZoomCoroutine(1.2f, 2f));
        DOTween.Kill(CameraRef);
        CameraRef.DOOrthoSize(2f, 1.2f);

        // Allow camera movement within bounds
        _freeCam = true;
        _followTarget = false;
        _lockedOnTarget = false;
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

    private IEnumerator ZoomAndMoveCoroutine(float duration, float targetSize, Vector3 targetPos)
    {
        float initialSize = CameraRef.orthographicSize;
        Vector3 startingPos = CameraRef.transform.position;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            CameraRef.orthographicSize = Mathf.Lerp(initialSize, targetSize, Mathf.SmoothStep(0, 1.0f, timer / duration));
            CameraRef.transform.position = Vector3.Lerp(startingPos, targetPos, Mathf.SmoothStep(0, 1.0f, timer / duration));
            yield return null;
        }
    }

    private IEnumerator MoveCoroutine(float duration, Vector3 target)
    {
        Vector3 startingPos = CameraRef.transform.position;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            CameraRef.transform.position = Vector3.Lerp(startingPos, target, Mathf.SmoothStep(0, 1.0f, timer / duration));            
            yield return null;
        }
    }
}
