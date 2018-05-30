using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FlyPiecesOfRockAway : MonoBehaviour
{
    [Header("Piece of rock can fly away")]
    public RockPiece rockPiecePrefab;
    public int rockPieceAmount;
    public Sprite[] rockPieceSprites;

    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        FlyAway();
    }

    void FlyAway()
    {
        // init array of gold and position
        var ownerPosition = transform.position;
        var pieces = new Transform[rockPieceAmount];
        var positions = new Vector2[rockPieceAmount];
        var trajectoryHeights = new float[rockPieceAmount];
        for (var i = 0; i < rockPieceAmount; i++)
        {
            var piece = Instantiate<RockPiece>(rockPiecePrefab, ownerPosition, Quaternion.identity); //new GameObject("Piece of Rock (" + i + ")");
            var renderer = piece.GetComponent<SpriteRenderer>();
            renderer.sprite = rockPieceSprites[Random.Range(0, rockPieceSprites.Length)];
            pieces[i] = piece.transform;
        }
        for (var i = 0; i < pieces.Length; i++)
        {
            positions[i] = Random.insideUnitCircle * Random.Range(.2f, .3f) + new Vector2(ownerPosition.x, ownerPosition.y);
            trajectoryHeights[i] = Random.Range(.09f, .175f);
        }
        StartCoroutine(FlyingAway(pieces, positions, trajectoryHeights));
    }

    IEnumerator FlyingAway(Transform[] pieces, Vector2[] positions, float[] trajectoryHeights)
    {
        var percent = 0f;
        var startPosition = transform.position;
        while (percent <= 1f)
        {
            percent += Time.deltaTime * 1.5f;
            for (var i = 0; i < pieces.Length; i++)
            {
                Utility.JumpToDestination(pieces[i], startPosition, positions[i], trajectoryHeights[i], percent);
            }
            yield return null;
        }
        // Get rigidbody component and change the bodyType to Dynamic
        // This action will allow each of piece can be able to interact by collider event
        var rigidPieces = pieces.Select(x => x.GetComponent<Rigidbody2D>())
            .Where(x => x != null && x is Object && !x.Equals(null))
            .ToArray();
        foreach(var rigidPiece in rigidPieces)
        {
            rigidPiece.bodyType = RigidbodyType2D.Dynamic;
        }
        yield return new WaitForSeconds(Time.deltaTime);
        
        // After interacting by collider event, each of piece should be changed to Static
        rigidPieces = rigidPieces.Where(x => x != null && x is Object && !x.Equals(null)).ToArray();
        foreach(var rigidPiece in rigidPieces)
        {
            rigidPiece.bodyType = RigidbodyType2D.Static;
        }
        Destroy(gameObject);
    }
}