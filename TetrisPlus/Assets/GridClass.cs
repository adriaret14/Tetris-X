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
                break;
            case Direction.RIGHT:
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

    public void InstaDropPiece(ref GameObject p)
    {

    }

    public void RotatePiece(ref GameObject p)
    {

    }

    public void AcceleratePiece(ref GameObject p)
    {

    }

    public bool CheckCollision(ref GameObject p, Direction d)
    {
        bool canMove = true;

        switch(d)
        {
            case Direction.LEFT:

                break;
            case Direction.RIGHT:

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
