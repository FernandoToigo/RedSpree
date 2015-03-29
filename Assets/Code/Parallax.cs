using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    private const int MAX_BLOCK_HEIGHT = 10;

    public Transform _prefab;
    public float _x;
    public float _z;
    public float _y;
    public float _spacing;
    private int _numberOfObjects;
    private Vector3 _startPosition;
    private float _lastDistance = 0.0f;
    private float _spriteWidth;
    private float _spriteHeight;

    private Vector3 _nextPosition;
    private Queue<Transform> _objectQueue;

    void Start()
    {
        float worldScreenHeight = UnityEngine.Camera.main.orthographicSize * 2 * UnityEngine.Camera.main.transform.localScale.y;
        float worldScreenWidth = UnityEngine.Camera.main.orthographicSize * 2 * UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.transform.localScale.x;

        var size = _prefab.GetComponent<SpriteRenderer>().sprite.bounds.size;
        _spriteWidth = size.x;
        _spriteHeight = size.y;

        _startPosition = new Vector3(_x + (-worldScreenWidth * 0.1f - (_spriteWidth * 2 + _spacing * 2)), _y + _spriteHeight * 0.5f, _z);
        _numberOfObjects = (int)(worldScreenWidth / (_spriteWidth + _spacing)) + 4;

        _objectQueue = new Queue<Transform>(_numberOfObjects);
        _nextPosition = _startPosition;
        for (int i = 0; i < _numberOfObjects; i++)
        {
            Transform o = (Transform)Instantiate(_prefab);
            o.localPosition = _nextPosition;
            _nextPosition.x += _spacing + _spriteWidth;
            _objectQueue.Enqueue(o);
        }
    }

    void Update()
    {
        if (Mathf.Abs(Player.distanceTraveled - _lastDistance) >= _spriteWidth + _spacing)
        {
            Transform o = _objectQueue.Dequeue();
            o.localPosition = _nextPosition;
            _nextPosition.x += _spacing + _spriteWidth;
            _objectQueue.Enqueue(o);
            _lastDistance += _spriteWidth + _spacing;
        }

        foreach (var item in _objectQueue)
        {
            item.transform.position += new Vector3(_z * Time.deltaTime * 0.5f, 0.0f, 0.0f);
        }
    }
}