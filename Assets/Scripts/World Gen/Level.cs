using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;


public static class Level
{
    public static bool isLoaded;
    public static int LevelNumber = 1;
    public static int LevelSeed; 
    public static int NumRoomsX = 3;
    public static int RoomSize = 60;
    public static int BorderSize = 10;
    public static Room[,] Rooms;

    public static int[,] pathPoints;
    public static int[,] openSpace;
    public static int MinMonsters = 3; // per pack
    public static int MaxMonsters = 5; // per pack 

    public static int TotalEnemies = 0;

    public static Room BossRoom;
    public static int NumChestSpawnPoints = 3;
    public static int NumEnemySpawnPoints = 5;

    public static int NumRoomsXCap = 6;
    public static int MaxMonsterCap = 10;
    public static int MinMonsterCap = 6;

    public static void Clear(){
        LevelNumber = 1;
        LevelSeed = 0;
        NumRoomsX = 3;
        RoomSize = 60;

        MinMonsters = 3;
        MaxMonsters = 5;

        NumChestSpawnPoints = 3;
        NumEnemySpawnPoints = 5;
    }

    public static void toString(){
        Debug.Log("LEVEL: " + LevelNumber + "Level Seed: " + LevelSeed + "\nNum Rooms X: " + NumRoomsX + "\nRoom Size: " + RoomSize + "\nMin Monsters: " + MinMonsters + "\nMax Monsters: " + MaxMonsters + "\nTotal Enemies" + TotalEnemies);
    }

    // checks the bounds of the points x and z, makes sure their radius does not exceed the map bounds.
    public static int[] boundPoints(int x, int z){
        int lowX = x-10;
        int lowZ = z-10;
        int highX = x+10;
        int highZ = z+10;

        if(lowX < 0){
            lowX = 0;
        }
        if(highX > openSpace.GetLength(0)-1){
            highX = openSpace.GetLength(0)-1;
        }
        if(lowZ < 0){
            lowZ = 0;
        }
        if(highZ > openSpace.GetLength(1)-1){
            highZ = openSpace.GetLength(1)-1;
        }
        int[] points = {lowX, lowZ, highX, highZ};
        return points;
    }


    // takes a point and disables a 20x20 area around it (as a square)
    public static void disableSpawn(Vector3 pt){
        int[] pts = boundPoints((int)pt.x, (int)pt.z);

        int lowX = pts[0];
        int lowZ = pts[1];
        int highX = pts[2];
        int highZ = pts[3];

        for(int x = lowX; x <= highX; x ++){
            for(int z = lowZ; z<= highZ; z ++){
                openSpace[x,z] = 0;
            }
        }

    }

    // handles the change of values between Levels, this is where we increment difficult.
    public static void SetNextLevel(){
        
        Level.LevelNumber += 1;
        
        if(Level.LevelNumber % 2 == 0){
            if(Level.NumRoomsXCap > Level.NumRoomsX){
                Level.NumRoomsX += 1;
                Level.NumChestSpawnPoints += 2;
                Level.NumEnemySpawnPoints += 3;
            }

            if(Level.MaxMonsterCap > Level.MaxMonsters){
                Level.MaxMonsters += 1;
            }
            if(Level.MinMonsterCap > Level.MinMonsters){
                Level.MinMonsters += 1;
            }
        }

        Enemy.damage *= 1.02f;
        Enemy.minSpeed *= 1.02f;
        Enemy.health *= 1.1f;

        Boss.MaxHealth *= 1.3f;
        Boss.damage *= 1.03f;
        toString();
    }

    public static void switchLevel(){
         // MarchingCubesGen nm = GameObject.FindWithTag("Map").GetComponent<MarchingCubesGen>();
        // nm.GenerateNextLevel();

        // PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        // player.teleport(Level.Rooms[0,0].Center);
        SceneManager.LoadScene("completed stage");
    }

    // prints a 2d matrix representation of the map into a file called MAP.txt 
    public static void prettyPrint2DSpace(){
        // int rowLength = openSpace.GetLength(0);
        // int colLength = openSpace.GetLength(1);

        string sb = "";
        for(int i=0; i< openSpace.GetLength(0); i++)
        {
            for(int j=0; j<openSpace.GetLength(1); j++)
            {
                sb += openSpace[j,i];
            }

            sb+="\n";
        }
        string saveFile = Application.persistentDataPath + "/MAP.txt";

        // Does it exist?
        if(File.Exists(saveFile))
        {
            File.WriteAllText(saveFile, sb.ToString());
            // File.WriteAllText(@"saveGame.json", json);
        }else{
            Debug.Log("FILE NOT FOUND");
            File.Create(saveFile); 
            File.WriteAllText(saveFile, sb.ToString());
        }

        // Debug.Log(sb.ToString());
    }
}
