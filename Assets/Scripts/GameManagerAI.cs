using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAI : GameManagerBase
{

    public static GameManagerAI Instance;

    private void Awake()
    {
        Instance = this;

    }
    void CheckAITurn() 
    {
        if (AIPlayer.Instance.AIPlayerColor == PlayerColor.WHITE &&
            (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN 
            || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK 
            || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.WHITE_PAWN_TO_QUEEN)) {

            AIPlayer.Instance.DelayAIMoveCall();
        
        }
        else if (AIPlayer.Instance.AIPlayerColor == PlayerColor.BLACK &&
            (GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN 
            || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PLAYER_TURN_WITH_CHECK 
            || GameManagerAI.Instance.GetBoardStatus() == BOARDSTATUS.BLACK_PAWN_TO_QUEEN)) {

            AIPlayer.Instance.DelayAIMoveCall();
        
        }
    }

    public override void ChangeBoardState(BOARDSTATUS state)
    {
        base.ChangeBoardState(state);
        CheckAITurn();
    }

    public override void PieceColorChange(string selectPiece)
    {

        //Debug.Log("Rpc is called");
        if (HumanPlayer.Instance.SelectedObject != null) HumanPlayer.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.white;
        ObjectPool.instance.InactiveAllActive();

        HumanPlayer.Instance.SelectedObject = GameObject.Find(selectPiece);
        HumanPlayer.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.red;

        selectedPieceValidMoves = HumanPlayer.Instance.SelectedObject.GetComponent<Piece>().CalculateValidMoves();

        foreach (Moves  i in selectedPieceValidMoves)
        {

            GameObject validBox = ObjectPool.instance.GetPooledObject();

            validBox.transform.localPosition = Board.Instance.CalculateLocalPosition(i.Destination);
            validBox.GetComponent<ValidBox>().ValidIndex = i.Destination;
        }

    }

    public override void IsBlackPawnToQueen(int index)
    {

        if (HumanPlayer.Instance.player == PlayerColor.BLACK && (int)(index / 8) == 0)
        {

            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.BLACK_PAWN_TO_QUEEN);
            PawntoQueenIndex = index;
            //UIManager.Instance.InteractOn();
            UIManagerVR.Instance.BlackDeckOn();


        }
        else
        {

            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_WHITE_PLAYER);
            this.IsCheckWhitePlayer();
            //HumanPlayer.Instance.player = PlayerColor.WHITE;


        }

    }

    public override void IsWhitePawnToQueen(int index)
    {
        //UIManager.Instance.DebugBannerUpdate("Move indes is " + index);

        if (HumanPlayer.Instance.player == PlayerColor.WHITE && (int)(index / 8) == 7)
        {
            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.WHITE_PAWN_TO_QUEEN);
            PawntoQueenIndex = index;
            UIManagerVR.Instance.WhiteDeckOn();
            Debug.Log(" @@@@@ White Pawn to Queen True");


        }
        else
        {

            GameManagerAI.Instance.ChangeBoardState(BOARDSTATUS.IS_CHECK_BLACK_PLAYER);
            this.IsCheckBlackPlayer();

            //HumanPlayer.Instance.player = PlayerColor.BLACK; ///only for the debugging purpose


        }

    }

    public override void PieceMove(int index)
    {

        selectedPieceComponent = HumanPlayer.Instance.SelectedObject.GetComponent<Piece>();
        selectedPieceComponent.ChangePosition(index);
        ObjectPool.instance.InactiveAllActive();
        Board.Instance.ActivePiecesUpdate();
    }

    public override void ClearSelectedPiece()
    {

        if (HumanPlayer.Instance.SelectedObject != null)
        {

            HumanPlayer.Instance.SelectedObject.GetComponent<Renderer>().material.color = Color.white;
            ObjectPool.instance.InactiveAllActive();

        }

    }

}
