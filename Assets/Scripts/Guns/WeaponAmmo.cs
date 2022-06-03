using UnityEngine;

//Silahlar�n mermileri i�in olu�turulan Script.
public class WeaponAmmo : MonoBehaviour {
    [SerializeField] private GameObject bulletToFire;       //ate� edilecek mermi
    [SerializeField] private Transform[] firePoint;         //mermi olu�turma noktalar�. 
    private Weapon weapon;                                  //at�� i�lemi yap�lan silah

    private void Start() {
        //Nesne olu�tu�u anda cephanesi yenilir ki tam say�da mermisi olabilsin.
        weapon = GetComponent<Weapon>();
        RefillAmmo();
    }

    //At�� i�leminin aktif olmas�yla birlikte mermi azalt�l�r. Azaltma s�ras�nda mermi sahnede olu�turulur.
    //Mermi at�� sesi silaha g�re oynat�l�r. Pompal�n�n birden fazla ate� etti�i mermi oldu�undan dolay�
    //her bir at�� noktas�nda bu i�lemler ger�ekle�tirilir. 1 veya 2 adet mermi kalmas� durumunda da i�lemler
    //ger�ekle�tirilir. Kontrol� sa�lanm��t�r
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

    //Cephane toplanmas� halinde mermi say�s�n�n tamamen yenilenmesini sa�layan fonksiyon
    public void RefillAmmo() {
        if (weapon.UseMagazine) {
            if (weapon.weaponName == "Machine Gun") {
                weapon.CurrentAmmo = weapon.MagazineSize - Random.Range(0, 50);
            }
            weapon.CurrentAmmo = weapon.MagazineSize;
        }
    }

    //Cephane toplanmas� halinde mermi say�s�n�n girilen de�er kadar artmas�n� sa�layan fonksiyon 
    public void RefillAmmo(int ammoGain) {
        if (weapon.UseMagazine) {
            weapon.CurrentAmmo += ammoGain;
            if (weapon.CurrentAmmo > weapon.MagazineSize) {
                weapon.CurrentAmmo = weapon.MagazineSize;
            }
        }
    }
}
