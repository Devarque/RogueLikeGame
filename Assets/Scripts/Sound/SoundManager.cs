using UnityEngine;

//Oyun i�erisinde oynat�lacak m�zik ve ses efektlerini tutan Script
//T�m ses oynatmalar� statik nesne �zerinden play yap�larak ger�ekle�tirilir.

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;

    public AudioSource levelMusic, gameOverMusic, victoryMusic, bossMusic;


    public AudioSource coin, ammoPickup, bossTp, healthRefill, dash, dashPU, enemyFire, enemyDamage, bossFire;
    public AudioSource shield, doorOpen, doorClose, playerDamage, boxBreak, purchase, teleport, bossDead;
    public AudioSource shotgun, rifle, pistol, machineGun, cantBuy, weaponPick, keyPick, chasePlayer;

    private void Awake() {
        instance = this;
    }

    //Oyuncu �ld���nde aktif olur
    public void PlayGameOver() {
        levelMusic.Stop();
        bossMusic.Stop();
        gameOverMusic.Play();
    }

    //Oyun kazan�ld���nda aktif olur
    public void PlayerVictory() {
        bossMusic.Stop();
        levelMusic.Stop();
        victoryMusic.Play();
    }

    //boss sava�ma ala�na girince aktif olur
    public void BossFight() {
        levelMusic.Stop();
        bossMusic.Play();
    }

}
