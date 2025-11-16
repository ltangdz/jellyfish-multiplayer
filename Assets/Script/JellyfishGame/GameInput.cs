using Script.EventSystem;
using UnityEngine.InputSystem;

/// <summary>
/// 新输入系统 处理玩家输入
/// </summary>
public class GameInput : Singleton<GameInput>
{
    // 输入系统
    private PlayerInput playerInput;
    public InputAction moveLeftAction;
    public InputAction moveRightAction;
    public InputAction previewAction;
    public InputAction dropAction;

    private bool isMoveLeft;
    public bool IsMoveLeft
    {
        get => isMoveLeft;
        set => isMoveLeft = value;
    }

    private bool isMoveRight;
    public bool IsMoveRight
    {
        get => isMoveRight;
        set => isMoveRight = value;
    }

    // 只有一个为真时 说明在移动
    public bool IsMove => isMoveLeft ^ isMoveRight;


    protected override void Awake()
    {
        base.Awake();
    
        playerInput = GetComponent<PlayerInput>();
        var inputActions = playerInput.actions;
        moveLeftAction = inputActions[Settings.INPUTACTION_MOVELEFT];
        moveRightAction = inputActions[Settings.INPUTACTION_MOVERIGHT];
        previewAction = inputActions[Settings.INPUTACTION_PREVIEW];
        dropAction = inputActions[Settings.INPUTACTION_DROP];
        
        // 初始时先禁用输入
        playerInput.actions.Disable();
        // Debug.Log("输入禁用");
    }

    private void OnEnable()
    {
        moveLeftAction.performed += MoveLeftAction_OnPerformed;
        moveLeftAction.canceled += MoveLeftAction_OnCanceled;

        moveRightAction.performed += MoveRightAction_OnPerformed;
        moveRightAction.canceled += MoveRightAction_OnCanceled;
        
        previewAction.performed += PreviewAction_OnPerformed;
        
        dropAction.canceled += DropAction_OnCanceled;
    }


    private void OnDisable()
    {
        moveLeftAction.performed -= MoveLeftAction_OnPerformed;
        moveLeftAction.canceled -= MoveLeftAction_OnCanceled;
        
        moveRightAction.performed -= MoveRightAction_OnPerformed;
        moveRightAction.canceled -= MoveRightAction_OnCanceled;
        
        previewAction.performed -= PreviewAction_OnPerformed;
        
        dropAction.canceled -= DropAction_OnCanceled;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.gamePaused, (sender, args) => playerInput.actions.Disable());
        EventManager.Instance.AddListener(EventName.gameUnpaused, (sender, args) => playerInput.actions.Enable());
        EventManager.Instance.AddListener(EventName.gameStart, (sender, args) => playerInput.actions.Enable());
        EventManager.Instance.AddListener(EventName.gameOver, (sender, args) => playerInput.actions.Disable());
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.gamePaused, (sender, args) => playerInput.actions.Disable());
        EventManager.Instance.RemoveListener(EventName.gameUnpaused, (sender, args) => playerInput.actions.Enable());
        EventManager.Instance.RemoveListener(EventName.gameStart, (sender, args) => playerInput.actions.Enable());
        EventManager.Instance.RemoveListener(EventName.gameOver, (sender, args) => playerInput.actions.Disable());
    }

    private void MoveLeftAction_OnPerformed(InputAction.CallbackContext obj)
    {
        isMoveLeft = true;
    }
    
    private void MoveLeftAction_OnCanceled(InputAction.CallbackContext obj)
    {
        isMoveLeft = false;
    }

    private void MoveRightAction_OnPerformed(InputAction.CallbackContext obj)
    {
        isMoveRight = true;
    }

    private void MoveRightAction_OnCanceled(InputAction.CallbackContext obj)
    {
        isMoveRight = false;
    }
    
    private void PreviewAction_OnPerformed(InputAction.CallbackContext obj)
    {
        this.TriggerEvent(EventName.trailPreviewed);    
    }
    
    private void DropAction_OnCanceled(InputAction.CallbackContext obj)
    {
        this.TriggerEvent(EventName.jellyfishDropped);
    }
}
