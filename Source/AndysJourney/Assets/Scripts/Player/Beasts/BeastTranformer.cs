using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastTranformer : MonoBehaviour
{
    public Beast beastPrefab;
    [SerializeField]
    Animator _boomEffectPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (beastPrefab == null || beastPrefab is Object && beastPrefab.Equals(null))
                return;
            if (_boomEffectPrefab != null && _boomEffectPrefab is Object && !_boomEffectPrefab.Equals(null))
            {
                var boomFx = Instantiate<Animator>(_boomEffectPrefab, transform.position, Quaternion.identity);
                boomFx.gameObject.SetActive(true);
                Destroy(boomFx.gameObject, boomFx.GetCurrentAnimatorStateInfo(0).length);
            }
            var beast = Instantiate<Beast>(beastPrefab, transform.position, Quaternion.identity);
            beast.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}