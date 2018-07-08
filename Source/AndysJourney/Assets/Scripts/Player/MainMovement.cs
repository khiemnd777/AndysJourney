using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMovement : MonoBehaviour
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

    Rigidbody2D _rb;
    Animator _anim;
    bool _isOnGround = true;
    bool _isMoving;
    bool _isJump;
    Vector3 _dir;
    float _x;
    float _y;
    int _extraJump;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _extraJump = extraJumpCount;
    }

    void Update()
    {
        if (_isOnGround)
        {
            _extraJump = extraJumpCount;
        }
        CheckOnGround();
        SetMovingState();
        SetJumpState();
        SetDirectionX();
    }

    void FixedUpdate()
    {
        CalculateVelocity();
        ForceForJump();
    }

    void SetDirectionX()
    {
        if (!_isMoving)
            return;
        _x = GetInputX();
        _anim.SetFloat("x", _x);
        _anim.SetFloat("y", _y);
    }

    void CalculateVelocity()
    {
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
		if(!_isOnGround && _extraJump > 0 && _isJump){
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
