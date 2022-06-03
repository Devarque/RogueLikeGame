using UnityEngine;

//Oyun içerisinde oynatýlacak müzik ve ses efektlerini tutan Script
//Tüm ses oynatmalarý statik nesne üzerinden play yapýlarak gerçekleþtirilir.

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;

    public AudioSource levelMusic, gameOverMusic, victoryMusic, bossMusic;


    public AudioSource coin, ammoPickup, bossTp, healthRefill, dash, dashPU, enemyFire, enemyDamage, bossFire;
    public AudioSource shield, doorOpen, doorClose, playerDamage, boxBreak, purchase, teleport, bossDead;
    public AudioSource shotgun, rifle, pistol, machineGun, cantBuy, weaponPick, keyPick, chasePlayer;

    private void Awake() {
        instance = this;
    }

    //Oyuncu öldüðünde aktif olur
    public void PlayGameOver() {
        levelMusic.Stop();
        bossMusic.Stop();
        gameOverMusic.Play();
    }

    //Oyun kazanýldýðýnda aktif olur
    public void PlayerVictory() {
        bossMusic.Stop();
        levelMusic.Stop();
        victoryMusic.Play();
    }

    //boss savaþma alaýna girince aktif olur
    public void BossFight() {
        levelMusic.Stop();
        bossMusic.Play();
    }

}
