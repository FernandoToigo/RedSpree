using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIBullets : MonoBehaviour
{
    public GameObject uiBulletPrefab;

    private const float OFF = 0.03f;

    private int _addCount;
    private int _removeCount;
    private List<GameObject> _currentBullets;
    private int _addIndex = 0;

    private Dictionary<GameObject, float> _currentAdding = null;
    private Dictionary<GameObject, int> _currentAddingIndexes = null;
    private Dictionary<GameObject, float> _currentRemoving = null;

    private float _animationCoolDown = 0.2f;

    void Start()
    {
        _addCount = 0;
        _removeCount = 0;
        _addIndex = 0;
        _currentBullets = new List<GameObject>();
        _currentAdding = new Dictionary<GameObject, float>();
        _currentAddingIndexes = new Dictionary<GameObject, int>();
        _currentRemoving = new Dictionary<GameObject, float>();

        this.AddBullets(10);
    }

    void Update()
    {
        var keysToRemove = new List<GameObject>();

        foreach (var current in _currentRemoving.Keys.ToList())
        {
            _currentRemoving[current] = Mathf.Max(0.0f, _currentRemoving[current] - Time.deltaTime);
            var off = OFF * ((_animationCoolDown - _currentRemoving[current]) / _animationCoolDown);
            current.transform.localPosition -= new Vector3(0.0f, off, 0.0f);
            var sprite = current.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - ((_animationCoolDown - _currentRemoving[current]) / _animationCoolDown));

            foreach (var other in _currentBullets)
                if (other != current && _currentAddingIndexes[current] > _currentAddingIndexes[other])
                    other.transform.localPosition -= new Vector3(off, 0.0f, 0.0f);

            if (_currentRemoving[current] <= 0)
                keysToRemove.Add(current);
        }

        foreach (var key in keysToRemove)
        {
            _currentBullets.Remove(key);
            _currentRemoving.Remove(key);
            _currentAddingIndexes.Remove(key);
            Destroy(key);
        }

        keysToRemove.Clear();

        foreach (var current in _currentAdding.Keys.ToList())
        {
            _currentAdding[current] = Mathf.Max(0.0f, _currentAdding[current] - Time.deltaTime);
            var off = OFF * ((_animationCoolDown - _currentAdding[current]) / _animationCoolDown);
            current.transform.localPosition += new Vector3(off, 0.0f, 0.0f);
            var sprite = current.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1.0f, 1.0f, 1.0f, ((_animationCoolDown - _currentAdding[current]) / _animationCoolDown));

            foreach (var other in _currentBullets)
                if (other != current && _currentAddingIndexes[current] > _currentAddingIndexes[other])
                    other.transform.localPosition += new Vector3(off, 0.0f, 0.0f);

            if (_currentAdding[current] <= 0)
                keysToRemove.Add(current);
        }

        foreach (var key in keysToRemove)
        {
            _currentAdding.Remove(key);
        }

        for (int i = 0; i < _removeCount; i++)
        {
            var removing = _currentBullets.Last(b => !_currentRemoving.Keys.Contains(b));
            _currentRemoving.Add(removing, _animationCoolDown);
        }
        _removeCount = 0;

        for (int i = 0; i < _addCount; i++)
        {
            var adding = (GameObject)Instantiate(uiBulletPrefab);
            adding.transform.parent = this.gameObject.transform;
            _currentAdding.Add(adding, _animationCoolDown);
            _currentAddingIndexes.Add(adding, _addIndex++);
            _currentBullets.Add(adding);
            adding.transform.localPosition = new Vector3(-OFF, 0.0f, 1.0f);
            var sprite = adding.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        _addCount = 0;
    }

    public void AddBullets(int count)
    {
        _addCount += count;
    }

    public void RemoveBullets(int count)
    {
        _removeCount += count;
    }
}