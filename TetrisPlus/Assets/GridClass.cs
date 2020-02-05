using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { L, L_INVERTED, S, S_INVERTED, BOX, BAR, T };

[System.Serializable]
public class GridClass
{
    [System.Serializable]
    public class Row
    {
        public List<GameObject> row;

        public Row()
        {
            row = new List<GameObject>();
        }
        
    }

    public List<Row> grid;
    private List<GameObject> piecePrefabs;
    private GameObject cubeTranslucidForDraw;

    public GridClass()
    {
        grid = new List<Row>();
    }

    public GridClass(int x, int y, GameObject g, List<GameObject> pP, GameObject placeHolder)
    {
        grid = new List<Row>();
        piecePrefabs = pP;
        cubeTranslucidForDraw = placeHolder;

        for(int i=0; i<y; i++)
        {
            Row r = new Row();
            for(int j=0; j<x; j++)
            {
                r.row.Add(g);

            }
            grid.Add(r);
        }
    }

    public GameObject InsertPiece(PieceType t, int r, Vector2Int p)
    {
        GameObject pieceActive;

        foreach (GameObject go in piecePrefabs)
        {
            if(go.GetComponent<Piece>().pType==t)
            {
                pieceActive = GameObject.Instantiate(go, new Vector3(p.x, -p.y, 0), Quaternion.identity);
                //Insertar la pieza en el grid
                for(int i=0; i< pieceActive.transform.GetChild(0).transform.childCount; i++)
                {
                    grid[Mathf.Abs((int)pieceActive.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)pieceActive.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = pieceActive.transform.GetChild(0).transform.GetChild(i).gameObject;
                }

                return pieceActive;
            }
        }

        return null;
    }

    public bool MovePiece(ref GameObject p, Direction d)
    {
        bool stackPiece = false;
        switch(d)
        {
            case Direction.LEFT:
                if (CheckCollision(ref p, d))
                {
                    //Primero vaciar las posiciones anteriores
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = null;
                    }
                    //Mover la pieza
                    p.transform.position += new Vector3(-1, 0, 0);
                    //Luego reasignar posiciones en el grid
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = p.transform.GetChild(0).transform.GetChild(i).gameObject;
                    }
                }
                break;
            case Direction.RIGHT:
                if (CheckCollision(ref p, d))
                {
                    //Primero vaciar las posiciones anteriores
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = null;
                    }
                    //Mover la pieza
                    p.transform.position += new Vector3(1, 0, 0);
                    //Luego reasignar posiciones en el grid
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = p.transform.GetChild(0).transform.GetChild(i).gameObject;
                    }
                }
                break;
            case Direction.DOWN:

                if(CheckCollision(ref p, d))
                {
                    //Primero vaciar las posiciones anteriores
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = null;
                    }
                    //Mover la pieza
                    p.transform.position += new Vector3(0, -1, 0);
                    //Luego reasignar posiciones en el grid
                    for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                    {
                        grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = p.transform.GetChild(0).transform.GetChild(i).gameObject;
                    }
                }
                else
                {
                    stackPiece = true;
                }
                break;
        }

        return stackPiece;
    }

    public void InstaDropPiece(ref GameObject p) //se queda monger, hay que revisar que no se salga por debajo de la grid  y seguir mirando
    {
        bool canGoDown = true;
        bool auxFlag = true;
        int linesDown = 0;

        while (canGoDown)
        {
            for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
            {
                auxFlag = true;
                if(grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y - 1)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] != null)
                {
                    auxFlag = false;
                }
            }

            if(!auxFlag)
            {
                canGoDown = false;
            }
            else
            {
                linesDown++;
            }
        }
        
    }

    public void RotatePiece(ref GameObject p)
    {
        Piece piece = p.GetComponent<Piece>();
        switch (piece.pType)
        {
            case PieceType.BAR:
                if(piece.r==0 || piece.r==2)
                {
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else
                {
                   if(CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
            case PieceType.BOX:

                break;
            case PieceType.L:
                if (piece.r == 0)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else if (piece.r == 1)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 2))
                    {
                        ApplyRotation(ref p, 2);
                    }
                }
                else if (piece.r == 2)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 3))
                    {
                        ApplyRotation(ref p, 3);
                    }
                }
                else
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
            case PieceType.L_INVERTED:
                if (piece.r == 0)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else if (piece.r == 1)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 2))
                    {
                        ApplyRotation(ref p, 2);
                    }
                }
                else if (piece.r == 2)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 3))
                    {
                        ApplyRotation(ref p, 3);
                    }
                }
                else
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
            case PieceType.S:
                if (piece.r == 0 || piece.r == 2)
                {
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else
                {
                    if (CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
            case PieceType.S_INVERTED:
                if (piece.r == 0)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else if (piece.r == 1)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 2))
                    {
                        ApplyRotation(ref p, 2);
                    }
                }
                else if (piece.r == 2)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 3))
                    {
                        ApplyRotation(ref p, 3);
                    }
                }
                else
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
            case PieceType.T:
                if (piece.r == 0)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 1))
                    {
                        ApplyRotation(ref p, 1);
                    }
                }
                else if(piece.r == 1)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 2))
                    {
                        ApplyRotation(ref p, 2);
                    }
                }
                else if(piece.r == 2)
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 3))
                    {
                        ApplyRotation(ref p, 3);
                    }
                }
                else
                {
                    Debug.Log(CanRotatePiece(p, 1));
                    if (CanRotatePiece(p, 0))
                    {
                        ApplyRotation(ref p, 0);
                    }
                }
                break;
        }
    }

    private bool CanRotatePiece(GameObject p, int futureRot)
    {
        bool canRotate = true;

        for(int i=1; i<p.transform.GetChild(0).transform.childCount; i++)
        {
            if((Mathf.Abs((int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.y + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].y) < grid.Count) && (((int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.x + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].x >= 0) && (int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.x + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].x < grid[0].row.Count))
            {
                if (grid[Mathf.Abs((int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.y + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].y)].row[(int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.x + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].x] != null)
                {
                    if (grid[Mathf.Abs((int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.y + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].y)].row[(int)p./*transform.GetChild(0).transform.GetChild(0).*/transform.position.x + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].x].transform.parent != p.transform.GetChild(0))
                    {
                        canRotate = false;
                    }
                }
            }
            else
            {
                canRotate = false;
            }
            
        }

        return canRotate;
    }

    private void ApplyRotation(ref GameObject p, int futureRot)
    {
        for(int i=0; i< p.transform.GetChild(0).transform.childCount; i++)
        {
            grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] = null;
            p.transform.GetChild(0).transform.GetChild(i).transform.position = new Vector3(p.transform.GetChild(0).transform.GetChild(0).transform.position.x + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].x, p.transform.GetChild(0).transform.GetChild(0).transform.position.y + (int)p.transform.GetChild(0).transform.GetChild(i).GetComponent<RotationInfo>().rotations[futureRot].y, 0);
            //grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[(int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x] = p.transform.GetChild(0).transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
        {
            grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[(int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x] = p.transform.GetChild(0).transform.GetChild(i).gameObject;
        }

        p.GetComponent<Piece>().r = futureRot;
    }

    public bool CheckCollision(ref GameObject p, Direction d)
    {
        bool canMove = true;

        switch(d)
        {
            case Direction.LEFT:
                for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                {
                    if (0 <= (int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x - 1)
                    {
                        if (grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x - 1)] != null)
                        {
                            if (grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x - 1)].transform.parent != p.transform.GetChild(0))
                            {
                                canMove = false;
                            }
                        }
                    }
                    else
                    {
                        canMove = false;
                    }

                }
                break;
            case Direction.RIGHT:
                for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                {
                    if (grid[0].row.Count > Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x + 1))
                    {
                        if (grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x + 1)] != null)
                        {
                            if (grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x + 1)].transform.parent != p.transform.GetChild(0))
                            {
                                canMove = false;
                            }
                        }
                    }
                    else
                    {
                        canMove = false;
                    }

                }
                break;
            case Direction.DOWN:
                for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
                {
                    if(grid.Count>Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y - 1))
                    {
                        if (grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y - 1)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)] != null)
                        {
                            if(grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y - 1)].row[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.x)].transform.parent != p.transform.GetChild(0))
                            {
                                canMove = false;
                            }
                        }
                    }
                    else
                    {
                        canMove = false;
                    }
                    
                }
                break;
        }

        return canMove;
    }

    public void CheckLine(ref GameObject p)
    {
        List<int> linesDone = new List<int>();

        Debug.Log("Revisando si se ha hecho una linea");
        for (int i = 0; i < p.transform.GetChild(0).transform.childCount; i++)
        {
            bool lineDone = true;
            for (int j = 0; j < grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row.Count; j++)
            {
                if(grid[Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)].row[j]==null)
                {
                    lineDone = false;
                }
            }

            if(lineDone)
            {
                //Debug.Log("Line: " + Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y) + " done!!");
                if(!linesDone.Contains(Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y)))
                {
                    linesDone.Add(Mathf.Abs((int)p.transform.GetChild(0).transform.GetChild(i).transform.position.y));
                }   
            }
        }

        for(int i=0; i<linesDone.Count; i++)
        {
            Debug.Log("Line " + i + " done");
        }

        if(linesDone.Count>0)
        {
            DestroyLines(linesDone, p);
        }
        
    }

    private void DestroyLines(List<int> lines, GameObject p)
    {
        //Debug.Log("A destruir lineas");
        for(int i=0; i<lines.Count; i++)
        {
            int c = grid[lines[i]].row.Count;
            for (int j=0; j<grid[lines[i]].row.Count; j++)
            {
                GameObject.Destroy(grid[lines[i]].row[j]);
            }
           
            grid[lines[i]].row=new List<GameObject>();
            for(int x=0; x<c; x++)
            {
                grid[lines[i]].row.Add(null);
            }
        }

        if(lines.Count>0)
        {
            MoveDownFullStack(lines, p);
        }
        
    }

    private void MoveDownFullStack(List<int> l, GameObject p) //Con más de triple y tetris la ultima se queda vacia pero no bajan del todo //Mirar de bajar directamente con el count
    {
        l.Sort();
        for(int i=l.Count-1; i>=0; i--)
        {
            for(int x=l[i]; x>=0; x--)
            {
                for (int j = 0; j < grid[x].row.Count; j++)
                {
                    if (grid[x].row[j] != null && grid[x].row[j].gameObject.transform.parent.transform.parent != p)
                    {
                        grid[Mathf.Abs((int)grid[x].row[j].gameObject.transform.position.y - 1)].row[j] = grid[x].row[j];
                        grid[x].row[j].gameObject.transform.position -= new Vector3(0.0f, 1.0f, 0.0f);
                        grid[x].row[j] = null;
                    }
                }
            }
        }
    }

    public void DrawTransLucidGrid(GameObject placeHoldersParent)
    {
        for(int i=0; i<grid.Count; i++)
        {
            for(int j=0;j<grid[i].row.Count; j++)
            {
                GameObject go=GameObject.Instantiate(cubeTranslucidForDraw, new Vector3(j, -i, 0), Quaternion.identity);
                go.transform.parent = placeHoldersParent.transform;
            }
        }
    }
   
}
