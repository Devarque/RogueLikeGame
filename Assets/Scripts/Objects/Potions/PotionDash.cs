using UnityEngine;
using Random = UnityEngine.Random;

//Oyuncunun dash yeteneðini güçlendiren dash iksiri için oluþturulan Script
//Toplanmasý halinde dash bekleme süresi çok kýsa sürelere düþürülür.
//Bu sayede belirlenen güçlendirme süresi sýrasýnda oyuncu çok sýk
//dash yeteneðini kullanabilir


public class PotionDash : MonoBehaviour {
    public float dashBoostCoolTime = 3f;            //Gtüçlendirici güçlendirme süresi
    public float dashBoostCoolDown = 0.1f;          //Güçlendirici esnasýnda dash bekleme süresi
    public float waitTimeForCollect = 0.5f;         //Ýksir sahnede oluþtuðunda oyuncunun alabilmesi için bekleme süresi

    public float moveSpeed = 3;                     //Oluþan iksir nesnesinin hareket hýzý (CoinPickup.cs Scripti ile benzer, incelenebilir)
    private Vector3 moveDirection;                  //hareket yönü
    public float deacceleration = 5f;               //yavaþlama ivmesi
        
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
