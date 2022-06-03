using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Oyun içi geçen oynama süresini oluþturan ve yöneten script

public class InGameCounter : MonoBehaviour {
    public static InGameCounter instance;

    public Text timeCounter;

    private TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;

    public SOPlayerData data;


    private void Awake() {
        instance = this;
    }

    private void Start() {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            timeCounter.text = "Time: 00:00.00";
            timerGoing = false;
            BeginTimer();
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BossScene")) {
            ContinueTimer();
        }
    }

    public void BeginTimer() {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void ContinueTimer() {
        timerGoing = true;
        StartCoroutine(UpdateTimer());
    }

    public void StopTimer() {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer() {
        while (timerGoing) {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BossScene")) {
                elapsedTime += Time.deltaTime;
                timePlaying = data.playTime + TimeSpan.FromSeconds(elapsedTime);
            } else {
                elapsedTime += Time.deltaTime;
                timePlaying = TimeSpan.FromSeconds(elapsedTime);
                data.playTime = timePlaying;
            }
            string timePlayingStr = timePlaying.ToString("mm':'ss'.'ff");
            timeCounter.text = timePlayingStr;
            data.totalTime = timePlayingStr;
            yield return null;
        }
    }
}
