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
    [System.NonSerialized]
    public bool stopX;
    [System.NonSerialized]
    public bool stopY;

    bool _isOnGround = true;
    bool _isMoving;
    bool _isJump;
    bool _lockMove;
    Vector3 _dir;
    float _x;
    float _y;
    int _extraJump;

    public override void Start()
    {
        base.Start();
        ControlLock.Register("Move", this);
        _extraJump = extraJumpCount;
    }

    public override void Update()
    {
        base.Update();
        if (_isOnGround)
        {
            _extraJump = extraJumpCount;
        }
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
        if(name == "Move"){
            _lockMove = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if(name == "Move"){
            _lockMove = false;
        }
    }

    void SetDirectionX()
    {
        if(_lockMove) 
            return;
        if (!_isMoving)
            return;
        _x = GetInputX();
        _anim.SetFloat("x", _x);
        _anim.SetFloat("y", _y);
    }

    void CalculateVelocity()
    {
        if(_lockMove) 
            return;
        var dirX = GetInputX() * (deltaDistance / Time.fixedDeltaTime);
        _rb.velocity = new Vector2(dirX, _rb.velocity.y);
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
}
