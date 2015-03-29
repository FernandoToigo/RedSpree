using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private float distanceTraveled = 0.0f;
    private int life = 1;

    void Start()
    {
        if (Player.zombiesKilled >= 100)
            life = 2;
    }

    void Update()
    {
        var delta = 25f * Time.deltaTime;
        distanceTraveled += delta;
        transform.Translate(Vector3.right * delta);

        if (distanceTraveled > 20.0f)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Zombie"))
        {
            var zombie = other.GetComponent<Zombie>();
            if (zombie.Kill())
            {
                life--;
                if (life == 0)
                    Destroy(gameObject);
            }
        }
        else if (other.gameObject.tag.Equals("FlyingMonster"))
        {
            var flyingMonster = other.GetComponent<FlyingMonster>();
            if (flyingMonster.Kill())
            {
                life--;
                if (life == 0)
                    Destroy(gameObject);
            }
        }
        else if (other.gameObject.tag.Equals("Ground"))
        {
            Destroy(gameObject);
        }
    }
}