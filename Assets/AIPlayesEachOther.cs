using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayesEachOther : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    AIPlayer aiPlayer;

    MoveEvaluation threadMove;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("2!I players playEach otheer");
        MoveEvaluation threadMove = aiPlayer.GetAIMove(PlayerColor.WHITE, 1, float.MinValue, float.MaxValue);

        if (threadMove.Pawnpromotion)
        {
            Board.Instance.ChessBoard[threadMove.Index].Eliminate();
            Board.Instance.OnePieceOrganize(PlayerColor.WHITE, threadMove.PromotionPiece, threadMove.GotoIndex);
        }
        else Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);



        threadMove = aiPlayer.GetAIMove(PlayerColor.BLACK, 1, float.MinValue, float.MaxValue);

        if (threadMove.Pawnpromotion)
        {
            Board.Instance.ChessBoard[threadMove.Index].Eliminate();
            Board.Instance.OnePieceOrganize(PlayerColor.BLACK, threadMove.PromotionPiece, threadMove.GotoIndex);
        }
        else Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);
    }
}
