using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTest : MonoBehaviour {
    public AnimationClip anim0;
    Animation anim;
    GameObject a;
    // Use this for initialization
    void Start () {
        a = GameObject.Find("Cha_Knight");
        a.AddComponent<Animation>();
        anim = a.GetComponent<Animation>();
        anim.wrapMode = WrapMode.Loop;
        anim.playAutomatically = true;
        anim.AddClip(anim0, "W");
        anim.Play("W");
    }
	
	// Update is called once per frame
	void Update () {
	}
}
