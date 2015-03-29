using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ZombieState
{
    Alive,
    Dying,
    Dead
}

public class Zombie : MonoBehaviour
{
    public float _maxSpeed = 5.0f;
    public float speed = 0;
    public int bullets = 0;
    public bool looted = false;
    public ZombieState state;
    private float lootTimer = 0.3f;
    private float dyingTimer = 0.5f;
    private Animator animator;
    private AudioSource killedAudio;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        killedAudio = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        switch (state)
        {
            case ZombieState.Alive:
                break;
            case ZombieState.Dying:
                dyingTimer -= Time.deltaTime;
                if (dyingTimer < 0)
                {
                    state = ZombieState.Dead;
                    animator.SetBool("Dead", true);
                }
                break;
        }

        if (transform.localPosition.x < Player.distanceTraveled - 5f)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (state != ZombieState.Dead && state != ZombieState.Dying)
            if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < _maxSpeed)
                GetComponent<Rigidbody2D>().velocity += new Vector2(Mathf.Min(-speed, -_maxSpeed - GetComponent<Rigidbody2D>().velocity.x), 0);
    }

    public bool Kill()
    {
        if (state == ZombieState.Alive)
        {
            state = ZombieState.Dying;
            animator.SetBool("Dying", true);

            var particleSystem = transform.Find("Blood").GetComponent<ParticleSystem>();
            particleSystem.Play();

            Player.zombiesKilled++;
            killedAudio.PlayOneShot(killedAudio.clip);

            gameObject.layer = LayerMask.NameToLayer("Body");

            return true;
        }

        return false;
    }

    public bool Loot()
    {
        if (looted || (state != ZombieState.Dying && state != ZombieState.Dead))
            return false;

        looted = true;
        var canvas = transform.Find("Canvas");
        var bulletsLootText = canvas.transform.Find("BulletsLootText");
        var text = bulletsLootText.GetComponent<Text>();
        text.text = string.Format("+{0}", bullets);
        canvas.gameObject.SetActive(true);

        return true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == ZombieState.Alive && other.gameObject.tag.Equals("Player"))
        {
            var player = other.GetComponent<Player>();
            player.Kill();
        }
    }
}
