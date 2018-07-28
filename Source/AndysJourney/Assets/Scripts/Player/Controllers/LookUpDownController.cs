using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUpDownController : PlayerController
{
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    BoxCollider2D _bound;
    [SerializeField]
    float _translatedVal;
    [SerializeField]
    float _holdIn;

    float _start;
    bool _firstInput;
    bool _handled;

    public override void Update()
    {
        if (_player.GetInputY() != 0)
        {
            if (!_firstInput)
            {
                _firstInput = true;
                _start = Time.time;
                return;
            }
            if (!_handled)
            {
                if (_start + _holdIn <= Time.time)
                {
                    ControlLock.Lock("Camera");
                    var edge = Utility.CameraBoundEdge(_theCamera, _bound, _theCamera.transform.position);
                    if (edge.y == 0)
                    {
						if(_player.GetInputY() == 1)
							return;
                    }
                    else if (edge.y == 1)
                    {
                        if(_player.GetInputY() == -1)
							return;
                    }
					StartCoroutine(Looking(_player.GetInputY()));
                    _handled = true;
                }
            }
        }
        else
        {
            _firstInput = false;
            _handled = false;
            _start = 0f;
            ControlLock.ReleaseLock("Camera");
        }
    }

    IEnumerator Looking(float y)
    {
        yield return StartCoroutine(Utility.VectorLerp(_theCamera.transform
            , _theCamera.transform.position
            , new Vector3(_theCamera.transform.position.x, _theCamera.transform.position.y + _translatedVal * y, _theCamera.transform.position.z)
            , .1f
            , () => new WaitForFixedUpdate()));
    }
}
