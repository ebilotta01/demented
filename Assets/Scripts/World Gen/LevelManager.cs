using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int NumberOfRoomsInX = 6;
    public int SizeOfRoom = 30;
    public int SizeOfBorder = 10;
    public int MinimumMonsters = 3;
    public int MaximumMonsters = 5;
    // private Room[,] rms;
    
    void Awake()
    {
        Debug.Log("this script does nothing");
        // if(!Level.isLoaded){
        //     System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
        //     int secondsSinceEpoch = (int)t.TotalSeconds;
            // rms = new Room[NumberOfRoomsInX, NumberOfRoomsInX];
            // Level.Rooms = rms; 
            // Level.NumRoomsX = NumberOfRoomsInX;
            // Level.RoomSize = SizeOfRoom;
            // Level.BorderSize = SizeOfBorder;
            // Level.MinMonsters = MinimumMonsters;
            // Level.MaxMonsters = MaximumMonsters;
            // Level.LevelNumber = 1;
            // Level.LevelSeed = secondsSinceEpoch;
            // Level.toString();

    }
}

