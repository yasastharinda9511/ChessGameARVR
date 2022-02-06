using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameManagerMultiplayer : GameManagerBase 
{
    // Start is called before the first frame updated
    PhotonView photonView;

    public static GameManagerMultiplayer Instance;
    private void Awake()
    {
        if (Instance  == null) Instance = this;

    }
    void Start()
    {
        Debug.Log("Instantiate <3");
        photonView = this.GetComponent<PhotonView>();
    }

    public override void IsWhitePawnToQueen(int index) 
    {

        base.IsWhitePawnToQueen(index);
        this.RPC_ChangeBoardState(GetBoardStatus());


    }
    public override void IsCheckBlackPlayer()
    {

        UpdateActivePiece();
        photonView.RPC("ChangePlayerTurn", RpcTarget.All, PlayerColor.BLACK);
        int kingIndex = Board.Instance.BlackKing.GetComponent<Piece>().Index;

        if (Board.Instance.IsCheckBlackPlayer())
        {

            ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_CHECK);
            this.RPC_ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_CHECK);
            IsCheckMateBlackPlayer();
            Board.Instance.BlackChecked = true;
            return;

        }

        ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
        RPC_ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
        Board.Instance.BlackChecked = false;

    }

    public override void IsCheckMateBlackPlayer()
    {

        base.IsCheckMateBlackPlayer();
        this.RPC_ChangeBoardState(GetBoardStatus());

    }

    public override void IsBlackPawnToQueen(int index)
    {
        base.IsBlackPawnToQueen(index);
        this.RPC_ChangeBoardState(GetBoardStatus());

    }

    public override void IsCheckWhitePlayer() 
    {

        UpdateActivePiece();
        photonView.RPC("ChangePlayerTurn", RpcTarget.All, PlayerColor.WHITE);
        int kingIndex = Board.Instance.WhiteKing.GetComponent<Piece>().Index;

        if (Board.Instance.IsCheckWhitePlayer())
        {

            ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_CHECK);
            IsCheckMateWhitePlayer();
            Board.Instance.WhiteChecked = true;
            return;

        }
        ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
        this.RPC_ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
        Board.Instance.WhiteChecked = false;

    }

    public override void IsCheckMateWhitePlayer() 
    {

        base.IsCheckMateWhitePlayer();
        this.RPC_ChangeBoardState(GetBoardStatus());

    }

    public void RPC_PieceColorChange(string selectPiece)
    {

        photonView.RPC("PieceColorChange", RpcTarget.All, selectPiece);

    }

    public void RPC_ChangeBoardState(BOARDSTATUS state)
    {

        photonView.RPC("ChangeBoardState", RpcTarget.All, state);

    }

    public void RPC_PieceMove(int index)
    {

        photonView.RPC("PieceMove", RpcTarget.All, index);

    }

    public void RPC_ClearSelectedPiece()
    {

        photonView.RPC("ClearSelectedPiece", RpcTarget.All);

    }

    public void RPC_ChangePlayerTurn(PlayerColor player)
    {

        photonView.RPC("ChangePlayerTurn", RpcTarget.All, player);

    }
    public void RPC_OnePieceOrganize(PlayerColor player, PIECENAME pieceName, int index)
    {

        photonView.RPC("OnePieceOrganize", RpcTarget.All, player, pieceName, index);

    }

    #region RPC_calls 
    [PunRPC]
    public override void PieceColorChange(string selectPiece)
    {

        base.PieceColorChange(selectPiece);

    }

    [PunRPC]
    public override void ChangeBoardState(BOARDSTATUS state)
    {

        //Debug.Log(state);
        base.ChangeBoardState(state);
        //UIManager.Instance.UpdateBoardStatusBanner(state.ToString());
    }

    [PunRPC]
    public override void PieceMove(int index)
    {

        base.PieceMove(index);

    }
    [PunRPC]
    public override void ClearSelectedPiece()
    {

        base.ClearSelectedPiece();

    }
    [PunRPC]
    public override void ChangePlayerTurn(PlayerColor color)
    {

        base.ChangePlayerTurn(color);  

    }

    [PunRPC]
    public override void OnePieceOrganize(PlayerColor player, PIECENAME pieceName, int index)
    {

        base.OnePieceOrganize(player, pieceName, index);    

    }

    #endregion
}
