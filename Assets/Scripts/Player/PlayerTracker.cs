using UnityEngine;
using UnityEngine.SceneManagement;

//OYUNCUNUN BOSS ODASINA GÝRÝÞÝNÝ KONTROL EDEN SCRIPT

public class PlayerTracker : MonoBehaviour {
    public SOPlayerData playerData;

    void Start() {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BossScene")) {
            PlayerController.instance.gameObject.transform.position = transform.position;

            PlayerAttributeController.instance.currentHealth = playerData.currentHealth;
            PlayerAttributeController.instance.maxHealth = playerData.maxHealth;

            UIController.instance.coinText.text = "0";
            UIController.instance.keyText.text = "0";
        }
    }
}
