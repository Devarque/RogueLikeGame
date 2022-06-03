using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyun sonu canavarýnýn saldýrýlarý sýrasýnda kullanacaðý mermiler için oluþturuldu.
public class BossBullet : MonoBehaviour
{
    public float speed = 4;         //Merminin hareket hýzý
    private Vector3 direction;      //Merminin gideceði yönü hesaplarken kullanýlacak yön vektörü

    void Start() {
        //Tasarým esnasýnda merminin yönü (rotasyonu) hangi açýyla duruyorsa, canavarýn oluþturacaðý 
        //merminlerin o yönde ilerlemelerini saðlamak için yön baþlangýçta transform.rigt ile belirlenir.
        direction = transform.right;    
    }

    void Update() {
        transform.position += direction * speed * Time.deltaTime;       //merminin hareket yönünde belirlenen hýzda ilerlemesini saðlar

        if (!BossController.instance.gameObject.activeInHierarchy) {
            Destroy(gameObject);                                        //Eðer boss ölürse tüm mermilerin yok olmasýný saðlar.
        }
    }

    //Mermiler oyuncuya çarparsa hasar vererek yok olur.
    //Mermiler herhangi baþka bir nesneye çarparsa yok olur.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && PlayerAttributeController.instance.currentHealth > 0) {
            PlayerAttributeController.instance.DamagePlayer(1); //Oyuncuya hasar verir.
            SoundManager.instance.playerDamage.Play();          //Oyuncu hasar alma sesi oynatýlýr
        } else if(other.tag == "BossRoom") {        
            Destroy(gameObject);                                //Mermi yok edilir
        }
        Destroy(gameObject);                                    //Diðer tüm durumlar içinde mermi yok edilir.
    }
}
