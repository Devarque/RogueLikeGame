using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportToBossScene : MonoBehaviour {
    public string bossLevel;            //Boss seviyesi Level ismi
    private Text warning;               //Anahtar uyarýsýný ekrana göstermek için kontrol

    private float fadeLength = 2f;      //Anahtar uyarý textinin kaybolma süresi
    private float tpWaitLength = 2f;    //Iþýnlanma bekleme süresi
    private bool wait = true;           

    private void Start() {
        warning = UIController.instance.bossTpWarning;
    }

    void Update() {
        //Uyarý ekranda gözüktükten sonra belirlenen fadeLength süresinin bitiþi ile ekranan kaybolur.
        if (warning.gameObject.activeInHierarchy) {
            if (PlayerAttributeController.instance.currentHealth == 0) {
                warning.gameObject.SetActive(false);
            }
            fadeLength -= Time.deltaTime;
            if (fadeLength <= 0) {
                warning.gameObject.SetActive(false);
                fadeLength = 2f;
            }
        }
        //Trigger aktiflendiðinde wait kontrolünün false olmasý ile belirlenen bekleme süresinin ardýndanü,
        //Iþýnlanma iþlemi gerçekleþir.
        if (wait == false) {
            tpWaitLength -= Time.deltaTime;

            if (tpWaitLength <= 0) {
                wait = true;
                SceneManager.LoadScene(bossLevel);
            }
        }
    }

    //Oyuncunun trigger alanýna girmesiyle üzerinde yeterli anahtar varsa ýþýnlanma gerçekleþir
    //yoksa uyarý yazýsý ekranda gözükür
    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (LevelManager.instance.currentKeys >= 3) {
                    SoundManager.instance.teleport.Play();
                    wait = false;
                } else {
                    warning.gameObject.SetActive(true);
                }
            }
        }
    }
}
