using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(ActorController), typeof(PlayerDamage))]
public class Player : Actor
{
    [SerializeField] float _moveSpeed = 16.5f;
    [SerializeField] float _groundMoveAcceleration = 9.865f;
    [SerializeField] float _groundMoveDeceleration = 19f;
    [SerializeField] float _airMoveAcceleration = 5.5f;
    [SerializeField] float _airMoveDeceleration = 8f;
    [SerializeField] float _runDustEffectDelay = 0.2f;

    [Space(10)]
    [SerializeField] float _jumpForce = 24f;
    [SerializeField] float _jumpHeight = 4.5f;
    [SerializeField] float _maxCoyoteTime = 0.06f;
    [SerializeField] float _maxJumpBuffer = 0.25f;

    [Space(10)]
    [SerializeField] int _power = 3;
    [SerializeField] float _basicAttackKnockBackForce = 8.0f;

    [Space(10)]
    [SerializeField] AudioClip _swingSound;
    [SerializeField] AudioClip _attackHitSound;
    [SerializeField] AudioClip _stepSound;
    [SerializeField] AudioClip _jumpSound;

    float _coyoteTime;
    float _maxJumpHeight;
    float _moveX;
    float _nextRunDustEffectTime = 0;

    bool _canMove = true;

    int _xAxis;
    bool _leftMoveInput;
    bool _rightMoveInput;

    bool _jumpInput;
    bool _jumpDownInput;
    float _jumpBuffer;

    bool _attackInput;

    bool _isJumped;
    bool _isFalling;
    bool _isAttacking;
    bool isBeingKnockedBack;

    KnockBack _basicAttackKnockBack = new KnockBack();

    Transform _backCliffChecked;
    Coroutine _knockedBackCoroutine = null;
    PlayerAttack _attack;
    PlayerDamage _damage;

    protected override void Awake()
    {
        base.Awake();

        _backCliffChecked = transform.Find("BackCliffChecked").GetComponent<Transform>();
        _damage = GetComponent<PlayerDamage>();
        _attack = GetComponent<PlayerAttack>();

        _basicAttackKnockBack.force = _basicAttackKnockBackForce;

        _damage.KnockBack += OnKnockedBack;
        _damage.Died += OnDied;
    }

    void Start()
    {
        if (GameManager.instance.playerStartPos != Vector2.zero)
        {
            actorTransform.position = GameManager.instance.playerStartPos;
            if (GameManager.instance.playerResurrectionPos == Vector2.zero)
            {
                GameManager.instance.playerResurrectionPos = GameManager.instance.playerStartPos;
            }
        }
        else
        {
            GameManager.instance.playerStartPos = actorTransform.position;
            GameManager.instance.playerResurrectionPos = actorTransform.position;
        }

        if (GameManager.instance.playerStartlocalScaleX != 0)
        {
            actorTransform.localScale = new Vector3(GameManager.instance.playerStartlocalScaleX, 1, 1);
        }
        else
        {
            GameManager.instance.playerStartlocalScaleX = actorTransform.localScale.x;
        }

        _damage.CurrentHealth = _damage.maxHealth;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
        }
    }

    void Update()
    {
        if (isDead) return;

        deltaTime = Time.deltaTime;

        HandleInput();
        HandleMove();
        HandleJump();
        HandleFallingAndLanding();
        HandleAttack();

        AnimationUpdate();
    }

    void HandleInput()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.Play) return;

        _leftMoveInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.MoveLeft);
        _rightMoveInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.MoveRight);
        _xAxis = _leftMoveInput ? -1 : _rightMoveInput ? 1 : 0;

        _jumpInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.Jump);
        _jumpDownInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Jump);

        _attackInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Attack);
    }

    void HandleMove()
    {
        if (!_canMove) return;

        if(_xAxis != 0)
        {
            _moveX += (controller.IsGrounded ? _groundMoveAcceleration : _airMoveAcceleration) * deltaTime;

            if (actorTransform.localScale.x != _xAxis)
            {
                if(!_isAttacking)
                {
                    Flip();
                    _moveX = 0;
                    if(controller.IsGrounded)
                    {
                        _nextRunDustEffectTime = 0;
                    }
                }
            }

            if(controller.IsGrounded)
            {
               _nextRunDustEffectTime -= deltaTime;
               if(_nextRunDustEffectTime <= 0)
               {
                    ObjectPoolManager.instance.GetPoolObject("RunDust", actorTransform.position, actorTransform.localScale.x);
                    _nextRunDustEffectTime = _runDustEffectDelay;
                    if (_stepSound != null)
                        SoundManager.instance.SoundEffectPlay(_stepSound);
                    GamepadVibrationManager.instance.GamepadRumbleStart(0.02f, 0.017f);
               }
            }
        }
        else
        {
            _moveX -= (controller.IsGrounded ? _groundMoveDeceleration : _airMoveDeceleration) * deltaTime;
            _nextRunDustEffectTime = 0;
        }
        _moveX = Mathf.Clamp(_moveX, 0f, 1f);
        controller.VelocityX = _xAxis * _moveSpeed * _moveX;
    }

    void HandleJump()
    {
        if (!_isJumped)
        {
            if(JumpInputBuffer() && CoyoteTime())
            {
                if (!_isAttacking && !isBeingKnockedBack)
                {
                    StartJump();
                }
            }
        }
        else
        {
            ContinueJumping();
        }
    }

    void StartJump()
    {
        _isJumped = true;

        _coyoteTime = _maxCoyoteTime;
        _jumpBuffer = _maxJumpBuffer;

        _maxJumpHeight = actorTransform.position.y + _jumpHeight;

        ObjectPoolManager.instance.GetPoolObject("JumpDust", actorTransform.position);
        GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);
        if (_stepSound != null)
            SoundManager.instance.SoundEffectPlay(_stepSound);
        if (_jumpSound != null)
            SoundManager.instance.SoundEffectPlay(_jumpSound);
    }

    void ContinueJumping()
    {
        controller.VelocityY = _jumpForce;
        if (_maxJumpHeight <= actorTransform.position.y)
        {
            _isJumped = false;
            controller.VelocityY = _jumpForce * 0.75f;
        }
        else if (!_jumpInput || controller.IsRoofed)
        {
            _isJumped = false;
            controller.VelocityY = _jumpForce * 0.5f;
        }
    }

    bool JumpInputBuffer()
    {
        if (_jumpInput)
        {
            if (_jumpBuffer < _maxJumpBuffer)
            {
                _jumpBuffer += deltaTime;
                return true;
            }
        }
        else
        {
            _jumpBuffer = 0;
        }
        return false;
    }

    bool CoyoteTime()
    {
        if(controller.IsGrounded)
        {
            _coyoteTime = 0;
            return true;
        }
        else if(_coyoteTime < _maxCoyoteTime)
        {
            _coyoteTime += deltaTime;
            return true;
        }

        return false;
    }

    void HandleFallingAndLanding()
    {
        if (controller.VelocityY < -5f)
        {
            _isFalling = true;
        }
        else if (controller.IsGrounded && _isFalling)
        {
            _isFalling = false;
            if (_stepSound != null)
                SoundManager.instance.SoundEffectPlay(_stepSound);
            ObjectPoolManager.instance.GetPoolObject("JumpDust", actorTransform.position);
            GamepadVibrationManager.instance.GamepadRumbleStart(0.25f, 0.05f);
        }
    }

    void HandleAttack()
    {
        if(_isAttacking || isBeingKnockedBack) return;

        if(_attackInput)
        {
            StartCoroutine(BasicAttack());
        }
    }

    IEnumerator BasicAttack()
    {
        _isAttacking = true;

        bool isHit = false;
        bool isNextAttacked = false;

        animator.SetTrigger(GetAnimationHash("BasicAttack"));
        animator.ResetTrigger(GetAnimationHash("AnimationEnd"));

        _basicAttackKnockBack.direction = actorTransform.localScale.x;

        if (controller.IsGrounded)
        {
            controller.SlideMove(_moveSpeed, actorTransform.localScale.x, 65f);
        }

        yield return null;

        if (_swingSound != null)
            SoundManager.instance.SoundEffectPlay(_swingSound);

        while(!IsAnimationEnded())
        {
            if(isBeingKnockedBack) break;

            if(!isHit)
            {
                if (_attack.IsAttacking(_power, _basicAttackKnockBack))
                {
                    if (_attackHitSound != null)
                        SoundManager.instance.SoundEffectPlay(_attackHitSound);
                    GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);

                    controller.SlideMove(11.5f, -actorTransform.localScale.x);

                    if (!controller.IsGrounded)
                    {
                        controller.VelocityY = 15;
                    }

                    isHit = true;
                }
            }

            if(!Physics2D.Raycast(_backCliffChecked.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground")))
            {
                controller.SlideCancle();
            }

            _canMove = !controller.IsGrounded;

            if (IsAnimatorNormalizedTimeInBetween(0.4f, 0.87f))
            {
                if (_attackInput)
                {
                    isNextAttacked = true;
                }
            }
            else if(IsAnimatorNormalizedTimeInBetween(0.87f, 1.0f))
            {
                if(isNextAttacked)
                {
                    if(_xAxis == actorTransform.localScale.x)
                    {
                        controller.SlideMove(_moveSpeed, actorTransform.localScale.x, 65f);
                    }
                    animator.SetTrigger(GetAnimationHash("NextAttack"));
                    SoundManager.instance.SoundEffectPlay(_swingSound);

                    isHit = isNextAttacked = false;
                }
            }

            yield return null;
        }

        AttackEnd();
    }

    void AttackEnd()
    {
        animator.SetTrigger(GetAnimationHash("AnimationEnd"));
        _isAttacking = false;
        _canMove = true;
    }

    void OnKnockedBack(KnockBack knockBack)
    {
        controller.UseGravity = true;
        actorTransform.Translate(new Vector2(0, 0.1f));

        controller.SlideMove(knockBack.force, knockBack.direction);
        _canMove = false;

        _isAttacking = _isJumped = false;
        isBeingKnockedBack = true;
        GamepadVibrationManager.instance.GamepadRumbleStart(0.8f, 0.068f);

        if(!isDead)
        {
            controller.VelocityY = 24f;
            if(_knockedBackCoroutine != null)
            {
                StopCoroutine(_knockedBackCoroutine);
                _knockedBackCoroutine = null;
            }
            _knockedBackCoroutine = StartCoroutine(KnockedBackCoroutine());
        }
    }

    IEnumerator KnockedBackCoroutine()
    {
        animator.SetTrigger(GetAnimationHash("KnockBack"));

        while(controller.IsSliding)
        {
            yield return null;
        }

        _canMove = true;
        isBeingKnockedBack = false;

        animator.SetTrigger(GetAnimationHash("KnockBackEnd"));

        _knockedBackCoroutine = null;
    }

    IEnumerator OnDied()
    {
        isDead = true;
        
        if (GameManager.instance != null)
        {
            GameManager.instance.HandlePlayerDeath();
        }
        
        if (animator != null)
        {
            animator.SetTrigger(GetAnimationHash("Die"));
        }
        
        if (ScreenEffect.instance != null)
        {
            ScreenEffect.instance.BulletTimeStart(0.3f, 1.0f);
        }
        
        yield return YieldInstructionCache.WaitForSecondsRealtime(1.5f);

        if (ScreenEffect.instance != null)
        {
            ScreenEffect.instance.TimeStopCancle();
        }
        
        Time.timeScale = 1f;

        if (GameManager.instance != null && GameManager.instance.playerStartPos == Vector2.zero)
        {
            GameManager.instance.playerStartPos = new Vector2(0, 0);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void AnimationUpdate()
    {
        animator.SetFloat(GetAnimationHash("MoveX"), _moveX);
        animator.SetFloat(GetAnimationHash("FallSpeed"), controller.VelocityY);
        animator.SetBool(GetAnimationHash("IsGrounded"), controller.IsGrounded);
    }
}
