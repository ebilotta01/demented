using System.Collections;
using System.Collections.Generic;

using UnityEngine.AI;
using UnityEngine;


public class MainMenuGen : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 gridSize;
    public int RoomSize = 100;
    public Material material;
    public int noiseLimit = 15;
    public int smooth = 5;
    int pathWidth;
    Mesh mesh = null;   
    private NavMeshSurface surface;



    void Start()
    {
        pathWidth = Mathf.RoundToInt(Level.RoomSize/2 * 0.75f);
        material = GetComponent<Renderer>().material;
        GenerateNextLevel();

        surface = gameObject.GetComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        //surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.BuildNavMesh();
    }
    void Update(){
        // ON LEVEL COMPLETEION CALL
        /*if(Input.GetKeyDown(KeyCode.F)){
            Debug.Log("Clicked");
            Level.SetNextLevel();
            GenerateNextLevel();
        }*/
    }
      

    public void GenerateNextLevel(){
        gridSize = new Vector3(RoomSize, 12, RoomSize);
        MarchingCube.Clear();
        
        MakeGrid();
        // little bit of noise to make things more oraganic
        Noise3d();
        // smooths the map 
        for(int i =0; i <smooth; i ++){
            SmoothMap();
        }

        // perform the march over the grid.
        March();
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
        System.Random prng = new System.Random("This Is An Easter Egg!".GetHashCode());
    

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
}

