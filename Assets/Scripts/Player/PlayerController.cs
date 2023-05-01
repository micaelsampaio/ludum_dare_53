

using System;
using Assets.Scripts.Map;
using Assets.Scripts.Player;
using Core.Managers;
using Core.States;
using Game.Characters;
using UnityEngine;

namespace Scripts.Player
{
  public class PlayerController : Character
  {
    public State InitialState;
    public StateMachine StateMachine;
    public PlayerInputActions Input;
    public PlayerSouls PlayerSouls;

    private Vector2 inputDirection;
    private Vector2 lookDirection;
    private bool jumpInput;
    public bool hasControl = true;
    public bool canUpdate = true;
    public bool canJump = false;
    public bool canMove = true;
    public bool canDash = true;
    public bool canUpdateCamera = true;
    public bool isJumping;

    [SerializeField] public float verticalVelocity;
    [SerializeField] private float fallingTime;

    private Transform mainCamera;

    [Header("[Player]")]

    [SerializeField] private float JumpSpeed;
    [SerializeField] private float JumpSpeedMax;
    [SerializeField] private float maxJumpTime = 0.5f;
    private bool jumpReset;
    [SerializeField] private float jumpTime = 0f;
    private float acceleration = 0f;
    private float rotationVelocity = 1f;


    [Header("[Camera]")]
    [SerializeField] private Transform CameraFollowTarget;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;
    [SerializeField] private float MouseSensibility = 1f;

    [Header("States")]
    [SerializeField] public GenericEmptyState IdleState;
    [SerializeField] public PlayerDash DashState;
    [SerializeField] public PlayerDeathState PlayerDeathState;

    private float cameraYaw;
    private float cameraPitch;

    public event IntegerEvent OnUpdateSouls;

    public override void Awake()
    {
      base.Awake();

      Input = GameManager.Instance.InputActions;
      mainCamera = GameManager.Instance.MainCamera.transform;
      jumpReset = true;
      isJumping = false;
      LoadGameState();
      SpawnObjects();
      BindInputs();
      CreateStates();

      OnDeath += (_self) =>
      {
        StateMachine.SetState(PlayerDeathState);
        StateMachine.Lock = true;
        Input.Player.Disable();
      };

      GameManager.Instance.OnEndGameEvent += () =>
      {
        canMove = false;
        canDash = false;
        canJump = false;
        StateMachine.SetState(IdleState);
        StateMachine.Lock = true;
        Animator.SetBool("move", false);
      };
    }
    private void CreateStates()
    {
      DashState.NextState = IdleState;
      var states = GetComponents<State>();
      StateMachine = new StateMachine(this, IdleState, states);
    }

    private void LoadGameState()
    {
      var playerState = GameManager.Instance.GameState.playerState;
      Hp = playerState.hp;
    }

    public override void Start()
    {
      base.Start();
      cameraYaw = CameraFollowTarget.rotation.eulerAngles.y;
      OnUpdateSouls?.Invoke(Hp);
    }

    public override void Update()
    {
      if (!canUpdate) return;

      base.Update();
      GetInputs();
      ApplyGravity();
      GroundedCheck();
      Move();
      StateMachine.Update();
      CheckVariables();
    }

    private void LateUpdate()
    {
      UpdateCameraRotation();
    }

    private void BindInputs()
    {
      Input.Player.Jump.started += (ctx) =>
      {
        jumpInput = true;
      };

      Input.Player.Jump.canceled += (ctx) =>
      {
        jumpInput = false;
      };
    }

    private void GetInputs()
    {
      inputDirection = Input.Player.Movement.ReadValue<Vector2>().normalized;

      if (isJumping && jumpInput)
      {
        jumpTime = Mathf.Clamp(jumpTime + Time.deltaTime, 0, maxJumpTime);

        if (jumpTime < maxJumpTime)
        {
          verticalVelocity = Mathf.Clamp(verticalVelocity + JumpSpeed * Time.deltaTime, JumpSpeed, JumpSpeedMax);
        }
      }
    }

    private void Move()
    {
      if (!canMove) return;

      var isMoving = inputDirection != Vector2.zero;
      var targetSpeed = Speed;
      var targetRotation = 0f;

      if (!isMoving)
      {
        targetSpeed = 0;
        acceleration = 0;
        rotationVelocity = 0;
      }

      if (isMoving)
      {
        targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, Time.deltaTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        acceleration = Mathf.Clamp(acceleration + Time.deltaTime * 2f, 0.1f, 1);
        targetSpeed *= acceleration;
      }

      Animator.SetBool("move", isMoving);

      var targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
      var velocity = targetDirection * targetSpeed * Time.deltaTime;
      velocity.y = verticalVelocity * Time.deltaTime;
      CharacterController.Move(velocity);
    }

    private void ApplyGravity()
    {
      var canjump = (IsGrounded || fallingTime < 0.5f) && jumpReset && !isJumping;

      Debug.DrawLine(transform.position, transform.position + Vector3.up * 5f, canjump ? Color.green : Color.red);

      if (IsGrounded && fallingTime > 0.2f)
      {
        Spawn("jump_dust", transform, transform);
      }

      if (jumpInput && (IsGrounded || fallingTime < 0.2f) && jumpReset && !isJumping)
      {
        jumpReset = false;
        isJumping = true;
        verticalVelocity = JumpSpeed;
        fallingTime = 0;
        jumpTime = 0.1f;

        Spawn("jump_start_dust", transform, transform);

        return;
      }

      if (isJumping)
      {

        var gravity = Utils.GRAVITY / 2;
        verticalVelocity += gravity * Time.deltaTime;

        if (verticalVelocity < 0)
        {
          isJumping = false;
        }
      }
      else
      {
        verticalVelocity = Mathf.Clamp(verticalVelocity + Utils.GRAVITY * Time.deltaTime, Utils.MAX_GRAVITY, Utils.MAX_GRAVITY_UP);
        fallingTime = IsGrounded ? 0 : fallingTime + Time.deltaTime;
        if (IsGrounded) verticalVelocity = 0f;
      }

      if (IsGrounded && !isJumping && !jumpInput && !jumpReset) jumpReset = true;
    }

    private void UpdateCameraRotation()
    {
      lookDirection = Input.Player.Look.ReadValue<Vector2>() * MouseSensibility;

      if (lookDirection.sqrMagnitude >= 0.01f)
      {
        var deltaTimeMultiplier = 1.0f;
        cameraYaw += lookDirection.x * deltaTimeMultiplier;
        cameraPitch += lookDirection.y * deltaTimeMultiplier;
      }

      cameraYaw = Utils.ClampAngle(cameraYaw, float.MinValue, float.MaxValue);
      cameraPitch = Utils.ClampAngle(cameraPitch, BottomClamp, TopClamp);
      CameraFollowTarget.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0.0f);
    }

    public void SetPosition(Vector3 position)
    {
      CharacterController.enabled = false;
      transform.position = position;
      CharacterController.enabled = true;
      CharacterController.Move(Vector3.zero);
    }
    public bool CanAddSoul(int amount)
    {
      return amount > 0;
    }
    public void AddSoul(int soulAmount)
    {
      Hp += soulAmount;
      OnUpdateSouls?.Invoke(Hp);
    }

    public int RemoveSoul()
    {
      if (Hp <= 1) return -1;

      Hp -= 1;
      OnUpdateSouls?.Invoke(Hp);
      return 1;
    }

    private void SpawnObjects()
    {
      PlayerSouls = Instantiate(PlayerSouls);
    }

    public void TakePlayerControl()
    {
      hasControl = false;
      canUpdate = false;
      CharacterController.enabled = false;
      Rigidbody.isKinematic = true;
      StateMachine.SetState(IdleState);
      Animator.SetBool("move", false);
      Animator.SetBool("isGrounded", true);
    }

    public void TakePlayerCameraControl()
    {
      canUpdateCamera = false;
    }

    public void GivePlayerControl()
    {
      if (Hp <= 0) return;

      hasControl = true;
      canUpdate = true;
      canUpdateCamera = true;
      inputDirection = Vector2.zero;
      CharacterController.enabled = true;
      verticalVelocity = 0;
      jumpReset = true;
      isJumping = false;
      IsGrounded = true;
      canMove = true;
      Rigidbody.isKinematic = false;
      Animator.SetBool("move", false);
      Animator.SetBool("isGrounded", true);
    }

    public void ResetPlayer()
    {
      Animator.SetBool("move", false);
      Animator.SetBool("isGrounded", true);

      hasControl = true;
      canUpdate = true;
      canUpdateCamera = true;
      inputDirection = Vector2.zero;
      CharacterController.enabled = true;
      verticalVelocity = 0;
      jumpReset = true;
      isJumping = false;
      IsGrounded = true;
      canMove = true;
      Rigidbody.isKinematic = false;
    }

    public void SetCheckPoint(string id, GameSpot spot)
    {
      var zoneState = GameManager.Instance.GameState.zoneState;

      if (id == zoneState.CheckPointId) return;

      if (!Physics.Raycast(transform.position, Vector3.down, out var hit, 0.5f, groundLayer, QueryTriggerInteraction.Ignore))
      {
        return;
      }

      Debug.DrawLine(transform.position, transform.position + Vector3.up * 5f, Color.yellow, 9999f);

      zoneState.CheckPointId = id;
      zoneState.CheckPointSpot = spot;
      zoneState.CheckPointPosition = transform.position;
    }

    public STATE_NAME GetStateName()
    {
      if (StateMachine == null || StateMachine.CurrentState == null) return STATE_NAME.IDLE;

      return StateMachine.CurrentState.Name;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
      if (hit.gameObject.TryGetComponent<PlatformCheckPoint>(out var platform))
      {
        platform.OnHitPlayer();
      }
    }

    private bool LastIsGrounded;

    private void CheckVariables()
    {

      if (LastIsGrounded != IsGrounded)
      {
        Animator.SetBool("isGrounded", IsGrounded);
        LastIsGrounded = IsGrounded;
      }

      if (!canDash && IsGrounded)
      {
        canDash = true;
      }

    }

  }
}
