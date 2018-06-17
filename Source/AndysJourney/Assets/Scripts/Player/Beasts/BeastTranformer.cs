using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastTranformer : MonoBehaviour
{
    public Beast beastPrefab;
    [SerializeField]
    bool _waitForEffect;
    [SerializeField]
    Animator _boomEffectPrefab;
    SpriteRenderer _renderer;

    void Start(){
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(Transform());
        }
    }

    IEnumerator Transform()
    {
        if (beastPrefab == null || beastPrefab is Object && beastPrefab.Equals(null))
            yield break;
        // transform to beast
        var beast = Instantiate<Beast>(beastPrefab, transform.position, Quaternion.identity);
        var beastRenderer = beast.GetComponent<SpriteRenderer>();
        // hidden for caching
        beastRenderer.enabled = false;
        // hidden the current sprite.
        _renderer.enabled = false;
        // active transformed effect.
        if (_boomEffectPrefab != null && _boomEffectPrefab is Object && !_boomEffectPrefab.Equals(null))
        {
            var boomFx = Instantiate<Animator>(_boomEffectPrefab, transform.position, Quaternion.identity);
            boomFx.gameObject.SetActive(true);
            var tranformLength = boomFx.GetCurrentAnimatorStateInfo(0).length;
            Destroy(boomFx.gameObject, tranformLength);
            if(_waitForEffect)
                yield return new WaitForSeconds(tranformLength);
        }
        // assure the beast must be actived.
        beast.gameObject.SetActive(true);
        // shown the beast after done.
        beastRenderer.enabled = true;
        Destroy(gameObject);
    }
}