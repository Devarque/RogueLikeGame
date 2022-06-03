using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//BOSS öldürülünce yüklenecek olan kazanma sahnesinin yüklenmesini yöneten Script

public class VictoryScreen : MonoBehaviour {
    public float waitForAnyKey = 2f;
    public GameObject anyKeyText;
    public string mainMenuScene;

    public SOPlayerData data;

    public Text totalEnemyKilled;
    public Text totalDamageTaken;
    public Text totalDamageDealt;


    void Start() {
        Time.timeScale = 1f;

        totalEnemyKilled.text = data.totalEnemyKilled.ToString();
        totalDamageTaken.text = data.totalDamageTaken.ToString();
        totalDamageDealt.text = data.totalDamageDealt.ToString();

    }

    void Update() {
        if (waitForAnyKey > 0) {
            waitForAnyKey -= Time.deltaTime;
            if (waitForAnyKey <= 0) {
                anyKeyText.SetActive(true);
            }
        } else {
            if (Input.anyKeyDown) {
                SceneManager.LoadScene(mainMenuScene);
            }
        }
    }
}
