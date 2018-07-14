using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected float _faceX;

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _faceX = 1;
    }

    public virtual void Update()
    {
        _faceX = Input.GetAxisRaw("Horizontal") == 0 ? _faceX : Input.GetAxisRaw("Horizontal");
        FlipX();
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    void FlipX()
    {
        var localScale = transform.localScale;
        var scaleVal = new Vector3(Mathf.Abs(localScale.x) * _faceX, localScale.y, localScale.z);
        transform.localScale = scaleVal;
    }
}