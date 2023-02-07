using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game 
{
    public LevelData Level;
    public PlayerData Player;

    public EnemyData Enemy;

    public BossData Boss;
    
}


[System.Serializable]
public class LevelData{
    public int Number;
    public int Seed;
    public int RoomX;
    public int Size;

    public int MinMonsters;
    public int MaxMonsters;

    public int NumChestSpawnPoints;
    public int NumEnemySpawnPoints;

}


[System.Serializable]
public class PlayerData{
    public float Health;
    public float MaxHealth;
    public float Damage;
    public float PlayerSpeed;
    public int EnemiesKilled;
    public int[] Items;

}


[System.Serializable]
public class EnemyData{
    public float Health;
    public float Damage;
    public float MinSpeed;
    public float Knockback;

}

[System.Serializable]
public class BossData{
    public float MaxHealth;
    public float Damage;

}