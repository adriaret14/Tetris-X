using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private List<Vector2> speedSettings=new List<Vector2>();
    [SerializeField] private int speedSetCont;
    [SerializeField] private int currentLevel;
    [SerializeField] private int currentScore;
    [SerializeField] private int currentLineCount;
    [SerializeField] private float currentMoveTime;
    [SerializeField] private float auxCurrentMoveTime;
    [SerializeField] private int contAccelerate;
    private float lastTimeMovedStandard;
    public GameObject currentPiece;
    public GameObject nextPiece;

    [Header("Game UI")]
    [SerializeField] private Text _fLevel;
    [SerializeField] private Text _fScore;
    [SerializeField] private Text _fLines;


    // Start is called before the first frame update
    void Start()
    {
        gridClass=new GridClass(sizeX, sizeY, null, piecePrefabs, placeHolder);

        currentScore = 0;
        currentLineCount = 0;
        speedSetCont = 0;
        currentLevel = (int)speedSettings[speedSetCont].x;
        currentMoveTime = speedSettings[speedSetCont].y;

        _fScore.text = currentScore.ToString();
        _fLines.text = currentLineCount.ToString();
        _fLevel.text = currentLevel.ToString();

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
                int auxScore = gridClass.CheckLine(ref currentPiece);
                currentLineCount += auxScore;
                switch(auxScore)
                {
                    case 0:
                        
                        break;
                    case 1:
                        currentScore += 40 * currentLevel;
                        break;
                    case 2:
                        currentScore += 80 * currentLevel;
                        break;
                    case 3:
                        currentScore += 120 * currentLevel;
                        break;
                    case 4:
                        currentScore += 400 * currentLevel;
                        break;
                }
                if(currentLineCount >= (currentLevel * 10) + 10 && (currentLevel>=1 && currentLevel<19))
                {
                    speedSetCont++;
                    currentLineCount = 0;
                    currentLevel = (int)speedSettings[speedSetCont].x;
                    currentMoveTime = speedSettings[speedSetCont].y;
                }
                SpawnNewPiece();
            }

            _fScore.text = currentScore.ToString();
            _fLines.text = currentLineCount.ToString();
            _fLevel.text = currentLevel.ToString();

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
            //Debug.Log("Mover Pieza Derecha");
            gridClass.MovePiece(ref currentPiece, Direction.RIGHT);
        }
        else if(Input.GetButtonDown("Left"))
        {
            //Left
            //Debug.Log("Mover Pieza Izquierda");
            gridClass.MovePiece(ref currentPiece, Direction.LEFT);
        }

        if(Input.GetButton("Down"))
        {
            //Accelerate
            //Debug.Log("Acelerar Pieza");
            if(contAccelerate<=0)
            {
                contAccelerate++;
                auxCurrentMoveTime = currentMoveTime;
                currentMoveTime /= 2;
            }
        }

        if(Input.GetButtonUp("Down"))
        {
            contAccelerate = 0;
            if(auxCurrentMoveTime!=0)
            {
                currentMoveTime = auxCurrentMoveTime;
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            //InstaDrop
            //Debug.Log("Drop Instantaneo");
            //gridClass.InstaDropPiece(ref currentPiece);
        }

        if(Input.GetButtonDown("Rotate"))
        {
            //Rotate piece
            gridClass.RotatePiece(ref currentPiece);
        }
    }
}
