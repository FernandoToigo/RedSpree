using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scenario : MonoBehaviour
{
    private const int MAX_BLOCK_HEIGHT = 10;

    public Transform _floorPrefab;
    public Transform _groundPrefab;
    public Transform _platformPrefab;

    private int _numberOfObjects;
    private float _spacing;
    private Vector3 _startPosition;
    private float _lastDistance = 0.0f;
    private int _currentHeight = 0;

    private Vector3 _nextPosition;
    private List<Transform> _floor;
    private List<Transform> _ground;

    private List<Transform> _platforms;

    public float GroundHeight
    {
        get { return 0.64f * _currentHeight; }
    }

    private Vector3 _lastPosition;
    public Vector3 LastPosition
    {
        get { return _lastPosition; }
    }

    void Start()
    {
        var worldScreenHeight = UnityEngine.Camera.main.orthographicSize * 2.0f * UnityEngine.Camera.main.transform.localScale.y;
        var worldScreenWidth = UnityEngine.Camera.main.orthographicSize * 2.0f * UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.transform.localScale.x;

        _startPosition = new Vector3(-worldScreenWidth * 0.1f - 1.28f, -0.88f, -1.0f);
        _numberOfObjects = (int)(worldScreenWidth / 0.64f) + 4;
        _spacing = 0.64f;

        _floor = new List<Transform>();
        _ground = new List<Transform>();
        _platforms = new List<Transform>();
        _nextPosition = _startPosition;
        for (int i = 0; i < _numberOfObjects; i++)
        {
            var floor = (Transform)Instantiate(_floorPrefab);
            floor.localPosition = _nextPosition;

            var ground = (Transform)Instantiate(_groundPrefab);
            ground.localPosition = _nextPosition - new Vector3(0.0f, 0.64f, 0.0f);

            var platform = (Transform)Instantiate(_platformPrefab);
            platform.localPosition = _nextPosition + 1.0f * new Vector3(0.0f, 0.64f, 2.0f);

            _lastPosition = _nextPosition;
            _nextPosition.x += _spacing;

            _floor.Add(floor);
            _ground.Add(ground);
            _platforms.Add(platform);
        }
    }

    void Update()
    {
        if (Mathf.Abs(Player.distanceTraveled - _lastDistance) >= 0.64f)
        {
            _lastPosition = _nextPosition;
            Destroy(_floor[0].gameObject);
            _floor.RemoveAt(0);
            Destroy(_ground[0].gameObject);
            _ground.RemoveAt(0);

            var floor = (Transform)Instantiate(_floorPrefab);
            floor.localPosition = _nextPosition;

            var ground = (Transform)Instantiate(_groundPrefab);
            ground.localPosition = _nextPosition - new Vector3(0.0f, 0.64f, 0.0f);

            var platform = (Transform)Instantiate(_platformPrefab);
            platform.localPosition = _nextPosition + 1.0f * new Vector3(0.0f, 0.64f, 2.0f);

            _floor.Add(floor);
            _ground.Add(ground);
            _platforms.Add(platform);

            _nextPosition.x += _spacing;
            _lastDistance += 0.64f;

            if (Random.value > 0.95)
            {
                if (Random.value < (float)_currentHeight / (float)MAX_BLOCK_HEIGHT)
                    _currentHeight--;
                else
                    _currentHeight++;

                _nextPosition = new Vector3(_nextPosition.x, -0.88f + 0.64f * _currentHeight, _nextPosition.z);
            }
        }
    }
}