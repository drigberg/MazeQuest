using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGenerator : MonoBehaviour
{
    public Rigidbody floorPrefab;
    public Rigidbody wallPrefab;
    public List<Rigidbody> walls;
    public float wallHeight = 2.0f;
    private System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        int mazeSideLength = 15;
        Rigidbody floor = Instantiate(floorPrefab);
        walls = new List<Rigidbody>();
        floor.transform.localScale = new Vector3((float)(mazeSideLength + 1), 1.0f, (float)(mazeSideLength + 1));
        floor.transform.position = new Vector3((float)(mazeSideLength + 1) / 2f, 0.0f, (float)(mazeSideLength + 1) / 2f);
        int[][] enclosingSection = new int[][]{
            new int[] {0, 0},
            new int[] {mazeSideLength, 0},
            new int[] {mazeSideLength, mazeSideLength},
            new int[] {0, mazeSideLength},
        };
        int[][][] dividedSections;
        int[][] openings;
        (dividedSections, openings) = divideSectionRecursive(enclosingSection);
        List<int[]> openingsList = new List<int[]>(openings);
        openingsList.Add(new int[]{1, 0});
        openingsList.Add(new int[]{mazeSideLength - 1, mazeSideLength});
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
        return new Vector3(
            (float)location[0] + 0.5f,
            wallHeight,
            (float)location[1] + 0.5f
        );
    }

    void buildWall(int[] location) {
        Vector3 pos = getPositionFromCoords(location);
        Rigidbody wall = Instantiate(wallPrefab);
        wall.transform.position = pos;
        wall.transform.localScale = new Vector3(1.0f, 3.0f, 1.0f);
        walls.Add(wall);
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
