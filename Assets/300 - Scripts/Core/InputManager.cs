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
    [HideInInspector] public UnityEvent OnCharacterSlot2;
    [HideInInspector] public UnityEvent OnCharacterSlot3;
    [HideInInspector] public UnityEvent OnCharacterSlot4;
    [HideInInspector] public UnityEvent OnCharacterSlot5;

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

    void OnSubmit()
    {
        if(playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_UI_MAP)
        {
            OnUISelect?.Invoke();
        }
    }

    void OnCancel()
    {
        OnUICancel?.Invoke();
    }

    void OnSlot1()
    {
        
        OnCharacterSlot1?.Invoke();
        
    }

    void OnSlot2()
    {
        OnCharacterSlot2?.Invoke();
    }

    void OnSlot3()
    {
        OnCharacterSlot3?.Invoke();
    }

    void OnSlot4()
    {
        OnCharacterSlot4?.Invoke();
    }

    void OnSlot5()
    {
        OnCharacterSlot5?.Invoke();
    }

    #endregion
}
