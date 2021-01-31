using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDestroySelf : MonoBehaviour
{
    private void Awake() {
        GetComponent<DialogueTrigger>().dialogue[0].onEnd += Destroy;
    }

    private void Destroy() {
        Destroy(GetComponent<Collider2D>());
    }
}
