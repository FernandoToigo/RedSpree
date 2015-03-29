using UnityEngine;
using System;
using System.Collections;

public enum FlyingMonsterState
{
    FlyingLeft,
    FlyingRight,
    Attacking,
    DyingLeft,
    DyingRight
}

public class FlyingMonster : MonoBehaviour
{
    public float speed = 0;

    private FlyingMonsterState state;
    private Animator animator;
    private int laps = 0;

    private float dyingTimer = 0.3f;

    private float currentSpeed = 0.0f;
    private float acceleration = 5.0f;
    private float deceleration = 20.0f;

    private AudioSource killedAudio;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        killedAudio = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 dir = new Vector3();

        switch (state)
        {
            case FlyingMonsterState.FlyingLeft:
                currentSpeed = speed * 0.5f;
                dir = new Vector3(-1.0f, 0.0f, 0.0f);
                if (transform.localPosition.x < Player.distanceTraveled + 1f)
                {
                    laps++;
                    if (laps > 3)
                    {
                        state = FlyingMonsterState.Attacking;
                        animator.SetInteger("State", (int)state);
                    }
                    else
                    {
                        state = FlyingMonsterState.FlyingRight;
                        animator.SetInteger("State", (int)state);
                    }
                }

                break;
            case FlyingMonsterState.FlyingRight:
                currentSpeed = speed;
                dir = new Vector3(1.0f, 0.0f, 0.0f);
                if (transform.localPosition.x > Player.distanceTraveled + 13f)
                {
                    laps++;
                    if (laps > 3)
                    {
                        state = FlyingMonsterState.Attacking;
                        animator.SetInteger("State", (int)state);
                    }
                    else
                    {
                        state = FlyingMonsterState.FlyingLeft;
                        animator.SetInteger("State", (int)state);
                    }
                }

                break;
            case FlyingMonsterState.Attacking:
                currentSpeed = 5.0f;
                dir = Player.position - transform.localPosition;
                dir.Normalize();
                break;

            case FlyingMonsterState.DyingLeft:
            case FlyingMonsterState.DyingRight:
                currentSpeed = 7.0f;
                dir = new Vector3(0f, -1f, 0f);
                dyingTimer -= Time.deltaTime;
                if (dyingTimer < 0)
                    Destroy(gameObject);

                break;
        }

        transform.Translate(dir * currentSpeed * Time.deltaTime);
    }

    public bool Kill()
    {
        if (state != FlyingMonsterState.DyingLeft && state != FlyingMonsterState.DyingRight)
        {
            if (state == FlyingMonsterState.FlyingLeft || state == FlyingMonsterState.Attacking)
                state = FlyingMonsterState.DyingLeft;
            else if (state == FlyingMonsterState.FlyingRight)
                state = FlyingMonsterState.DyingRight;

            animator.SetInteger("State", (int)state);

            var particleSystem = transform.Find("Blood").GetComponent<ParticleSystem>();
            particleSystem.Play();

            Player.zombiesKilled++;
            killedAudio.PlayOneShot(killedAudio.clip);

            return true;
        }

        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state != FlyingMonsterState.DyingLeft && state != FlyingMonsterState.DyingRight && other.gameObject.tag.Equals("Player"))
        {
            var player = other.GetComponent<Player>();
            player.Kill();

            if (state == FlyingMonsterState.FlyingLeft || state == FlyingMonsterState.Attacking)
                state = FlyingMonsterState.DyingLeft;
            else if (state == FlyingMonsterState.FlyingRight)
                state = FlyingMonsterState.DyingRight;

            animator.SetInteger("State", (int)state);
        }
    }
}