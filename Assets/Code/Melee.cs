using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour
{
    public bool _active = false;

    void Start()
    {
        _active = false;
    }

    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!_active)
            return;

        if (other.gameObject.tag.Equals("Zombie"))
        {
            var zombie = other.GetComponent<Zombie>();
            zombie.Kill();
        }
    }
}