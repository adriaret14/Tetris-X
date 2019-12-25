using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { LEFT, RIGHT, DOWN }

public class GridController : MonoBehaviour
{

    [Header("Grid Composure")]
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeY;

    [Header("Cells Composure")]
    [SerializeField] private int cellSize;

    [Header("GameObjects")]
    [SerializeField] private GridClass gridClass;
    [SerializeField] private GameObject placeHoldersParent;

    [Header("Pieces")]
    [SerializeField] private List<GameObject> piecePrefabs=new List<GameObject>();
    [SerializeField] private GameObject placeHolder;

    [Header("Game Settings")]
    [SerializeField] private int currentLevel;
    [SerializeField] private float currentMoveTime;
    private float lastTimeMovedStandard;
    public GameObject currentPiece;



    // Start is called before the first frame update
    void Start()
    {
        gridClass=new GridClass(sizeX, sizeY, null, piecePrefabs, placeHolder);


        gridClass.DrawTransLucidGrid(placeHoldersParent);
        SpawnNewPiece();
        lastTimeMovedStandard = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputsPlayer();
        CheckForStandardMove();
    }
    private void CheckForStandardMove()
    {
        if(lastTimeMovedStandard+currentMoveTime<=Time.time)
        {
            if(gridClass.MovePiece(ref currentPiece, Direction.DOWN))
            {
                SpawnNewPiece();
            }

            lastTimeMovedStandard = Time.time;
        }
    }


    private void SpawnNewPiece(/*PieceType t, int r, Vector2Int p*/)
    {
        int n = Random.Range(0, piecePrefabs.Count);

        currentPiece=gridClass.InsertPiece(piecePrefabs[n].GetComponent<Piece>().pType, 0, new Vector2Int(5, 2));
    }

    private void CheckInputsPlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetButtonDown("Right"))
        {
            //Right
            Debug.Log("Mover Pieza Derecha");
            gridClass.MovePiece(ref currentPiece, Direction.RIGHT);
        }
        else if(Input.GetButtonDown("Left"))
        {
            //Left
            Debug.Log("Mover Pieza Izquierda");
            gridClass.MovePiece(ref currentPiece, Direction.LEFT);
        }

        if(Input.GetButtonDown("Down"))
        {
            //Accelerate
            Debug.Log("Acelerar Pieza");
        }

        if(Input.GetButtonDown("Jump"))
        {
            //InstaDrop
            Debug.Log("Drop Instantaneo");
        }
    }
}
