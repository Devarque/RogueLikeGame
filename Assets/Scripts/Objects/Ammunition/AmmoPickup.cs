using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Karakterin o an için elinde olan silahýn cephanesini doldurmak için kullanýlacak olan
//Cephane paketleri için oluþturulan Script

public class AmmoPickup : MonoBehaviour
{
    public bool isBig;              //true ise oyuncunun elindeki silahýn tüm cephanesi yenilenir.
    public int ammoGain = 1;        //isBig false ise girilen deðer kadar cephane yenilenir.
    public float waitTimeForCollect = 0.5f; //Eþya oluþduktan sonra oyuncunun bu eþyayý alabilmesi için geçmesi gereken süre
    private WeaponAmmo currentGun;  //Oyuncunun o an için elinde olan silahý tutan deðiþken

    

    private void Update() {
        //nesne oluþduktan sonra bekleme süresi sayacýný aktif eder.
        currentGun = PlayerController.instance.avaliableGuns[PlayerController.instance.currentGunIndex].GetComponent<WeaponAmmo>();

        if (waitTimeForCollect > 0) {
            waitTimeForCollect -= Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    }


    //trigger aktif olduðunda seçeneðe göre ya tüm cephane ya da belirlenen sayýda cephane yenilenir.
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
