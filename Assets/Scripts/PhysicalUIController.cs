using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalUIController : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public Transform redBlockPrefab;
    public Transform blueBlockPrefab;
    public string state;
    public Vector3 cameraPosition;

    private List<GameObject> createdObjects;

    // Start is called before the first frame update
    void Start() {}

    void Awake() {
        createdObjects = new List<GameObject>();
        state = "UNDEFINED";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void destroyObjects() {
        for (int i = 0; i < createdObjects.Count; i++) {
            Destroy(createdObjects[i]);
        }
        createdObjects = new List<GameObject>();
    }

    public void HiddenState() {
        state = "HIDDEN";
        destroyObjects();
    }

    public PhysicalUIBlockController CreateBlock(
            Transform prefab, string text,
            Vector3 position,
            Vector3 scale,
            PhysicalUIBlockController.Action action) {
        Transform block = Instantiate(
           prefab,
           position,
           Quaternion.identity);
        block.transform.localScale = scale;
        PhysicalUIBlockController blockText = block.GetComponent<PhysicalUIBlockController>();
        float textStretch = scale.z / scale.x;
        blockText.SetText(text, textStretch);
        blockText.SetAction(action);
        createdObjects.Add(block.gameObject);
        return blockText;
    }

    public void SilentDoNothing() {

    }

    public void StartMenuState() {
        state = "START_MENU";
        cameraPosition = new Vector3(0.0f, 2.0f, 0.0f);
        mazeGenerator.timerText.gameObject.SetActive(false);
        CreateBlock(
            redBlockPrefab,
            "MAZE\nQUEST",
            new Vector3(0.0f, -2.0f, 0.5f),
            new Vector3(2.0f, 1.0f, 2.0f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "NEW\nGAME",
            new Vector3(0.0f, -3.0f, -1.5f),
            new Vector3(1.0f, 1.0f, 1.0f),
            OnNewGame);
    }

    public void StartLevelState(int level) {
        state = "START_LEVEL";
        destroyObjects();
        CreateBlock(
            redBlockPrefab,
            "LEVEL\n" + level,
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.47222f - 4.75f,
                (float)mazeGenerator.mazeSideLength * 0.9074f + 2.859f,
                (float)mazeGenerator.mazeSideLength * 0.509f  +2.417f),
            new Vector3(3.0f, 1.0f, 3.0f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "START",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.5463f + 6.583f,
                (float)mazeGenerator.mazeSideLength * 0.888f,
                (float)mazeGenerator.mazeSideLength * 0.5f - 3.5f),
            new Vector3(3.0f, 1.0f, 3.0f),
            OnStartLevel);
        mazeGenerator.timerText.gameObject.SetActive(true);
    }

    public void GameOverState(int level, int totalScore) {
        state = "GAME_OVER";
        mazeGenerator.timerText.gameObject.SetActive(false);
        CreateBlock(
            redBlockPrefab,
            "LEVEL " + level + "\nFAILED",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.47222f - 4.75f,
                (float)mazeGenerator.mazeSideLength * 0.9074f + 2.859f,
                (float)mazeGenerator.mazeSideLength * 0.509f + 2.417f),
            new Vector3(4.0f, 1.0f, 3.0f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "MAIN\nMENU",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.5463f + 6.583f,
                (float)mazeGenerator.mazeSideLength * 0.888f,
                (float)mazeGenerator.mazeSideLength * 0.5f - 3.5f),
            new Vector3(3.0f, 1.0f, 3.0f),
            OnMainMenu);
    }

    public void SuccessState(int level, int score, int totalScore) {
        state = "SUCCESS";
        mazeGenerator.timerText.gameObject.SetActive(false);
        CreateBlock(
            redBlockPrefab,
            "LEVEL " + level + "\nCOMPLETE",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.47222f - 4.75f,
                (float)mazeGenerator.mazeSideLength * 0.9074f + 2.859f,
                (float)mazeGenerator.mazeSideLength * 0.509f + 2.417f),
            new Vector3(4.0f, 1.0f, 3.0f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "NEXT\nLEVEL",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.5463f + 6.583f,
                (float)mazeGenerator.mazeSideLength * 0.888f,
                (float)mazeGenerator.mazeSideLength * 0.5f - 3.5f),
            new Vector3(3.0f, 1.0f, 3.0f),
            OnNextLevel);
    }

    void OnNewGame() {
        mazeGenerator.Reset();
        StartLevelState(mazeGenerator.level);
    }

    void OnNextLevel() {
        destroyObjects();
        mazeGenerator.NextLevel();
    }

    void OnStartLevel() {
        destroyObjects();
        mazeGenerator.StartLevel();
    }

    public void OnMainMenu() {
        mazeGenerator.Destroy();
        StartMenuState();
    }
}
