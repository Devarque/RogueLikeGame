using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncu boss odas�na girdi�inde aktif olan Script.

public class PlayerEntry : MonoBehaviour
{
    public SOPlayerData data;                //Kullan�c� verileri.

    public static PlayerEntry instance; 
    public bool playerIsPresent = false;    //Oyuncunun odaya girme kontrol�
    public GameObject doors;                //Boss odas� kap�lar�. Kapatmak i�in
    private float endGameWaitTime = 3.5f;   //Oyun bitince biti� ekran�na 

    private void Awake() {
        instance = this;                    
    }
    private void Start() {
        //Oyuncu boss Level�na girdi�inde oyuncunun sa�l�k de�erleri, �ncesinde olu�turulan data'dan �eker
        PlayerAttributeController.instance.currentHealth = data.currentHealth;
        PlayerAttributeController.instance.maxHealth = data.maxHealth;
    }

    private void Update() {
        //UI �zerinde sa�l�k de�erinin do�ru g�z�kmesini sa�lar.
        UIController.instance.healthSlider.maxValue = data.maxHealth;
        UIController.instance.healthSlider.minValue = 0;
        UIController.instance.healthSlider.value = data.currentHealth;

        //Bossun ya�ama durumunu kontrol eder. �l�rse aktifle�ir.
        if (BossController.instance.bossIsDead) {
            endGameWaitTime -= Time.deltaTime;
            

            SoundManager.instance.bossMusic.Stop();
            if (endGameWaitTime <= 0) {
                Destroy(PlayerController.instance.gameObject);
                Destroy(CameraFollow.instance.gameObject);
                SceneManager.LoadScene("Victory");
            }
        }
    }

    //Oyunucunun boss odas�na girmesini kontrol eder.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            doors.SetActive(true);
            playerIsPresent = true;
            if (BossController.instance.gameObject.activeInHierarchy) {
                SoundManager.instance.BossFight();
                UIController.instance.bossHealthBar.gameObject.SetActive(true);
            }
        }
    }
}
