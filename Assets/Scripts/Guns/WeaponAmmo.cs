using UnityEngine;

//Silahlarýn mermileri için oluþturulan Script.
public class WeaponAmmo : MonoBehaviour {
    [SerializeField] private GameObject bulletToFire;       //ateþ edilecek mermi
    [SerializeField] private Transform[] firePoint;         //mermi oluþturma noktalarý. 
    private Weapon weapon;                                  //atýþ iþlemi yapýlan silah

    private void Start() {
        //Nesne oluþtuðu anda cephanesi yenilir ki tam sayýda mermisi olabilsin.
        weapon = GetComponent<Weapon>();
        RefillAmmo();
    }

    //Atýþ iþleminin aktif olmasýyla birlikte mermi azaltýlýr. Azaltma sýrasýnda mermi sahnede oluþturulur.
    //Mermi atýþ sesi silaha göre oynatýlýr. Pompalýnýn birden fazla ateþ ettiði mermi olduðundan dolayý
    //her bir atýþ noktasýnda bu iþlemler gerçekleþtirilir. 1 veya 2 adet mermi kalmasý durumunda da iþlemler
    //gerçekleþtirilir. Kontrolü saðlanmýþtýr
    public void ConsumeAmmo() {
        if (weapon.UseMagazine) {
            if (weapon.weaponName != "Shotgun") {
                if (weapon.weaponName == "Rifle") {
                    SoundManager.instance.rifle.Play();
                } else {
                    SoundManager.instance.machineGun.Play();
                }
                Instantiate(bulletToFire, firePoint[0].position, firePoint[0].rotation);
                weapon.CurrentAmmo -= 1;
            } else {
                if (weapon.CurrentAmmo >= firePoint.Length) {
                    foreach (var fp in firePoint) {
                        Instantiate(bulletToFire, fp.position, fp.rotation);
                        SoundManager.instance.shotgun.Play();
                        weapon.CurrentAmmo -= 1;
                    }
                } else {
                    for (int i = 0; i < weapon.CurrentAmmo; i++) {
                        var fp = firePoint[i];
                        Instantiate(bulletToFire, fp.position, fp.rotation);
                        SoundManager.instance.shotgun.Play();
                    }
                    weapon.CurrentAmmo -= weapon.CurrentAmmo;
                }
            }
        }
        if (weapon.gameObject.name == "Pistol") {
            foreach (var fp in firePoint) {
                SoundManager.instance.pistol.Play();
                Instantiate(bulletToFire, fp.position, fp.rotation);
            }
        }
    }

    public bool CanUseWeapon() {
        if (weapon.CurrentAmmo > 0) {
            return true;
        }
        return false;
    }

    //Cephane toplanmasý halinde mermi sayýsýnýn tamamen yenilenmesini saðlayan fonksiyon
    public void RefillAmmo() {
        if (weapon.UseMagazine) {
            if (weapon.weaponName == "Machine Gun") {
                weapon.CurrentAmmo = weapon.MagazineSize - Random.Range(0, 50);
            }
            weapon.CurrentAmmo = weapon.MagazineSize;
        }
    }

    //Cephane toplanmasý halinde mermi sayýsýnýn girilen deðer kadar artmasýný saðlayan fonksiyon 
    public void RefillAmmo(int ammoGain) {
        if (weapon.UseMagazine) {
            weapon.CurrentAmmo += ammoGain;
            if (weapon.CurrentAmmo > weapon.MagazineSize) {
                weapon.CurrentAmmo = weapon.MagazineSize;
            }
        }
    }
}
