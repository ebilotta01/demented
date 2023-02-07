using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MapGeneration : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;


    // Relates to drawing the path
    public float LineWidth = 4f;
    public bool renderPath = true;

    // num of vertices (per axis) on our grid. (x and z direction)
    private int xSize;
    private int sizeMult;
    private int xRooms;
    private int borderSize;
    // private Room[,] rooms;

    
    // Start is called before the first frame update
    // private GameObject PathPoint;

    void Start()
    {
        // xSize = Level.RoomSize * Level.NumRoomsX;
        // generateRooms();
        // CreateLinks();

       
 
        // for(int row = 0; row < Level.Rooms.Length; row ++){
        //     for(int col = 0; col < Level.Rooms.Length; col ++){
        //        Level.Rooms[row,col].toString(); 
        //     }
        // }


    }
    
    // public void generateRooms(){

    //     Level.Rooms = new Room[Level.NumRoomsX, Level.NumRoomsX];

    //     int rowCounter = Level.NumRoomsX-1;
    //     for(int row = 0; row < Level.NumRoomsX; row ++){
    //         for(int col = 0; col < Level.NumRoomsX; col ++){
    //             int maxDoors = 0;
    //             if ((row == 0 && col == 0) || (row == Level.NumRoomsX-1 && col == Level.NumRoomsX-1) || (row == Level.NumRoomsX-1 && col == 0) ||(row == 0 && col == Level.NumRoomsX-1)){
    //                 maxDoors = 2;
    //             }else if((row != 0 && col!=0) || (row != Level.NumRoomsX-1 && col!=Level.NumRoomsX-1)){
    //                 maxDoors = 4;
    //             }else{
    //                 maxDoors = 3;
    //             }
    //             int numDoors = Random.Range(1, maxDoors);
    //             Room R = new Room(numDoors, row, col);
    //             R.setCenter(new Vector3((col * Level.RoomSize) + Level.RoomSize/2, 1, (rowCounter * Level.RoomSize) + Level.RoomSize/2));
    //             Level.Rooms[row, col] = R;
    //             // Level.Rooms[row, col].toString();
    //         }
    //         rowCounter --;
    //     }
    //     for(int row = 0; row < xRooms; row++){
    //         for(int col = 0; col < xRooms; col++){
    //             int[] currentDoors = Level.Rooms[row, col].doors;
    //             Level.Rooms[row,col].toString();
    //             if(currentDoors[0] == 1){
    //                Level.Rooms[row-1, col].setDoor(2);
    //             }
    //             if(currentDoors[1] == 1){
    //                 Level.Rooms[row, col+1].setDoor(3);
    //             }
    //             if(currentDoors[2] == 1){
    //                 Level.Rooms[row+1, col].setDoor(0);
    //             }
    //             if(currentDoors[3] == 1){
    //                 Level.Rooms[row, col-1].setDoor(1);
    //             }
    //          }
    //     }
    // }
    // public void CreateLinks(){

    //     for(int row = 0; row < xRooms; row++){
    //         for(int col = 0; col < xRooms; col++){
    //             int[] currentDoors = Level.Rooms[row, col].doors;
    //             Level.Rooms[row,col].toString();
    //             if(currentDoors[0] == 1){
    //                Level.Rooms[row-1, col].setDoor(2);
    //             }
    //             if(currentDoors[1] == 1){
    //                 Level.Rooms[row, col+1].setDoor(3);
    //             }
    //             if(currentDoors[2] == 1){
    //                 Level.Rooms[row+1, col].setDoor(0);
    //             }
    //             if(currentDoors[3] == 1){
    //                 Level.Rooms[row, col-1].setDoor(1);
    //             }
    //          }
    //     }
    // }

 


    // void LinkCenters(){
    //     Stack<Room> stack = new Stack<Room>();

    //     bool[,] visited = new bool[Level.NumRoomsX, Level.NumRoomsX];
        
    //     stack.Push(Level.Rooms[0,0]);

    //     while(stack.Count > 0){
    //         Room CurrentRoom = stack.Pop();

    //         if (visited[CurrentRoom.row, CurrentRoom.col] == true){
    //             continue;
    //         } else{
    //             visited[CurrentRoom.row, CurrentRoom.col] = true;
    //         }

    //         // CurrentRoom.toString();

    //         if(CurrentRoom.doors[0] == 1){
    //             Room nextRoom = Level.Rooms[CurrentRoom.row-1, CurrentRoom.col];
    //             if(renderPath){
    //                 GameObject myLine = new GameObject();
    //                 myLine.transform.position = CurrentRoom.Center;
    //                 myLine.AddComponent<LineRenderer>();
    //                 LineRenderer lr = myLine.GetComponent<LineRenderer>();
    //                 lr.startColor = Color.red;
    //                 lr.endColor = Color.red;
    //                 lr.SetWidth(LineWidth, LineWidth);
    //                 lr.SetPosition(0, CurrentRoom.Center);
    //                 lr.SetPosition(1, nextRoom.Center);
    //             }

    //             // GameObject.Destroy(myLine, duration);

    //             stack.Push(nextRoom);
                
    //         }
    //         if(CurrentRoom.doors[1] == 1){
    //             Room nextRoom = Level.Rooms[CurrentRoom.row, CurrentRoom.col+1];

    //             if(renderPath){
    //                 GameObject myLine = new GameObject();
    //                 myLine.transform.position = CurrentRoom.Center;
    //                 myLine.AddComponent<LineRenderer>();
    //                 LineRenderer lr = myLine.GetComponent<LineRenderer>();
    //                 lr.startColor = Color.red;
    //                 lr.endColor = Color.red;
    //                 lr.SetWidth(LineWidth, LineWidth);
    //                 lr.SetPosition(0, CurrentRoom.Center);
    //                 lr.SetPosition(1, nextRoom.Center);
    //             }
                
    //             // GameObject.Destroy(myLine, duration);
                
    //             stack.Push(nextRoom);
    //         }
    //         if(CurrentRoom.doors[2] == 1){

    //             Room nextRoom = Level.Rooms[CurrentRoom.row+1, CurrentRoom.col];

    //             if(renderPath){
    //                 GameObject myLine = new GameObject();
    //                 myLine.transform.position = CurrentRoom.Center;
    //                 myLine.AddComponent<LineRenderer>();
    //                 LineRenderer lr = myLine.GetComponent<LineRenderer>();
    //                 lr.startColor = Color.red;
    //                 lr.endColor = Color.red;
    //                 lr.SetWidth(LineWidth, LineWidth);
    //                 lr.SetPosition(0, CurrentRoom.Center);
    //                 lr.SetPosition(1, nextRoom.Center);
    //             }

    //             // GameObject.Destroy(myLine, duration);

    //             stack.Push(nextRoom);
    //         }
    //         if(CurrentRoom.doors[3] == 1){

    //             Room nextRoom = Level.Rooms[CurrentRoom.row, CurrentRoom.col-1];

    //             if(renderPath){
    //                 GameObject myLine = new GameObject();
    //                 myLine.transform.position = CurrentRoom.Center;
    //                 myLine.AddComponent<LineRenderer>();
    //                 LineRenderer lr = myLine.GetComponent<LineRenderer>();
    //                 lr.startColor = Color.red;
    //                 lr.endColor = Color.red;
    //                 lr.SetWidth(LineWidth, LineWidth);
    //                 lr.SetPosition(0, CurrentRoom.Center);
    //                 lr.SetPosition(1, nextRoom.Center);
    //             }               
    //             // GameObject.Destroy(myLine, duration);
                
    //             stack.Push(nextRoom);
    //         }
    //     }


    // }
}