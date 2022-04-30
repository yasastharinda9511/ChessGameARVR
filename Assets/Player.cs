using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject trackingHand;
    [SerializeField]
    GameObject XRRig;

    [SerializeField]
    GameObject whitePlayerPos;
    [SerializeField]
    GameObject blackPlayerPos;

    const int delayConstant = 5;
    int delayValue = 0;

    public static Player Instance;
    public PlayerColor player { get; set; }

    RaycastHit hit;

    public GameObject SelectedObject { get; set; }

    int goToIndex;

    UnityEngine.XR.InputDevice device;
    bool triggerValue;
    bool collide;

    GameObject prevHit;

    GameObject defaultGameObject;
    BOARDSTATUS prevStatus;
    private void Awake()
    {

        if (Instance == null) Instance = this;

    }

    void Start()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {

            player = PlayerColor.WHITE;
            XRRig.transform.position = whitePlayerPos.transform.position;
            Debug.Log("Initializing game manager");
            PhotonNetwork.Instantiate("GameManager", new Vector3(0, 0, 0), Quaternion.identity);

        }
        else {

            player = PlayerColor.BLACK;
            XRRig.transform.position = blackPlayerPos.transform.position;

        }

       
        Board.Instance.OrganizeBoard();
        this.GetInputDevice();

        SelectedObject = GameObject.Find("Board");
        defaultGameObject = SelectedObject;

        
    }

    // Update is called once per frame
    void Update()
    {

        collide = trackingHand.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out hit);

        

        if (GameManagerMultiplayer.Instance != null &&
            player == PlayerColor.WHITE &&
            (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK))
        {
            if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) && !collide)
            {

                GameManagerMultiplayer.Instance.RPC_ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject != null &&
                collide && hit.collider.transform.gameObject.name == SelectedObject.name &&
                hit.collider.gameObject.GetComponent<Piece>() != null &&
                hit.collider.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE)
            {
                delayValue = 0;
                GameManagerMultiplayer.Instance.RPC_ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT);

            }

            else if (collide &&
            SelectedObject.name != hit.collider.transform.gameObject.name &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE)
            {

                GameManagerMultiplayer.Instance.RPC_PieceColorChange(hit.collider.transform.gameObject.name);

            }
        }

        else if (GameManagerMultiplayer.Instance != null && GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT && player == PlayerColor.WHITE)
        {
            if (collide &&
                hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                (delayValue <= delayConstant))
            {
                Debug.Log("Delay value is :" + delayValue);
                delayValue ++;
            
            
            }
            else if (collide &&
                hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                (delayValue > delayConstant))
            {

                delayValue = 0;
                goToIndex = hit.collider.transform.gameObject.GetComponent<ValidBox>().ValidIndex;
                GameManagerMultiplayer.Instance.RPC_PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {
                    //UIManager.Instance.UpdateBoardStatusBanner("WhitePawnMove");
                    GameManagerMultiplayer.Instance.IsWhitePawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerMultiplayer.Instance.IsCheckBlackPlayer();
                    player = PlayerColor.BLACK;

                }

            }

            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK &&
            SelectedObject.gameObject.GetComponent<Piece>().CalculateValidMoves().Find(x => x.Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index).Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index &&
            (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            )
            {

                goToIndex = hit.collider.transform.gameObject.GetComponent<Piece>().Index;
                GameManagerMultiplayer.Instance.RPC_PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {
                    //UIManager.Instance.UpdateBoardStatusBanner("WhitePawnMove");
                    GameManagerMultiplayer.Instance.IsWhitePawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerMultiplayer.Instance.IsCheckBlackPlayer();
                    player = PlayerColor.BLACK;

                }


            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject.name != hit.collider.gameObject.name)
            {

                GameManagerMultiplayer.Instance.RPC_ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
                GameManagerMultiplayer.Instance.RPC_ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }
        }
        else if (GameManagerMultiplayer.Instance && GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN && player == PlayerColor.WHITE)
        {

            if (collide &&
               hit.collider.gameObject.tag == "WhitePawntoQueen" &&
               (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {

                GameManagerMultiplayer.Instance.RPC_OnePieceOrganize(player, hit.collider.gameObject.GetComponent<Piece>().pieceName, GameManagerMultiplayer.Instance.PawntoQueenIndex);
                GameManagerMultiplayer.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
                GameManagerMultiplayer.Instance.IsCheckBlackPlayer();

                UIManagerVR.Instance.WhiteDeckOff();

                player = PlayerColor.BLACK;

            }
            else if (collide &&
            hit.collider.gameObject.tag == "WhitePawntoQueen" &&
            prevHit != hit.collider.gameObject)
            {

                if (prevHit != null ) prevHit.GetComponent<Renderer>().material.color = Color.white;
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                prevHit = hit.collider.gameObject;

            }


        }

        else if (GameManagerMultiplayer.Instance != null &&
             player == PlayerColor.BLACK &&
             (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN_WITH_CHECK))
        {

            //Debug.Log("Inside black Player select");

            if (collide &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && (triggerValue)) &&
                SelectedObject != null &&
                SelectedObject.transform.name == hit.collider.gameObject.name &&
                hit.collider.gameObject.GetComponent<Piece>() != null &&
                hit.collider.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK
                )
            {

                delayValue = 0;
                GameManagerMultiplayer.Instance.RPC_ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT);

            }

            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK)
            {

                GameManagerMultiplayer.Instance.RPC_PieceColorChange(hit.transform.gameObject.name);

            }
            else if (!collide && (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && (triggerValue)))
            {

                GameManagerMultiplayer.Instance.RPC_ClearSelectedPiece();

            }
        }
        else if (GameManagerMultiplayer.Instance != null && GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT &&
                player == PlayerColor.BLACK)
        {

            if (collide &&
                hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) && 
                (delayValue <= delayConstant))
            {

                delayValue++;


            }

            else if (collide && hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) && 
                (delayValue > delayConstant))
            {
                delayValue = 0;
                goToIndex = hit.collider.transform.gameObject.GetComponent<ValidBox>().ValidIndex;
                GameManagerMultiplayer.Instance.RPC_PieceMove(goToIndex);


                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {

                    GameManagerMultiplayer.Instance.IsBlackPawnToQueen(goToIndex);

                }
                else
                {


                    GameManagerMultiplayer.Instance.IsCheckWhitePlayer();
                    Player.Instance.player = PlayerColor.WHITE;

                }

            }
            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE &&
            SelectedObject.GetComponent<Piece>().CalculateValidMoves().Find(x => x.Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index).Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index &&
            (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {


                goToIndex = hit.collider.transform.gameObject.GetComponent<Piece>().Index;
                GameManagerMultiplayer.Instance.RPC_PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {

                    GameManagerMultiplayer.Instance.IsBlackPawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerMultiplayer.Instance.IsCheckWhitePlayer();
                    player = PlayerColor.WHITE;

                }


            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject.name != hit.collider.gameObject.name)
            {

                GameManagerMultiplayer.Instance.RPC_ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
                GameManagerMultiplayer.Instance.RPC_ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }

        }
        else if (GameManagerMultiplayer.Instance && GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN && player == PlayerColor.BLACK)
        {

            if (collide &&
               hit.collider.gameObject.tag == "BlackPawntoQueen" &&
               (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {

                GameManagerMultiplayer.Instance.RPC_OnePieceOrganize(player, hit.collider.gameObject.GetComponent<Piece>().pieceName, GameManagerMultiplayer.Instance.PawntoQueenIndex);
                GameManagerMultiplayer.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
                GameManagerMultiplayer.Instance.IsCheckWhitePlayer();

                UIManagerVR.Instance.BlackDeckOff();

                player = PlayerColor.WHITE;

            }
            else if (collide &&
            hit.collider.gameObject.tag == "WhitePawntoQueen" &&
            prevHit != hit.collider.gameObject)
            {

                prevHit.GetComponent<Renderer>().material.color = Color.white;
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                prevHit = hit.collider.gameObject;

            }


        }

        if (prevStatus != GameManagerMultiplayer.Instance.GetBoardStatus()) {
            Debug.Log("Board status is " + GameManagerMultiplayer.Instance.GetBoardStatus());
            prevStatus = GameManagerMultiplayer.Instance.GetBoardStatus();
        }
        

    }

    public void PawnToQueen(int i)
    {

        if (i == 1 && (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {
            UIManager.Instance.DebugBannerUpdate("Update index is " + GameManagerMultiplayer.Instance.PawntoQueenIndex);
            GameManagerMultiplayer.Instance.RPC_OnePieceOrganize( player , PIECENAME.ROOK, GameManagerMultiplayer.Instance.PawntoQueenIndex);
            

        }
        else if (i == 2 && (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerMultiplayer.Instance.RPC_OnePieceOrganize(player, PIECENAME.KNIGHT, GameManagerMultiplayer.Instance.PawntoQueenIndex);
            

        }
        else if (i == 3 && (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerMultiplayer.Instance.RPC_OnePieceOrganize(player , PIECENAME.BISHOP, GameManagerMultiplayer.Instance.PawntoQueenIndex);
            

        }
        else if (i == 4 && (GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerMultiplayer.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerMultiplayer.Instance.RPC_OnePieceOrganize(player , PIECENAME.QUEEN, GameManagerMultiplayer.Instance.PawntoQueenIndex);
            

        }

        UIManager.Instance.InteractOff();

        if (player == PlayerColor.WHITE)
        {

            GameManagerMultiplayer.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
            GameManagerMultiplayer.Instance.IsCheckBlackPlayer();

        }
        else if (player == PlayerColor.BLACK) {

            GameManagerMultiplayer.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
            GameManagerMultiplayer.Instance.IsCheckWhitePlayer();

        }
           

    }

    void GetInputDevice() {

        var  rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand,
                                                         rightHandDevices);
        while (rightHandDevices.Count == 0) {

            rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand,
                                                             rightHandDevices);

        }


        device = rightHandDevices[0];
        Debug.Log("Device name is :"+ device.name);

        trackingHand = GameObject.Find("RightHand Controller");
        trackingHand.GetComponent<XRInteractorLineVisual>().lineWidth = 0.01f;
    }
}


