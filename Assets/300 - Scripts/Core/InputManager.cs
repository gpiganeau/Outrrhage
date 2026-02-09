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

    void OnMove(InputValue value)
    {
        _leftStickInputVector = value.Get<Vector2>();
        OnCharacterMovement?.Invoke(_leftStickInputVector);
        //Logger.Core("InputManager: OnMove: " + _leftStickInputVector);
    }

    void OnSubmit(InputValue value)
    {
        if(playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_UI_MAP)
        {
            OnUISelect?.Invoke();
        }
    }

    void OnCancel(InputValue value)
    {
        OnUICancel?.Invoke();
    }

    bool slot1Pressed;
    void OnSlot1(InputValue value)
    {
        if(value.isPressed && !slot1Pressed)
        {
            slot1Pressed = true;
            OnCharacterSlot1?.Invoke();
        }
        else if(!value.isPressed)
        {
            slot1Pressed = false;
            OnCharacterSlot1Released?.Invoke();
        }
    }

    bool slot2Pressed;
    void OnSlot2(InputValue value)
    {
        if(value.isPressed && !slot2Pressed)
        {
            slot2Pressed = true;
            OnCharacterSlot2?.Invoke();
        }
        else if(!value.isPressed)
        {
            slot2Pressed = false;
            OnCharacterSlot2Released?.Invoke();
        }
    }

    bool slot3Pressed;
    void OnSlot3(InputValue value)
    {
        if(value.isPressed && !slot3Pressed)
        {
            slot3Pressed = true;
            OnCharacterSlot3?.Invoke();
        }
        else if(!value.isPressed)
        {
            slot3Pressed = false;
            OnCharacterSlot3Released?.Invoke();
        }
    }
    
    bool slot4Pressed;
    void OnSlot4(InputValue value)
    {
        if(value.isPressed && !slot4Pressed)
        {
            slot4Pressed = true;
            OnCharacterSlot4?.Invoke();
        }
        else if(!value.isPressed)
        {
            slot4Pressed = false;
            OnCharacterSlot4Released?.Invoke();
        }
    }

    bool slot5Pressed;
    void OnSlot5(InputValue value)
    {
        if(value.isPressed && !slot5Pressed)
        {
            slot5Pressed = true;
            OnCharacterSlot5?.Invoke();
        }
        else if(!value.isPressed)
        {
            slot5Pressed = false;
            OnCharacterSlot5Released?.Invoke();
        }
    }

    #endregion
}
