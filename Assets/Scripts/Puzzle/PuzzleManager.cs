using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public GameObject dollBoardPrefab;
    public GameObject partPrefab;
    public Sprite headSpr, torsoSpr, armLSpr, armRSpr, legLSpr, legRSpr;

    GameObject board;
    int placedCount;

    void Awake() { Instance = this; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            StartPuzzle();
    }

    void StartPuzzle()
    {
        if (board != null) return;
        placedCount = 0;

        board = Instantiate(dollBoardPrefab, Vector3.zero, Quaternion.identity);

        // Columnita a la derecha:
        Vector3 basePos = new Vector3(3f, 1.8f, 0f);
        SpawnPart("Head", headSpr, basePos + new Vector3(0, -0.0f, 0));
        SpawnPart("Torso", torsoSpr, basePos + new Vector3(0, -0.8f, 0));
        SpawnPart("ArmL", armLSpr, basePos + new Vector3(0, -1.6f, 0));
        SpawnPart("ArmR", armRSpr, basePos + new Vector3(0, -2.4f, 0));
        SpawnPart("LegL", legLSpr, basePos + new Vector3(0, -3.2f, 0));
        SpawnPart("LegR", legRSpr, basePos + new Vector3(0, -4.0f, 0));
    }

    void SpawnPart(string id, Sprite spr, Vector3 pos)
    {
        var go = Instantiate(partPrefab, pos, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = spr;
        var dp = go.GetComponent<DraggablePart>();
        dp.targetId = id;
    }

    public void NotifyPlaced()
    {
        placedCount++;
        if (placedCount >= 6)
        {
            // TODO: feedback + cerrar puzzle
            Debug.Log("PuzzleSolved");
        }
    }
}
