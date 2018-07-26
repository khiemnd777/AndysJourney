using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, IControlLocker
{
	public Transform target;
	public float smoothSpeed = .125f;
	public Vector3 offset;
	public BoxCollider2D bound;

	Camera theCamera;

	bool _lock;

	void Start()
	{
		theCamera = GetComponent<Camera>();
		ControlLock.Register(this, "Camera");
	}

	void FixedUpdate()
	{
		if(_lock)
			return;
		var desiredPos = target.position + offset;
		var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
		transform.position = Utility.CameraInBound(theCamera, bound, smoothedPos);
	}

    public void Lock(string name)
    {
        if(name == "Camera"){
			_lock = true;
		}
    }

    public void ReleaseLock(string name)
    {
		if(name == "Camera"){
			_lock = false;
		}
    }
}