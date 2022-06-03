using UnityEngine;


//Kutulardan silah düþürülmesi veya marketten silah alýnabilmesi için oluþturulan script.
//Oyuncu bu silahlarý alarak oyun içerisinde kullanabilir.

public class WeaponPickup : MonoBehaviour {
    public Weapon gun;
    public float waitTimeForCollect = 0.5f;

    private void Update() {
        if (waitTimeForCollect > 0) {
            waitTimeForCollect -= Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && waitTimeForCollect <= 0) {
            bool hasThisGun = false;
            foreach (var gunToCheck in PlayerController.instance.avaliableGuns) {
                if (gun.weaponName == gunToCheck.weaponName) {
                    hasThisGun = true;
                }
            }

            if (!hasThisGun) {
                SoundManager.instance.weaponPick.Play();

                Weapon gunClone = Instantiate(gun);
                gunClone.transform.parent = PlayerController.instance.gunArm;
                gunClone.transform.position = PlayerController.instance.gunArm.position;
                gunClone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                gunClone.transform.localScale = Vector3.one;

                foreach (var gun in PlayerController.instance.avaliableGuns) {
                    gun.gameObject.SetActive(false);
                }

                PlayerController.instance.avaliableGuns.Add(gunClone);
                PlayerController.instance.currentGunIndex = PlayerController.instance.avaliableGuns.Count - 1;
                PlayerController.instance.SwitchGun();
            }
            Destroy(gameObject);
        }
    }
}
