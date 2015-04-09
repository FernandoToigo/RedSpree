using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityEngine
{
    public static class Vector3Extensions
    {
        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}

public enum PlayerState
{
    Running,
    ShootingFront,
    ShootingUp,
    Dying,
    Dead
}

public class Player : MonoBehaviour
{
    public PlayerState state;
    public GameObject bulletPrefab;
    public static Vector3 position;
    public static float distanceTraveled;
    public static int _bullets = 10;
    public static int _zombiesKilled = 0;

    public static int Bullets
    {
        get { return _bullets; }
        set
        {
            var diff = value - _bullets;
            _bullets = value;

            if (diff > 0)
                uiBullets.AddBullets(diff);
            else
                uiBullets.RemoveBullets(-diff);
        }
    }

    public static int zombiesKilled
    {
        get
        {
            return _zombiesKilled;
        }
        set
        {
            if (_zombiesKilled < 100 && value >= 100)
                message.ShowMessage("Pierce +1", 5.0f);

            _zombiesKilled = value;
        }
    }

    public float _maxSpeed = 5.0f;

    private float _currentJumpForce = 0.0f;
    private float _jumpForce = 800.0f;

    private float shootDelay = 0.2f;
    private float totalShootTimer = 0.2f;
    private float shootTimer;

    private float dyingTimer = 0.8f;
    private float deadTimer = 3.0f;

    private static UIBullets uiBullets;
    private static Message message;
    private AudioSource shotAudio;
    private Animator animator;
    private List<Zombie> currentZombies = new List<Zombie>();
    private Zombie lootingZombie = null;
    private bool _jump = false;
    private bool _releasedJump = false;
    private bool _canRun = true;
    private bool _grounded = true;
    private Transform[] _groundChecks;
    private Transform[] _headChecks;
    private Transform _wallCheckStart;
    private Transform _wallCheckEnd;
    private Transform _gun;
    private Rigidbody2D _rigidBody;
    private bool _wantToFall = false;

    void Start()
    {
        _rigidBody = this.GetComponent<Rigidbody2D>();
        _groundChecks = transform.Find("GroundChecks").GetComponentsInChildren<Transform>();
        _headChecks = transform.Find("HeadChecks").GetComponentsInChildren<Transform>();
        _wallCheckStart = transform.Find("WallChecks").Find("WallCheckStart");
        _wallCheckEnd = transform.Find("WallChecks").Find("WallCheckEnd");
        _gun = transform.Find("Gun");
        uiBullets = GameObject.Find("UIBullets").GetComponent<UIBullets>();
        message = GameObject.Find("Message").GetComponent<Message>();
        _bullets = 10;
        zombiesKilled = 0;
        shootTimer = totalShootTimer;
        animator = this.GetComponent<Animator>();
        shotAudio = this.GetComponent<AudioSource>();
        //Physics2D.IgnoreLayerCollision(10, 11, true);
    }

    void Update()
    {
        var oldGrounded = _grounded;
        foreach (Transform check in _groundChecks)
        {
            _grounded = Physics2D.Raycast(check.position, -Vector2.up, 0.05f, 1 << LayerMask.NameToLayer("Ground"));
            if (_grounded)
                break;
            else
            {
                _grounded = Physics2D.Raycast(check.position, -Vector2.up, 0.05f, 1 << LayerMask.NameToLayer("Platform"));
                if (_grounded)
                    break;
            }
        }

        if (_grounded)
        {
            _currentJumpForce = _jumpForce;
            _releasedJump = false;
            //Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, 1.0f), transform.position + Vector3.up * 2.0f + new Vector3(0.0f, 0.0f, 1.0f), Color.red);
        }

        if (!_grounded && !_releasedJump)
        {
            if (Input.GetButton("Jump"))
            {
                var force = Mathf.Min(_currentJumpForce, 400.0f * Time.deltaTime);
                this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * force);
                _currentJumpForce -= force;
            }
            else
                _releasedJump = true;
        }

        if (state == PlayerState.Running)
        {
            if (Bullets > 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    state = PlayerState.ShootingFront;
                    animator.SetInteger("State", (int)state);
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    state = PlayerState.ShootingUp;
                    animator.SetInteger("State", (int)state);
                }
            }
        }

        if (Input.GetButtonDown("Jump") && _grounded)
            _jump = true;

        switch (state)
        {
            case PlayerState.ShootingFront:
                Shoot(false);
                break;

            case PlayerState.ShootingUp:
                Shoot(true);
                break;

            case PlayerState.Dying:
                dyingTimer -= Time.deltaTime;
                if (dyingTimer < 0)
                {
                    state = PlayerState.Dead;
                    animator.SetInteger("State", (int)state);
                }

                break;
            case PlayerState.Dead:
                deadTimer -= Time.deltaTime;
                if (deadTimer < 0 && Input.anyKey)
                    Application.LoadLevel(Application.loadedLevel);
                break;
        }

        if (Input.GetAxis("Vertical") < 0)
            _wantToFall = true;

        _canRun = !Physics2D.Linecast(_wallCheckStart.position, _wallCheckEnd.position, 1 << LayerMask.NameToLayer("Ground"));

        distanceTraveled = transform.localPosition.x;
        position = transform.localPosition;
    }

    void FixedUpdate()
    {
        if (_canRun && state != PlayerState.Dead && state != PlayerState.Dying)
            if (_rigidBody.velocity.x < _maxSpeed)
                _rigidBody.velocity += new Vector2(Mathf.Min(5.0f, _maxSpeed - GetComponent<Rigidbody2D>().velocity.x), 0);

        if (_jump)
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300.0f);
            _jump = false;
        }

        Transform transform;
        if (IsOnPlatform(out transform))
        {
            //Debug.Log("On Platform");

            if (_wantToFall)
            {
                //Physics2D.IgnoreLayerCollision(10, 11, true);
                ChangeLayersOfChildrens(transform, "PlatformGhost");
                _wantToFall = false;
            }
            else
            {
                //Physics2D.IgnoreLayerCollision(10, 11, false);
                ChangeLayersOfChildrens(transform, "Platform");
            }
        }
        else if (IsUnderPlatform(out transform))
        {
            //Debug.Log("Under Platform");
            //Physics2D.IgnoreLayerCollision(10, 11, true);
            ChangeLayersOfChildrens(transform, "PlatformGhost");
        }

        if (HasPlatformForward(out transform))
            ChangeLayersOfChildrens(transform, "PlatformGhost");
    }

    private bool IsNearPlatform(Vector3 direction, Transform[] checkers, out Transform transform)
    {
        float smallDistance = 0.05f;
        //Layer
        int platformLayer = LayerMask.NameToLayer("Platform");
        int platformGhostLayer = LayerMask.NameToLayer("PlatformGhost");
        //Only hit the platform layer
        int layerMask = (1 << platformLayer) | (1 << platformGhostLayer);
        foreach (Transform check in checkers)
        {
            var hit = Physics2D.Raycast(check.position.ToVector2(), direction.ToVector2(), smallDistance, layerMask);
            if (hit)
            {
                transform = hit.transform;
                return true;
            }
        }

        transform = null;
        return false;
    }

    private bool HasPlatformForward(out Transform transform)
    {
        //Layer
        int platformLayer = LayerMask.NameToLayer("Platform");
        int platformGhostLayer = LayerMask.NameToLayer("PlatformGhost");
        //Only hit the platform layer
        int layerMask = (1 << platformLayer) | (1 << platformGhostLayer);

        var hit = Physics2D.Linecast(_wallCheckStart.position, _wallCheckEnd.position, layerMask);
        if (hit)
        {
            transform = hit.transform;
            return true;
        }

        transform = null;
        return false;
    }

    private bool IsOnPlatform(out Transform transform)
    {
        return IsNearPlatform(Vector3.down, _groundChecks, out transform);
    }

    private bool IsUnderPlatform(out Transform transform)
    {
        return IsNearPlatform(Vector3.up, _headChecks, out transform);
    }

    public static void ChangeLayersOfChildrens(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersOfChildrens(child, name);
        }
    }

    void Shoot(bool up)
    {
        float oldTimer = shootTimer;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= shootDelay && oldTimer >= shootDelay)
        {
            //Bullets--;
            GameObject o = (GameObject)Instantiate(bulletPrefab);
            var mouse = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //var angle = Vector2.Angle(new Vector2(mouse.x, mouse.y) - new Vector2(_gun.position.x, _gun.position.y), Vector2.right + new Vector2(_gun.position.x, _gun.position.y));
            var mouseOrigin = new Vector2(mouse.x, mouse.y) - new Vector2(_gun.position.x, _gun.position.y);
            var rightOrigin = Vector2.right + new Vector2(_gun.position.x, _gun.position.y);
            var angle = (Mathf.Atan2(mouseOrigin.y, mouseOrigin.x) - Mathf.Atan2(rightOrigin.y, rightOrigin.x)) * Mathf.Rad2Deg;
            //Debug.Log(string.Format("[{0}];[{1}]:{2}", new Vector2(mouse.x, mouse.y) - new Vector2(_gun.position.x, _gun.position.y), Vector2.right - new Vector2(_gun.position.x, _gun.position.y), angle));
            //Debug.Log(angle);
            o.transform.Rotate(new Vector3(0f, 0f, angle));
            o.transform.localPosition = _gun.position;

            shotAudio.PlayOneShot(shotAudio.clip);
        }
        if (shootTimer < 0.0f)
        {
            shootTimer = totalShootTimer;
            state = PlayerState.Running;
            animator.SetInteger("State", (int)state);
        }
    }

    public void Kill()
    {
        if (state != PlayerState.Dying && state != PlayerState.Dead)
        {
            state = PlayerState.Dying;
            animator.SetInteger("State", (int)state);
            message.ShowMessage(string.Format("You died!\nSurvived {0:00}:{1:00}\nKilled {2} enemies", (int)(Time.timeSinceLevelLoad / 60), (int)(Time.timeSinceLevelLoad % 60), Player.zombiesKilled), 99999999);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Zombie"))
        {
            var zombie = other.GetComponent<Zombie>();
            if (!zombie.looted && zombie.Loot())
                Bullets += zombie.bullets;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //if (other.gameObject.tag.Equals("Zombie"))
        //{
        //    var zombie = other.GetComponent<ZombieBehviour>();
        //    if (currentZombies.Contains(zombie))
        //        currentZombies.Remove(zombie);
        //}
    }
}