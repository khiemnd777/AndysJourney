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
		var offset = target.position - transform.position;
		var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
		var angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, angleAxis, Time.deltaTime * 10000f);
		_anim.enabled = true;
		_anim.Play("Directed Dust");
		yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
		Destroy(gameObject);
	}
}
