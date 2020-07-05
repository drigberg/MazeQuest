using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
DONE: 41 points
    - Maze generation logic (L)
    - Maze building (L)
    - Player controls (M)
    - Camera behavior (M)
    - Entrance and exit platforms (S)
    - Allow smooth movement on floor and against walls (M)
    - Walls at exit (S)
    - Different color for entrance and exit platforms (S)
    - Detect success when reaching exit, display "SUCCESS!" (M)
    - When level ends, wait 3 seconds and then reset to next level with larger maze (M)
    - Start level with aerial view (S)
    - Aerial view: allow player to click "Go" when ready, switch to player view (S)
    - Time each level at (side_length ** 1.5) / 10 seconds (S)
    - Display time remaining (M)
    - If timer runs out, player loses (S)
    - Game over: final level is displayed with total time from all levels, and option to restart (M)
    - Use points instead of total time (S)
    - Revise lighting (S)
    - Build (S)
 */

public class MazeGenerator : MonoBehaviour
{
    // prefabs
    public Transform floorPrefab;
    public Transform wallPrefab;
    public Transform startWallPrefab;
    public Transform targetWallPrefab;
    public Transform targetDetectorPrefab;
    public Rigidbody playerPrefab;

    // GUI
    public TMPro.TextMeshProUGUI mainText;
    public TMPro.TextMeshProUGUI secondaryText;
    public TMPro.TextMeshProUGUI timerText;
    public Button startButton;
    public Button newGameButton;

    // camera
    public CameraController mainCamera;

    // private
    private System.Random rnd = new System.Random();
    private float wallHeight = 2.0f;
    private List<Transform> walls;
    private List<GameObject> createdObjects;
    private int level = 1;
    private bool playing = false;
    private float maxLevelTime = 0.0f;
    private float levelTimer = 0.0f;
    private int totalScore = 0;
    private float untilNextLevelTimer = 0.0f;


    // Start is called before the first frame update
    void Start() {
        // init listeners and objects
        createdObjects = new List<GameObject>();
        startButton.onClick.AddListener(StartLevel);
        newGameButton.onClick.AddListener(Reset);
    
        // handle GUI
        mainText.text = "MAZE QUEST";
        secondaryText.text = "Move & Turn: AWSD\nStrafe: QE";
        mainText.gameObject.SetActive(true);
        secondaryText.gameObject.SetActive(true); 
        timerText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(true);
    }
 
    void Update() {
        timerText.text = FormatFloatToOnePlace(maxLevelTime - levelTimer) + "s";
        if (untilNextLevelTimer > 0.0f) {
            untilNextLevelTimer -= Time.deltaTime;
            if (untilNextLevelTimer <= 0.0f) {
                level += 1;
                Reset();
            }
        }

        if (playing) {
            levelTimer += Time.deltaTime;
            if (levelTimer >= maxLevelTime) {
                OnGameOver();
            }
        }
    }

    void destroyObjects() {
        for (int i = 0; i < createdObjects.Count; i++) {
            Destroy(createdObjects[i]);
        }
        createdObjects = new List<GameObject>();
    }

    void SetAerialCamera(int mazeSideLength) {
        mainCamera.mazeSideLength = (float)mazeSideLength;
    }

    void Reset() {
        // handle maze
        destroyObjects();
        int mazeSideLength = 7 + level * 2;
        SetAerialCamera(mazeSideLength);
        createFloor((float)mazeSideLength);
        createWalls(mazeSideLength);
        createPlatforms(mazeSideLength);

        maxLevelTime = Mathf.Round(Mathf.Pow((float)mazeSideLength, 1.25f));
        levelTimer = 0.0f;

        // handle GUI
        mainText.text = "LEVEL " + level;   
        mainText.gameObject.SetActive(true); 
        startButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        secondaryText.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);
    }

    void StartLevel() {
        mainText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        createPlayer();
        playing = true;
    }

    void createPlayer() {
        Vector3 playerPos = getPositionFromCoords(new int[]{1, -1});
        Rigidbody player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        mainCamera.player = player;
        createdObjects.Add(player.gameObject);
    }

    void createPlatforms(int mazeSideLength) {
        createStartPlatform(mazeSideLength);
        createTargetPlatform(mazeSideLength);
    }

    public void OnSuccess() {
        untilNextLevelTimer = 3.0f;
        playing = false;
        int score = (int)Mathf.Round(maxLevelTime - levelTimer) * 10;
        totalScore += score;

        // handle GUI
        mainText.text = "LEVEL " + level + " COMPLETE";
        secondaryText.text = "Level score: " + score;
        secondaryText.text = secondaryText.text + "\nTotal score: " + totalScore;
        mainText.gameObject.SetActive(true);
        secondaryText.gameObject.SetActive(true); 
        timerText.gameObject.SetActive(false);
    }

    public string FormatFloatToOnePlace(float f) {
        float rounded = Mathf.Round(f * 10.0f) / 10.0f;
        return string.Format("{0:0.0}", rounded);
    }

    public void OnGameOver() {
        // stop game
        playing = false;

        // handle GUI
        mainText.text = "LEVEL " + level + " FAILED";
        secondaryText.text = "Total score: " + totalScore;
        mainText.gameObject.SetActive(true);
        secondaryText.gameObject.SetActive(true); 
        timerText.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(true);
        
        // reset total time and level
        totalScore = 0;
        level = 1;
    }

    void createStartPlatform(int mazeSideLength) {
        // Start platform
        Vector3 startPlatformPositionXZ = getPositionFromFloatCoords(new float[]{1.0f, -2.0f});
        Vector3 startPlatformPosition = new Vector3(startPlatformPositionXZ.x, 0.0f, startPlatformPositionXZ.z);
        Transform startPlatform = Instantiate(floorPrefab, startPlatformPosition, Quaternion.identity);
        startPlatform.transform.localScale = new Vector3(3.0f, 1.0f, 3.0f);
        createdObjects.Add(startPlatform.gameObject);

        // TODO: refactor this and reuse
        Transform wall1 = Instantiate(startWallPrefab, getPositionFromFloatCoords(new float[]{-1.0f, -2.0f}), Quaternion.identity);
        wall1.transform.localScale = new Vector3(1.0f, 3.0f, 5.0f);
        walls.Add(wall1);
        Transform wall2 = Instantiate(startWallPrefab, getPositionFromFloatCoords(new float[]{1.5f, -4.0f}), Quaternion.identity);
        wall2.transform.localScale = new Vector3(4.0f, 3.0f, 1.0f);
        walls.Add(wall2);
        Transform wall3 = Instantiate(startWallPrefab, getPositionFromFloatCoords(new float[]{3.0f, -2.0f}), Quaternion.identity);
        wall3.transform.localScale = new Vector3(1.0f, 3.0f, 3.0f);
        walls.Add(wall3);
        createdObjects.Add(wall1.gameObject);
        createdObjects.Add(wall2.gameObject);
        createdObjects.Add(wall3.gameObject);
    }

    void createTargetPlatform(int mazeSideLength) {
        Vector3 targetPlatformPositionXZ = getPositionFromFloatCoords(new float[]{
            (float)mazeSideLength - 2.0f,
            (float)mazeSideLength + 1.0f});
        Vector3 targetPlatformPosition = new Vector3(targetPlatformPositionXZ.x, 0.0f, targetPlatformPositionXZ.z);
        Transform targetPlatform = Instantiate(floorPrefab, targetPlatformPosition, Quaternion.identity);
        targetPlatform.transform.localScale = new Vector3(3.0f, 1.0f, 3.0f);
        createdObjects.Add(targetPlatform.gameObject);

        Vector3 targetDetectorPosition = new Vector3(targetPlatformPositionXZ.x, 1.5f, targetPlatformPositionXZ.z);
        Transform targetDetector = Instantiate(targetDetectorPrefab, targetDetectorPosition, Quaternion.identity);
        TargetDetectorController tdController = targetDetector.GetComponent<TargetDetectorController>();
        tdController.parent = GetComponent<MazeGenerator>();
        createdObjects.Add(targetDetector.gameObject);

        // TODO: refactor this and reuse
        float mazeEnd = (float)mazeSideLength - 1.0f;
        Transform wall1 = Instantiate(targetWallPrefab, getPositionFromFloatCoords(new float[]{mazeEnd + 1.0f, mazeEnd + 2.0f}), Quaternion.identity);
        wall1.transform.localScale = new Vector3(1.0f, 3.0f, 5.0f);
        walls.Add(wall1);
        Transform wall2 = Instantiate(targetWallPrefab, getPositionFromFloatCoords(new float[]{mazeEnd - 1.5f, mazeEnd + 4.0f}), Quaternion.identity);
        wall2.transform.localScale = new Vector3(4.0f, 3.0f, 1.0f);
        walls.Add(wall2);
        Transform wall3 = Instantiate(targetWallPrefab, getPositionFromFloatCoords(new float[]{mazeEnd - 3.0f, mazeEnd + 2.0f}), Quaternion.identity);
        wall3.transform.localScale = new Vector3(1.0f, 3.0f, 3.0f);
        walls.Add(wall3);
        createdObjects.Add(wall1.gameObject);
        createdObjects.Add(wall2.gameObject);
        createdObjects.Add(wall3.gameObject);
    }

    void createFloor(float mazeSideLength) {
        Vector3 floorPos = new Vector3(mazeSideLength / 2f, 0.0f, mazeSideLength / 2f);
        Transform floor = Instantiate(floorPrefab, floorPos, Quaternion.identity);
        createdObjects.Add(floor.gameObject);
        floor.transform.localScale = new Vector3(mazeSideLength, 1.0f, mazeSideLength);
    }

    void createWalls(int mazeSideLength) {
        walls = new List<Transform>();
        int[][] enclosingSection = new int[][]{
            new int[] {0, 0},
            new int[] {mazeSideLength - 1, 0},
            new int[] {mazeSideLength - 1, mazeSideLength - 1},
            new int[] {0, mazeSideLength - 1},
        };
        int[][][] dividedSections;
        int[][] openings;
        (dividedSections, openings) = divideSectionRecursive(enclosingSection);
        List<int[]> openingsList = new List<int[]>(openings);
        openingsList.Add(new int[]{1, 0});
        openingsList.Add(new int[]{mazeSideLength - 2, mazeSideLength - 1});
        buildMaze(enclosingSection, dividedSections, openingsList);
    }

    void buildMaze(int[][] enclosingSection, int[][][] sections, List<int[]> openings) {
        int[][] border = getAllLocationsForSection(enclosingSection);
        List<int[]> buildLocations = new List<int[]>(border);
        for (int i = 0; i < sections.Length; i++) {
            int[][] sectionLocations = getAllLocationsForSection(sections[i]);
            buildLocations.AddRange(sectionLocations);
        }
        int skipped = 0;
        List<int[]> built = new List<int[]>();
        for (int i = 0; i < buildLocations.Count; i++) {
            int[] location = buildLocations[i];
            // only build if we haven't built there yet and the location isn't an opening
            if (!locationInList(built, location) && !locationInList(openings, location)) {
                buildWall(location);
                built.Add(location);
            } else {
                skipped++;
            }
        }
    }

    bool locationInList(List<int[]> list, int[] location) {
        for (int i = 0; i < list.Count; i++) {
            if (list[i][0] == location[0] && list[i][1] == location[1]) {
                return true;
            }
        }
        return false;
    }

    Vector3 getPositionFromCoords(int[] location) {
        return getPositionFromFloatCoords(new float[] {
            (float)location[0],
            (float)location[1]
        });
    }

    Vector3 getPositionFromFloatCoords(float[] location) {
        return new Vector3(
            location[0] + 0.5f,
            wallHeight,
            location[1] + 0.5f
        );
    }

    void buildWall(int[] location) {
        Vector3 pos = getPositionFromCoords(location);
        Transform wall = Instantiate(wallPrefab, pos, Quaternion.identity);
        wall.transform.localScale = new Vector3(1.0f, 3.0f, 1.0f);
        walls.Add(wall);
        createdObjects.Add(wall.gameObject);
    }

    Dictionary<string, int> getSectionBounds(int[][] section) {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        dict.Add("left", section[0][0]);
        dict.Add("right", section[1][0]);
        dict.Add("lower", section[0][1]);
        dict.Add("upper", section[2][1]);
        return dict;
    }

    bool canSubdivideSection(int[][] section) {
        /*
        Section must have room for a wall in either direction: there must be an
        even number between the bounds
        */
        Dictionary<string, int> bounds = getSectionBounds(section);
        return (
            bounds["right"] - bounds["left"] >= 4
            && bounds["upper"] - bounds["lower"] >= 4);
    }

    int randomEvenNumberBetweenEvens(int low, int high) {
        return low + 2 * rnd.Next(1, (high - low) / 2 - 1);
    }

    int randomOddNumberBetweenEvens(int low, int high) {
        return low + 1 + 2 * rnd.Next(0, (high - low) / 2 - 1);
    }

    (int[][][], int[][]) divideSection(int[][] section) {
        Dictionary<string, int> bounds = getSectionBounds(section);
        int verticalSplit = randomEvenNumberBetweenEvens(
            bounds["left"], bounds["right"]);
        int horizontalSplit = randomEvenNumberBetweenEvens(
            bounds["lower"], bounds["upper"]);
        int[][][] newSections = new int[][][] {
            new int[][] {
                new int[] {bounds["left"], bounds["lower"]},
                new int[] {verticalSplit, bounds["lower"]},
                new int[] {verticalSplit, horizontalSplit},
                new int[] {bounds["left"], horizontalSplit}
            },
            new int[][] {
                new int[] {verticalSplit, bounds["lower"]},
                new int[] {bounds["right"], bounds["lower"]},
                new int[] {bounds["right"], horizontalSplit},
                new int[] {verticalSplit, horizontalSplit}
            },
            new int[][] {
                new int[] {bounds["left"], horizontalSplit},
                new int[] {verticalSplit, horizontalSplit},
                new int[] {verticalSplit, bounds["upper"]},
                new int[] {bounds["left"], bounds["upper"]}
            },
            new int[][] {
                new int[] {verticalSplit, horizontalSplit},
                new int[] {bounds["right"], horizontalSplit},
                new int[] {bounds["right"], bounds["upper"]},
                new int[] {verticalSplit, bounds["upper"]}
            }
        };
        
        int[][] possibleOpenings = new int[][] {
            new int[] {
                randomOddNumberBetweenEvens(bounds["left"], verticalSplit),
                horizontalSplit
            },
            new int[] {
                randomOddNumberBetweenEvens(verticalSplit, bounds["right"]),
                horizontalSplit
            },
            new int[] {
                verticalSplit,
                randomOddNumberBetweenEvens(bounds["lower"], horizontalSplit)
            },
            new int[] {
                verticalSplit,
                randomOddNumberBetweenEvens(horizontalSplit, bounds["upper"])
            },
        };

        int deselectedIndex = rnd.Next(0, 3);
        List<int[]> possibleOpeningsList = new List<int[]>(possibleOpenings);
        possibleOpeningsList.RemoveAt(deselectedIndex);
        int[][] openings = possibleOpeningsList.ToArray();
        return (newSections, openings);
    }

    (int[][][], int[][]) divideSections(int[][][] sections) {
        List<int[][]> divided = new List<int[][]>();
        List<int[]> openings = new List<int[]>();
        for (int i = 0; i < sections.Length; i++) {
            int[][] section = sections[i];
            int[][][] newSections;
            if (canSubdivideSection(section)) {
                int[][] newOpenings;
                (newSections, newOpenings) = divideSection(section);
                openings.AddRange(newOpenings);
            } else {
                newSections = new int[][][] {section};
            }
            divided.AddRange(newSections);
        }
        return (divided.ToArray(), openings.ToArray());
    }

    bool doneSubdividing(int[][][] sections) {
        bool done = true;
        for (int i = 0; i < sections.Length; i++) {
            if (canSubdivideSection(sections[i])) {
                done = false;
                break;
            }
        }
        return done;
    }

    (int[][][], int[][]) divideSectionRecursive(int[][] section) {
        int[][][] divided = new int[][][]{section};
        List<int[]> openings = new List<int[]>();
        while (!doneSubdividing(divided)) {
            int[][] newOpenings;
            (divided, newOpenings) = divideSections(divided);
            openings.AddRange(newOpenings);
        }
        return (divided, openings.ToArray());
    }

    int[][] getAllLocationsForSection(int[][] section) {
        int[][] directions = new int[][] {
            new int[] {1, 0},
            new int[] {0, 1},
            new int[] {-1, 0},
            new int[] {0, -1}
        };
        int[] currentPos = section[0];
        List<int[]> buildLocations = new List<int[]>();
        Dictionary<string, int> bounds = getSectionBounds(section);
        for (int i = 0; i < directions.Length; i++) {
            int[] direction = directions[i];
            int sideLength;
            if (direction[0] != 0) {
                sideLength = bounds["right"] - bounds["left"];
            } else {
                sideLength = bounds["upper"] - bounds["lower"];
            }
                                
            for (int j = 0; j < sideLength; j++) {
                currentPos = new int[] {
                    currentPos[0] + direction[0],
                    currentPos[1] + direction[1]
                };
                buildLocations.Add(currentPos);
            }
        }
        return buildLocations.ToArray();
    }
}
