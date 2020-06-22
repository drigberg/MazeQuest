using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGenerator : MonoBehaviour
{
    public Rigidbody floorPrefab;
    public Rigidbody wallPrefab;
    public List<Rigidbody> walls;
    public Vector3 entrance;
    public Vector3 exit;
    public float mazeSideLength = 11.0f;
    public float wallHeight = 2.0f;
    private System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        Rigidbody floor = Instantiate(floorPrefab);
        walls = new List<Rigidbody>();
        floor.transform.localScale = new Vector3(mazeSideLength, 1.0f, mazeSideLength);
        floor.transform.position = new Vector3(mazeSideLength / 2, 0.0f, mazeSideLength / 2);
        entrance = new Vector3(0.5f, 2.0f, 0.5f);
        exit = new Vector3(mazeSideLength - 0.5f, 2.0f, mazeSideLength - 0.5f);
        createBounds();
        // createInnerWallsRecursive();
    }

    void createInnerWallsRecursive() {
        // Maze generation with recursive division method
        bool done = false;
        List<int> horizontalWallStarts = new List<int>(new int[]{ 0, (int)mazeSideLength });
        List<int> verticalWallStarts = new List<int>(new int[]{ 0, (int)mazeSideLength });
        while (!done) {
            for (int i = 0; i < horizontalWallStarts.Length - 1; i++) {
                for (int j = 0; j < verticalWallStarts.Length - 1; j++) {
                    createHorizontalWall(horizontalWallStarts, i);
                    createVericalWall();
                }
            }
        }
    }

    void createHorizontalWall(List<int> horizontalWallStarts, int index) {
        int start = horizontalWallStarts[index];
        int end = horizontalWallStarts[index+1];
        float newPos = (float)rnd.Next(start, end);
        horizontalWallStarts.Add(newPos);
        horizontalWallStarts.Sort();
    }

    void createBounds() {
        float[][] directions = {
            new float[2]{1.0f, 0.0f},
            new float[2]{0.0f, 1.0f},
            new float[2]{-1.0f, 0.0f},
            new float[2]{0.0f, -1.0f},
        };
        Vector3 currentPos = new Vector3(0.5f, wallHeight, 0.5f);
        for (int i = 0; i < directions.Length; i++) {
            float[] direction = directions[i];
            for (int j = 0; j < mazeSideLength - 1.0f; j++) {
                currentPos = new Vector3(
                    currentPos.x + direction[0],
                    wallHeight,
                    currentPos.z + direction[1]);
                if (currentPos != entrance && currentPos != exit) {
                    createWall(currentPos);
                }
            }
        }
    }

    void createWall(Vector3 pos) {
        Rigidbody wall = Instantiate(wallPrefab);
        wall.transform.position = pos;
        wall.transform.localScale = new Vector3(1.0f, 3.0f, 1.0f);
        walls.Add(wall);
    }
}
