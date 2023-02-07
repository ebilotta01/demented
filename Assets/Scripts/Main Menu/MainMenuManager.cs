using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuManager: MonoBehaviour
{
    public UnityEngine.UI.Text Lab;

    public void playGame(){
        Level.isLoaded = false;
        System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        Level.LevelSeed = secondsSinceEpoch;
        SceneManager.LoadScene("SampleScene");
    }
    
    public void loadGame(){
        string saveFile = Application.persistentDataPath + "/gamedata.json";
         // string json = JsonSerializer.Serialize(_data);
        
        Game save = JsonUtility.FromJson<Game>(File.ReadAllText(saveFile));

        if(save.Level.Number > 1){
            Level.LevelNumber = save.Level.Number;
            Level.LevelSeed = save.Level.Seed;
            Level.NumRoomsX = save.Level.RoomX;
            Level.RoomSize = save.Level.Size;
            Level.MinMonsters = save.Level.MinMonsters;
            Level.MaxMonsters = save.Level.MaxMonsters;
            Level.NumChestSpawnPoints = save.Level.NumChestSpawnPoints;
            Level.NumEnemySpawnPoints = save.Level.NumEnemySpawnPoints;

            Player.health = save.Player.Health;
            // Player.damage = save.Player.Damage;
            // Player.maxHealth = save.Player.MaxHealth;
            // Player.playerSpeed = save.Player.PlayerSpeed;
            Player.enemiesKilled = save.Player.EnemiesKilled;

            for(int i = 0; i < save.Player.Items.Length; i ++){
                Player.UpdatePlayer(i, save.Player.Items[i]);
            }
            // Debug.Log(save.Player.Items.GetType());
            // Player.itemList = save.Player.Items;
            

            Enemy.health = save.Enemy.Health;
            Enemy.damage = save.Enemy.Damage;
            Enemy.minSpeed = save.Enemy.MinSpeed;
            Enemy.knockback = save.Enemy.Knockback;

            Boss.MaxHealth = save.Boss.MaxHealth;
            Boss.damage = save.Boss.Damage;

            Level.isLoaded = true;
            SceneManager.LoadScene("SampleScene");
            // Debug.Log(Player.itemList);
            Level.toString();
            Player.toString();
        } else {
            Lab.text = "NO SAVE FILE";
        }

    }
}

