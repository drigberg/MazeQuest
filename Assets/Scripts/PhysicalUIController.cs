using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalUIController : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public Transform redBlockPrefab;
    public Transform blueBlockPrefab;
    public Transform greenBlockPrefab;
    public Transform redBlockInputPrefab;
    public string state;
    public Vector3 cameraPosition;

    private List<GameObject> createdObjects;
    private PhysicalUIBlockInputController levelSelectInput;

    // movement
    private float localTime = 0.0f;
    private float glideTime = 1.0f;
    private bool gliding = false;
    private Vector3 glideStartPosition;
    private Vector3 glideTargetPosition;

    // screen positions
    private Vector3 cameraOffset;
    private Vector3 levelSelectOrigin;
    private Vector3 mainScreenOrigin;
    private Vector3 controlsOrigin;

    // Start is called before the first frame update
    void Start() {}

    void Awake() {
        createdObjects = new List<GameObject>();
        state = "UNDEFINED";
        mainScreenOrigin = new Vector3(0.0f, 0.0f, 0.0f);
        levelSelectOrigin = mainScreenOrigin + new Vector3(15.0f, 0.0f, 0.0f);
        controlsOrigin = mainScreenOrigin + new Vector3(-15.0f, 0.0f, 0.0f);
        cameraOffset = new Vector3(0.0f, 2.0f, 0.0f);
    }

    void FixedUpdate() {
        if (gliding) {
            Glide();
        }
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

    public PhysicalUIBlockInputController CreateInputBlock(
            Transform prefab,
            Vector3 position,
            Vector3 scale) {
        Transform block = Instantiate(
           prefab,
           position,
           Quaternion.identity);
        block.transform.localScale = scale;
        PhysicalUIBlockInputController blockText = block.GetComponent<PhysicalUIBlockInputController>();
        createdObjects.Add(block.gameObject);
        return blockText;
    }

    public void SilentDoNothing() {

    }

    public void StartMenuState() {
        state = "START_MENU";
        cameraPosition = cameraOffset;
        mazeGenerator.timerText.gameObject.SetActive(false);
        CreateMainScreenBlocks(mainScreenOrigin);
        CreateLevelSelectBlocks(levelSelectOrigin);
        CreateControlsBlocks(controlsOrigin);
    }

    public void CreateMainScreenBlocks(Vector3 localOrigin) {
        CreateBlock(
            redBlockPrefab,
            "MAZE\nQUEST",
            localOrigin + new Vector3(0.0f, -2.0f, 0.5f),
            new Vector3(2.0f, 1.0f, 2.0f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "NEW\nGAME",
            localOrigin + new Vector3(0.0f, -3.0f, -1.5f),
            new Vector3(1.0f, 1.0f, 1.0f),
            OnNewGame);
        CreateBlock(
            greenBlockPrefab,
            "LEVEL\nSELECT",
            localOrigin + new Vector3(3.0f, -3.0f, 0.0f),
            new Vector3(1.3f, 1.0f, 1.0f),
            OnGlideToLevelSelect);
        CreateBlock(
            greenBlockPrefab,
            "CONTROLS",
            localOrigin + new Vector3(-3.0f, -3.0f, 0.0f),
            new Vector3(1.3f, 1.0f, 1.0f),
            OnGlideToControls);
    }

    public void CreateLevelSelectBlocks(Vector3 localOrigin) {
        levelSelectInput = CreateInputBlock(
            redBlockInputPrefab,
            localOrigin + new Vector3(0.0f, -2.0f, 0.5f),
            new Vector3(1.0f, 1.0f, 1.0f));
        levelSelectInput.inputField.text = "1";
        CreateBlock(
            blueBlockPrefab,
            "JUMP TO\nLEVEL",
            localOrigin + new Vector3(0.0f, -3.0f, -1.5f),
            new Vector3(1.5f, 1.0f, 1.0f),
            OnLevelSelect);
        CreateBlock(
            greenBlockPrefab,
            "MAIN\nSCREEN",
            localOrigin + new Vector3(-3.0f, -3.0f, 0.0f),
            new Vector3(1.3f, 1.0f, 1.0f),
            OnGlideToMainScreen);
    }

    public void CreateControlsBlocks(Vector3 localOrigin) {
        CreateBlock(
            redBlockPrefab,
            "CONTROLS",
            localOrigin + new Vector3(0.0f, -3.0f, 1.5f),
            new Vector3(3.0f, 1.0f, 1.5f),
            SilentDoNothing);
        CreateBlock(
            redBlockPrefab,
            "MOVE",
            localOrigin + new Vector3(-1.0f, -2.0f, 0f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "AWSD",
            localOrigin + new Vector3(1.0f, -2.0f, 0f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            redBlockPrefab,
            "LOOK",
            localOrigin + new Vector3(-1.0f, -2.0f, -0.8f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "MOUSE",
            localOrigin + new Vector3(1.0f, -2.0f, -0.8f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            redBlockPrefab,
            "STRAFE",
            localOrigin + new Vector3(-1.0f, -2.0f, -1.6f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "QE",
            localOrigin + new Vector3(1.0f, -2.0f, -1.6f),
            new Vector3(1.0f, 1.0f, 0.75f),
            SilentDoNothing);
        CreateBlock(
            greenBlockPrefab,
            "MAIN\nSCREEN",
            localOrigin + new Vector3(3.0f, -3.0f, 0.0f),
            new Vector3(1.3f, 1.0f, 1.0f),
            OnGlideToMainScreen);
    }

    public void StartGlideToPosition(Vector3 target) {
        localTime = 0.0f;
        gliding = true;
        glideStartPosition = cameraPosition;
        glideTargetPosition = target + cameraOffset;
    }

    public void Glide() {
        localTime += Time.deltaTime / glideTime;
        cameraPosition = Vector3.Lerp(glideStartPosition, glideTargetPosition, localTime);
        if (localTime >= glideTime) {
            gliding = false;
        }
    }

    public void OnGlideToControls() {
        StartGlideToPosition(controlsOrigin);
    }

    public void OnGlideToLevelSelect() {
        StartGlideToPosition(levelSelectOrigin);
    }

    public void OnGlideToMainScreen() {
        StartGlideToPosition(mainScreenOrigin);
    }

    public void OnLevelSelect() {
        string levelString = levelSelectInput.inputField.text;
        int level = 1;
        if (levelString != "") {
            level = int.Parse(levelString);
            if (level < 1) {
                level = 1;
            }
            else if (level > mazeGenerator.maxLevelReached) {
                level = mazeGenerator.maxLevelReached;
            }
        }
        NewGame(level);
    }

    public void NewGame(int level) {
        mazeGenerator.NewGameFromLevel(level);
        StartLevelState(level);
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
        NewGame(1);
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
