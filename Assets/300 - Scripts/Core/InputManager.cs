using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static InputManager Instance { get; private set; }
    [SerializeField] private PlayerInput playerInputComponent;

    private Vector2 _leftStickInputVector;

    [HideInInspector] public UnityEvent<Vector2> OnUINavigation;
    [HideInInspector] public UnityEvent OnUISelect;
    [HideInInspector] public UnityEvent OnUICancel;
    [HideInInspector] public UnityEvent<Vector2> OnCharacterMovement;
    [HideInInspector] public UnityEvent OnCharacterSlot1;
    [HideInInspector] public UnityEvent OnCharacterSlot1Released;
    [HideInInspector] public UnityEvent OnCharacterSlot2;
    [HideInInspector] public UnityEvent OnCharacterSlot2Released;
    [HideInInspector] public UnityEvent OnCharacterSlot3;
    [HideInInspector] public UnityEvent OnCharacterSlot3Released;
    [HideInInspector] public UnityEvent OnCharacterSlot4;
    [HideInInspector] public UnityEvent OnCharacterSlot4Released;
    [HideInInspector] public UnityEvent OnCharacterSlot5;
    [HideInInspector] public UnityEvent OnCharacterSlot5Released;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerInputComponent.SwitchCurrentActionMap(playerInputComponent.defaultActionMap);
    }

    #region Message Handlers

    public void OnMove(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            _leftStickInputVector = obj.ReadValue<Vector2>();
            OnCharacterMovement?.Invoke(_leftStickInputVector);
            //Logger.Core("InputManager: OnMove: " + _leftStickInputVector);
        }
    }

    public void OnSubmit(InputAction.CallbackContext obj)
    {
        if(obj.performed && playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_UI_MAP)
        {
            OnUISelect?.Invoke();
        }
    }

    public void OnCancel(InputAction.CallbackContext obj)
    {
        if(obj.performed && playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_UI_MAP)
            OnUICancel?.Invoke();
    }

    bool slot1Pressed;
    public void OnSlot1(InputAction.CallbackContext obj)
    {
        if(!slot1Pressed && obj.phase == InputActionPhase.Started)
        {
            slot1Pressed = true;
            OnCharacterSlot1?.Invoke();
        }
        else if(slot1Pressed && obj.phase == InputActionPhase.Canceled)
        {
            slot1Pressed = false;
            OnCharacterSlot1Released?.Invoke();
        }
    }

    bool slot2Pressed;
    public void OnSlot2(InputAction.CallbackContext obj)
    {
        if(!slot2Pressed && obj.phase == InputActionPhase.Started)
        {
            slot2Pressed = true;
            OnCharacterSlot2?.Invoke();
        }
        else if(slot2Pressed && obj.phase == InputActionPhase.Canceled)
        {
            slot2Pressed = false;
            OnCharacterSlot2Released?.Invoke();
        }
    }

    bool slot3Pressed;
    public void OnSlot3(InputAction.CallbackContext obj)
    {
        if(!slot3Pressed && obj.phase == InputActionPhase.Started)
        {
            slot3Pressed = true;
            OnCharacterSlot3?.Invoke();
        }
        else if(slot3Pressed && obj.phase == InputActionPhase.Canceled)
        {
            slot3Pressed = false;
            OnCharacterSlot3Released?.Invoke();
        }
    }
    
    bool slot4Pressed;
    public void OnSlot4(InputAction.CallbackContext obj)
    {
        if(!slot4Pressed && obj.phase == InputActionPhase.Started)
        {
            slot4Pressed = true;
            OnCharacterSlot4?.Invoke();
        }
        else if(slot4Pressed && obj.phase == InputActionPhase.Canceled)
        {
            slot4Pressed = false;
            OnCharacterSlot4Released?.Invoke();
        }
    }

    bool slot5Pressed;
    public void OnSlot5(InputAction.CallbackContext obj)
    {
        if(!slot5Pressed && obj.phase == InputActionPhase.Started)
        {
            slot5Pressed = true;
            OnCharacterSlot5?.Invoke();
        }
        else if(slot5Pressed && obj.phase == InputActionPhase.Canceled)
        {
            slot5Pressed = false;
            OnCharacterSlot5Released?.Invoke();
        }
    }

    #endregion
}
