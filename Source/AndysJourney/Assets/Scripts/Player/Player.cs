using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IControlLocker
{
    public float gravity = .45f;
    public Transform frontCollisionCheck;
    public float collisionCheckRadius;
    public LayerMask collisionLayer;
    [System.NonSerialized]
    public float faceX;
    [System.NonSerialized]
    public bool isFrontCollision;

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
    Movement2D _movement;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
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
        var state = Physics2D.OverlapCircle(frontCollisionCheck.position, collisionCheckRadius, collisionLayer);
        isFrontCollision = state == true;
    }

    public float GetInputX()
    {
        return GetInputHorizontal();
    }

    public float GetInputHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
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
