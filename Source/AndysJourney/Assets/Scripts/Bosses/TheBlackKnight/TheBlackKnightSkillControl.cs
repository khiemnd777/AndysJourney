using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSkillControl : MonoBehaviour
{

    TheBlackKnightSlash _slash;
    TheSlashingKi _slashingKi;
    TheJumpAndDampingFire _jumpAndDampingFire;
    TheDashingDownWithPower _dashingDownWithPower;
    Skill[] _skills;

    // Use this for initialization
    void Start()
    {
        _slash = GetComponent<TheBlackKnightSlash>();
        _slashingKi = GetComponent<TheSlashingKi>();
        _jumpAndDampingFire = GetComponent<TheJumpAndDampingFire>();
        _dashingDownWithPower = GetComponent<TheDashingDownWithPower>();
        // _skills = new Skill[] { _slash, _slashingKi, _jumpAndDampingFire };
        _skills = new Skill[] { _slashingKi };
		StartCoroutine(Play());
    }

	IEnumerator Play(){
		while(true){
			var rand = Random.Range(0, _skills.Length);
			yield return StartCoroutine(_skills[rand].Play());
		}
	}
}
