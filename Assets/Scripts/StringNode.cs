using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StringNode : MonoBehaviour
{
    HashSet<Collider2D> others;

    private bool isActive;
    public bool hasTarget;
    public float wobbleSpeed;
    public float wobbleIntensity;
    float wobbleTimer;
    float wobbleActive;
    Tween wobbleTween;

    public StringNode otherNode;
    public SoundString soundString;

    private void Start() {
        others = new HashSet<Collider2D>();
        soundString.gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        wobbleTimer += Time.deltaTime * wobbleSpeed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(wobbleTimer) * wobbleActive);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player") && others.Add(other) && others.Count == 1) {
            // something is on it!
            isActive = true;
            if (otherNode.isActive) {
                soundString.gameObject.SetActive(true);
            }

            wobbleTween?.Kill();
            wobbleTween = DOTween.To(() => wobbleActive, w => wobbleActive = w, wobbleIntensity, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (others.Remove(other) && others.Count == 0) {
            // nothing is on it
            isActive = false;
            soundString.gameObject.SetActive(false);
            
            wobbleTween?.Kill();
            wobbleTween = DOTween.To(() => wobbleActive, w => wobbleActive = w, 0f, 0.5f);
        }
    }
}
