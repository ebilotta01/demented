using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class LoadGame
{
    // after every level Save Game
    public static void saveGame() {

        Game _data = new Game();
        LevelData Level_Data = new LevelData();
        PlayerData Player_Data = new PlayerData();
        EnemyData Enemy_Data = new EnemyData();
        BossData Boss_Data = new BossData();

        Level_Data.Number = Level.LevelNumber;
        Level_Data.Seed = Level.LevelSeed;
        Level_Data.RoomX = Level.NumRoomsX;
        Level_Data.Size = Level.RoomSize;
        Level_Data.MinMonsters = Level.MinMonsters;
        Level_Data.MaxMonsters = Level.MaxMonsters;
        Level_Data.NumChestSpawnPoints = Level.NumChestSpawnPoints;
        Level_Data.NumEnemySpawnPoints = Level.NumEnemySpawnPoints;


        Player_Data.Health = Player.health;
        Player_Data.Damage = Player.damage;
        Player_Data.Items = Player.itemList;
        Player_Data.MaxHealth = Player.maxHealth;
        Player_Data.PlayerSpeed = Player.playerSpeed;
        Player_Data.EnemiesKilled = Player.enemiesKilled;

        Enemy_Data.Health = Enemy.health;
        Enemy_Data.Damage = Enemy.damage;
        Enemy_Data.MinSpeed = Enemy.minSpeed;
        Enemy_Data.Knockback = Enemy.knockback;

        Boss_Data.MaxHealth = Boss.MaxHealth;
        Boss_Data.Damage = Boss.damage;


        _data.Level = Level_Data;
        _data.Player = Player_Data;
        _data.Enemy = Enemy_Data;
        _data.Boss = Boss_Data;


        string saveFile = Application.persistentDataPath + "/gamedata.json";
         // string json = JsonSerializer.Serialize(_data);
        string json = JsonUtility.ToJson(_data);    

        // Debug.Log(saveFile);
        // Debug.Log(json);

        // Does it exist?
        if(File.Exists(saveFile))
        {
            File.WriteAllText(saveFile, json);
            // File.WriteAllText(@"saveGame.json", json);
        }else{
            Debug.Log("FILE NOT FOUND");
            File.Create(saveFile); 
            File.WriteAllText(saveFile, json);
        }

    }

}
