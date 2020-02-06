using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceType pType;           //Piece Type
    public int r;                   //Rotation
    public Vector2Int pos0;         //Center
    public Vector2Int pos1;         
    public Vector2Int pos2;
    public Vector2Int pos3;

    public void CheckIfNeedAutodestroy()
    {
        //Debug.Log("Child 0 name : " + transform.GetChild(0).name);
        //Debug.Log("Child count: " + transform.GetChild(0).transform.childCount);
        if (transform.GetChild(0).transform.childCount <= 0)
        {
            //Debug.Log("Destruyendo : " + this.gameObject.name);
            Destroy(this.gameObject);
        }
    }
}
