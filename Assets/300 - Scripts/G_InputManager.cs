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

    public UnityEvent<Vector2> OnUINavigation;
    public UnityEvent OnUISelect;
    public UnityEvent OnUICancel;

    public UnityEvent<Vector2> OnCharacterMovement;
    public UnityEvent OnCharacterSlot1;
    public UnityEvent OnCharacterSlot2;
    public UnityEvent OnCharacterSlot3;
    public UnityEvent OnCharacterSlot4;
    public UnityEvent OnCharacterSlot5;

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

    // Update is called once per frame
    void Update()
    {
        if(playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_CHARACTER_MAP)
        {
            if(_leftStickInputVector != Vector2.zero)
            {
                OnCharacterMovement?.Invoke(_leftStickInputVector);

            }
           
        }
        else if(playerInputComponent.currentActionMap.name == SettingsManager.Instance.Standards.INPUT_UI_MAP)
        {
            OnUINavigation?.Invoke(_leftStickInputVector);
        }
    }

    #region Message Handlers

    void OnMove(InputValue value)
    {
        _leftStickInputVector = value.Get<Vector2>();
        //Debug.Log("InputManager: OnMove: " + _leftStickInputVector);
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
