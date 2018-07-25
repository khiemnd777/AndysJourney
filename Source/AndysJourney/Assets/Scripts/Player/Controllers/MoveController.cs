using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : PlayerController, IControlLocker
{
    public float moveSpeed = .008f;
    public float jumpForce;
    public int extraJumpCount;
    [Header("Ground check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    [Header("Collision check")]
    public Transform behindCollisionCheck;
    public float collisionCheckRadius;
    public LayerMask collisionLayer;
    [Header("Wall sliding/jumping")]
    public float frictionY;
    public bool allowWallSliding;
    [System.NonSerialized]
    public bool stopX;
    [System.NonSerialized]
    public bool stopY;

    bool _isOnGround = true;
    bool _isMoving;
    bool _isJump;
    bool _isBehindCollision;
    bool _jumpByWall;
    bool _lockMove;
    bool _lockMoveOnGround;
    bool _lockJump;
    bool _flippedByWallSliding;
    Vector3 _dir;
    float _y;
    int _extraJump;

    public override void Start()
    {
        base.Start();
        ControlLock.Register(this, "Move", "MoveOnGround", "Jump");
        _extraJump = extraJumpCount;
    }

    public override void Update()
    {
        base.Update();
        if (_isOnGround)
        {
            _extraJump = extraJumpCount;
        }
        SetDirectionX();
        SetMovingState();
        SetJumpState();
        SetWallSlidingState();
        ForceForJump();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckBehindCollision();
        CheckOnGround();
        CalculateVelocity();
    }

    public void Lock(string name)
    {
        if (name == "Move")
        {
            _lockMove = true;
        }
        else if (name == "MoveOnGround")
        {
            _lockMoveOnGround = true;
        }
        else if (name == "Jump")
        {
            _lockJump = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "Move")
        {
            _lockMove = false;
        }
        else if (name == "MoveOnGround")
        {
            _lockMoveOnGround = false;
        }
        else if (name == "Jump")
        {
            _lockJump = false;
        }
    }

    void SetDirectionX()
    {
        if (_lockMoveOnGround && _isOnGround)
            return;
        if (_lockMove)
            return;
        if (!_isMoving)
            return;
        _anim.SetFloat("x", _player.faceX);
        _anim.SetFloat("y", _y);
    }

    void CalculateVelocity()
    {
        if (_lockMoveOnGround && _isOnGround){
            _player.HandleExtraForces();
            _player.ResetExtraForces();
            return;
        }
        if (_lockMove){
            _player.HandleExtraForces();
            _player.ResetExtraForces(); 
            return;
        }
        var inputX = _player.GetInputHorizontal();
        var dirX = inputX * moveSpeed * Time.fixedDeltaTime;
        if ((_player.isFrontCollision && (inputX > 0 && _player.faceX > 0 || inputX < 0 && _player.faceX < 0)) && !_isOnGround)
        {
            // When player is wall sliding or colliding
            if (_rb.velocity.y < 0)
            {
                _rb.gravityScale = allowWallSliding && frictionY > 0 ? 0 : _player.gravity;
                var frictionYVal = allowWallSliding ? frictionY : 0f;
                _rb.velocity = new Vector2(_rb.velocity.x, -10 / frictionYVal * Time.fixedDeltaTime);
            }
        }
        else
        {
            _rb.gravityScale = _player.gravity;
            _rb.velocity = new Vector2(dirX, _rb.velocity.y);
            // another forces.
            _player.HandleExtraForces();
            _player.ResetExtraForces();
        }
    }

    void ForceForJump()
    {
        if (_lockJump)
        {
            return;
        }
        if (!_player.isJump)
            return;
        // when jumping, the gravity value is not zero
        _rb.gravityScale = _player.gravity;
        if (!_isOnGround && _extraJump > 0)
        {
            ControlLock.ReleaseLock("Move");
            _anim.SetBool("isKickDown", false);
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _extraJump--;
        }
        else if (_extraJump <= 0 && _jumpByWall)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce); //Vector2.up * jumpForce;
        }
        else if (_isOnGround)
        {
            ControlLock.ReleaseLock("Move");
            _anim.SetBool("isKickDown", false);
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce); //Vector2.up * jumpForce;
        }
    }

    void SetMovingState()
    {
        if (_lockMoveOnGround && _isOnGround)
        {
            _anim.SetBool("isMoving", false);
            return;
        }
        if (_lockMove)
        {
            _anim.SetBool("isMoving", false);
            return;
        }
        _isMoving = _player.GetInputX() != 0 && !IsWallSlidingState();
        _anim.SetBool("isMoving", _isMoving);
    }

    void SetJumpState()
    {
        if (_lockJump)
        {
            _anim.SetBool("isJump", false);
            return;
        }
        _player.isJump = Input.GetKeyDown(KeyCode.K);
        _jumpByWall = IsWallSlidingState();
        _anim.SetBool("isJump", (!_isOnGround && _extraJump > 0 || _player.isJump) && !IsWallSlidingState());
        if (!_isOnGround && _extraJump > 0 && _player.isJump)
        {
            _anim.Play("Jump", -1, 0);
        }
    }

    void SetWallSlidingState()
    {
        var isWallSlidingState = IsWallSlidingState();
        _anim.SetBool("isWallSliding", isWallSlidingState);
        if (isWallSlidingState)
        {
            _extraJump = extraJumpCount;
            // Flip when sliding
            if (!_flippedByWallSliding)
            {
                ControlLock.Lock("FlipX");
                _flippedByWallSliding = true;
                _sprite.flipX = true;
            }
        }
        else
        {
            if (_flippedByWallSliding)
            {
                ControlLock.ReleaseLock("FlipX");
                _flippedByWallSliding = false;
                _sprite.flipX = false;
            }
        }
    }

    bool IsWallSlidingState()
    {
        return _player.isFrontCollision
                    && (_player.GetInputX() > 0 && transform.localScale.x > 0 || _player.GetInputX() < 0 && transform.localScale.x < 0)
                    && !_isOnGround
                    && allowWallSliding
                    && frictionY > 0
                    && _rb.velocity.y < 0;
    }

    void SetOnGroundState(bool state)
    {
        _isOnGround = state;
        _anim.SetBool("isOnGround", state);
    }

    void CheckOnGround()
    {
        var state = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        SetOnGroundState(state == true);
    }

    void CheckBehindCollision()
    {
        var state = Physics2D.OverlapCircle(behindCollisionCheck.position, collisionCheckRadius, collisionLayer);
        _isBehindCollision = state == true;
    }
}
