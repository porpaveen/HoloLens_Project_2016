using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

public class onLook : MonoBehaviour
{
    public static onLook Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    public bool isSit = false;
    float moveX = 0;
    float moveY = 0;
    float moveZ = 0;

    GameObject seat;

    Animator anim;
    int walk_F = Animator.StringToHash("WALK00_F");
    int walk_L = Animator.StringToHash("WALK00_L");
    int walk_R = Animator.StringToHash("WALK00_R");
    int wait00 = Animator.StringToHash("WAIT00");

    int turn;

    void Start()
    {
        seat = GameObject.Find("seat");
        anim = GetComponent<Animator>();

    }
  

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        if (!isSit)
        {
            anim.Play(walk_F);

            transform.LookAt(seat.transform);
            transform.Translate(seat.transform.position * Time.deltaTime * 0.5f);

            if(Vector3.Distance( seat.transform.position, this.transform.position) < 0.8f)
            {
                isSit = true;
                turn = walk(seat.transform.position - this.transform.position);
            }
            

        }
        else{
            // Do a raycast into the world based on the user's
            // head position and orientation.
            var headPosition = Camera.main.transform.position;
            Quaternion rotation = Quaternion.LookRotation(headPosition - this.transform.position);
            anim.Play(turn);
            if (turn != wait00 && Quaternion.Angle(transform.rotation, rotation) <= 5f )
                turn = wait00;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime);
        }
    }

    int walk(Vector3 vec)
    {
        if (vec.x > 0)
            return walk_R;
        else if (vec.x < 0)
            return walk_L;
        else
            return wait00;
    }
}