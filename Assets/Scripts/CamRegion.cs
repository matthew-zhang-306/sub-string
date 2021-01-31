using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRegion : MonoBehaviour
{
    private void OnDrawGizmos() {
        Collider2D coll = GetComponent<Collider2D>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(coll.bounds.center, coll.bounds.size);
    }

    private void OnDrawGizmosSelected() {
        // do nothing
    }
}
