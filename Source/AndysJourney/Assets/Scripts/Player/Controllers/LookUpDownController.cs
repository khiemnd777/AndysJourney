using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUpDownController : PlayerController
{
    [SerializeField]
    Camera _theCamera;
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
                    StartCoroutine(Looking());
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

    IEnumerator Looking()
    {
		Debug.Log( _theCamera.transform.position.z);
        yield return StartCoroutine(Utility.VectorLerp(_theCamera.transform
			, _theCamera.transform.position
			, new Vector3(_theCamera.transform.position.x, _theCamera.transform.position.y + _translatedVal * _player.GetInputY(), _theCamera.transform.position.z)
			, .1f
			, () => null));
    }
}
