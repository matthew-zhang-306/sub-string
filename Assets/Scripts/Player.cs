using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Player : Entity
{
    public LevelManager levelManager;

    public Color pingColor;
    public Color unpingColor;
    Color baseSpriteColor;

    public GameObject pingRing;

    [System.Serializable] public class PlayerEvent : UnityEvent<Player> {}
    public PlayerEvent pingEvent;
    public PlayerEvent unpingEvent;

    private HashSet<Follower> followers;
    private GrabbableBox grabbedBox;
    public Holster holster;

    public float maxSpeed;
    public float accel;
    public float turnSpeed;

    Vector2 facingDir;

    public float pingCooldown;
    bool oldPingInput;
    float pingInputBuffer;
    float pingTimer;
    Tween pingTween;

    public AudioClip pingAudio;
    public AudioClip unpingAudio;

    public bool isInCutscene;

    protected override void Start() {
        base.Start();

        holster.SetPlayer(this);
        followers = new HashSet<Follower>();
        baseSpriteColor = sprite.color;
    }

    private void FixedUpdate() {
        HandleMovement();
        HandlePing();
    }

    private void HandleMovement() {
        if (isDead) {
            rb2d.angularVelocity = 0;
            rb2d.velocity = Vector2.zero;
            return;
        }

        Vector2 velocity = rb2d.velocity;

        Vector2 input = Vector2.zero;
        if (!isInCutscene)
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        
        if (input.magnitude > 0.1f) {
            velocity += input * accel;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

            if (velocity.magnitude > 0.1f) {
                facingDir = Vector2.Lerp(input, velocity.normalized, 0.5f);
            }
        } else {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, accel);
        }

        float turnAngle = velocity.sqrMagnitude != 0 ? Vector2.SignedAngle(transform.right, facingDir) : 0;
        rb2d.angularVelocity = Mathf.Clamp(turnAngle, -turnSpeed, turnSpeed) / Time.deltaTime;
        rb2d.velocity = velocity;
    }

    private void HandlePing() {
        if (isInCutscene || isDead)
            pingTimer = pingCooldown;
        else
            pingTimer = Mathf.Max(pingTimer - Time.deltaTime, 0);

        bool pingInput = Input.GetAxisRaw("Ping") > 0;
        pingInputBuffer = Mathf.Max(pingInputBuffer - Time.deltaTime, 0);
        if (pingInput && !oldPingInput)
            pingInputBuffer = 0.1f;
        oldPingInput = pingInput;
        
        if (pingTimer == 0 && pingInputBuffer > 0) {
            // ping! (or unping)
            pingTimer = pingCooldown;
            bool isPing = ContactPingTarget();

            // player animation
            pingTween?.Complete();
            sprite.transform.localRotation = Quaternion.identity;
            sprite.color = isPing ? pingColor : unpingColor;
            pingTween = DOTween.Sequence()
                .Insert(0, sprite.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.8f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic))
                .Insert(0, sprite.DOColor(baseSpriteColor, 0.6f).SetEase(Ease.InCubic));

            // ring
            PingRing ring = Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>();
            if (isPing)
                ring.Init(2f, 5f, 0.2f, pingColor.WithA(0.5f));
            else
                ring.Init(1f, 4f, 0.2f, Color.black.WithA(0.5f));

            // sound
            audioSource.PlayOneShot(isPing ? pingAudio : unpingAudio);
        }
    }

    private bool ContactPingTarget() {
        // check for dialogue
        DialogueTrigger dialogue = levelManager.dialogues.FirstOrDefault(d => d.canInteract);
        if (dialogue != null) {
            dialogue.StartDialogue();
            return true;
        }

        // check for place box
        if (grabbedBox != null) {
            grabbedBox.Place(this);
            grabbedBox = null;
            return false;
        }

        // check for grab box
        GrabbableBox box = null;
        float minDist = 2f;
        foreach (GrabbableBox b in levelManager.boxes) {
            float dist = (b.transform.position - transform.position).magnitude;
            if (dist <= minDist) {
                box = b;
                minDist = dist;
            }
        }

        if (box != null) {
            // grab this box
            grabbedBox = box;
            box.Grab(this);
            return true;
        }

        // check for follower to stop
        //if (followers.Count > 0) {
        //    foreach (Follower follower in followers)
        //        follower.OnUnping(this);
        //    return false;
        //}

        // check for followers to start
        Follower[] newFollowers = levelManager.followers; // filter this later
        if (newFollowers.Length > 0) {
            foreach (Follower follower in newFollowers)
                follower.OnPing(this);
            return true;
        }

        // no targets
        return true;
    }


    public void AddFollower(Follower follower) {
        followers.Add(follower);
    }

    public void RemoveFollower(Follower follower) {
        followers.Remove(follower);
    }
}
