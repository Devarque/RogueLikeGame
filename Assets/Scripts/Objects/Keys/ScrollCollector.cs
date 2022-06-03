using UnityEngine;

//Oyuncunun boss seviyesine ulaþabilmesi için ihtiyacý olan anahtarlarda kullanýlan Script

public class ScrollCollector : MonoBehaviour {

    //oyuncu ile temas anýnda anahtar sayýsý 1 arttýrlýr. 
    //UI güncellemesi yapýlýr
    //Anahtar toplama sesi aktifleþtirilir.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            LevelManager.instance.currentKeys += 1;
            UIController.instance.keyText.text = LevelManager.instance.currentKeys.ToString();
            SoundManager.instance.keyPick.Play();
            Destroy(gameObject);
        }
    }
}
