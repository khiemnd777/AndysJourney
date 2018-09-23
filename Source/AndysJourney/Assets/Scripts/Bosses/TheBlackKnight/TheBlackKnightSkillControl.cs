using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSkillControl : MonoBehaviour
{

    TheBlackKnightSlash _slash;
    TheSlashingKi _slashingKi;
    TheJumpAndDampingFire _jumpAndDampingFire;
    TheDashingDownWithPower _dashingDownWithPower;
    TheSlashingKiWhenGetBack _slashingKiWhenGetBack;
    TheBlackKnightGetBack _getBack;

    Skill[] _skills;

    // Use this for initialization
    void Start()
    {
        _slash = GetComponent<TheBlackKnightSlash>();
        _slashingKi = GetComponent<TheSlashingKi>();
        _jumpAndDampingFire = GetComponent<TheJumpAndDampingFire>();
        _dashingDownWithPower = GetComponent<TheDashingDownWithPower>();
        _slashingKiWhenGetBack = GetComponent<TheSlashingKiWhenGetBack>();
        _getBack = GetComponent<TheBlackKnightGetBack>();
        // _skills = new Skill[] { _slash, _slashingKi, _dashingDownWithPower };
        // _skills = new Skill[] { _dashingDownWithPower };
        _skills = new Skill[] { _getBack };
		StartCoroutine(Play());
    }

	IEnumerator Play(){
		while(true){
            if(_skills.Length <= 0){
                yield break;
            }
			var rand = Random.Range(0, _skills.Length);
			yield return StartCoroutine(_skills[rand].Play());
            yield return new WaitForSeconds(.5f);
		}
	}
}
