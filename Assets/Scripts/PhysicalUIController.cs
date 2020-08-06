﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalUIController : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public Transform redBlockPrefab;
    public Transform blueBlockPrefab;

    private List<GameObject> createdObjects;

    // Start is called before the first frame update
    void Start() {}

    void Awake() {
        createdObjects = new List<GameObject>();
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
        destroyObjects();
    }

    public PhysicalUIBlockController CreateBlock(Transform prefab, string text, Vector3 position, Vector3 scale, PhysicalUIBlockController.Action action) {
       Transform block = Instantiate(
           prefab,
           position,
           Quaternion.identity);
        block.transform.localScale = scale;
        PhysicalUIBlockController blockText = block.GetComponent<PhysicalUIBlockController>();
        blockText.SetText(text);
        blockText.SetAction(action);
        createdObjects.Add(block.gameObject);
        return blockText;
    }

    public void SilentDoNothing() {

    }

    public void StartMenuState() {
        mazeGenerator.timerText.gameObject.SetActive(false);
        CreateBlock(redBlockPrefab, "MAZE\nQUEST", new Vector3(0.5f, -2.0f, 1.0f), new Vector3(2.0f, 1.0f, 2.0f), SilentDoNothing);
        CreateBlock(blueBlockPrefab, "NEW\nGAME", new Vector3(0.5f, -3.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), OnNewGame);

        // mainText.text = "MAZE QUEST";
        // secondaryText.text = "Move & Turn: AWSD\nStrafe: QE";
        // mainText.gameObject.SetActive(true);
        // secondaryText.gameObject.SetActive(true);
        // timerText.gameObject.SetActive(false);
        // startButton.gameObject.SetActive(false);
        // newGameButton.gameObject.SetActive(true);
    }

    public void StartLevelState(int level) {
        destroyObjects();
        CreateBlock(
            redBlockPrefab,
            "LEVEL\n" + level,
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.47222f - 4.75f,
                (float)mazeGenerator.mazeSideLength *0.9074f + 2.859f,
                (float)mazeGenerator.mazeSideLength *0.509f  +2.417f),
            new Vector3(2.0f, 1.0f, 2.0f), SilentDoNothing);
        CreateBlock(
            blueBlockPrefab,
            "START",
            new Vector3(
                (float)mazeGenerator.mazeSideLength * 0.5463f + 6.583f,
                (float)mazeGenerator.mazeSideLength *0.888f,
                (float)mazeGenerator.mazeSideLength *0.5f - 3.5f),
            new Vector3(3.0f, 1.0f, 3.0f), OnStartLevel);
        mazeGenerator.timerText.gameObject.SetActive(true);
    }

    public void GameOverState(int level, int totalScore) {
        mazeGenerator.timerText.gameObject.SetActive(false);
        // mainText.text = "LEVEL " + level + " FAILED";
        // secondaryText.text = "Total score: " + totalScore;
        // mainText.gameObject.SetActive(true);
        // secondaryText.gameObject.SetActive(true);
        // timerText.gameObject.SetActive(false);
        // newGameButton.gameObject.SetActive(true);
    }

    public void SuccessState(int level, int score, int totalScore) {
        mazeGenerator.timerText.gameObject.SetActive(false);
        // mainText.text = "LEVEL " + level + " COMPLETE";
        // secondaryText.text = "Level score: " + score;
        // secondaryText.text = secondaryText.text + "\nTotal score: " + totalScore;
        // mainText.gameObject.SetActive(true);
        // secondaryText.gameObject.SetActive(true);
    }

    void OnNewGame() {
        mazeGenerator.Reset();
        StartLevelState(mazeGenerator.level);
    }

    void OnStartLevel() {
        destroyObjects();
        mazeGenerator.StartLevel();
    }
}
