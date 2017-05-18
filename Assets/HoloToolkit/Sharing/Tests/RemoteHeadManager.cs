// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Broadcasts the head transform of the local user to other users in the session,
/// and adds and updates the head transforms of remote users.
/// Head transforms are sent and received in the local coordinate space of the GameObject
/// this component is on.
/// </summary>
public class RemoteHeadManager : Singleton<RemoteHeadManager>
{
    public class RemoteHeadInfo
    {
        public long UserID;
        public GameObject HeadObject;
    }

    /// <summary>
    /// Keep a list of the remote heads, indexed by XTools userID
    /// </summary>
    Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();
    public GameObject avatar;
    public GameObject lookPoint;
    public AnimationClip animW;
    public AnimationClip animWt;
    Animation animWait;
    Animation animWalk;

    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = this.UpdateHeadTransform;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.chess] = this.UpdateChessTransform;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.looking] = this.UpdatelookingTransform;
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
        SharingSessionTracker.Instance.SessionLeft += Instance_SessionLeft;
    }

    void Update()
    {
        // Grab the current head transform and broadcast it to all the other users in the session
        Transform headTransform = Camera.main.transform;

        // Transform the head position and rotation from world space into local space
        Vector3 headPosition = this.transform.InverseTransformPoint(headTransform.position);
        Quaternion headRotation = Quaternion.Inverse(this.transform.rotation) * headTransform.rotation;

        CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);
    }

    /// <summary>
    /// Called when an existing user leaves the session.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionLeft(object sender, SharingSessionTracker.SessionLeftEventArgs e)
    {
        if (e.exitingUserId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            RemoveRemoteHead(this.remoteHeads[e.exitingUserId].HeadObject);
            this.remoteHeads.Remove(e.exitingUserId);
        }
    }

    /// <summary>
    /// Called when a remote user is joins the session.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (e.joiningUser.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            GetRemoteHeadInfo(e.joiningUser.GetID());
        }
    }

    /// <summary>
    /// Gets the data structure for the remote users' head position.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public RemoteHeadInfo GetRemoteHeadInfo(long userID)
    {
        RemoteHeadInfo headInfo;

        // Get the head info if its already in the list, otherwise add it
        if (!this.remoteHeads.TryGetValue(userID, out headInfo))
        {
            headInfo = new RemoteHeadInfo();
            headInfo.UserID = userID;
            headInfo.HeadObject = CreateRemoteHead();

            this.remoteHeads.Add(userID, headInfo);
        }

        return headInfo;
    }

    /// <summary>
    /// Called when a remote user sends a head transform.
    /// </summary>
    /// <param name="msg"></param>
    void UpdateHeadTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

        Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);
        GameObject chessboard = GameObject.Find("Chessboard");


        RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);


        headPos = chessboard.transform.position;

        headPos.z = headPos.z + chessboard.transform.localScale.z + 0.5f;
        headPos.x = headPos.x + (headInfo.HeadObject.transform.transform.localScale.x * 0.25f);
        headPos.y = chessboard.transform.position.y - (headInfo.HeadObject.transform.transform.localScale.y * 0.25f);
        headInfo.HeadObject.transform.localPosition = headPos;
        headInfo.HeadObject.transform.localRotation = headRot;

    }

    /// <summary>
    /// Called when a remote user sends a chess transform.
    /// </summary>
    /// <param name="msg"></param>
    void UpdateChessTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        string name = CustomMessages.Instance.ReadString1(msg);

        updateOpponentChess(name);
    }

    void UpdatelookingTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        string name = CustomMessages.Instance.ReadString1(msg);

        updateLookingChess(name);
    }

    void updateLookingChess(string name)
    {
        GameObject to;
        if (GameObject.Find("lookingtochess") == null)
        {    
            to = Instantiate(lookPoint) as GameObject;
            to.transform.localRotation *= Quaternion.Euler(0, 180, 0);
            to.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            to.name = "lookingtochess";

            // to.AddComponent<Animation>();
            animWalk = to.GetComponent<Animation>();
            animWalk.wrapMode = WrapMode.Loop;
            animWalk.playAutomatically = true;
            animWalk.AddClip(animW, "W");
            animWalk.Play("W");

        }
        else
        {
            to = GameObject.Find("lookingtochess");
        }
        if(name == "")
        {
            // lookingPoint.transform.localScale = new Vector3(0, 0, 0);
            to.transform.position = avatar.transform.position;
        }
        else
        {
            GameObject lookingat = GameObject.Find(name);       
            to.transform.position = lookingat.transform.position;
        }

    }

    void updateOpponentChess(string name)
    {
        //		var text = new GameObject();
        //		var textmesh = text.AddComponent<TextMesh>();
        //		var meshrender = text.AddComponent<MeshRenderer>();
        //
        //		textmesh.text = name;
        //
        //		text.transform.Translate(0,0,5);
        //
        //		string[] splitString = textmesh.text.Split(' ');

        string[] splitString = name.Split(' ');

        GameObject token = GameObject.Find(splitString[0]);

        GameObject block = GameObject.Find(splitString[1]);

        int locXlast = token.GetComponent<tokenstruct>().locationX;
        int locYlast = token.GetComponent<tokenstruct>().locationY;

        GameObject.Find("block" + (locXlast) + "_" + (locYlast)).GetComponent<blockstruct>().isempty = true;

        Vector3 moveto = block.transform.position - token.transform.position;
        moveto.y = moveto.y + token.transform.localScale.y;
        token.transform.Translate(moveto);
        token.GetComponent<tokenstruct>().locationX = block.GetComponent<blockstruct>().locationX;
        token.GetComponent<tokenstruct>().locationY = block.GetComponent<blockstruct>().locationY;
        token.GetComponent<tokenstruct>().selected = false;
        block.GetComponent<blockstruct>().isempty = false;
    }


    /// <summary>
    /// Creates a new game object to represent the user's head.
    /// </summary>
    /// <returns></returns>
    GameObject CreateRemoteHead()
    {
        GameObject ava = (GameObject)Instantiate(avatar);
        ava.name = "HelloAvatar";
        // ava.AddComponent<Animation>();
        animWait = ava.GetComponent<Animation>();
        animWait.wrapMode = WrapMode.Loop;
        animWait.playAutomatically = true;
        animWait.AddClip(animWt, "W");
        animWait.Play("W");
        return ava;
    }

    /// <summary>
    /// When a user has left the session this will cleanup their
    /// head data.
    /// </summary>
    /// <param name="remoteHeadObject"></param>
    void RemoveRemoteHead(GameObject remoteHeadObject)
    {
        DestroyImmediate(remoteHeadObject);
    }
}