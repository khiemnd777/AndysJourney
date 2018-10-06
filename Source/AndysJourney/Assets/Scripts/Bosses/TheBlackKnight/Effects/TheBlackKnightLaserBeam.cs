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

    [SerializeField]
    AnimationCurve _curve;

    [SerializeField]
    Transform _from;

    [SerializeField]
    Transform _to;

    [SerializeField]
    float _duration;

    LineRenderer _lr;

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        StartCoroutine(RotateTheBeam());
    }

    void Update()
    {
        _lr.SetPosition(0, new Vector2(_start.position.x, _start.position.y));
    }

    IEnumerator RotateTheBeam()
    {
        var fromPos = _from.position;
        var toPos = _to.position;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / _duration;
            var expPos = Vector3.Lerp(fromPos, toPos, _curve.Evaluate(t));
            var expDir = (expPos - _start.position).normalized;
            var hit = Physics2D.Raycast(_start.position, expDir, float.MaxValue, _layerMask);
            if(hit.collider != null){
                var endPoint = hit.point;
                _lr.SetPosition(1, new Vector2(endPoint.x, endPoint.y));
            }
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(RotateTheBeam());
    }
}