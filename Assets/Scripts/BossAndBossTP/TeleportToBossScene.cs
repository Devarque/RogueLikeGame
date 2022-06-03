using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportToBossScene : MonoBehaviour {
    public string bossLevel;            //Boss seviyesi Level ismi
    private Text warning;               //Anahtar uyar�s�n� ekrana g�stermek i�in kontrol

    private float fadeLength = 2f;      //Anahtar uyar� textinin kaybolma s�resi
    private float tpWaitLength = 2f;    //I��nlanma bekleme s�resi
    private bool wait = true;           

    private void Start() {
        warning = UIController.instance.bossTpWarning;
    }

    void Update() {
        //Uyar� ekranda g�z�kt�kten sonra belirlenen fadeLength s�resinin biti�i ile ekranan kaybolur.
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
        //Trigger aktiflendi�inde wait kontrol�n�n false olmas� ile belirlenen bekleme s�resinin ard�ndan�,
        //I��nlanma i�lemi ger�ekle�ir.
        if (wait == false) {
            tpWaitLength -= Time.deltaTime;

            if (tpWaitLength <= 0) {
                wait = true;
                SceneManager.LoadScene(bossLevel);
            }
        }
    }

    //Oyuncunun trigger alan�na girmesiyle �zerinde yeterli anahtar varsa ���nlanma ger�ekle�ir
    //yoksa uyar� yaz�s� ekranda g�z�k�r
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
