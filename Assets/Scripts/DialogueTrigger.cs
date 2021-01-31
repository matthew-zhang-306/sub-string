using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public delegate void DialogueAction();

    [System.Serializable]
    public class DialogueData {
        [TextArea(3, 10)] public string lines;
        public bool shouldIncreaseLevel;
        public DialogueAction onStart;
        public DialogueAction onEnd;
    }

    public bool ignoreFacingAngle;

    public int level;
    public DialogueData[] dialogue;
    public float textSpeed;

    private Collider2D playerColl;
    private Player player;

    public Collider2D coll;
    public Transform popup;
    public Transform speech;
    public SpriteRenderer speechSprite;
    public TextMeshPro speechText;
    public CinemachineVirtualCamera speechCam;
    public AudioSource speechAudio;

    private Vector3 popupScale;
    private Vector3 speechScale;
    private Vector3 popupOffset;
    private Vector3 speechOffset;

    public GameObject pingRing;

    private bool oldCanInteract;
    public bool canInteract { get {
        return playerColl != null &&
            (ignoreFacingAngle || Vector3.Dot(player.transform.right, transform.position - player.transform.position) > 0);
    }}
    public bool isInteracting { get; private set; }

    private void Start() {
        popup.gameObject.SetActive(true);
        speech.gameObject.SetActive(true);

        popupScale = popup.localScale;
        speechScale = speech.localScale;
        popupOffset = popup.position - transform.position;
        speechOffset = speech.position - transform.position;

        speechSprite.transform.rotation = Quaternion.Euler(0, 0, -4f);
        speechSprite.transform.DORotate(Vector3.forward * 4f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        popup.localScale = Vector3.zero;
        speech.localScale = Vector3.zero;
        popup.position = transform.position;
        speech.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Player p = other.GetComponent<Player>();
        if (p != null) {
            player = p;
            playerColl = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other == playerColl) {
            playerColl = null;
        }
    }

    private void FixedUpdate() {
        if (canInteract && !oldCanInteract) {
            // display popup
            popup.DOKill();
            popup.DOScale(popupScale, 0.3f).SetEase(Ease.OutCubic);
            popup.DOMove(transform.position + popupOffset, 0.3f).SetEase(Ease.OutCubic);
        }
        else if (!canInteract && oldCanInteract) {
            // remove popup
            popup.DOKill();
            popup.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
            popup.DOMove(transform.position, 0.3f).SetEase(Ease.OutCubic);
        }

        oldCanInteract = canInteract;
    }


    public void StartDialogue() {
        DOTween.Sequence().InsertCallback(0.4f, () => {
            Instantiate(pingRing, transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(2f, 4f, 0.2f, Color.black.WithA(0.5f));
    
            StartCoroutine(DoDialogue());
        });
    }

    IEnumerator DoDialogue() {
        bool oldPingInput = false;
        bool pingInput = Input.GetAxisRaw("Ping") > 0;

        isInteracting = true;
        player.isInCutscene = true;
        speechCam.Priority = 90;
        coll.enabled = false;

        speech.DOKill();
        speech.position = transform.position;
        speech.DOScale(speechScale, 0.3f).SetEase(Ease.OutCubic);
        speech.DOMove(transform.position + speechOffset, 0.3f).SetEase(Ease.OutCubic);

        DialogueData dialogueData = dialogue[level];
        dialogueData.onStart?.Invoke();

        string[] lines = dialogueData.lines.Split('\n');
        foreach (string line in lines) {
            speechText.text = "";

            oldPingInput = pingInput;
            yield return 0;
            pingInput = Input.GetAxisRaw("Ping") > 0;
            
            var sb = new System.Text.StringBuilder();
            float t = 0;
            
            speechAudio.DOComplete();
            speechAudio.volume = 1;
            speechAudio.Play();
            for (int c = 0; c < line.Length; t += Time.deltaTime) {
                if (pingInput && !oldPingInput) {
                    // skip
                    speechText.text = line;
                    break;
                }
                
                while (c < line.Length && c * textSpeed <= t) {
                    sb.Append(line[c]);
                    speechText.text = sb.ToString();
                    c++;
                }

                oldPingInput = pingInput;
                yield return 0;
                pingInput = Input.GetAxisRaw("Ping") > 0;
            }
            speechAudio.DOFade(0, 0.1f).OnComplete(() => speechAudio.Stop());

            do {
                oldPingInput = pingInput;
                yield return 0;
                pingInput = Input.GetAxisRaw("Ping") > 0;
            } while (!(pingInput && !oldPingInput));
            
            speechText.text = "";
        }

        if (dialogueData.shouldIncreaseLevel) {
            SetLevel(level + 1);
        }
        dialogueData.onEnd?.Invoke();

        speech.DOKill();
        speech.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InCubic).OnComplete(() => speech.position = transform.position);

        coll.enabled = true;
        speechCam.Priority = 0;
        player.isInCutscene = false;
        isInteracting = false;
    }


    public void SetLevel(int newLevel) {
        level = newLevel;
    }
}
