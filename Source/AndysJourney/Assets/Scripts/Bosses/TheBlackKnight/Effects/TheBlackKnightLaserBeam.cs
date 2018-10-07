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

    [SerializeField]
    Transform _impactedLaserPrefab;
    Transform _impactedLaser;

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
            if (hit.collider != null)
            {
                var endPoint = hit.point;
                InstantiateTheImpacted(endPoint, (BoxCollider2D)hit.collider);
                _lr.SetPosition(1, new Vector2(endPoint.x, endPoint.y));
                UpdateTheImpacted(endPoint, (BoxCollider2D)hit.collider);
            }
            yield return null;
        }
        yield return new WaitForSeconds(.15f);
        DestroyTheImpacted();
        StartCoroutine(RotateTheBeam());
    }

    void InstantiateTheImpacted(Vector3 position, BoxCollider2D collider)
    {
        if (_impactedLaser != null && _impactedLaser is Object && !_impactedLaser.Equals(null))
        {
            return;
        }
        var angle = GetHitSurfaceAngleOfBoxCollider2D(collider, position) - 90f;
        var qt = Quaternion.AngleAxis(angle, Vector3.forward);
        _impactedLaser = Instantiate<Transform>(_impactedLaserPrefab, position, qt);
        _impactedLaser.localScale = _impactedLaser.localScale * 1.12f;
        _impactedLaser.gameObject.SetActive(true);
    }

    void UpdateTheImpacted(Vector3 position, BoxCollider2D collider)
    {
        if (_impactedLaser == null || _impactedLaser is Object && _impactedLaser.Equals(null))
        {
            return;
        }
        var angle = GetHitSurfaceAngleOfBoxCollider2D(collider, position) - 90f;
        var qt = Quaternion.AngleAxis(angle, Vector3.forward);
        _impactedLaser.rotation = qt;
        _impactedLaser.position = position;
    }

    void DestroyTheImpacted(){
        if (_impactedLaser == null || _impactedLaser is Object && _impactedLaser.Equals(null))
            return;
        Destroy(_impactedLaser.gameObject);
    }

    float GetHitSurfaceAngleOfBoxCollider2D(BoxCollider2D collider, Vector2 hitPoint)
    {
        var bounds = collider.bounds;
        var min = bounds.min;
        var max = bounds.max;
        var displacement = Vector3.zero;
        // left
        if (Mathf.Approximately(hitPoint.x, min.x) && Mathf.Clamp(hitPoint.y, min.y, max.y) == hitPoint.y)
        {
            displacement = new Vector3(min.x, max.y, min.z) - min;
        }
        // right
        if (Mathf.Approximately(hitPoint.x, max.x) && Mathf.Clamp(hitPoint.y, min.y, max.y) == hitPoint.y)
        {
            displacement = max - new Vector3(max.x, min.y, max.z);
        }
        // up
        if (Mathf.Approximately(hitPoint.y, max.y) && Mathf.Clamp(hitPoint.x, min.x, max.x) == hitPoint.x)
        {
            displacement = max - new Vector3(min.x, max.y, min.z);
        }
        // bottom
        if (Mathf.Approximately(hitPoint.y, min.y) && Mathf.Clamp(hitPoint.x, min.x, max.x) == hitPoint.x)
        {
            displacement = new Vector3(max.x, min.y, max.z) - min;
        }
        var ninety = Vector3.Cross(Vector3.forward, displacement);
        var angle = Mathf.Atan2(ninety.y, ninety.x) * Mathf.Rad2Deg;
        return angle;
    }
}