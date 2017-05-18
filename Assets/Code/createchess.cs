using UnityEngine;
using System.Collections;

public class createchess : MonoBehaviour {

    private GameObject chess;
    public int ColumnLength;
    public int RowHeight;
    public float scale;
    private GameObject[,] chessArray;
    private GameObject checkertoken;
    public Vector3 positionXYZ;
    // Use this for initialization
    void Start()
    {
        chessArray = new GameObject[ColumnLength, RowHeight];
        this.transform.position = new Vector3(0, 0, 0);
        int playeri = 0;
        int opponenti = 0;
        
        for (int i = 0; i < ColumnLength; i++)
        {
            for (int j = 0; j < RowHeight; j++)
            {

                //chessArray[i, j] = (GameObject)Instantiate(chess, new Vector3(positionXYZ.x + (i * scale), positionXYZ.y, positionXYZ.z + (j * scale)), Quaternion.identity);

                chessArray[i, j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //chessArray[i, j] = (GameObject)Instantiate(chess);
                chessArray[i, j].transform.position = new Vector3((i * scale), 0, (j * scale));
                chessArray[i, j].transform.localScale = new Vector3(scale, scale * 0.1F, scale);
                chessArray[i, j].GetComponent<Renderer>().material.color = Color.white;
                chessArray[i, j].AddComponent<blockstruct>();
                chessArray[i, j].AddComponent<selectpath>(); 
                if ((i + j) % 2 == 0)
                {
                    chessArray[i, j].transform.localScale = new Vector3(scale, scale * 0.2F, scale);
                    chessArray[i, j].GetComponent<Renderer>().material.color = Color.black;
                    chessArray[i, j].tag = "Walkpath";

                }
                chessArray[i, j].name = "block" + i + "_" + j;
                chessArray[i, j].transform.parent = this.transform;
                chessArray[i, j].GetComponent<blockstruct>().name = chessArray[i,j].name;
                chessArray[i, j].GetComponent<blockstruct>().locationX = i;
                chessArray[i, j].GetComponent<blockstruct>().locationY = j;
                chessArray[i, j].GetComponent<blockstruct>().isempty = true;    

                if(j < 3)
                {
                    if((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                    {
                        checkertoken = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        checkertoken.transform.Translate(new Vector3((i * scale), (scale * 0.1F), (j * scale)));
                       // checkertoken = (GameObject)Instantiate(clone_token, new Vector3((i * scale), (scale * 0.1F), (j * scale)), Quaternion.identity);
                        checkertoken.transform.localScale = new Vector3(scale, scale * 0.1F, scale);
                        checkertoken.GetComponent<Renderer>().material.color = Color.red;
                        checkertoken.AddComponent<tokenstruct>();
                        checkertoken.name = "player" + playeri;
                        checkertoken.transform.parent = this.transform;
                        checkertoken.tag = "Player";
                        checkertoken.GetComponent<tokenstruct>().name = checkertoken.name;
                        checkertoken.GetComponent<tokenstruct>().locationX = i;                        ;
                        checkertoken.GetComponent<tokenstruct>().locationY = j;
                        checkertoken.GetComponent<tokenstruct>().selected= false;
                        checkertoken.GetComponent<tokenstruct>().forward = true;
                        checkertoken.GetComponent<tokenstruct>().player1 = true;
                        checkertoken.GetComponent<tokenstruct>().king = false;
                        chessArray[i, j].GetComponent<blockstruct>().isempty = false;
                        checkertoken.AddComponent<tokenselect>();
                        playeri++;
                    }
                }
                else if (j > RowHeight - 4)
                {
                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                    {
                        checkertoken = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        checkertoken.transform.Translate(new Vector3((i * scale), (scale * 0.1F), (j * scale)));

                        //checkertoken = (GameObject)Instantiate(clone_token, new Vector3((i * scale), (scale * 0.1F), (j * scale)), Quaternion.identity);
                        checkertoken.transform.localScale = new Vector3(scale, scale * 0.1F, scale);
                        checkertoken.GetComponent<Renderer>().material.color = Color.blue;
                        checkertoken.AddComponent<tokenstruct>();
                        checkertoken.name = "opponent" + opponenti;
                        checkertoken.transform.parent = this.transform;
                        checkertoken.tag = "Opponent";
                        checkertoken.GetComponent<tokenstruct>().name = checkertoken.name;
                        checkertoken.GetComponent<tokenstruct>().locationX = i;
                        checkertoken.GetComponent<tokenstruct>().locationY = j;
                        checkertoken.GetComponent<tokenstruct>().selected = false;
                        checkertoken.GetComponent<tokenstruct>().forward = false;
                        checkertoken.GetComponent<tokenstruct>().player1 = false;
                        checkertoken.GetComponent<tokenstruct>().king = false;
                        chessArray[i, j].GetComponent<blockstruct>().isempty = false;
                        checkertoken.AddComponent<tokenselect>();
                        opponenti++;
                    }
                }

            }

        }

        GameObject binP = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //chessArray[i, j] = (GameObject)Instantiate(chess);
        binP.transform.position = new Vector3((9 * scale), 0, (1 * scale));
        binP.transform.localScale = new Vector3(2 * scale, scale * 0.1F, 2 * scale);
        binP.GetComponent<Renderer>().material.color = Color.white;
        binP.AddComponent<blockstruct>();
        binP.AddComponent<selectpath>();
        binP.tag = "Walkpath";
        binP.name = "binP";
        binP.transform.parent = this.transform;
        binP.GetComponent<blockstruct>().name = binP.name;
        binP.GetComponent<blockstruct>().locationX = 0;
        binP.GetComponent<blockstruct>().locationY = 0;
        binP.GetComponent<blockstruct>().isempty = true;

        GameObject binO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //chessArray[i, j] = (GameObject)Instantiate(chess);
        binO.transform.position = new Vector3((-2 * scale), 0, (6 * scale));
        binO.transform.localScale = new Vector3(2 * scale, scale * 0.1F, 2 * scale);
        binO.GetComponent<Renderer>().material.color = Color.white;
        binO.AddComponent<blockstruct>();
        binO.AddComponent<selectpath>();
        binO.tag = "Walkpath";
        binO.name = "binO";
        binO.transform.parent = this.transform;
        binO.GetComponent<blockstruct>().name = binO.name;
        binO.GetComponent<blockstruct>().locationX = 0;
        binO.GetComponent<blockstruct>().locationY = 0;
        binO.GetComponent<blockstruct>().isempty = true;

        this.transform.position = new Vector3(positionXYZ.x - (scale * 3.5f), positionXYZ.y, positionXYZ.z);

    }

}
