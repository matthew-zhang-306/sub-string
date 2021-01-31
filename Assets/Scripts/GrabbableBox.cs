using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrabbableBox : Entity
{
    public bool grabbed { get; private set; }
    private Transform originalParent;
    private BoxCollider2D coll;

    public GameObject pingRing;
    public Color pingColor;

    public AudioClip grabClip;
    public AudioClip placeClip;

    private Tween moveTween;

    protected override void Start() {
        base.Start();
        originalParent = transform.parent;
    
        coll = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
        transform.rotation = Quaternion.identity;
    }

    public void Grab(Player player) {
        DOTween.Sequence().InsertCallback(0.4f, () => {
            Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(2f, 4f, 0.2f, pingColor.WithA(0.5f));
            
            audioSource.PlayOneShot(grabClip);
            grabbed = true;

            moveTween?.Kill();
            coll.enabled = false;
            transform.SetParent(player.holster.transform);
            moveTween = transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
        })
        .SetTarget(gameObject)
        .SetLink(gameObject);
    }

    public void Place(Player player) {
        Vector3 placePosition = player.transform.position;

        DOTween.Sequence().InsertCallback(0.4f, () => {
            Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(2f, 4f, 0.2f, pingColor.WithA(0.5f));
            audioSource.PlayOneShot(placeClip);
            
            grabbed = false;
            
            moveTween?.Kill();
            transform.SetParent(originalParent);
            moveTween = transform.DOMove(placePosition, 0.5f).SetEase(Ease.OutCubic)
                .OnComplete(() => {
                    coll.enabled = true;
                });
        })
        .SetTarget(gameObject)
        .SetLink(gameObject);
    }
}
