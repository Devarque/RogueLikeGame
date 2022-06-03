using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Karakterin o an i�in elinde olan silah�n cephanesini doldurmak i�in kullan�lacak olan
//Cephane paketleri i�in olu�turulan Script

public class AmmoPickup : MonoBehaviour
{
    public bool isBig;              //true ise oyuncunun elindeki silah�n t�m cephanesi yenilenir.
    public int ammoGain = 1;        //isBig false ise girilen de�er kadar cephane yenilenir.
    public float waitTimeForCollect = 0.5f; //E�ya olu�duktan sonra oyuncunun bu e�yay� alabilmesi i�in ge�mesi gereken s�re
    private WeaponAmmo currentGun;  //Oyuncunun o an i�in elinde olan silah� tutan de�i�ken

    

    private void Update() {
        //nesne olu�duktan sonra bekleme s�resi sayac�n� aktif eder.
        currentGun = PlayerController.instance.avaliableGuns[PlayerController.instance.currentGunIndex].GetComponent<WeaponAmmo>();

        if (waitTimeForCollect > 0) {
            waitTimeForCollect -= Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    }


    //trigger aktif oldu�unda se�ene�e g�re ya t�m cephane ya da belirlenen say�da cephane yenilenir.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && waitTimeForCollect <= 0 && PlayerController.instance.currentGunIndex != 0) {
            if (isBig) {
                currentGun.RefillAmmo();
            } else {
                currentGun.RefillAmmo(ammoGain);
            }
            SoundManager.instance.ammoPickup.Play();
            Destroy(gameObject);
        }
    }
}
