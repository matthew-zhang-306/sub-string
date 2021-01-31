using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFade : MonoBehaviour
{
    public float targetAlpha;

    private void Start() {
        Image image = GetComponent<Image>();
        image.color = image.color.WithA(1 - targetAlpha);
        image.DOFade(targetAlpha, 1f).OnComplete(() => {
            if (targetAlpha == 0)
                Destroy(transform.parent.gameObject);
        });
    }
}
