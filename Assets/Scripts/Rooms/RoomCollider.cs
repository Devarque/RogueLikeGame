using UnityEngine;

//Oyuncu bir odaya girdiðinde odanýn kapýlarýný kapatan Script.
//ayný zamanda 1. odayý ziyaret edilmiþ olarak belirler ve bu sayede ilk oda haritada gözükür.

public class RoomCollider : MonoBehaviour {
    [SerializeField] private Room room;

    private void Start() {
        if (room.name == "Room_1") {
            room.isPlayerVisited = true;
            room.roomMask.gameObject.SetActive(false);
            PlayerController.instance.room = DungeonGenerator.instance.roomsInGameList[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            PlayerController.instance.room = room;
            room.isPlayerVisited = true;
            room.roomMask.gameObject.SetActive(false);
            if (!room.noEnemy) {
                foreach (var door in room.roomDoors) {
                    if (!door.activeInHierarchy) {
                        SoundManager.instance.doorClose.Play();
                        door.SetActive(true);
                    }
                }
            }
        }
    }
}
