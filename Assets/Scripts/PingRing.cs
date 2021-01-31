using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PingRing : MonoBehaviour
{
    LineRenderer line;

    private Vector2[] unitCircle;
    private float currentRadius;
	
    private void CreateUnitCircle(int segments) {
        unitCircle = new Vector2[segments];

        float angle = 0f;
        for (int i = 0; i < segments; i++) {
			unitCircle[i].x = Mathf.Sin(Mathf.Deg2Rad * angle);
			unitCircle[i].y = Mathf.Cos(Mathf.Deg2Rad * angle);
			angle += 360f / segments;
		}
    }

	private void DrawCircle(Vector2 origin, float radius, int segments) {	
        currentRadius = radius;
		for (int i = 0; i < segments; i++) {
			line.SetPosition (i, (unitCircle[i] * radius + origin).xyz());
		}
	}
    
    public void Init(float lifetime, float maxSize, float thickness, Color baseColor, int segments = 50) {
        transform.localScale = Vector3.zero;

        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
		line.useWorldSpace = true;
        line.SetWidth(thickness);
        line.startColor = baseColor;
        line.endColor = baseColor;

        CreateUnitCircle(segments);

        DOTween.Sequence()
            .Insert(0,
                // fade
                DOTween.To(
                    () => line.startColor.a,
                    (alpha) => line.SetColor(baseColor.WithA(alpha)),
                    0, lifetime
                )
                .SetEase(Ease.InQuad)
            )
            .Insert(0,
                // thickness
                DOTween.To(
                    () => line.startWidth,
                    (width) => line.SetWidth(width),
                    0, lifetime
                )
                .SetEase(Ease.InQuad)
            )
            .Insert(0,
                // radius
                DOTween.To(
                    () => currentRadius,
                    (radius) => DrawCircle(transform.position, radius, segments),
                    maxSize, lifetime
                )
                .SetEase(Ease.OutQuad)
            )
            .SetUpdate(UpdateType.Fixed)
            .SetTarget(gameObject)
            .SetLink(gameObject)
            .OnComplete(() => Destroy(gameObject));
    }
}
