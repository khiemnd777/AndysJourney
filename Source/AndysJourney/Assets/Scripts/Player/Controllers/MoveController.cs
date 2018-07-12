using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : PlayerController, IControlLocker
{
    public float deltaDistance = .008f;
    public float jumpForce;
    public int extraJumpCount;
    [Header("Ground check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    [Header("Collision check")]
    public float friction;
    public Transform leftCollisionCheck;
    public Transform rightCollisionCheck;
    public float collisionCheckRadius;
    public LayerMask collisionLayer;
    [System.NonSerialized]
    public bool stopX;
    [System.NonSerialized]
    public bool stopY;

    bool _isOnGround = true;
    bool _isMoving;
    bool _isJump;
    bool _isLeftCollision;
    bool _isRightCollision;
    bool _lockMove;
    bool _lockMoveOnGround;
    Vector3 _dir;
    float _y;
    int _extraJump;

    public override void Start()
    {
        base.Start();
        ControlLock.Register("Move", this);
        ControlLock.Register("MoveOnGround", this);
        _extraJump = extraJumpCount;
    }

    public override void Update()
    {
        base.Update();
        if (_isOnGround)
        {
            _extraJump = extraJumpCount;
        }
        CheckLeftCollision();
        CheckRightCollision();
        CheckOnGround();
        SetMovingState();
        SetJumpState();
        SetDirectionX();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CalculateVelocity();
        ForceForJump();
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
    }

    void SetDirectionX()
    {
        if(_lockMoveOnGround && _isOnGround)
            return;
        if (_lockMove)
            return;
        if (!_isMoving)
            return;
        _anim.SetFloat("x", _faceX);
        _anim.SetFloat("y", _y);
    }

    void CalculateVelocity()
    {
        if(_lockMoveOnGround && _isOnGround)
            return;
        if (_lockMove)
            return;
        var inputX = GetInputX();
        var dirX = inputX * (deltaDistance / Time.fixedDeltaTime);
        if ((_isLeftCollision && inputX < 0 || _isRightCollision && inputX > 0) && !_isOnGround)
        {
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * friction;
            }
        }
        else
        {
            _rb.velocity = new Vector2(dirX, _rb.velocity.y);
        }
    }

    void ForceForJump()
    {
        if (!_isJump)
            return;
        if (!_isOnGround && _extraJump > 0)
        {
            _rb.velocity = Vector2.up * jumpForce;
            _extraJump--;
        }
        else if (_isOnGround)
        {
            _rb.velocity = Vector2.up * jumpForce;
        }
    }

    void SetMovingState()
    {
        if(_lockMoveOnGround && _isOnGround){
            _anim.SetBool("isMoving", false);
            return;
        }
        if(_lockMove){
            _anim.SetBool("isMoving", false);
            return;
        }
        _isMoving = GetInputX() != 0;
        _anim.SetBool("isMoving", _isMoving);
    }

    void SetJumpState()
    {
        _isJump = Input.GetKeyDown(KeyCode.K);
        _anim.SetBool("isJump", !_isOnGround && _extraJump > 0 || _isJump);
        if (!_isOnGround && _extraJump > 0 && _isJump)
        {
            _anim.Play("Jump", -1, 0);
        }
    }

    void SetOnGroundState(bool state)
    {
        _isOnGround = state;
        _anim.SetBool("isOnGround", state);
    }

    float GetInputX()
    {
        return !stopX ? Input.GetAxisRaw("Horizontal") : 0f;
    }

    void CheckOnGround()
    {
        var state = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        SetOnGroundState(state == true);
    }

    void CheckLeftCollision()
    {
        var state = Physics2D.OverlapCircle(leftCollisionCheck.position, collisionCheckRadius, collisionLayer);
        _isLeftCollision = state == true && _faceX < 0;
    }

    void CheckRightCollision()
    {
        var state = Physics2D.OverlapCircle(rightCollisionCheck.position, collisionCheckRadius, collisionLayer);
        _isRightCollision = state == true && _faceX > 0;
    }
}
