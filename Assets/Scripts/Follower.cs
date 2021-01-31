using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Follower : Entity
{
    private Collider2D coll;

    public Color pingColor;
    public Color nodeColor;
    public GameObject pingRing;

    public AudioClip pingClip;

    public Transform fixedPingLocation;
    private Tween moveTween;
    private Vector2 pingLocation;
    private bool isMoving;

    public float maxSpeed;
    public float accel;
    public float turnSpeed;
    float stoppedTimer;

    public bool canFollow { get; private set; }

    protected override void Start()
    {
        base.Start();
        canFollow = true;
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate() {
        if (isDead) {
            rb2d.angularVelocity = 0;
            rb2d.velocity = Vector2.zero;
            return;
        }

        if (!isMoving) {
            rb2d.angularVelocity = turnSpeed * 0.05f / Time.deltaTime;
            rb2d.velocity = Vector2.MoveTowards(rb2d.velocity, Vector2.zero, accel);
            return;
        }

        Vector2 velocity = rb2d.velocity;

        Vector2 move = pingLocation - transform.position.xy();
        if (move.magnitude < 0.1f) {
            StopFollowing();
            
            rb2d.angularVelocity = 0;
            rb2d.velocity = Vector2.zero;
        }
        else {
            Debug.Log(velocity.magnitude);
            if (velocity.magnitude < 1f) {
                // not moving even though we want to move? there might be a wall in front of us
                stoppedTimer += Time.deltaTime;
                if (stoppedTimer >= 1f)
                    StopFollowing();
            } else {
                stoppedTimer = 0f;
            }

            velocity = 3f * move;
            velocity = Vector2.ClampMagnitude(velocity, Mathf.Min(maxSpeed, velocity.magnitude + accel));
            
            // remove follower early if they are moving pretty slowly
            if (velocity.magnitude < maxSpeed * 0.8f) {
                LevelManager.levelManager.player.RemoveFollower(this);
            }

            float turnAngle = velocity.sqrMagnitude != 0 ? Vector2.SignedAngle(transform.right, move) : 0;
            rb2d.angularVelocity = Mathf.Clamp(turnAngle, -turnSpeed, turnSpeed) / Time.deltaTime;
            rb2d.velocity = velocity;
        }
    }

    public void OnPing(Player player) {
        if (!canFollow)
            return;

        Vector2 nextPingLocation = fixedPingLocation != null ? fixedPingLocation.position : player.transform.position;

        DOTween.Sequence().InsertCallback(0.6f, () => {
            Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(2f, 4f, 0.2f, pingColor.WithA(0.5f));
            
            audioSource.PlayOneShot(pingClip);

            pingLocation = nextPingLocation;
            isMoving = true;
            stoppedTimer = 0f;

            LevelManager.levelManager.player.AddFollower(this);
        })
        .SetTarget(gameObject)
        .SetLink(gameObject);
    }

    public void OnUnping(Player player) {
        if (!canFollow)
            return;
        
        DOTween.Sequence().InsertCallback(0.1f, () => {
            Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(2f, 4f, 0.2f, pingColor.WithA(0.5f));
            audioSource.PlayOneShot(pingClip);
            
            StopFollowing();
        })
        .SetTarget(gameObject)
        .SetLink(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        
        if (other.CompareTag("Node")) {
            StringNode node = other.GetComponentInParent<StringNode>();

            if (!node.hasTarget) {
                // go to the string node and stay at it
                canFollow = false;
                pingLocation = other.transform.position;
                LevelManager.levelManager.player.RemoveFollower(this);

                coll.isTrigger = true;
                node.hasTarget = true;
                other.enabled = false;
                
                Instantiate(pingRing, transform.position, Quaternion.identity)
                    .GetComponent<PingRing>().Init(2f, 4f, 0.2f, nodeColor.WithA(0.5f));
                audioSource.PlayOneShot(pingClip);
            }
        }
    }

    private void StopFollowing() {
        isMoving = false;
        LevelManager.levelManager.player.RemoveFollower(this);
    }
}
