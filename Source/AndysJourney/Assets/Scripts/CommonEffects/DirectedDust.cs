using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedDust : MonoBehaviour {

	Animator _anim;

	// Use this for initialization
	void Awake () {
		_anim = GetComponent<Animator>();
	}

	public IEnumerator Play(Transform target){
		yield return new WaitForSeconds(.02f);
		transform.rotation = Utility.RotateToTarget(transform, target, Time.deltaTime * 10000f);
		_anim.enabled = true;
		_anim.Play("Directed Dust");
		yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
		Destroy(gameObject);
	}
}
