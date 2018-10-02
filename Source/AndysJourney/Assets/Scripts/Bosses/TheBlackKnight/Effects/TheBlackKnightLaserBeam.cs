using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightLaserBeam : MonoBehaviour
{
    [SerializeField]
    Camera _theCamera;

    [SerializeField]
    Transform _start;

    [SerializeField]
    LayerMask _layerMask;

    LineRenderer _lr;

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        var mousePoint = _theCamera.ScreenToWorldPoint(Input.mousePosition);
        _lr.SetPosition(0, new Vector2(_start.position.x, _start.position.y));
        var direction = (mousePoint - _start.position).normalized;
        var hit = Physics2D.Raycast(_start.position, direction, float.MaxValue, _layerMask);
        if (hit.collider != null)
        {
			var endPoint = hit.point;
			_lr.SetPosition(1, new Vector2(endPoint.x, endPoint.y));
        }
    }
}