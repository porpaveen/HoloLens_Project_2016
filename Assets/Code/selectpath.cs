using UnityEngine;
using System.Collections;
using HoloToolkit.Sharing;

public class selectpath : MonoBehaviour {

    private GameObject[] player;
    private GameObject[] path;

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // If the sphere has no Rigidbody component, add one to enable physics.
        if (this.tag == "Walkpath")
        {
            bool isPlayer = tokenselect.getIsPlayer(); 
            if(this.GetComponent<Renderer>().material.color == Color.green)
            {
                if(isPlayer)
                    player = GameObject.FindGameObjectsWithTag("Player");
                else
                    player = GameObject.FindGameObjectsWithTag("Opponent");
                path = GameObject.FindGameObjectsWithTag("Walkpath");
                int i = 0;
                while( i < player.Length)
                {
                    if (player[i].GetComponent<tokenstruct>().selected)
                        break;
                    i++;
                }
                if(isPlayer)
                    player[i].GetComponent<Renderer>().material.color = Color.red;
                else
                    player[i].GetComponent<Renderer>().material.color = Color.blue;


                Vector3 moveto = this.transform.position - player[i].transform.position;
                moveto.y = moveto.y + player[i].transform.localScale.y;
                player[i].transform.Translate(moveto);
                player[i].GetComponent<tokenstruct>().locationX = this.GetComponent<blockstruct>().locationX;
                player[i].GetComponent<tokenstruct>().locationY = this.GetComponent<blockstruct>().locationY;
                player[i].GetComponent<tokenstruct>().selected = false;

                for(int walk = 0; walk < path.Length; walk++)
                {
                    if(path[walk].GetComponent<blockstruct>().name == tokenselect.getLastTokenPosition() 
                        && path[walk].GetComponent<blockstruct>().name != this.GetComponent<blockstruct>().name)
                        path[walk].GetComponent<blockstruct>().isempty = true;
                    path[walk].GetComponent<Renderer>().material.color = Color.black;
                    if (path[walk].GetComponent<blockstruct>().name == "binP" || path[walk].GetComponent<blockstruct>().name == "binO")
                        path[walk].GetComponent<Renderer>().material.color = Color.white;
                }

                string thisblock = this.GetComponent<blockstruct>().name;

                this.GetComponent<blockstruct>().isempty = false;
//              tokenselect.setIsPlayer();
                gazeChessBoard.selected = false;

                updateOpponentChess( player[i].name , thisblock);

            }
        }
    }


    void updateOpponentChess( string name, string toblock)
    {
        name = name + " " + toblock;
        CustomMessages.Instance.SendChessTransform(name);
    }

}
