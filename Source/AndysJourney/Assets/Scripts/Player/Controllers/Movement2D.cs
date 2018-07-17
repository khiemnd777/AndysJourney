using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    List<Vector2> _modifiers = new List<Vector2>();
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        foreach (var modifier in _modifiers)
        {
            _rb.velocity = modifier;
        }
    }

    public void AddHorizontal(float x)
    {
        _modifiers.Add(new Vector2(x, _rb.velocity.y));
    }

    public void AddVertical(float y){
        _modifiers.Add(new Vector2(_rb.velocity.x, y));
    }
}
