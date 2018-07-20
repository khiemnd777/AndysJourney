using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;
	public float smoothSpeed = .125f;
	public Vector3 offset;

	void FixedUpdate()
	{
		var desiredPos = target.position + offset;
		var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
		transform.position = smoothedPos;
		// transform.LookAt(target);
	}
}