using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingDoor : Triggerable
{
    public float angle;
    public float time;
    float baseAngle;

    Tween moveTween;

    private bool state;
    public Color pingColor;

    private void Start() {
        baseAngle = transform.rotation.eulerAngles.z;
    }

    public override void Trigger(SoundString trigger)
    {
        base.Trigger(trigger);
        state = !state;

        Instantiate(trigger.pingRing, transform.position, Quaternion.identity)
            .GetComponent<PingRing>().Init(1.5f, 3f, 0.1f, pingColor.WithA(0.5f));

        moveTween?.Kill();
        moveTween = DOTween.Sequence().InsertCallback(0.5f, () => {
            moveTween = transform.DORotate(Vector3.forward * (state ? baseAngle + angle : baseAngle), time)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(UpdateType.Fixed);
        })
        .SetTarget(gameObject)
        .SetLink(gameObject);
        
    }

    private void OnDrawGizmos() {
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
            
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, -transform.right * sprite.size.x);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, angle) * -transform.right * sprite.size.x);
    }
}
