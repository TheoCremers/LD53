using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager i { get; private set; }

    [HideInInspector] public PlayerInput Input = null;
    [HideInInspector] public InputAction MousePosition = null;
    [HideInInspector] public InputAction LeftClickAction = null;
    [HideInInspector] public InputAction RightClickAction = null;

    void Awake()
    {
        i = this;
        Input = GetComponent<PlayerInput>();
        MousePosition = Input.actions["Cursor"];
        LeftClickAction = Input.actions["Leftclick"];
        RightClickAction = Input.actions["Rightclick"];
    }

    private void OnDestroy ()
    {
        if (i == this)
        {
            i = null;
        }
    }
}
