using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mover : MonoBehaviour
{
    public float startTime;
    public float moveTime;
    public float pauseTime;
    public Vector2 moveOffset;
    public Ease moveEaseForward;
    public Ease moveEaseBackward;

    Vector2 startPosition;
    Vector2 endPosition { get { return startPosition + moveOffset; }}

    private void Start() {
        startPosition = transform.position;
        DOTween.Sequence().SetDelay(startTime)
            .Insert(0, transform.DOMove(endPosition, moveTime).SetEase(moveEaseForward))
            .Insert(moveTime + pauseTime, transform.DOMove(startPosition, moveTime).SetEase(moveEaseBackward))
            .Insert(2 * (moveTime + pauseTime), DOTween.Sequence())
            .SetLoops(-1)
            .SetUpdate(UpdateType.Fixed);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, moveOffset);
        Gizmos.DrawWireCube(transform.position + moveOffset.xyz(), transform.localScale);
    }
}
