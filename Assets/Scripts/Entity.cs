using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{
    public SpriteRenderer sprite;
    protected Rigidbody2D rb2d;
    protected AudioSource audioSource;

    public bool canDie = true;
    public Color deathColor;
    public GameObject deathParticles;
    public AudioClip deathSound;
    public AudioClip explodeSound;
    protected bool isDead;

    public delegate void EntityDelegate(Entity entity);
    public static EntityDelegate OnDeath;

    protected virtual void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Hazard")) {
            Die(other);
        }
    }

    public virtual void Die(Collider2D hazard) {
        if (isDead || !canDie)
            return;
        
        isDead = true;

        // determine knockback direction
        Vector2 knockback = -(hazard.ClosestPoint(transform.position) - transform.position.xy());
        if (knockback == Vector2.zero)
            knockback = -rb2d.velocity;
        if (knockback == Vector2.zero)
            knockback = -transform.right;
        knockback = knockback.normalized;

        sprite.color = deathColor;
        audioSource.PlayOneShot(deathSound);
        DOTween.Sequence()
            .Insert(0, transform.DOMove(transform.position + knockback.xyz(), 0.5f).SetEase(Ease.OutCubic))
            .Insert(0, sprite.transform.DOShakePosition(0.5f, 1, 100, 90))
            .InsertCallback(0.5f, () => {
                audioSource.PlayOneShot(explodeSound);
            })
            .Insert(0.5f, sprite.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack))
            .InsertCallback(0.6f, () => {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            })
        .SetTarget(gameObject)
        .SetLink(gameObject)
        .SetUpdate(UpdateType.Fixed);
    
        OnDeath?.Invoke(this);
    }
}
