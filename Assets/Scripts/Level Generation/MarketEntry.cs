using UnityEngine;

//Oyuncu market kapýsýýnýn triggerýný aktif ettiðinde E tuþuna basarak
//Market alanýna ýþýnlanýr.

public class MarketEntry : MonoBehaviour
{
    public static MarketEntry instance;     //market alaný
    private bool playerPresent = false;     //oyuncu var mý kontrolü
    private bool pressed = false;           //tuþa basýldý mý kontrolü

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        //Oyuncu trigger alaný üzerinde ise e tuþuna basarak market alanýna geçiþ yapar
        if (playerPresent) {
            if (Input.GetKeyDown(KeyCode.E)) {
                PlayerController.instance.marketHoldPosition = gameObject.transform.position;
                PlayerController.instance.transform.position = MarketExit.instance.transform.position;
                pressed = true;
                SoundManager.instance.bossTp.Play();
            }
        }
        //tuþa bir kez basýldýktan sonra oyuncunun konumu artýk market alaný içerisinde bir konum
        //olacaðýndan dolayý tekrar e ye basýp ayný yere ýþýnlanmamsý için market giriþ alanýndaki
        //deðiþkenler false yapýlýr. 
        if (pressed) {
            playerPresent = false;
            pressed = false;
        }
    }

    //trigger iþlemini kontrol eder
    //oyuncu girdiðinde market alanýna geçiþ için onay verilir.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            if (!playerPresent) {
                playerPresent = true;
            }
        }
    }

    //oyuncu alandan ayrýldýðýnda market alanýna e tuþuna basarak geçiþ
    //yapamamasý için onay kaldýrýlýr.
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player") {
            playerPresent = false;
        }
    }
}
