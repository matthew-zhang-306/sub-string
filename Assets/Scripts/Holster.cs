using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Holster : MonoBehaviour
{
    public float radius;
    public float angle;

    public void SetPlayer(Player player) {
        // start movement
        DOTween.To(
            () => angle,
            (newAngle) => {
                angle = (float)newAngle;
                transform.rotation = Quaternion.identity;
                transform.position = player.transform.position + Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
            },
            360, 2f
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1)
        .SetUpdate(UpdateType.Fixed)
        .SetLink(player.gameObject);
    }
}
