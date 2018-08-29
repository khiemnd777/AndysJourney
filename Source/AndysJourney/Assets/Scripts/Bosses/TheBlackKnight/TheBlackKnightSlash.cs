using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSlash : MonoBehaviour
{
    [SerializeField]
    Animator _dustFxPrefab;
	[SerializeField]
	AnimationClip[] _sequence;
	[SerializeField]
	float[] _velocities;

	Animator _anim;
	Rigidbody2D _rigid;

	void Start(){
		_anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody2D>();
		StartCoroutine(Play());
	}

	public IEnumerator Play(){
		var inx = 0;
		foreach(var sq in _sequence){
			_anim.Play(sq.name);
			InstantiateTheDust(inx);
			_rigid.velocity = Vector2.right * Time.fixedDeltaTime * _velocities[inx];
			yield return new WaitForSeconds(sq.length);
			_rigid.velocity = Vector2.zero;
			inx++;
		}
		// _anim.Play("Idle", 0);
	}

	IEnumerator Dash(){
		_rigid.velocity = Vector2.right * Time.deltaTime * 25f;
		yield return new WaitForFixedUpdate();
	}

	void InstantiateTheDust(int index){
		var ins = Instantiate<Animator>(_dustFxPrefab, transform.position, Quaternion.identity);
		if(index == _sequence.Length - 1){
			ins.transform.localScale = Vector3.one * 1.45f;
		}
		Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
	}
}
