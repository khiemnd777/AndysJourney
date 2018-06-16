using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndyTranformer : MonoBehaviour
{
    public Transform andyPrefab;
    [SerializeField]
    Animator _boomEffectPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (andyPrefab == null || andyPrefab is Object && andyPrefab.Equals(null))
                return;
            if (_boomEffectPrefab != null && _boomEffectPrefab is Object && !_boomEffectPrefab.Equals(null))
            {
                var boomFx = Instantiate<Animator>(_boomEffectPrefab, transform.position, Quaternion.identity);
                boomFx.gameObject.SetActive(true);
                Destroy(boomFx.gameObject, boomFx.GetCurrentAnimatorStateInfo(0).length);
            }
            var beast = Instantiate<Transform>(andyPrefab, transform.position, Quaternion.identity);
            beast.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}