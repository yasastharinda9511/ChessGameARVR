using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame upda

    private enum TURN
    {

        BLACK,
        WHITE
    }

    [SerializeField]
    GameObject Board;

    Board chessBoard;

    public BOARDSTATUS status;

    [SerializeField]
    private Camera camera;

    private Coroutine activeCoroutine;

    public PlayerColor playerTurn { get; set; }

    public static GameManager Instance;
    public List<Piece> WhiteActivePieces;
    public List<Piece> BlackActivePieces;

    public GameObject BlackKing { get; set; }
    public GameObject WhiteKing { get; set; }

    private RaycastHit hit;
    private Ray mouseRay;
    private GameObject hitObject;
    private GameObject selectedPiece;

    private int pawntToQueenIndex ;

    int goToIndex;
    Piece selectedPieceComponent;
    Piece opponentSelect;
    List<int> selectedPieceValidMoves;

    public bool BlackChecked { get; set; }
    public bool WhiteChecked { get; set; }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        status = BOARDSTATUS.WHITE_PLAYER_TURN;
        WhiteActivePieces = new List<Piece>();
        BlackActivePieces = new List<Piece>();

        chessBoard = Board.GetComponent<Board>();
        chessBoard.OrganizeBoard();

        playerTurn = PlayerColor.WHITE;
        mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        selectedPiece = null;

        BlackChecked = false;
        WhiteChecked = false;

        selectedPieceValidMoves = new List<int>();

    }

    // Update is called once per frame
    void Update()
    {

        State(status);
        Debug.Log("Board status is : " + status);

    }

    void UpdateActivePiece()
    {

        chessBoard.GameManagerSetup();

    }

    void IsCheckBlackPlayer() {

        UpdateActivePiece();
        int kingIndex = BlackKing.GetComponent<Piece>().Index;

        foreach (Piece piece in WhiteActivePieces)
        {

            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex)
            {

                Debug.Log(piece.pieceName);
                status = BOARDSTATUS.BLACK_PLAYER_CHECK;
                BlackChecked = true;
                return;

            }

        }

        status = BOARDSTATUS.BLACK_PLAYER_TURN;
        BlackChecked = false;

    }

    void IsCheckMateBlackPlayer() {

        int totalMoves = 0;
        int pieceCount;
        UpdateActivePiece();

        foreach (Piece piece in BlackActivePieces)
        {
            pieceCount = piece.CalculateValidMoves().Count;
            totalMoves += pieceCount;
            foreach ( var i in piece.CalculateValidMoves()) {

                Debug.Log("@123" + piece.name + "Index" + piece.Index + " ValidMove is :" + i);

            }
          


        }
        Debug.Log("total moves : " + totalMoves);
        if (totalMoves == 0) status = BOARDSTATUS.BLACK_CHECKMATE;
        else status = BOARDSTATUS.BLACK_PLAYER_TURN;

    }



    public void ActivePieceSelector(PlayerColor color , int index )
    {

        UIManager.Instance.InteractOn();
        pawntToQueenIndex = index;
        
    }

    public void PawnToQueen(int i) {

        if (i == 1 && status == BOARDSTATUS.WHITE_PAWN_TO_QUEEN) 
        {

            chessBoard.OnePieceOrganize(PlayerColor.WHITE, PIECENAME.ROOK, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_BLACK_PLAYER;

        }
        else if (i == 2 && status == BOARDSTATUS.WHITE_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.WHITE, PIECENAME.KNIGHT, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_BLACK_PLAYER;
            
        }
        else if (i == 3 && status == BOARDSTATUS.WHITE_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.WHITE, PIECENAME.BISHOP, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_BLACK_PLAYER;

        }
        else if (i == 4 && status == BOARDSTATUS.WHITE_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.WHITE, PIECENAME.QUEEN, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_BLACK_PLAYER;

        }
        else if (i == 1 && status == BOARDSTATUS.BLACK_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.BLACK, PIECENAME.ROOK, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_WHITE_PLAYER;

        }
        else if (i == 2 && status == BOARDSTATUS.BLACK_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.BLACK, PIECENAME.KNIGHT, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_WHITE_PLAYER;

        }
        else if (i == 3 && status == BOARDSTATUS.BLACK_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.BLACK , PIECENAME.BISHOP, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_WHITE_PLAYER;

        }
        else if (i == 4 && status == BOARDSTATUS.BLACK_PAWN_TO_QUEEN)
        {

            chessBoard.OnePieceOrganize(PlayerColor.BLACK, PIECENAME.QUEEN, pawntToQueenIndex);
            UIManager.Instance.InteractOff();
            status = BOARDSTATUS.IS_CHECK_WHITE_PLAYER;

        }

    }



    private void State(BOARDSTATUS state) {

        switch (state) {

            case (BOARDSTATUS.WHITE_PLAYER_TURN):
                playerTurn = PlayerColor.WHITE;
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.WHITE_PLAYER_TURN.ToString());
                WhitePlayerPieceSelect();                
                break;
            case (BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT):
                whitePlayerMovePieceSelected();
                break;
            case (BOARDSTATUS.IS_CHECK_BLACK_PLAYER):
                playerTurn = PlayerColor.BLACK;
                IsCheckBlackPlayer();
                break;
            case (BOARDSTATUS.BLACK_PLAYER_CHECK):
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.BLACK_PLAYER_CHECK.ToString());
                IsCheckMateBlackPlayer();
                break;
            case (BOARDSTATUS.BLACK_CHECKMATE):
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.BLACK_CHECKMATE.ToString());
                break;
            case (BOARDSTATUS.BLACK_PLAYER_TURN):
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.BLACK_PLAYER_TURN.ToString());
                BlackPlayerPieceSelect();
                break;
            case (BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT):
                BlackPlayerMovePieceSelected();
                break;
            case (BOARDSTATUS.IS_CHECK_WHITE_PLAYER):
                playerTurn = PlayerColor.WHITE;
                IsCheckWhitePlayer();
                break;
            case (BOARDSTATUS.WHITE_PLAYER_CHECK):
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.WHITE_PLAYER_CHECK.ToString());
                IsCheckMateWhitePlayer();
                break;
            case (BOARDSTATUS.WHITE_CHECKMATE):
                UIManager.Instance.UpdateBoardStatusBanner(BOARDSTATUS.WHITE_PLAYER_CHECK.ToString());
                break;
            case (BOARDSTATUS.IS_BLACK_PAWN_TO_QUEEN):
                IsBlackPawnToQueen();
                break;
            case (BOARDSTATUS.IS_WHITE_PAWN_TO_QUEEN):
                IsWhitePawnToQueen();
                break;
            case (BOARDSTATUS.WHITE_PAWN_TO_QUEEN):
                break;
            case (BOARDSTATUS.BLACK_PAWN_TO_QUEEN):
                break;


        }
    }

    private void IsCheckMateWhitePlayer()
    {
        int totalMoves = 0;

        UpdateActivePiece();
        foreach (Piece piece in WhiteActivePieces)
        {

            totalMoves += piece.CalculateValidMoves().Count;

        }

        if (totalMoves == 0) status = BOARDSTATUS.WHITE_CHECKMATE;
        else status = BOARDSTATUS.WHITE_PLAYER_TURN;
    }

    private void IsCheckWhitePlayer()
    {
        UpdateActivePiece();
        int kingIndex = WhiteKing.GetComponent<Piece>().Index;

        foreach (Piece piece in BlackActivePieces)
        {

            if (piece.CalculateValidMoves().Find(x => x == kingIndex) == kingIndex)
            {

                Debug.Log(piece.pieceName);
                status = BOARDSTATUS.WHITE_PLAYER_CHECK;
                WhiteChecked = true;
                return;

            }

        }

        status = BOARDSTATUS.WHITE_PLAYER_TURN;
        WhiteChecked = false;
    }


    private void BlackPlayerMovePieceSelected()
    {
        mouseRay = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out hit) && hit.transform.gameObject.tag == "ValidBox" && Input.GetMouseButtonDown(0))
        {
            goToIndex = hit.transform.gameObject.GetComponent<ValidBox>().ValidIndex;
            selectedPieceComponent = selectedPiece.GetComponent<Piece>();
            selectedPieceComponent.ChangePosition(hit.transform.gameObject.GetComponent<ValidBox>().ValidIndex);
            ObjectPool.instance.InactiveAllActive();

            status = BOARDSTATUS.IS_BLACK_PAWN_TO_QUEEN;

        }
        else if (Physics.Raycast(mouseRay, out hit) && hit.transform.gameObject.tag == "Piece" && Input.GetMouseButtonDown(0))
        {

            opponentSelect = hit.transform.gameObject.GetComponent<Piece>();

            Debug.Log(opponentSelect);
            if (opponentSelect.playerColor != PlayerColor.WHITE) return;

            if (selectedPieceValidMoves.Find(x => x == opponentSelect.Index) == opponentSelect.Index)
            {

                selectedPieceComponent = selectedPiece.GetComponent<Piece>();
                selectedPieceComponent.ChangePosition(opponentSelect.Index);
                ObjectPool.instance.InactiveAllActive();
                status = BOARDSTATUS.IS_BLACK_PAWN_TO_QUEEN;

            }

        }
        else if (Input.GetMouseButtonDown(0))
        {

            status = BOARDSTATUS.BLACK_PLAYER_TURN;

        }
    }

    private void WhitePlayerPieceSelect()
    {
        BlackChecked = false;

        mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && selectedPiece != null) {

            status = BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT;

        }

        if (Physics.Raycast(mouseRay, out hit) &&
            selectedPiece != hit.transform.gameObject &&
            hit.transform.gameObject.tag == "Piece" &&
            hit.transform.gameObject.GetComponent<Piece>() != null &&
            hit.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.WHITE)
        {
            if (selectedPiece != null) selectedPiece.gameObject.GetComponent<Renderer>().material.color = Color.white;
            hitObject = hit.transform.gameObject;
            hitObject.GetComponent<Renderer>().material.color = Color.red;
            ObjectPool.instance.InactiveAllActive();

            selectedPieceValidMoves = hitObject.GetComponent<Piece>().CalculateValidMoves();

            foreach (int i in selectedPieceValidMoves)
            {

                GameObject validBox = ObjectPool.instance.GetPooledObject();

                validBox.transform.localPosition = chessBoard.CalculateLocalPosition(i);
                validBox.GetComponent<ValidBox>().ValidIndex = i;
            }

            selectedPiece = hit.transform.gameObject;
        }

        else if (Physics.Raycast(mouseRay, out hit) &&
                hit.transform.gameObject.tag != "Piece")
        {

            ObjectPool.instance.InactiveAllActive();
            if (selectedPiece != null) selectedPiece.gameObject.GetComponent<Renderer>().material.color = Color.white;
            selectedPiece = null;

        } 


    }

    private void whitePlayerMovePieceSelected()
    {
        mouseRay = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out hit) && hit.transform.gameObject.tag == "ValidBox" && Input.GetMouseButtonDown(0)) {

            goToIndex = hit.transform.gameObject.GetComponent<ValidBox>().ValidIndex;
            selectedPieceComponent = selectedPiece.GetComponent<Piece>();
            selectedPieceComponent.ChangePosition(hit.transform.gameObject.GetComponent<ValidBox>().ValidIndex);
            ObjectPool.instance.InactiveAllActive();

            status = BOARDSTATUS.IS_WHITE_PAWN_TO_QUEEN;


        } else if (Physics.Raycast(mouseRay, out hit) && hit.transform.gameObject.tag == "Piece" && Input.GetMouseButtonDown(0)) {

             opponentSelect= hit.transform.gameObject.GetComponent<Piece>();

            Debug.Log(opponentSelect);
            if (opponentSelect.playerColor != PlayerColor.BLACK) return;

            if (selectedPieceValidMoves.Find(x => x == opponentSelect.Index) == opponentSelect.Index) {

                selectedPieceComponent = selectedPiece.GetComponent<Piece>();
                selectedPieceComponent.ChangePosition(opponentSelect.Index);
                ObjectPool.instance.InactiveAllActive();
                status = BOARDSTATUS.IS_WHITE_PAWN_TO_QUEEN;

            }

        }
        else if (Input.GetMouseButtonDown(0)) {

            status = BOARDSTATUS.WHITE_PLAYER_TURN;

        }
    }


    private void BlackPlayerPieceSelect()
    {
        WhiteChecked = false;

        mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && selectedPiece != null)
        {

            status = BOARDSTATUS.BLACK_PLAYER_PIECE_SELECT;

        }

        if (Physics.Raycast(mouseRay, out hit) &&
            selectedPiece != hit.transform.gameObject &&
            hit.transform.gameObject.tag == "Piece" &&
            hit.transform.gameObject.GetComponent<Piece>() != null &&
            hit.transform.gameObject.GetComponent<Piece>().playerColor == PlayerColor.BLACK)
        {
            if (selectedPiece != null) selectedPiece.gameObject.GetComponent<Renderer>().material.color = Color.white;

            hitObject = hit.transform.gameObject;
            hitObject.GetComponent<Renderer>().material.color = Color.red;
            ObjectPool.instance.InactiveAllActive();
            selectedPieceValidMoves = hitObject.GetComponent<Piece>().CalculateValidMoves();

            foreach (int i in selectedPieceValidMoves)
            {

                GameObject validBox = ObjectPool.instance.GetPooledObject();

                validBox.transform.localPosition = chessBoard.CalculateLocalPosition(i);
                validBox.GetComponent<ValidBox>().ValidIndex = i;
            }

            selectedPiece = hit.transform.gameObject;
        }

        else if (Physics.Raycast(mouseRay, out hit) &&
                hit.transform.gameObject.tag != "Piece")
        {

            ObjectPool.instance.InactiveAllActive();
            if (selectedPiece != null) selectedPiece.gameObject.GetComponent<Renderer>().material.color = Color.white;
            selectedPiece = null;

        }
    }

    public void IsBlackPawnToQueen() {

        if (selectedPieceComponent.pieceName == PIECENAME.PAWN && selectedPieceComponent.playerColor == PlayerColor.BLACK && (int)(goToIndex / 8) == 0)
        {

            status = BOARDSTATUS.BLACK_PAWN_TO_QUEEN;

        }
        else
        {

            status = BOARDSTATUS.IS_CHECK_WHITE_PLAYER;

        }

    }

    public void IsWhitePawnToQueen()
    {

        if (selectedPieceComponent.pieceName == PIECENAME.PAWN && selectedPieceComponent.playerColor == PlayerColor.WHITE && (int)(goToIndex / 8) == 7)
        {

            status = BOARDSTATUS.WHITE_PAWN_TO_QUEEN;

        }
        else
        {

            status = BOARDSTATUS.IS_CHECK_BLACK_PLAYER;

        }

    }
}
