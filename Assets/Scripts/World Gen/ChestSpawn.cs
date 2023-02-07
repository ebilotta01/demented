using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChestSpawn: MonoBehaviour
{
    // number of row rooms and each room size
    int nRowRooms;
    int roomSize;
    // queue that holds spawnpoints for enemies on the map
    Queue<Vector3> spawnQueue;
    [SerializeField]
    public GameObject chest;

    // Start is called before the first frame update
    void Start()
    {
        spawnQueue = new Queue<Vector3>();
        nRowRooms = Level.NumRoomsX;
        roomSize = Level.RoomSize;
        // fills the spawn queue with the centers of each room on the map
        setSpawnPoints();
        // instantiate enemies into the spawn points
        spawnChests();
    }

    Vector3 newPoint(){
        int x = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
        int z = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
        // Debug.Log("PATH POINTS");
        // Debug.Log(x + " -- " + z);
        // Debug.Log(Level.pathPoints.GetLength(0));

        while(Level.openSpace[(int)x, (int)z] == 0){
            x = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
            z = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
        }

        Vector3 pt = new Vector3(x, 0, z);
        return pt;
    }

    void setSpawnPoints(){
        
        for(int i = 0; i < Level.NumChestSpawnPoints; i ++){
            Vector3 pt = newPoint();
            while(MarchingCube.grd[(int)pt.x, (int)pt.y, (int)pt.z].On){
                pt.y = pt.y + 1.0f;
                if(pt.y >= MarchingCube.grd.GetLength(1)){
                    pt = newPoint();
                }
            }
            
            pt.y = pt.y - .5f;
            spawnQueue.Enqueue(pt);
            Level.disableSpawn(pt);
        }
    
    }

    void spawnChests(){
        Vector3 pt; 
        while (spawnQueue.Count != 0){
            pt = spawnQueue.Dequeue();
            GameObject newChest = Instantiate(chest, pt, Quaternion.identity);
            newChest.transform.parent = transform.Find("Chests").gameObject.transform;
            
        }
    }
}

