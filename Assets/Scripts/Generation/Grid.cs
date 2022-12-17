using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grid : MonoBehaviour
{
    [SerializeField] private int sizeX = 100;
    [SerializeField] private int sizeY = 100;
    [SerializeField] private GameObject horizontalWall;
    [SerializeField] private GameObject verticalWall;
    [SerializeField] private float wallNoiseScale = .1f;
    [SerializeField, Range(0f, 1f)] private float wallBias = .45f;
    [SerializeField, Range(0f, 1f)] private float outerWallBias = .99f;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private GameObject ceiling;
    [SerializeField] private GameObject light;
    [SerializeField] private Material floorMaterial;

    private Cell[,] grid;

    [SerializeField] NavMeshBaker navMeshBaker;
    List<NavMeshSurface> navMeshSurfaces;

    private void Start() {
        grid = new Cell[sizeX, sizeY];

        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                Cell cell = new Cell();

                //float xOffset1 = Random.Range(-10000f, 10000f);
                //float yOffset1 = Random.Range(-10000f, 10000f);
                //float noiseValue1 = Mathf.PerlinNoise(x * wallNoiseScale + xOffset1, y * wallNoiseScale + yOffset1);
                
                //cell.isTraversable = noiseValue1 < blankTileBias;
                cell.isTraversable = true;
                grid[x, y] = cell;
            }
        }

        navMeshSurfaces = new List<NavMeshSurface>();
        navMeshSurfaces.Add(gameObject.GetComponent<NavMeshSurface>());

        DrawGameTerrain(grid);
        AddTexture(grid);
        DrawGameWalls(grid);
        navMeshBaker.buildNavMeshes(navMeshSurfaces);
    }

    private void AddTexture(Cell[,] grid) {
        Texture2D texture = new Texture2D(sizeX, sizeY);
        Color[] colorMap = new Color[sizeX * sizeY];
        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                colorMap[x * sizeY + y] = new Color(128, 120, 79, 1);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = floorMaterial;
        meshRenderer.material.mainTexture = texture;
    }

    private void DrawGameTerrain(Cell[,] grid) {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                Vector3 a = new Vector3(x * cellSize - cellSize/2, 0 ,y * cellSize + cellSize/2);
                Vector3 b = new Vector3(x * cellSize + cellSize/2, 0 ,y * cellSize + cellSize/2);
                Vector3 c = new Vector3(x * cellSize - cellSize/2, 0 ,y * cellSize - cellSize/2);
                Vector3 d = new Vector3(x * cellSize + cellSize/2, 0 ,y * cellSize - cellSize/2);
                Vector3[] v = new Vector3[] {a, b, c, b, d, c};
                for(int i = 0; i < 6; i++) {
                    vertices.Add(v[i]);
                    triangles.Add(triangles.Count);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    private void DrawGameWalls(Cell[,] grid) {
        float xOffset1, yOffset1, xOffset2, yOffset2, noiseValue1, noiseValue2;

        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                Cell cell = grid[x, y];
                xOffset1 = Random.Range(-10000f, 10000f);
                yOffset1 = Random.Range(-10000f, 10000f);
                xOffset2 = Random.Range(-10000f, 10000f);
                yOffset2 = Random.Range(-10000f, 10000f);
                noiseValue1 = Mathf.PerlinNoise(x * wallNoiseScale + xOffset1, y * wallNoiseScale + yOffset1);
                noiseValue2 = Mathf.PerlinNoise(x * wallNoiseScale + xOffset2, y * wallNoiseScale + yOffset2);
                
                //Handles edge walls
                float addHWall = 0;
                float addVWall = 0;

                if(((x == sizeX - 3 || x == 2) ^ (y == sizeY - 3 || y == 2))) {
                    if((x == sizeX - 3) || (x == 2)){
                        addHWall = outerWallBias;
                        addVWall = 0;
                    }
                    else if(y == sizeY - 3 || y == 2){
                        addHWall = 0;
                        addVWall = outerWallBias;
                    }
                }
                //Handles non-edge walls
                else {
                    addHWall = wallBias;
                    addVWall = wallBias;
                }

                if(noiseValue1 < addHWall) {
                    GameObject hWall = Instantiate(horizontalWall, new Vector3( x * cellSize, 0, y * cellSize), Quaternion.identity);
                }
                if(noiseValue2 < addVWall) {
                    GameObject vWall = Instantiate(verticalWall, new Vector3( x * cellSize, 0, y * cellSize), Quaternion.identity);
                }

                Instantiate(ceiling, new Vector3(x * cellSize, 9, y * cellSize), Quaternion.Euler(90, 0, 0));
                if(light != null && Random.Range(0,5) == 0)Instantiate(light, new Vector3(x * cellSize, 8, y * cellSize), Quaternion.Euler(90, 0, 0));
            }
        }

        bool horizontalWallSpawned = false;
    }
}
