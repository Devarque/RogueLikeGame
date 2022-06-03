using UnityEngine;

//Bu Scriptin atandýðý nesneler eðer collision komponentine sahip ve trigger true ise
//oyuncu ile temas anýnda oyuncuya verilen deðer kadar hasar verirler ve oyuncunu caný azalýr.


public class DamagePlayer : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
    }
}
