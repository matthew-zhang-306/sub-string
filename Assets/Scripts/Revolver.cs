using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Revolver : MonoBehaviour
{
    public float startTime;
    public float rotateTime;
    public float pauseTime;
    public float rotateAmount;
    public Ease rotateEase;
    public bool shouldRewind;

    private void Start() {
        if (shouldRewind) {
            float startRotation = transform.rotation.eulerAngles.z;
            float endRotation = transform.rotation.eulerAngles.z + rotateAmount;
            DOTween.Sequence().SetDelay(startTime)
                .Insert(0, transform.DOLocalRotate(Vector3.forward * endRotation, rotateTime, RotateMode.FastBeyond360).SetEase(rotateEase))
                .Insert(rotateTime + pauseTime, transform.DOLocalRotate(Vector3.forward * startRotation, rotateTime, RotateMode.FastBeyond360).SetEase(rotateEase))
                .Insert(2 * (rotateTime + pauseTime), DOTween.Sequence())
                .SetLoops(-1)
                .SetUpdate(UpdateType.Fixed);
        }
        else {
            DOTween.Sequence().SetDelay(startTime)
                .Insert(0, transform.DOLocalRotate(Vector3.forward * rotateAmount, rotateTime, RotateMode.FastBeyond360).SetEase(rotateEase))
                .Insert(rotateTime + pauseTime, DOTween.Sequence())
                .SetLoops(-1, LoopType.Incremental)
                .SetUpdate(UpdateType.Fixed);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
        Gizmos.DrawRay(transform.position, transform.right * transform.localScale.x);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, rotateAmount) * transform.right * transform.localScale.x);
    }
}
