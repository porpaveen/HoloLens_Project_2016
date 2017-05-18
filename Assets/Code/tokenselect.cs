using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tokenselect : MonoBehaviour {
    private GameObject[] path;
    private GameObject[] player,opponent;
    private static bool isPlayer = true;
    private static string lastTokenPos = "";

    public static bool getIsPlayer()
    {
        return isPlayer;
    }

    public static string getLastTokenPosition()
    {
        return lastTokenPos;
    }

    private string checkcross(GameObject[] token, string loc, int locX, int locY, bool forward, bool jumpLeft)
    {
        int locYY = 0;
        int locXX = 0;
        if (forward)
        {
            locYY = locY + 1;
        }
        else
        {
            locYY = locY - 1;
        }
        if (jumpLeft)
            locXX = locX - 1;
        else
            locXX = locX + 1;
        if(loc != "no")
            loc = "block" + (locXX) + "_" + (locYY);
        for (int i = token.Length - 1; i >= 0; i--)
        {
            if (token[i])
            {
                if (token[i].GetComponent<tokenstruct>().locationY == locYY)
                {

                    if (token[i].GetComponent<tokenstruct>().locationX == locXX)
                    {
                        loc = "no";
                    }
                }
            }
        }

        return loc;
    }

    private string[] getcheck(GameObject[] token, string loc1, string loc2, int locX, int locY, bool p, bool forward )
    {
        int locYY = 0;

        if(forward)
            locYY = locY + 1;
        else
            locYY = locY - 1;
        if (loc1 == "" && loc2 == "")
        {
            loc1 = "block" + (locX + 1) + "_" + (locYY);
            loc2 = "block" + (locX - 1) + "_" + (locYY);
        }
        for (int i = token.Length - 1; i >= 0; i--)
        {
            if (token[i])
            {
                if (token[i].GetComponent<tokenstruct>().locationY == locYY)
                {
                    if (token[i].GetComponent<tokenstruct>().locationX == locX + 1)
                    {

                        loc1 = "no move";
                        if( token[i].GetComponent<tokenstruct>().player1 != p)
                        {
                            loc1 = checkcross(GameObject.FindGameObjectsWithTag("Opponent"), loc1, locX + 1, locYY, forward, false);
                            loc1 = checkcross(GameObject.FindGameObjectsWithTag("Player"), loc1, locX + 1, locYY, forward, false);
                        }
                    }
                    if (token[i].GetComponent<tokenstruct>().locationX == locX - 1)
                    {
                        loc2 = "no move";
                        if (token[i].GetComponent<tokenstruct>().player1 != p)
                        {
                            loc2 = checkcross(GameObject.FindGameObjectsWithTag("Opponent"), loc2, locX - 1, locYY, forward, true);
                            loc2 = checkcross(GameObject.FindGameObjectsWithTag("Player"), loc2, locX - 1, locYY, forward, true);
                        }
                    }
                }
                token[i].GetComponent<tokenstruct>().selected = false;
               
                if (token[i].GetComponent<tokenstruct>().player1)
                    token[i].GetComponent<Renderer>().material.color = Color.red;
                else
                    token[i].GetComponent<Renderer>().material.color = Color.blue;
            }
        }

        return new string[] { loc1, loc2 };
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // If the sphere has no Rigidbody component, add one to enable physics.
        if (this.tag == "Player" || this.tag == "Opponent")
        {
            //           if (!isPlayer)
            //               return;
            isPlayer = this.GetComponent<tokenstruct>().player1;
            int locX = this.GetComponent<tokenstruct>().locationX;
            int locY = this.GetComponent<tokenstruct>().locationY;
            path = GameObject.FindGameObjectsWithTag("Walkpath");
            player = GameObject.FindGameObjectsWithTag("Player");
            opponent = GameObject.FindGameObjectsWithTag("Opponent");
            lastTokenPos = "block" + (locX) + "_" + (locY);
            string locname1 = ""; // "block" + (locX+1) + "_" + (locY+1);
            string locname2 = ""; //"block" + (locX-1) + "_" + (locY+1);
            string bin = "";
            if (this.tag == "Player")
            {
                if (this.GetComponent<tokenstruct>().locationY == 7)
                {
                    this.GetComponent<tokenstruct>().king = true;
                }
                bin = "binP";
            }
            else
            {
                if (this.GetComponent<tokenstruct>().locationY == 0)
                {
                    this.GetComponent<tokenstruct>().king = true;
                }
                bin = "binO";
            }
            bool farward = this.GetComponent<tokenstruct>().forward;
            string[] temploc1 = getcheck(player, locname1, locname2, locX, locY, isPlayer, farward);
            string[] temploc2 = getcheck(opponent, temploc1[0], temploc1[1], locX, locY, isPlayer, farward);

            locname1 = temploc2[0];
            locname2 = temploc2[1];

            this.GetComponent<Renderer>().material.color = Color.grey;
            for (int i = 0; i < path.Length; i++)
            {
                if (this.GetComponent<tokenstruct>().king)
                {
                    if (path[i].GetComponent<blockstruct>().isempty) {
                        int blocX = path[i].GetComponent<blockstruct>().locationX;
                        int blocY = path[i].GetComponent<blockstruct>().locationY;
                        if( (Mathf.Abs(blocX - locX) == Mathf.Abs(blocY - locY)) && path[i].GetComponent<blockstruct>().isempty)
                        {
                            gazeChessBoard.selected = true;
                            path[i].GetComponent<Renderer>().material.color = Color.green;
                            this.GetComponent<tokenstruct>().selected = true;
                        }
                        else if (path[i].name == bin)
                        {
                            gazeChessBoard.selected = true;
                            path[i].GetComponent<Renderer>().material.color = Color.green;
                            this.GetComponent<tokenstruct>().selected = true;
                        }
                        else
                        {
                            path[i].GetComponent<Renderer>().material.color = Color.black;
                        }
                    }
                }
                else if (path[i].name == locname1 || path[i].name == locname2)
                {
                    gazeChessBoard.selected = true;
                    path[i].GetComponent<Renderer>().material.color = Color.green;
                    this.GetComponent<tokenstruct>().selected = true;
                }
                else if (path[i].name == bin) {
                    gazeChessBoard.selected = true;
                    path[i].GetComponent<Renderer>().material.color = Color.green;
                    this.GetComponent<tokenstruct>().selected = true;
                }
                else
                {
                    path[i].GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }
    }
}
