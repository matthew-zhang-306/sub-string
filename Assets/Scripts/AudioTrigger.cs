using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public enum AudioCommand {
        PLAY,
        STOP,
        FADEOUT,
        FADEIN,
        NONE
    }
    public AudioCommand leftCommand;
    public AudioCommand rightCommand;
    public int number;

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            // pick the right command
            var cmd = Vector3.Dot(transform.right, other.transform.position - transform.position) > 0 ? rightCommand : leftCommand;
            switch (cmd) {
                case AudioCommand.PLAY:
                    GlobalManager.Instance.PlaySong(number);
                    break;
                case AudioCommand.STOP:
                    GlobalManager.Instance.StopSong();
                    break;
            }
        }
    }
}
