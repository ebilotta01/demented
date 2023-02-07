using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemySpawn : MonoBehaviour
{
    // number of row rooms and each room size
    int nRowRooms;
    int roomSize;
    public float batchSpawnRadius = 6;
    // queue that holds spawnpoints for enemies on the map
    Queue<Vector3> spawnQueue;
    public GameObject monsterOne;

    // Start is called before the first frame update
    void Start()
    {
        spawnQueue = new Queue<Vector3>();
        nRowRooms = Level.NumRoomsX;
        roomSize = Level.RoomSize;
        Debug.Log(validatePoint(90,90));
        // fills the spawn queue with the centers of each room on the map
        setSpawnPoints();
        // instantiate enemies into the spawn points
        spawnEnemies();

        Debug.Log("Total Enemies Spawned: " + Level.TotalEnemies);
    }
    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawSphere(new Vector3(90, 1, 90), 1.0f);

    //     for(int x = 0; x < Level.openSpace.GetLength(0); x ++){
    //         for(int z = 0; z< Level.openSpace.GetLength(1); z ++){
    //             if(Level.openSpace[x,z] == 1){
    //                 Gizmos.color = Color.yellow;
    //                 Gizmos.DrawSphere(new Vector3(x, 3, z), .05f);
    //             } else if(Level.openSpace[x,z] == 2){
    //                 Gizmos.color = Color.red;
    //                 Gizmos.DrawSphere(new Vector3(x, 3, z), .05f);
    //             }


    //         }
    //     }

    // }

    Vector3 newPoint(){
        int x = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
        int z = Random.Range(0, Level.NumRoomsX * Level.RoomSize-1);
        
        bool valid = validatePoint(x, z);

        if(Level.openSpace[(int)x, (int)z] != 1 || !valid){
            Debug.Log("Invalid Point -- VALID: " + valid + " pt: " + x + " " + z);
            return newPoint();
        }


        Vector3 pt = new Vector3(x, 1, z);
        Debug.Log("VALID Point -- VALID: " + valid + " pt: " + pt );
        // Debug.Log("NEW POINT");
        // Debug.Log(pt);
        // pt = elevatePt(pt);
        return pt;
    }

    bool validatePoint(int _x, int _z){
        int[] pts = Level.boundPoints(_x, _z);

        int lowX = pts[0];
        int lowZ = pts[1];
        int highX = pts[2];
        int highZ = pts[3];
        // Debug.Log(lowX + " " + lowZ + " " + highX + " " + highZ);

        for(int x = lowX; x <= highX; x ++){
            for(int z = lowZ; x <= highZ; z ++){
                // Debug.Log(Level.openSpace[x, z]);
               if(Level.openSpace[x, z] == 0){
                   return false;
               }
            
            }
        }
        return true;
    }


    void setSpawnPoints(){
        Debug.Log("SET PTS");
        for(int i = 0; i < Level.NumEnemySpawnPoints; i ++){

            Vector3 pt = newPoint();
            Debug.Log(pt);
            spawnQueue.Enqueue(pt); 
            Level.disableSpawn(pt);
        }

    }

    void spawnEnemies(){
        Vector3 pt, spawnPos;
        int numEnemies;
        float theta;

        // a batch of size numEnemies will be spawned in the spawnpoint
        while (spawnQueue.Count != 0){
            pt = spawnQueue.Dequeue();
            spawnPos = pt;
            spawnPos.y = 2;
            numEnemies = Random.Range(Level.MinMonsters, Level.MaxMonsters+1);
            theta = 2.0f * Mathf.PI / numEnemies;

            // spawn the enemies in a circle around the spawnpoint
            for (int i = 0; i < numEnemies; i++){
                spawnPos.x = pt.x + batchSpawnRadius * Mathf.Cos(theta * i);
                spawnPos.z = pt.z + batchSpawnRadius * Mathf.Sin(theta * i);
                GameObject e = Instantiate(monsterOne, spawnPos, Quaternion.identity);
                e.transform.parent = transform.Find("Enemies").gameObject.transform;
            }
        }

        // pick a random middle group and pick a member of its children
        GameObject go = GameObject.Find("SpawnManager/Enemies");
        Debug.Log(go.name + " has " + go.transform.childCount + " children");
        Level.TotalEnemies = go.transform.childCount; 
        Level.prettyPrint2DSpace();
    }
}
