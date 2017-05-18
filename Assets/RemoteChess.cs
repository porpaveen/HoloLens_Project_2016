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
public class RemoteChess : Singleton<RemoteChess>
{
    public class RemoteChessInfo
    {
        public long UserID;
        public GameObject HeadObject;
    }

    /// <summary>
    /// Keep a list of the remote heads, indexed by XTools userID
    /// </summary>
    Dictionary<long, RemoteChessInfo> remoteHeads = new Dictionary<long, RemoteChessInfo>();

    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.chess] = this.UpdateChessTransform;

        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
        SharingSessionTracker.Instance.SessionLeft += Instance_SessionLeft;
    }

    void Selected(Vector3 vec3pos, Quaternion rotation)
    {
        CustomMessages.Instance.SendHeadTransform(vec3pos, rotation);
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
            GetRemoteChessInfo(e.joiningUser.GetID());
        }
    }

    /// <summary>
    /// Gets the data structure for the remote users' head position.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public RemoteChessInfo GetRemoteChessInfo(long userID)
    {
        RemoteChessInfo chessInfo;

        // Get the head info if its already in the list, otherwise add it
        if (!this.remoteHeads.TryGetValue(userID, out chessInfo))
        {
            chessInfo = new RemoteChessInfo();
            chessInfo.UserID = userID;
            chessInfo.HeadObject = CreateRemoteHead();

            this.remoteHeads.Add(userID, chessInfo);
        }

        return chessInfo;
    }

    /// <summary>
    /// Called when a remote user sends a head transform.
    /// </summary>
    /// <param name="msg"></param>
    void UpdateChessTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

        Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

        RemoteChessInfo headInfo = GetRemoteChessInfo(userID);
        headInfo.HeadObject.transform.localPosition = headPos;
        headInfo.HeadObject.transform.localRotation = headRot;
    }

    /// <summary>
    /// Creates a new game object to represent the user's head.
    /// </summary>
    /// <returns></returns>
    GameObject CreateRemoteHead()
    {
        //GameObject newHeadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject newHeadObj = this.gameObject;
        return newHeadObj;
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