using System.Collections;
using System.Collections.Generic;

using UnityEngine.AI;
using UnityEngine;

public class MarchingCubesGen : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 gridSize;
    public Material material;
    int pathWidth;
    public int noiseLimit = 20;
    public int smooth = 5;
    private NavMeshSurface surface;

    Mesh mesh = null;

    public GameObject portal;
    Vector3 portalSpawnPoint;
    Vector3 playerSpawnPt;
    
    System.Random prng;
    public GameObject bossPrefab;

    

    void Start()
    {
        pathWidth = Mathf.RoundToInt(Level.RoomSize/2 * 0.75f);
        material = GetComponent<Renderer>().material;
        prng = new System.Random(Level.LevelSeed.GetHashCode());
        
        GenerateNextLevel();

        surface = gameObject.GetComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        //surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.BuildNavMesh();
        
    } 

    public void GenerateNextLevel(){
        gridSize = new Vector3(Level.NumRoomsX * Level.RoomSize, 12, Level.NumRoomsX * Level.RoomSize);
        Level.pathPoints = new int[(int)(gridSize.x), (int)(gridSize.z)];

        MarchingCube.Clear();

        generateRooms();
        MakeGrid();

        
        // Turns off the points that lay on path
        TurnOffPathPoints();     
        Level.openSpace = (int[,])Level.pathPoints.Clone();   
        turnOffBossRoomPoints();
        

        // little bit of noise to make things more oraganic
        Noise3d();

        

        TurnOnWalls();

        // smooths the map 
        for(int i =0; i <smooth; i ++){
            SmoothMap();
        }

        // perform the march over the grid.
        March();
        spawnPortal(); 

        GameObject player = GameObject.FindWithTag("Player");
        Vector3 bossSpawnPoint = Level.BossRoom.Center;
        bossSpawnPoint.y += 16.0f;
        Debug.Log(Level.BossRoom.Center);
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint, Quaternion.identity);
        //GameObject boss = Instantiate(bossPrefab, Level.BossRoom.Center, Quaternion.identity);
        boss.transform.rotation = Quaternion.identity;
        //boss.transform.parent = transform.Find("Enemies").gameObject.transform;
        // player.transform.position = playerSpawnPt;
    }

    void spawnPortal(){
        Vector3 pt = newPoint();
        
        Instantiate(portal, pt, Quaternion.identity);
        Level.disableSpawn(pt);
    }

    Vector3 newPoint(){
        int x = prng.Next(Level.RoomSize, Level.NumRoomsX * Level.RoomSize-1);
        int z = prng.Next(Level.RoomSize, Level.NumRoomsX * Level.RoomSize-1);
        
        bool valid = false;

        while(Level.openSpace[(int)x, (int)z] != 1 || !valid){
            x = prng.Next(Level.RoomSize, Level.NumRoomsX * Level.RoomSize-1);
            z = prng.Next(Level.RoomSize, Level.NumRoomsX * Level.RoomSize-1);
            valid = validatePoint(x, z);
        }
        Vector3 pt = new Vector3(x, 1, z);
        pt = elevatePt(pt);
        return pt;
    }

    Vector3 elevatePt(Vector3 pt){
        while(MarchingCube.grd[(int)pt.x, (int)pt.y, (int)pt.z].On){
            pt.y = pt.y + 1.0f;
            if(pt.y >= MarchingCube.grd.GetLength(1)){
                pt = newPoint();
            }
        }
            
        pt.y = pt.y - .5f;
        return pt;
    }
    bool validatePoint(int _x, int _z){

        for(int x = (int)_x - 10; x <= (int)_x+10; x ++){
            for(int z = (int)_z - 10; x <= (int)_z+10; z ++){
               if(Level.openSpace[x, z] != 1){
                   return false;
               }
            
            }
        }
        return true;
    }

    void setBossRoom(){
        int RandomX = prng.Next(1, Level.NumRoomsX);
        int RandomY = prng.Next(1, Level.NumRoomsX);
        // Debug.Log("BOSS ROOM:" + RandomX + " " + RandomY);
        Level.Rooms[RandomX, RandomY].clearDoors();
        Level.Rooms[RandomX, RandomY].isBossRoom = true;
        // Level.Rooms[RandomX, RandomY].toString();
        Level.BossRoom = Level.Rooms[RandomX, RandomY];

        int newRandomX = prng.Next(0, Level.NumRoomsX);
        int newRandomY = prng.Next(0, Level.NumRoomsX);

        while(newRandomX == RandomX){
            newRandomX = prng.Next(0, Level.NumRoomsX);
        }
        while(newRandomY == RandomY){
            newRandomY = prng.Next(0, Level.NumRoomsX);
        }

        this.portalSpawnPoint = Level.Rooms[0,0].Center;
        this.portalSpawnPoint.x -= 7.0f;
        this.portalSpawnPoint.z -= 7.0f;
    }

    void setPlayerSpawn(){
        for(int z = 0; z < gridSize.z; z++){
            for(int x = 0; x < gridSize.x; x++){
                if(Level.pathPoints[x,z] == 1){
                    this.playerSpawnPt = new Vector3(x, 1.0f, z); 
                    break;
                }
            }
        }
    }
    void turnOffBossRoomPoints(){
        for(int x = (int)(Level.BossRoom.Center.x) - (int)Level.RoomSize/2; x < (int)(Level.BossRoom.Center.x) + (int)Level.RoomSize/2; x++){
            for(int z = (int)(Level.BossRoom.Center.z) - (int)Level.RoomSize/2; z < (int)(Level.BossRoom.Center.z) + (int)Level.RoomSize/2; z ++){
                Level.pathPoints[x,z] = 1;
                // 
            }
        }
    }

    // Makes a grid of size "gridSize" so we can march our cubes through
    void MakeGrid(){
        MarchingCube.grd = new GridPoint[(int)gridSize.x, (int)gridSize.y,(int)gridSize.z];

        for(int z = 0; z < gridSize.z; z++){
            for(int y = 0; y < gridSize.y; y++){
                for(int x = 0; x < gridSize.x; x++){
                    MarchingCube.grd[x,y,z] = new GridPoint();
                    MarchingCube.grd[x,y,z].Position = new Vector3(x,y,z);
                    MarchingCube.grd[x,y,z].On = false;

                }
            }
 
        }
    }


    // Randomly turns on points in our MarchingCubes Grid using a noise function so its smooth-ish
    void Noise3d(){

        for(int z = 0; z < gridSize.z; z++){
            for(int y = 0; y < gridSize.y; y++){
                for(int x = 0; x < gridSize.x; x++){
                    if(x == 0 || x == gridSize.x-1 || y == 0 || y == gridSize.y-1 || z == 0 || z == gridSize.z -1){
                        MarchingCube.grd[x,y,z].On = true; 
                    // } else if (x == 1 || x == gridSize.x-2 || y == 1 || y == gridSize.y-2 || z == 1 || z == gridSize.z -2){
                    } else {
                        // Debug.Log("next "+ ps.Next(1,100));
                        MarchingCube.grd[x,y,z].On = (prng.Next(0,100) < noiseLimit);

                    }
                }

            }
 
        } 
    }

    // Idea : random perlin noise map, make sure the door points and path points are turned off and then march.

    // Gets the amount of points that are turned around a given point.
    int getSurround(int gridX, int gridY, int gridZ){

        int surr = 0;

        // Debug.Log(MarchingCube.grd[gridX,gridY,gridZ].On);
        for(int z = gridZ - 1; z <= gridZ+1; z++){
            for(int y = gridY-1; y <= gridY+1; y++){
                for(int x = gridX-1; x <= gridX+1; x++){
                    if (x >= 0 && x < gridSize.x && z >= 0 && z < gridSize.z && y >= 0 && y < gridSize.y){
                        if(x != gridX || y != gridY || z != gridZ){

                            // Debug.Log("X-Y-Z " + x + " " + y + " " + z);
                            // Debug.Log("grid[X,Y,Z]" + MarchingCube.grd[x,y,z].On);
                            if (MarchingCube.grd[x,y,z].On){
                                surr++;
                            }
                        }
                    } else{
                        surr ++;
                    }
             
                }
            }
        }
        // Debug.Log(surr);
        return surr;
    }


    // attempts to smooth the map out by gathering information about surrounding points and changing the current point so it falls in line with its surroundings
    void SmoothMap(){

       for(int z = 0; z < gridSize.z; z++){
            for(int y = 0; y < gridSize.y; y++){
                for(int x = 0; x < gridSize.x; x++){
                    int nei = getSurround(x,y,z);
                    // Debug.Log("NEI "+nei);

                    if(nei > 13){
                        
                        MarchingCube.grd[x,y,z].On = true;
                        // Debug.Log("IN");

                    } else if(nei < 13){
                        // Debug.Log("OUT");
                        MarchingCube.grd[x,y,z].On = false;  
                    }
                }

            }
       }
    }
    

    // marches the cubes and creates the mesh, also ties a mesh collider to the map.
    void March(){

        GameObject go = this.gameObject;
        mesh = MarchingCube.GetMesh(ref go, ref material);
        MarchingCube.Clear();
        MarchingCube.MarchCubes();

        MarchingCube.SetMesh(ref mesh);

        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh; 
    
    }
    public void generateRooms(){
        
        Level.Rooms = new Room[Level.NumRoomsX, Level.NumRoomsX];
        int rowCounter = Level.NumRoomsX-1;

        if(Level.NumRoomsX == 3){
            for(int row = 0; row < Level.NumRoomsX; row ++){
                for(int col = 0; col < Level.NumRoomsX; col ++){ 
                    if ((row == 0 && col == 0) || (row == Level.NumRoomsX-1 && col == Level.NumRoomsX-1) || (row == Level.NumRoomsX-1 && col == 0) ||(row == 0 && col == Level.NumRoomsX-1)){
                        Room R = new Room(2, row, col);
                        R.setCenter(new Vector3((col * Level.RoomSize) + Level.RoomSize/2, 1, (rowCounter * Level.RoomSize) + Level.RoomSize/2));
                        Level.Rooms[row, col] = R;
                    }else if((row != 0 && col!=0) || (row != Level.NumRoomsX-1 && col!=Level.NumRoomsX-1)){
                        Room R = new Room(4, row, col);
                        R.setCenter(new Vector3((col * Level.RoomSize) + Level.RoomSize/2, 1, (rowCounter * Level.RoomSize) + Level.RoomSize/2));
                        Level.Rooms[row, col] = R;
                    }else{
                        Room R = new Room(3, row, col);
                        R.setCenter(new Vector3((col * Level.RoomSize) + Level.RoomSize/2, 1, (rowCounter * Level.RoomSize) + Level.RoomSize/2));
                        Level.Rooms[row, col] = R;
                    }
                }
            rowCounter --;
            }
        } else{
            for(int row = 0; row < Level.NumRoomsX; row ++){
                for(int col = 0; col < Level.NumRoomsX; col ++){
                    int maxDoors = 0;
                    if ((row == 0 && col == 0) || (row == Level.NumRoomsX-1 && col == Level.NumRoomsX-1) || (row == Level.NumRoomsX-1 && col == 0) ||(row == 0 && col == Level.NumRoomsX-1)){
                        maxDoors = 2;
                    }else if((row != 0 && col!=0) || (row != Level.NumRoomsX-1 && col!=Level.NumRoomsX-1)){
                        maxDoors = 4;
                    }else{
                        maxDoors = 3;
                    }
                    int numDoors = prng.Next(1, maxDoors);
                    Room R = new Room(numDoors, row, col);
                    R.setCenter(new Vector3((col * Level.RoomSize) + Level.RoomSize/2, 1, (rowCounter * Level.RoomSize) + Level.RoomSize/2));
                    Level.Rooms[row, col] = R;
                    // Level.Rooms[row, col].toString();
                }
                rowCounter --;
            }

        }
       

        setBossRoom();
        // setPlayerSpawn();

        // // Used to link adjacent rooms
        for(int row = 0; row < Level.NumRoomsX; row++){
            for(int col = 0; col < Level.NumRoomsX; col++){
                int[] currentDoors = Level.Rooms[row, col].doors;

                if(currentDoors[0] == 1 && !(Level.Rooms[row-1, col].isBossRoom)){
                    Level.Rooms[row-1, col].setDoor(2);
                }
                if(currentDoors[0] == 1 && Level.Rooms[row-1, col].isBossRoom){
                    Level.Rooms[row,col].doors[0] = 0;
                }
                if(currentDoors[1] == 1 && !(Level.Rooms[row, col+1].isBossRoom)){
                    Level.Rooms[row, col+1].setDoor(3);
                }
                if(currentDoors[1] == 1 && Level.Rooms[row, col+1].isBossRoom){
                    Level.Rooms[row,col].doors[1] = 0;
                }
                if(currentDoors[2] == 1 && !(Level.Rooms[row+1, col].isBossRoom)){
                    Level.Rooms[row+1, col].setDoor(0);
                }
                if(currentDoors[2] == 1 && Level.Rooms[row+1, col].isBossRoom){
                    Level.Rooms[row,col].doors[2] = 0;
                }
                if(currentDoors[3] == 1 && !(Level.Rooms[row, col-1].isBossRoom)){
                    Level.Rooms[row, col-1].setDoor(1);
                }
                if(currentDoors[3] == 1 && Level.Rooms[row, col-1].isBossRoom){
                    Level.Rooms[row,col].doors[3] = 0;
                }
             }
        }
    }

    void TurnOffPathPoints(){

        Room FirstRoom = Level.Rooms[1,0];

        Queue<Room> que = new Queue<Room>();
        bool[,] visited = new bool[Level.NumRoomsX, Level.NumRoomsX];

        que.Enqueue(FirstRoom);

        while(que.Count > 0){
            Room currentRoom = que.Dequeue();
           
            if (visited[currentRoom.row, currentRoom.col] == true){
                continue;
            } else{
                visited[currentRoom.row, currentRoom.col] = true;
            }


            // INSTEAD OF GOING FROM y = 1 to 1= height-1 , check to see which x or z position were at
            //  (according to path width) and if were close to a wall then we should try and smooth the door so theres an arch
            if(currentRoom.doors[0] == 1){
                // There is a door to the north
                Room nextRoom = Level.Rooms[currentRoom.row-1, currentRoom.col];
                que.Enqueue(nextRoom);

                for(int x = (int)(currentRoom.Center.x) - pathWidth; x < (int)(currentRoom.Center.x) + pathWidth; x++){
                    for(int z = (int)(currentRoom.Center.z); z < (int)(nextRoom.Center.z); z ++){
                        // marchingcube.grd[x,y,z].on = false; 
                        Level.pathPoints[x,z] = 1;

                    }

                }
            }
            if(currentRoom.doors[1] == 1){
                // There is a door to the East 
                Room nextRoom = Level.Rooms[currentRoom.row, currentRoom.col+1];
                que.Enqueue(nextRoom);
                for(int z = (int)(currentRoom.Center.z) - pathWidth; z < (int)(currentRoom.Center.z) + pathWidth; z++){
                    for(int x = (int)(currentRoom.Center.x); x < (int)(nextRoom.Center.x); x ++){
                        // MarchingCube.grd[x,y,z].On = false; 

                        Level.pathPoints[x,z] = 1;
                    }
                    
                }
            }
            
            if(currentRoom.doors[2] == 1){
                // There is a door to the South
                Room nextRoom = Level.Rooms[currentRoom.row+1, currentRoom.col];
                que.Enqueue(nextRoom);
                for(int x = (int)(currentRoom.Center.x) - pathWidth; x < (int)(currentRoom.Center.x) + pathWidth; x++){
                    for(int z = (int)(nextRoom.Center.x); z < (int)(currentRoom.Center.x); z ++){

                        // MarchingCube.grd[x,y,z].On = false; 
                        Level.pathPoints[x,z] = 1;

                    }
                
                }
            }
            if(currentRoom.doors[3] == 1){
                // There is a door to the West 
                Room nextRoom = Level.Rooms[currentRoom.row, currentRoom.col-1];
                que.Enqueue(nextRoom);
                for(int z = (int)(currentRoom.Center.z) - pathWidth; z < (int)(currentRoom.Center.z) + pathWidth; z++){
                    for(int x = (int)(nextRoom.Center.x); x < (int)(currentRoom.Center.x); x ++){
                        // MarchingCube.grd[x,y,z].On = false; 
                        Level.pathPoints[x,z] = 1;

                    }
                }
            }

        }

    }

    void TurnOnWalls(){
        
        for(int x = 0; x < Level.pathPoints.GetLength(0); x ++){
            for(int z = 0; z < Level.pathPoints.GetLength(1); z ++){
                if(Level.pathPoints[x, z] != 1){
                    for(int y = 1; y < gridSize.y-1; y ++)
                        MarchingCube.grd[x,y,z].On = true;
                }
            }
        }
    }

}
