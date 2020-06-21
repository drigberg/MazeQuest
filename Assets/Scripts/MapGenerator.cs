using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public Rigidbody floorPrefab;
    public Rigidbody wallPrefab;
    public List<Rigidbody> walls;
    public Vector3 entrance;
    public Vector3 exit;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody floor = Instantiate(floorPrefab);
        walls = new List<Rigidbody>();
        floor.transform.localScale = new Vector3(9.0f, 1.0f, 9.0f);
        floor.transform.position = new Vector3(4.5f, 0.0f, 4.5f);
        entrance = new Vector3(0.5f, 2.0f, 4.5f);
        exit = new Vector3(8.5f, 2.0f, 4.5f);
        float[][] directions = {
            new float[2]{1.0f, 0.0f},
            new float[2]{0.0f, 1.0f},
            new float[2]{-1.0f, 0.0f},
            new float[2]{0.0f, -1.0f},
        };
        float wallHeight = 2.0f;
        Vector3 currentPos = new Vector3(0.5f, wallHeight, 0.5f);
        for (int i = 0; i < directions.Length; i++) {
            float[] direction = directions[i];
            for (int j = 0; j < 8; j++) {
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
        Debug.Log(pos);
        Rigidbody wall = Instantiate(wallPrefab);
        wall.transform.position = pos;
        wall.transform.localScale = new Vector3(1.0f, 3.0f, 1.0f);
        walls.Add(wall);
    }
}
