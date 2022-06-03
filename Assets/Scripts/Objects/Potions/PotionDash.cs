using UnityEngine;
using Random = UnityEngine.Random;

//Oyuncunun dash yetene�ini g��lendiren dash iksiri i�in olu�turulan Script
//Toplanmas� halinde dash bekleme s�resi �ok k�sa s�relere d���r�l�r.
//Bu sayede belirlenen g��lendirme s�resi s�ras�nda oyuncu �ok s�k
//dash yetene�ini kullanabilir


public class PotionDash : MonoBehaviour {
    public float dashBoostCoolTime = 3f;            //Gt��lendirici g��lendirme s�resi
    public float dashBoostCoolDown = 0.1f;          //G��lendirici esnas�nda dash bekleme s�resi
    public float waitTimeForCollect = 0.5f;         //�ksir sahnede olu�tu�unda oyuncunun alabilmesi i�in bekleme s�resi

    public float moveSpeed = 3;                     //Olu�an iksir nesnesinin hareket h�z� (CoinPickup.cs Scripti ile benzer, incelenebilir)
    private Vector3 moveDirection;                  //hareket y�n�
    public float deacceleration = 5f;               //yava�lama ivmesi
        
    private bool canMove = true;

    private void Start() {
        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }

    private void Update() {
        if (waitTimeForCollect > 0) {
            waitTimeForCollect -= Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        if (canMove) {
            transform.position += moveDirection * Time.deltaTime;
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, deacceleration * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        canMove = false;
        if (other.tag == "Player" && waitTimeForCollect <= 0) {
            SoundManager.instance.dashPU.Play();
            PlayerController.instance.boostedDash = true;
            UIController.instance.unlimitedDashSlider.gameObject.SetActive(true);
            PlayerController.instance.boostedDashTime = dashBoostCoolTime;
            PlayerController.instance.boostedDashTimeCool = dashBoostCoolDown;
            UIController.instance.unlimitedDashSlider.maxValue = dashBoostCoolTime;
            Destroy(gameObject);
        }
    }
}
