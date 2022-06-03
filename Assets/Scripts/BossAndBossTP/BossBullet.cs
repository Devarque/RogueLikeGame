using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyun sonu canavar�n�n sald�r�lar� s�ras�nda kullanaca�� mermiler i�in olu�turuldu.
public class BossBullet : MonoBehaviour
{
    public float speed = 4;         //Merminin hareket h�z�
    private Vector3 direction;      //Merminin gidece�i y�n� hesaplarken kullan�lacak y�n vekt�r�

    void Start() {
        //Tasar�m esnas�nda merminin y�n� (rotasyonu) hangi a��yla duruyorsa, canavar�n olu�turaca�� 
        //merminlerin o y�nde ilerlemelerini sa�lamak i�in y�n ba�lang��ta transform.rigt ile belirlenir.
        direction = transform.right;    
    }

    void Update() {
        transform.position += direction * speed * Time.deltaTime;       //merminin hareket y�n�nde belirlenen h�zda ilerlemesini sa�lar

        if (!BossController.instance.gameObject.activeInHierarchy) {
            Destroy(gameObject);                                        //E�er boss �l�rse t�m mermilerin yok olmas�n� sa�lar.
        }
    }

    //Mermiler oyuncuya �arparsa hasar vererek yok olur.
    //Mermiler herhangi ba�ka bir nesneye �arparsa yok olur.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && PlayerAttributeController.instance.currentHealth > 0) {
            PlayerAttributeController.instance.DamagePlayer(1); //Oyuncuya hasar verir.
            SoundManager.instance.playerDamage.Play();          //Oyuncu hasar alma sesi oynat�l�r
        } else if(other.tag == "BossRoom") {        
            Destroy(gameObject);                                //Mermi yok edilir
        }
        Destroy(gameObject);                                    //Di�er t�m durumlar i�inde mermi yok edilir.
    }
}
