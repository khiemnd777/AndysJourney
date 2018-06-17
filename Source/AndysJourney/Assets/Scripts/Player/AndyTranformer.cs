using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndyTranformer : MonoBehaviour
{
    public Transform andyPrefab;
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
        if (andyPrefab == null || andyPrefab is Object && andyPrefab.Equals(null))
            yield break;
        // transform to beast
        var andy = Instantiate<Transform>(andyPrefab, transform.position, Quaternion.identity);
        var andyRenderer = andy.GetComponent<SpriteRenderer>();
        // hidden for caching
        andyRenderer.enabled = false;
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
        andy.gameObject.SetActive(true);
        // shown the beast after done.
        andyRenderer.enabled = true;
        Destroy(gameObject);
    }
}