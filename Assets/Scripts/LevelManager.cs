using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager { get; private set; }

    public CinemachineBrain cam;
    public CinemachineVirtualCamera deathCam;
    public Player player;

    public Transform dialogueContainer;
    public Transform followerContainer;
    public Transform boxContainer;
    public DialogueTrigger[] dialogues { get; private set; }
    public Follower[] followers { get; private set; }
    public GrabbableBox[] boxes { get; private set; }

    public LoadingZone entrance;
    public LoadingZone exit;
    public GameObject fadeIn;
    public GameObject fadeOut;
    private bool isLevelTransitioning;

    private void Awake() {
        // replace with actual singleton check later
        levelManager = this;

        player.levelManager = this;
        dialogues = dialogueContainer.GetComponentsInChildren<DialogueTrigger>(true);
        followers = followerContainer.GetComponentsInChildren<Follower>(true);
        boxes = boxContainer.GetComponentsInChildren<GrabbableBox>(true);

        if (GlobalManager.Instance.previousBuildIndexIncrement < 0)
            player.transform.position = exit.spawn;
        else  
            player.transform.position = entrance.spawn;

        Instantiate(fadeIn);
    }

    private void OnEnable() {
        Entity.OnDeath += DeathSequence;
    }

    private void OnDisable() {
        Entity.OnDeath -= DeathSequence;
    }

    private void FixedUpdate() {
        if (!isLevelTransitioning && Input.GetKeyDown(KeyCode.R)) {
            isLevelTransitioning = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void DeathSequence(Entity entity) {
        if (isLevelTransitioning)
            return;
        isLevelTransitioning = true;

        deathCam.m_Priority = 100;
        deathCam.Follow = entity.transform;

        DOTween.Sequence().InsertCallback(2f, () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void NextLevel(int buildIndexIncrement) {
        if (isLevelTransitioning)
            return;
        isLevelTransitioning = true;

        player.isInCutscene = true;
        GlobalManager.Instance.previousBuildIndexIncrement = buildIndexIncrement;

        Instantiate(fadeOut);
        DOTween.Sequence().InsertCallback(1f, () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + buildIndexIncrement);
        });
    }
}
