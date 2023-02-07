using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int xRooms = Level.NumRoomsX;
    // doors are an int array of size 4 (either 0 or 1's)
    // [0] = North,  [1] = East,  [2] = South,  [3] = West
    public int[] doors = new int[4];
    public int[] impossibleDoors = new int[4];

    // Center point of the room as a Vector3
    public Vector3 Center;

    public int row, col, numDoors;

    public bool isBossRoom;
    public Room(int d, int r, int c)
    {   
    
        row = r;
        col = c;
        numDoors = d;
        isBossRoom = false;
        
        // Checks to see if a room is on a corner or edge and sets impossible doors for that room
    
        if(row == 0){
            impossibleDoors[0] = 1;
        }
        if(col == 0){
            impossibleDoors[3] = 1;
        }
        if(row == xRooms-1){
            impossibleDoors[2] = 1;
        }
        if(col == xRooms-1){
            impossibleDoors[1] = 1;
        }
        
        // inserts (d) doors into the doors array
        int populated = 0;
        for(int i = 0; i < 4; i ++){
            if(impossibleDoors[i]==1){
                continue;
            } else{
                doors[i] = 1;
                populated+=1;
            }
            if(populated == d){
                break;
            }
        }
    }

    // set a door to true
    public void setDoor(int pos){
        doors[pos] = 1;
    }
    public void setCenter(Vector3 c){
        this.Center = c;
    }
    public void toString(){
        Debug.Log("Room: ["+ row + ", "+ col + "]" + "--> [" + doors[0] + ", " + doors[1]+", "+doors[2]+", "+doors[3]+"]" + " Center: "+ Center);
    }
    public void clearDoors(){
        this.doors = new int[4];
    }

}
