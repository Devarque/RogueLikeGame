using UnityEngine;

//SCR�PT KULLANILMIYOR. Test ama�l� olu�turulmu�tur.

public class WandererDetectCollisionBefore : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (EnemyController.instance.shouldWander) {
            EnemyController.instance.wanderCounter = 0f;
            EnemyController.instance.pauseCounter = Random.Range(EnemyController.instance.pauseLength * 0.75f, EnemyController.instance.pauseLength * 1.5f);
        }
    }
}
