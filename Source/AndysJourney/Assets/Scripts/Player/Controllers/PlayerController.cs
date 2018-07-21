using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected SpriteRenderer _sprite;
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
        _sprite = GetComponent<SpriteRenderer>();
        _faceX = 1;
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        _faceX = _player.faceX;
    }

    public virtual void LateUpdate()
    {

    }
}