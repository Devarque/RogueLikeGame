using UnityEngine;


//Market giriþ alanýnda olduðu gibi ayný mantýk ile bu sefer oyuncunun zindana tekrar dönebilmesi
//için gerekli kontroller ve iþlemler yapýlýr

public class MarketExit : MonoBehaviour {
    public static MarketExit instance;
    private bool playerPresent = false;
    private bool pressed = false;
    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (playerPresent) {
            if (Input.GetKeyDown(KeyCode.E)) {
                SoundManager.instance.bossTp.Play();
                PlayerController.instance.transform.position = PlayerController.instance.marketHoldPosition;
                pressed = true;
            }
        }
        if (pressed) {
            playerPresent = false;
            pressed = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {
            if (!playerPresent) {
                playerPresent = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            playerPresent = false;
        }
    }
}
