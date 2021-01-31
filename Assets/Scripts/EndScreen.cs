using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class EndScreen : MonoBehaviour
{
    public GameObject fadeOut;

    // Start is called before the first frame update
    void Start()
    {
        float delay = -0.1f;
        var text = GetComponent<TMPro.TextMeshPro>();

        GlobalManager.Instance?.StopAmbience();
        DOTween.Sequence().InsertCallback(3f, () => {
            GetComponent<AudioSource>().Play();
            DOTween.Sequence()
                .InsertCallback(1.44f  + delay, () => text.text = "i can't see far in the river")
                .InsertCallback(7.23f  + delay, () => text.text = "but i can hear your soft murmur")
                .InsertCallback(13.01f + delay, () => text.text = "i'll hold you close in the waterfall")
                .InsertCallback(18.97f + delay, () => text.text = "and listen for your call")
                .InsertCallback(24.76f + 2*delay, () => text.text = "with you i'm reaching for the stars")
                .InsertCallback(29.82f + 2*delay, () => text.text = "thank you for playing!\n~ 1f1n1ty")
                .InsertCallback(42f, () => {
                    Instantiate(fadeOut);
                    DOTween.Sequence().InsertCallback(1f, () => {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    });
                })
                .SetUpdate(UpdateType.Fixed);
        });
    }
}
