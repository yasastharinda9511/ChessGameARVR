using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerBase : MonoBehaviour
{
    [SerializeField]
    protected PlayerColor player;
    GameObject selectedObject;

    PieceModel selecetedPiece;
    int goToIndex;

    public void ChangeState(GameManagerBase gameManager ,  bool touched , GameObject gameObject) 
    {
        PieceModel pieceModel = gameObject.GetComponent<PieceModel>();
        ValidBox validBox = gameObject.GetComponent<ValidBox>();

        if (pieceModel != null && pieceModel.Piece.playerColor != player) return;

        if (player == PlayerColor.WHITE &&
            (gameManager.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN || gameManager.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK)) 
        {
            if (touched && (pieceModel != null || selectedObject != pieceModel.transform.gameObject))
            {
                ClearSelectedPiece();
                pieceModel.ShowValidMoves();
                selectedObject = (pieceModel != null) ? pieceModel.transform.gameObject : null;
                selecetedPiece = pieceModel;
                gameManager.ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT);
            }
            else if (pieceModel != null)
            {
                ClearSelectedPiece();
                pieceModel.ShowValidMoves();
            }
        }
        else if(player == PlayerColor.WHITE &&
                gameManager.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT)
        {
            
            if (touched && validBox != null)
            {
                goToIndex = validBox.ValidIndex;
                MovePiece(goToIndex, selecetedPiece);
                gameManager.ChangeBoardState(BOARDSTATUS.BLACK_PLAYER_TURN);
            }
            else if (touched && validBox == null) 
            { 
                if (pieceModel != null && pieceModel != selecetedPiece) 
                {
                    ClearSelectedPiece();
                    gameManager.ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_TURN);
                    selectedObject = null;
                    selecetedPiece = null;
                }
                
            }

        }
    }

    public virtual void ClearSelectedPiece()
    {
        ObjectPool.instance.InactiveAllActive();
    }

    public void MovePiece(int index , PieceModel selectedPiece)
    {
        ClearSelectedPiece();
        selecetedPiece.ChangePosition(goToIndex);
    }

}
