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

    Piece selecetedPiece;
    int goToIndex;

    public void ChangeState(GameManagerBase gameManager ,  bool touched , GameObject gameObject) 
    {
        Piece piece = gameObject.GetComponent<Piece>();
        ValidBox validBox = gameObject.GetComponent<ValidBox>();

        if (piece != null && piece.playerColor != player) return;

        if (player == PlayerColor.WHITE &&
            (gameManager.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN || gameManager.GetBoardStatus() == BOARDSTATUS.WHITE_PLAYER_TURN_WITH_CHECK)) 
        {
            if (touched && (piece != null || selectedObject != piece.transform.gameObject))
            {
                ClearSelectedPiece();
                piece.ShowValidMoves();
                selectedObject = (piece != null) ? piece.transform.gameObject : null;
                selecetedPiece = piece;
                gameManager.ChangeBoardState(BOARDSTATUS.WHITE_PLAYER_PIECE_SELECT);
            }
            else if (piece != null)
            {
                ClearSelectedPiece();
                piece.ShowValidMoves();
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
                if (piece != null && piece != selecetedPiece) 
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

    public void MovePiece(int index , Piece selectedPiece)
    {
        ClearSelectedPiece();
        selecetedPiece.ChangePosition(goToIndex);
    }

}
