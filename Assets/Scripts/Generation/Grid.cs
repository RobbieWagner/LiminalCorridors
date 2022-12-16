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
    [SerializeField] private float wallBias = .45f;
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

                //float xOffset = Random.Range(-10000f, 10000f);
                //float yOffset = Random.Range(-10000f, 10000f);
                //float noiseValue = Mathf.PerlinNoise(x * wallNoiseScale + xOffset, y * wallNoiseScale + yOffset);
                
                //cell.isTraversable = noiseValue < blankTileBias;
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
        float xOffset, yOffset, noiseValue;

        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                Cell cell = grid[x, y];
                xOffset = Random.Range(-10000f, 10000f);
                yOffset = Random.Range(-10000f, 10000f);
                noiseValue = Mathf.PerlinNoise(x * wallNoiseScale + xOffset, y * wallNoiseScale + yOffset);

                float addHWall = Random.Range(0, wallBias);
                float addVWall = Random.Range(0, wallBias);
                if(noiseValue < addHWall) {
                    GameObject hWall = Instantiate(horizontalWall, new Vector3( x * cellSize, 0, y * cellSize), Quaternion.identity);
                }
                if(noiseValue < addVWall) {
                    GameObject vWall = Instantiate(verticalWall, new Vector3( x * cellSize, 0, y * cellSize), Quaternion.identity);
                }

                Instantiate(ceiling, new Vector3(x * cellSize, 9, y * cellSize), Quaternion.Euler(90, 0, 0));
                if(light != null && Random.Range(0,5) == 0)Instantiate(light, new Vector3(x * cellSize, 8, y * cellSize), Quaternion.Euler(90, 0, 0));
            }
        }

        bool horizontalWallSpawned = false;
    }
}
