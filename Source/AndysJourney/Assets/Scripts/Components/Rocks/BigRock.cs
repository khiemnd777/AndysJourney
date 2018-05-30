using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigRock : InteractableObject
{
    public Sprite[] destructSprites;
    [Header("Piece of rock can fly away")]
    public FlyPiecesOfRockAway piecesOfRockFlyerPrefab;
    public Transform bigRockBelowPrefab;

    int _destructSpriteTime;
    int _impactCount;
    int _destructSpriteIndex;

    SpriteRenderer _spriteRenderer;

    public override void Start()
    {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        destructable = true;
        destructableHealth = destructableHealth == 1 ? 4f : destructableHealth;
        _destructSpriteTime = Mathf.FloorToInt(destructableHealth / destructSprites.Length);

        onDestruct += () =>
        {
            var pos = transform.position;
            Instantiate<FlyPiecesOfRockAway>(piecesOfRockFlyerPrefab, pos, Quaternion.identity);
            var bigRockBelow = Instantiate<Transform>(bigRockBelowPrefab, pos, Quaternion.identity);
            bigRockBelow.transform.localScale = Vector3.one * 1.5f;
            Destroy(bigRockBelow.gameObject, .5f);
        };
    }

    public override void TakeDamage()
    {
        if(_impactCount == 0)
            _spriteRenderer.sprite = destructSprites[0];
        if(destructableHealth == 2){
            _spriteRenderer.sprite = destructSprites[destructSprites.Length - 1];
        }
        else if (_impactCount % _destructSpriteTime == 0)
        {
            _spriteRenderer.sprite = destructSprites[_destructSpriteIndex++];
        }
        _impactCount++;
        base.TakeDamage();
    }
}