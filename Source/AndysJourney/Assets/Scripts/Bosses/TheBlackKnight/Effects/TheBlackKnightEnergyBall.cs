using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightEnergyBall : MonoBehaviour
{
	public float speed;

    void Start()
    {

    }

    void Update()
    {
		var go = transform.rotation * Vector3.right;
		transform.position += go * speed * Time.fixedDeltaTime;
    }
}
