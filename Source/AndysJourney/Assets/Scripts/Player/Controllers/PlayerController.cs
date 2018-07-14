using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected float _faceX;
    protected Player _player;

    public virtual void Awake()
    {
        _player = GetComponent<Player>();
    }

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _faceX = 1;
    }

    public virtual void Update()
    {
        _faceX = _player.faceX;
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }
}