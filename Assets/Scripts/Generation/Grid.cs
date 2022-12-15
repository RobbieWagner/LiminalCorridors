using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private int sizeX = 100;
    [SerializeField]
    private int sizeY = 100;
    [SerializeField]
    private float noiseScale = .1f;
    [SerializeField]
    private float blankTileBias = .8f;
    [SerializeField]
    private int cellSize = 10;
    [SerializeField]
    private GameObject blankTile;
    [SerializeField]
    private GameObject ceiling;
    [SerializeField]
    private GameObject light;
    [SerializeField]
    private GameObject[] wallTiles;

    [SerializeField]
    private Material floorMaterial;

    private Cell[,] grid;

    private void Start() {
        grid = new Cell[sizeX, sizeY];

        for(int x = 0; x < sizeX; x++) {
            for(int y = 0; y < sizeY; y++) {
                Cell cell = new Cell();

                //float xOffset = Random.Range(-10000f, 10000f);
                //float yOffset = Random.Range(-10000f, 10000f);
                //float noiseValue = Mathf.PerlinNoise(x * noiseScale + xOffset, y * noiseScale + yOffset);
                
                //cell.isTraversable = noiseValue < blankTileBias;
                cell.isTraversable = true;
                grid[x, y] = cell;
            }
        }

        //for(int i = 0; i < sizeX; i++) {
            //for(int j = 0; j < sizeY; j++) {
                //Cell cell = grid[i, j];
                //if(cell.isTraversable) {
                    //Quaternion rotation = blankTile.transform.rotation;
                    //cell.tile = Instantiate(blankTile, new Vector3(i * cellSize, 0, j * cellSize), rotation);
                //}
                //else{
                    //int tileToUse = Random.Range(0, wallTiles.Length);
                    //int rotationPosition = Random.Range(0, 2);
                    //Quaternion rotation = wallTiles[tileToUse].transform.rotation * Quaternion.Euler(0, (float) (90 * rotationPosition), 0);
                    //cell.tile = Instantiate(wallTiles[tileToUse], new Vector3(i * cellSize, 0, j * cellSize), rotation);
                //}

                //Instantiate(ceiling, new Vector3(i * cellSize, 9, j * cellSize), Quaternion.Euler(90, 0, 0));
                //if(light != null && Random.Range(0,5) == 0)Instantiate(light, new Vector3(i * cellSize, 8, j * cellSize), Quaternion.Euler(90, 0, 0));
            //}
        //}

        DrawGameTerrain(grid);
        AddTexture(grid);
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
}
