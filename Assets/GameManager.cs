using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManagerBase : MonoBehaviour
{
    // Start is called before the first frame upda
    private BOARDSTATUS status;

    protected List<int> selectedPieceValidMoves;
    protected Piece selectedPieceComponent;
    public PlayerColor playerTurn { get; set; }

    public GameObject BlackKing { get; set; }
    public GameObject WhiteKing { get; set; }

    int goToIndex;

    public int PawntoQueenIndex { get; set; }
    void Start()
    {
        AIPlayer.Instance.AIPlayerColor = PlayerColor.BLACK;
        ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);

        playerTurn = PlayerColor.WHITE;
        selectedPieceValidMoves = new List<int>() ; 

    }

    public BOARDSTATUS GetBoardStatus() {

        return status;
    }
    public virtual void IsWhitePawnToQueen(int index)
    {
        //UIManager.Instance.DebugBannerUpdate("Move indes is " + index);
        if (Player.Instance.player == PlayerColor.WHITE  && (int)(index / 8) == 7)
        { 
            ChangeBoardState(BOARDSTATUS.WHITE_PAWN_TO_QUEEN);
            PawntoQueenIndex = index;
            UIManagerVR.Instance.WhiteDeckOn();



        }
        else
        {

            ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
            this.IsCheckBlackPlayer();

            //Player.Instance.player= PlayerColor.BLACK; ///only for the debugging purpose


        }

    }

    public virtual void IsCheckBlackPlayer()
    {

        UpdateActivePiece();
        int kingIndex = Board.Instance.BlackKing.GetComponent<Piece>().Index;

        if (Board.Instance.IsCheckBlackPlayer()) {

            ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_CHECK);
            IsCheckMateBlackPlayer();
            Board.Instance.BlackChecked = true;
            return;

        }

        ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
        Board.Instance.BlackChecked = false;

    }

    public virtual void IsCheckMateBlackPlayer()
    {

        UpdateActivePiece();

        if (Board.Instance.IsCheckMateBlackPlayer())
        {

            ChangeBoardState(BOARDSTATUS.BLACK_CHECKMATE);

        }
        else 
        {

            ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN_WITH_CHECK);

        } 

    }

    protected void UpdateActivePiece()
    {

        Board.Instance.ActivePiecesUpdate();

    }

    public virtual void PieceColorChange(string selectPiece)
    {

        //Debug.Log("Rpc is called");
        if (Player.Instance.SelectedObject != null) Player.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.white;
        ObjectPool.instance.InactiveAllActive();

        Player.Instance.SelectedObject = GameObject.Find(selectPiece);
        Player.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.red;

        selectedPieceValidMoves = Player.Instance.SelectedObject.GetComponent<Piece>().CalculateValidMoves();

        foreach (int i in selectedPieceValidMoves)
        {

            GameObject validBox = ObjectPool.instance.GetPooledObject();

            validBox.transform.localPosition = Board.Instance.CalculateLocalPosition(i);
            validBox.GetComponent<ValidBox>().ValidIndex = i;
        }

    }
    public virtual void IsBlackPawnToQueen(int index)
    {

        if (Player.Instance.player == PlayerColor.BLACK && (int)(index / 8) == 0)
        {

            ChangeBoardState(BOARDSTATUS.BLACK_PAWN_TO_QUEEN);
            PawntoQueenIndex = index;
            //UIManager.Instance.InteractOn();
            UIManagerVR.Instance.BlackDeckOn();


        }
        else
        {

            ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
            this.IsCheckWhitePlayer();
            //Player.Instance.player = PlayerColor.WHITE;


        }

    }
    public virtual void IsCheckWhitePlayer()
    {
        UpdateActivePiece();
        int kingIndex = Board.Instance.WhiteKing.GetComponent<Piece>().Index;

        if (Board.Instance.IsCheckWhitePlayer()) {

            ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_CHECK);
            IsCheckMateWhitePlayer();
            Board.Instance.WhiteChecked = true;
            return;

        }
        ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
        Board.Instance.WhiteChecked = false;
    }

    public virtual void IsCheckMateWhitePlayer()
    {

        UpdateActivePiece();

        if (Board.Instance.IsCheckMateWhitePlayer())
        {

            ChangeBoardState(BOARDSTATUS.WHITE_CHECKMATE);

        }
        else 
        {
            ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK);
        } 

    }

    public virtual void ChangeBoardState(BOARDSTATUS state) {

        status = state;
    }

    public virtual void PieceMove(int index)
    {

        selectedPieceComponent = Player.Instance.SelectedObject.GetComponent<Piece>();
        selectedPieceComponent.ChangePosition(index);
        ObjectPool.instance.InactiveAllActive();
        Board.Instance.ActivePiecesUpdate();
    }

    public virtual void ClearSelectedPiece()
    {

        if (Player.Instance.SelectedObject != null)
        {

            Player.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.white;
            ObjectPool.instance.InactiveAllActive();

        }

    }

    public virtual void ChangePlayerTurn(PlayerColor color)
    {

        Board.Instance.playerTurn = color;
        this.playerTurn = color;

    }

    public virtual void OnePieceOrganize(PlayerColor player, PIECENAME pieceName, int index)
    {

        Board.Instance.OnePieceOrganize(player, pieceName, index);
        Board.Instance.ActivePiecesUpdate();

    }

}