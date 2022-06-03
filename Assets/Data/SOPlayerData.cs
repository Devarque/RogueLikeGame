using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyun sýrasýnda kullacýnýn istatistiklerini tutan Scriptable Object nesnesi.
//Oyuncu öldüðünde veya oyunu bitirdiðinde ekranda deðerlerin gösterilmesi için oluþturuldu.

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data")]
public class SOPlayerData : ScriptableObject {
    public int currentHealth = 6;   //Oyuncunun anlýk saðlýðý   
    public int maxHealth = 6;       //Oyuncunun maksiumum saðlýðý
    public int currentCoins = 0;    //Oyuncunun anlýk altýn sayýsý

    public TimeSpan playTime;       //Oyun oynama süresi. Oynamaya baþlandýðý andan itibaren artmaya baþlar

    public string totalTime;        
    public int totalEnemyKilled = 0;    //Öldürülen toplam düþman sayýsý
    public int totalDamageTaken = 0;    //Alýnan toplam hasar
    public int totalDamageDealt = 0;    //Verilen toplam hasar


    
}


