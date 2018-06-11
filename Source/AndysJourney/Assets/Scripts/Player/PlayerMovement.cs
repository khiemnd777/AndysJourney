using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Range(0f, 1f)]
    public float speed = .4f;
    public Transform carrier;
    [Header("Surfing")]
    public bool allowSurfing;
    public float surfingExtraDistance = .15f;
    public float surfingSeconds = .5f;
    public bool isMoving
    {
        get
        {
            return _direction.x != 0 || _direction.y != 0;
        }
    }
    [Space]
    public bool carrying;
    [SerializeField]
    Transform _runningDustPrefab;
    [SerializeField]
    Transform _dustPosition;

    Animator _animator;
    Rigidbody2D _rigidbody;
    SpriteRenderer _renderer;
    PlayerStateHandler _stateHandler;
    InteractableObject _currentInteractableObject;
    DirectionGetter2D _directionGetter;

    public bool stopX;
    public bool stopY;
    
    bool isSurfing;

    int lastStance = 3;
    Vector3 _direction;
    bool _canMove;
    float _dustTime;
    bool _t;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _stateHandler = GetComponent<PlayerStateHandler>();
        _directionGetter = GetComponent<DirectionGetter2D>();
    }

    void Update()
    {
        CalculateDirection();
        HandleLayers();
    }

    void FixedUpdate()
    {
        Move();
        Surf();
        Dust();
    }

    void Move()
    {
        _rigidbody.velocity = _direction * speed;
    }

    void Dust()
    {
        if (!isMoving)
        {
            _dustTime = 0f;
            _t = false;
            return;
        }
        var stepTime = .75f; //Utility.TimeByFrame(6, 30);
        if (!_t)
        {
            _dustTime = Time.time + stepTime;
            _t = true;
        }
        if (Time.time >= _dustTime)
        {
            _t = false;
            _dustTime = Time.time + stepTime;
            var dustPosition = _dustPosition.transform.position;
            StartCoroutine(Dusting(dustPosition));
        }
    }

    IEnumerator Dusting(Vector3 dustPosition, float size = 1f, bool instantiateImmediately = false)
    {
        if(!instantiateImmediately)
            yield return new WaitForSeconds(.1f);
        var runningDust = Instantiate<Transform>(_runningDustPrefab, dustPosition, Quaternion.identity);
        runningDust.transform.localScale = Vector3.one * size;
        Destroy(runningDust.gameObject, .5f);
    }

    void AnimateMovement(Vector2 direction)
    {
        var animLayerName = _stateHandler.state == PlayerState.Carrying ? "Walk and Carry" : "Walk";
        Utility.ActiveLayer(_animator, animLayerName);
        _animator.SetFloat("vertical", direction.x);
        _animator.SetFloat("horizontal", direction.y);
    }

    void HandleLayers()
    {
        if (isMoving)
        {
            AnimateMovement(_direction);
        }
        else
        {
            _animator.SetLayerWeight(1, 0);
            var animLayerName = _stateHandler.state == PlayerState.Carrying ? "Idle and Carry" : "Idle";
            Utility.ActiveLayer(_animator, animLayerName);
        }
    }

    void CalculateDirection()
    {
        var x = !stopX ? Input.GetAxisRaw("Horizontal") : 0f;
        var y = !stopY ? Input.GetAxisRaw("Vertical") : 0f;
        _direction = new Vector2(x, y);
    }

    void Surf()
    {
        if(!allowSurfing || !Input.GetKeyDown(KeyCode.I))
            return;
        if(isSurfing)
            return;
        isSurfing = true;
        StartCoroutine(Surfing());
    }

    IEnumerator Surfing()
    {
        stopX = stopY = true;
        var t = 0f;
        var orgPos = transform.position;
        var distance = _directionGetter.direction * surfingExtraDistance;
        var targetPos = orgPos + new Vector3(distance.x, distance.y, 0f);
        // Dust
        StartCoroutine(Dusting(_dustPosition.transform.position, 1.15f, true));
        var inx = 0;
        var colorA = 0f;
        while(t <= 1f){
            t += Time.fixedDeltaTime / surfingSeconds;
            transform.position = Vector3.Lerp(orgPos, targetPos, t);
            // Surfing Shadow
            var spriteObj = new GameObject("Surfing Shadow", typeof(SpriteRenderer));
            spriteObj.transform.position = transform.position;
            spriteObj.transform.localScale = transform.lossyScale;
            var spriteObjRenderer = spriteObj.GetComponent<SpriteRenderer>();
            spriteObjRenderer.sprite = _renderer.sprite;
            spriteObjRenderer.sortingOrder = 50 + inx;
            colorA += inx % 2 == 0 ? .07f : 0;
            spriteObjRenderer.color = new Color(255f, 255f, 255f, colorA);
            inx++;
            Destroy(spriteObj, .1625f);
            yield return new WaitForFixedUpdate();
        }
        stopX = stopY = false;
        isSurfing = false;
    }
}
