using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleScreenPlayer : MonoBehaviour
{
    Player actualPlayer;
    private bool hasStarted;

    private AudioSource audioSource;
    public GameObject fadeOut;

    void Start()
    {
        actualPlayer = GetComponent<Player>();    
        audioSource = GetComponent<AudioSource>();

        GlobalManager.Instance.PlayAmbience();
        GlobalManager.Instance.PlaySong(3);
    }

    private void FixedUpdate() {
        if (!hasStarted && Input.GetAxis("Ping") > 0) {
            hasStarted = true;

            // player animation
            Color baseSpriteColor = actualPlayer.sprite.color;
            actualPlayer.sprite.transform.localRotation = Quaternion.identity;
            actualPlayer.sprite.color = actualPlayer.pingColor;
            DOTween.Sequence()
                .Insert(0, actualPlayer.sprite.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.8f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic))
                .Insert(0, actualPlayer.sprite.DOColor(baseSpriteColor, 0.6f).SetEase(Ease.InCubic));

            // ring
            PingRing ring = Instantiate(actualPlayer.pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>();
            ring.Init(2f, 5f, 0.2f, actualPlayer.pingColor.WithA(0.5f));

            // sound
            audioSource.PlayOneShot(actualPlayer.pingAudio);

            // start level transition
            GlobalManager.Instance.StopSong();
            DOTween.Sequence().InsertCallback(1f, () => {
                Instantiate(fadeOut);
            }).InsertCallback(2f, () => {
                SceneManager.LoadScene(2);
            });
        }
    }




}
