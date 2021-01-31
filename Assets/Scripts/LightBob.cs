using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightBob : MonoBehaviour
{
    public float multiplier;
    public float speed;

    private void Start() {
      transform.DOScale(transform.localScale * multiplier, speed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
