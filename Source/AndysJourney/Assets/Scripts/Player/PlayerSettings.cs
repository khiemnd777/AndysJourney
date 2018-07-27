using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour, IControlLocker
{
    public Transform frontCollisionCheck;
    public float collisionCheckRadius;
    public LayerMask collisionLayer;

    public float gravity = .45f;
    [System.NonSerialized]
    public float faceX;
    [System.NonSerialized]
    public float extraForceX;
    [System.NonSerialized]
    public float extraForceY;
    [System.NonSerialized]
    public bool isFrontCollision;

    [System.NonSerialized]
    public bool isJump;

    public Movement2D movement
    {
        get
        {
            return _movement;
        }
    }

    bool _lockFlipX;

    Rigidbody2D _rb;
    Animator _anim;
    BoxCollider2D _col;
    Movement2D _movement;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _movement = GetComponent<Movement2D>();
        _rb.gravityScale = gravity;
        faceX = 1;
        ControlLock.Register("FlipX", this);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        faceX = Input.GetAxisRaw("Horizontal") == 0 ? faceX : Input.GetAxisRaw("Horizontal");
        HandleFlipX();
        CheckFrontCollision();
    }

    void HandleFlipX()
    {
        if (_lockFlipX)
            return;
        var localScale = transform.localScale;
        var scaleVal = new Vector3(Mathf.Abs(localScale.x) * faceX, localScale.y, localScale.z);
        if (localScale.x != faceX)
        {
            _anim.SetBool("isWallSliding", false);
            transform.localScale = scaleVal;
        }
    }

    void CheckFrontCollision()
    {
        // var state = Physics2D.OverlapCircle(frontCollisionCheck.position, collisionCheckRadius, collisionLayer);
        var state = Physics2D.OverlapBox(frontCollisionCheck.position, _col.bounds.extents, 90, collisionLayer);
        isFrontCollision = state == true;
    }

    public float GetInputX()
    {
        return GetInputHorizontal();
    }

    public float GetInputY()
    {
        return GetInputVertical();
    }

    public float GetInputHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetInputVertical()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public void HandleExtraForces()
    {
        _rb.velocity = _rb.velocity + new Vector2(extraForceX, extraForceY);
    }

    public void ResetExtraForces()
    {
        extraForceX = 0f;
        extraForceY = 0f;
    }

    public void Lock(string name)
    {
        if (name == "FlipX")
        {
            _lockFlipX = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "FlipX")
        {
            _lockFlipX = false;
        }
    }
}
