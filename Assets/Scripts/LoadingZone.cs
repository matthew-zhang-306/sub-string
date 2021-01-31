using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZone : MonoBehaviour
{
    public Vector2 spawnOffset;
    public Vector3 spawn { get { return transform.position + spawnOffset.xyz(); }}
    public int buildIndexIncrement;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LevelManager.levelManager.NextLevel(buildIndexIncrement);
        }    
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawn, 1f);
    }
}
