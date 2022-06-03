using UnityEngine;

//Oyuncunun boss seviyesine ula�abilmesi i�in ihtiyac� olan anahtarlarda kullan�lan Script

public class ScrollCollector : MonoBehaviour {

    //oyuncu ile temas an�nda anahtar say�s� 1 artt�rl�r. 
    //UI g�ncellemesi yap�l�r
    //Anahtar toplama sesi aktifle�tirilir.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            LevelManager.instance.currentKeys += 1;
            UIController.instance.keyText.text = LevelManager.instance.currentKeys.ToString();
            SoundManager.instance.keyPick.Play();
            Destroy(gameObject);
        }
    }
}
