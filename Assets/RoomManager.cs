using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected");
        Debug.LogFormat("PUN2 client connected to region: {0}", PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("I am in the lobby");
    }

    public void Creatroom() {

        Photon.Realtime.RoomOptions roomops = new Photon.Realtime.RoomOptions() { IsVisible = true, MaxPlayers = 2, IsOpen = true };
        PhotonNetwork.CreateRoom("Room1", roomops);

    }
    public void JoinRandomRoom()
    {

        Debug.Log("Random room connecting");
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        Debug.Log("Ranadom room connection is failed");
        int randomRoomName = 1 ;
        Photon.Realtime.RoomOptions roomops = new Photon.Realtime.RoomOptions() { IsVisible = true, MaxPlayers = 2, IsOpen = true };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomops);
    }

    public void JoinSpecificRoom()
    {

        Debug.Log("Called JoinSpecific Room");
        PhotonNetwork.JoinRoom("Room1");


    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        PhotonNetwork.LoadLevel(1);
    }

    public void AIplayerMode() {

        SceneManager.LoadScene("VRChessAI");

    }


}
