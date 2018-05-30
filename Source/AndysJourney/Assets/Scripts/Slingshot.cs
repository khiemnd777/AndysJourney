using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
	public Sprite[] spriteDown;
	public Sprite[] spriteUp;
	public Sprite[] spriteLeft;
	public Sprite[] spriteRight;

	Animator _animator;

	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	void Update()
	{

	}

	void Prepare()
	{
		if(Input.GetKey(KeyCode.J))
		{

		}
		
	}
}
