using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HumanPlayer : MonoBehaviour 
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

    public static HumanPlayer Instance;
    public PlayerColor player { get; set; }

    RaycastHit hit;
    Ray mouseRay;

    [SerializeField]
    private Camera camera;

    public GameObject SelectedObject { get; set; }
    public GameObject WhiteKing;
    public GameObject BlackKing;

    int goToIndex;

    public List<Piece> WhiteActivePieces;
    public List<Piece> BlackActivePieces;

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

        WhiteActivePieces = new List<Piece>();
        BlackActivePieces = new List<Piece>();


        player = PlayerColor.WHITE;
        XRRig.transform.position = whitePlayerPos.transform.position;
        Debug.Log("Initializing game manager");


        //Board.Instance.OrganizeBoard();
        this.GetInputDevice();

        SelectedObject = GameObject.Find("Board");
        defaultGameObject = SelectedObject;

    }

    // Update is called once per frame
    void Update()
    {

        //collide = trackingHand.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out hit);
        collide = Physics.Raycast(trackingHand.transform.position, trackingHand.transform.forward , out hit);

        if (GameManagerAI.Instance != null &&
            player == PlayerColor.WHITE &&
            (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK))
        {
            if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) && !collide)
            {

                GameManagerAI.Instance.ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject != null &&
                collide && hit.collider.transform.gameObject.name == SelectedObject.name &&
                hit.collider.gameObject.GetComponent<Piece>() != null &&
                hit.collider.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE)
            {
                delayValue = 0;
                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT);

            }

            else if (collide &&
            SelectedObject.name != hit.collider.transform.gameObject.name &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE)
            {

                GameManagerAI.Instance.PieceColorChange(hit.collider.transform.gameObject.name);

            }
        }

        else if (GameManagerAI.Instance != null && GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT && player == PlayerColor.WHITE)
        {
            if (collide &&
                hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                (delayValue <= delayConstant))
            {
                Debug.Log("Delay value is :" + delayValue);
                delayValue++;


            }
            else if (collide &&
                hit.collider.transform.gameObject.tag == "ValidBox" &&
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                (delayValue > delayConstant))
            {

                delayValue = 0;
                goToIndex = hit.collider.transform.gameObject.GetComponent<ValidBox>().ValidIndex;
                GameManagerAI.Instance.PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {

                    GameManagerAI.Instance.IsWhitePawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerAI.Instance.IsCheckBlackPlayer();
                    //player = PlayerColor.BLACK;

                }

            }

            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK &&
            SelectedObject != null &&
            SelectedObject.gameObject.GetComponent<Piece>().CalculateValidMoves().Find(x => x.Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index) != null &&
            (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            )
            {

                goToIndex = hit.collider.transform.gameObject.GetComponent<Piece>().Index;
                GameManagerAI.Instance.PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {
                    //UIManager.Instance.UpdateBoardStatusBanner("WhitePawnMove");
                    GameManagerAI.Instance.IsWhitePawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerAI.Instance.IsCheckBlackPlayer();
                    //player = PlayerColor.BLACK;

                }


            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject.name != hit.collider.gameObject.name)
            {

                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
                GameManagerAI.Instance.ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }
        }
        else if (GameManagerAI.Instance != null && GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN && player == PlayerColor.WHITE)
        {

            if (collide &&
               hit.collider.gameObject.tag == "WhitePawntoQueen" &&
               (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {

                Debug.Log("pawn to queen :" + hit.collider.GetComponent<Piece>().pieceName);
                GameManagerAI.Instance.OnePieceOrganize(player, hit.collider.gameObject.GetComponent<Piece>().pieceName, GameManagerAI.Instance.PawntoQueenIndex);
                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
                GameManagerAI.Instance.IsCheckBlackPlayer();

                UIManagerVR.Instance.WhiteDeckOff();

                //player = PlayerColor.BLACK;

            }
            else if (collide &&
            hit.collider.gameObject.tag == "WhitePawntoQueen" &&
            prevHit != hit.collider.gameObject)
            {

                if (prevHit != null) prevHit.GetComponent<Renderer>().material.color = Color.white;
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                prevHit = hit.collider.gameObject;

            }


        }

        else if (GameManagerAI.Instance != null &&
             player == PlayerColor.BLACK &&
             (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN_WITH_CHECK))
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
                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT);

            }

            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK)
            {

                GameManagerAI.Instance.PieceColorChange(hit.transform.gameObject.name);

            }
            else if (!collide && (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && (triggerValue)))
            {

                GameManagerAI.Instance.ClearSelectedPiece();

            }
        }
        else if (GameManagerAI.Instance != null && GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT &&
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
                GameManagerAI.Instance.PieceMove(goToIndex);


                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {

                    GameManagerAI.Instance.IsBlackPawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerAI.Instance.IsCheckWhitePlayer();
                    //Player.Instance.player = PlayerColor.WHITE;

                }

            }
            else if (collide &&
            hit.collider.transform.gameObject.tag == "Piece" &&
            hit.collider.transform.gameObject.GetComponent<Piece>() != null &&
            hit.collider.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE &&
            SelectedObject != null && 
            SelectedObject.GetComponent<Piece>().CalculateValidMoves().Find(x => x.Destination == hit.collider.transform.gameObject.GetComponent<Piece>().Index) != null &&
            (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {


                goToIndex = hit.collider.transform.gameObject.GetComponent<Piece>().Index;
                GameManagerAI.Instance.PieceMove(goToIndex);

                if (SelectedObject.GetComponent<Piece>().pieceName == PIECENAME.PAWN)
                {

                    GameManagerAI.Instance.IsBlackPawnToQueen(goToIndex);

                }
                else
                {

                    GameManagerAI.Instance.IsCheckWhitePlayer();
                    //player = PlayerColor.WHITE;

                }


            }
            else if ((device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) &&
                SelectedObject.name != hit.collider.gameObject.name)
            {

                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
                GameManagerAI.Instance.ClearSelectedPiece();
                SelectedObject = defaultGameObject;

            }

        }
        else if (GameManagerAI.Instance != null && GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN && player == PlayerColor.BLACK)
        {

            if (collide &&
               hit.collider.gameObject.tag == "BlackPawntoQueen" &&
               (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue))
            {

                GameManagerAI.Instance.OnePieceOrganize(player, hit.collider.gameObject.GetComponent<Piece>().pieceName, GameManagerAI.Instance.PawntoQueenIndex);
                GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
                GameManagerAI.Instance.IsCheckWhitePlayer();

                UIManagerVR.Instance.BlackDeckOff();

                //player = PlayerColor.WHITE;

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

        if (prevStatus != GameManagerAI.Instance.GetBoardStatus())
        {
            Debug.Log("Board status is " + GameManagerAI.Instance.GetBoardStatus());
            prevStatus = GameManagerAI.Instance.GetBoardStatus();
        }
    }

    public void PawnToQueen(int i)
    {

        if (i == 1 && (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {
            UIManager.Instance.DebugBannerUpdate("Update index is " + GameManagerAI.Instance.PawntoQueenIndex);
            GameManagerAI.Instance.OnePieceOrganize(player, PIECENAME.ROOK, GameManagerAI.Instance.PawntoQueenIndex);


        }
        else if (i == 2 && (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerAI.Instance.OnePieceOrganize(player, PIECENAME.KNIGHT, GameManagerAI.Instance.PawntoQueenIndex);


        }
        else if (i == 3 && (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerAI.Instance.OnePieceOrganize(player, PIECENAME.BISHOP, GameManagerAI.Instance.PawntoQueenIndex);


        }
        else if (i == 4 && (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN))
        {

            GameManagerAI.Instance.OnePieceOrganize(player, PIECENAME.QUEEN, GameManagerAI.Instance.PawntoQueenIndex);


        }

        UIManager.Instance.InteractOff();

        if (player == PlayerColor.WHITE)
        {

            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
            GameManagerAI.Instance.IsCheckBlackPlayer();

        }
        else if (player == PlayerColor.BLACK)
        {

            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
            GameManagerAI.Instance.IsCheckWhitePlayer();

        }


    }

    void GetInputDevice()
    {

        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand,
                                                         rightHandDevices);
        while (rightHandDevices.Count == 0)
        {

            rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand,
                                                             rightHandDevices);

        }


        device = rightHandDevices[0];
        Debug.Log("Device name is :" + device.name);

        trackingHand = GameObject.Find("RightHand Controller");
        trackingHand.GetComponent<XRInteractorLineVisual>().lineWidth = 0.01f;
    }
}
