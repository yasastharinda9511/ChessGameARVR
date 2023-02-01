using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Text;
using UnityEngine.Apple;
using System.Drawing;
using System;

enum AIGameState { 
    
    BeginGame ,
    Middle ,
    End 
}
public struct MoveEvaluation {

    public float EvaluationValue { get; set; }
    public int Index { get; set; }
    public int GotoIndex { get; set; }
    public bool Pawnpromotion { get; set; }
    public PIECENAME PromotionPiece { get; set; }

    public MoveEvaluation(float evaluationValue , int index , int gotoindex) { 
    
        this.EvaluationValue = evaluationValue;
        this.Index = index;
        this.GotoIndex = gotoindex;
        Pawnpromotion = false;
        PromotionPiece = PIECENAME.NOPIECE;
    }

    public MoveEvaluation(float evaluationValue, int index, int gotoindex , PIECENAME promotionPiece)
    {

        this.EvaluationValue = evaluationValue;
        this.Index = index;
        this.GotoIndex = gotoindex;
        Pawnpromotion = true;
        this.PromotionPiece = promotionPiece;
    }

}

public struct BoardUndoNode {

    public Piece EliminatedPiece { get; set; }
    public Piece PromotedPawnPiece { get; set; }
    public bool MovePieceOneIsFirstMove { get; set; }
    public bool MovePieceTwoIsFirstMove { get; set; }
    public Moves UndoMove { get; set; }
    public BoardUndoNode(Moves undoMove,Piece eliminatedPiece, Piece promotedPawnPiece, bool movePieceOneIsFirstMove , bool movePieceTwoIsFirstMove) {

        this.EliminatedPiece = eliminatedPiece;
        this.PromotedPawnPiece = promotedPawnPiece;
        this.MovePieceOneIsFirstMove = movePieceOneIsFirstMove;
        this.MovePieceTwoIsFirstMove = movePieceTwoIsFirstMove;
        this.UndoMove = undoMove;

    }

}
public class AIPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public static AIPlayer Instance { get; set; }

    public PlayerColor AIPlayerColor { get; set; }

    private MoveEvaluation threadMove;

    public bool aiThreadStart;

    Thread ai;                                                                                             

    float[] pawnPeiceSquareTable = new float[64] {0,0,0,0,0,0,0,0,
                                            5,5,5,5,5,5,5,5,
                                            1,1,2,3,3,2,1,1,
                                            1,1,2,3,3,2,1,1,
                                            0.5f,0.5f,1,2.5f,2.5f,1,0.5f,0.5f,
                                            0,0,0,2,2,0,0,0,
                                            0.5f,-0.5f,-1,0,0,-1,-0.5f,0.5f,
                                            0,0,0,0,0,0,0,0
                                            };

    float[] rookPeiceSquareTable = new float[64] {0,0,0,0,0,0,0,0,
                                            0.5f, 1, 1, 1, 1, 1, 1, 0.5f,
                                            -0.5f, 0, 0, 0, 0, 0, 0, -0.5f,
                                            -0.5f, 0, 0, 0, 0, 0, 0, -0.5f,
                                            -0.5f, 0, 0, 0, 0, 0, 0, -0.5f,
                                           -0.5f, 0, 0, 0, 0, 0, 0, -0.5f,
                                            -0.5f, 0, 0, 0, 0, 0, 0, -0.5f,
                                            0,0,0,0,0,0,0,0
                                            };
    int[] kingSquareTable = new int[64] {-3, -4, -4, -5, -5, -4, -4, -3,
                                            -3, -4, -4, -5, -5, -4, -4, -3,
                                            -3, -4, -4, -5, -5, -4, -4, -3,
                                            -3, -4, -4, -5, -5, -4, -4, -3,
                                            -2, -3, -3, -4, -4, -3, -3, -2,
                                            -1, -2, -2, -2, -2, -2, -2, -1,
                                            2, 2, 0, 0, 0, 0, 2, 2,
                                            2, 3, 1, 0, 0, 1, 3, 2
                                            };
    float[] bishopPeiceSquareTable = new float[64] {-2, -1, -1, -1, -1, -1, -1, -2,
                                            -1, 0, 0, 0, 0, 0, 0, -1,
                                            -1, 0, 0.5f, 1, 1, 0.5f, 0, -1,
                                            -1, 0.5f, 0.5f, 1, 1, 0.5f, 0.5f, -1,
                                            -1, 0, 1, 1, 1, 1, 0, -1,
                                            -1, 1, 1, 1, 1, 1, 1, -1,
                                            -1, 0.5f, 0, 0, 0, 0, 0.5f, -1,
                                            -2, -1, -1, -1, -1, -1, -1, -2
                                            };
    float [] queenSquareTable = new float[64] {-2, -1, -1, -0.5f, -0.5f, -1, -1, -2,
                                            -1, 0, 0, 0, 0, 0, 0, -1,
                                            -1, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, -1,
                                            -0.5f, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, -0.5f,
                                            0, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, -0.5f,
                                            -1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0, -1,
                                            -1, 0, 0.5f, 0, 0, 0, 0, -1,
                                            -2, -1, -1, -0.5f, -0.5f, -1, -1, -2
                                            };
    float[] knightPeiceSquareTable = new float[64] {-5, -4, -3, -3, -3, -3, -4, -5,
                                            -4, -2, 0, 0, 0, 0, -2, -4,
                                            -3, 0, 1, 1.5f, 1.5f, 1, 0, -3,
                                            -3, 0.5f, 1.5f, 2, 2, 1.5f, 0.5f, -3,
                                            -3, 0, 1.5f, 2, 2, 1.5f, 0, -3.0f,
                                            -3, 0.5f, 1, 1.5f, 1.5f, 1, 0.5f, -3,
                                            -4, -2, 0, 0.5f, 0.5f, 0, -2, -4,
                                            -5, -4, -3, -3, -3, -3, -4, -5
                                            };

    int GloabalCount = 0;
    int findInsideCount = 0;

    Dictionary<ulong , MoveEvaluation> EvalBoads = new Dictionary<ulong, MoveEvaluation>();

    List<ulong> HashValueList = new List<ulong>();
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        aiThreadStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!aiThreadStart) return;
        //else
        //{

        //    if (!ai.IsAlive)
        //    {

        //        Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);

        //        if (AIPlayer.Instance.AIPlayerColor == PlayerColor.WHITE)
        //        {
        //            GameManagerAI.Instance.IsCheckBlackPlayer();

        //        }
        //        else if (AIPlayer.Instance.AIPlayerColor == PlayerColor.BLACK)
        //        {

        //            GameManagerAI.Instance.IsCheckWhitePlayer();

        //        }
        //        aiThreadStart = false;

        //    }

        //}
    }

    public void DelayAIMoveCall() {

        Invoke("AIMove" , 1.5f );
    
    }
    public void AIMove()
    {
        aiThreadStart = true;
        //ai = new Thread(() => GetMove(Board.Instance.ChessBoard, this.AIPlayerColor, 2, float.MinValue, float.MaxValue));
        //ai.Start();
        GloabalCount = 0;
        findInsideCount = 0;

        var watch = System.Diagnostics.Stopwatch.StartNew();
        ulong hash = Board.Instance.GetHashValue();

        threadMove = GetAIMove(this.AIPlayerColor, 2 , float.MinValue, float.MaxValue);

        if (threadMove.Pawnpromotion) 
        {
            Board.Instance.ChessBoard[threadMove.Index].Eliminate();
            Board.Instance.OnePieceOrganize(AIPlayerColor, threadMove.PromotionPiece, threadMove.GotoIndex);
        } 
        else Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);
        
        watch.Stop();
        if (hash == Board.Instance.GetHashValue()) Debug.Log("Same Hash value value");

        //Board.Instance.ActivePiecesUpdate();

        if (AIPlayer.Instance.AIPlayerColor == PlayerColor.WHITE)
        {
            GameManagerAI.Instance.IsCheckBlackPlayer();

        }
        else if (AIPlayer.Instance.AIPlayerColor == PlayerColor.BLACK)
        {

            GameManagerAI.Instance.IsCheckWhitePlayer();

        }

        Board.Instance.UpdateValidMoves();
        DebugEvaluateBoard(Board.Instance.ChessBoard);

    }
    float EvaluateBoard(Piece[] board) {
        float value = 0;

        foreach (Piece piece in Board.Instance.WhiteActivePieces)
        {
            value += (piece.PieceValue + piece.ValidMoves.Count) ;

        }
        foreach (Piece piece in Board.Instance.BlackActivePieces)
        {
            
            value += (piece.PieceValue - piece.ValidMoves.Count);
        }

        return value;
        
    }
    float DebugEvaluateBoard(Piece[] board)
    {

        float value = 0;
        foreach (Piece piece in Board.Instance.WhiteActivePieces)
        {
            PrintPiece(piece);
            value += (piece.PieceValue);
            if (piece.ThreatScore > 0) value -= piece.ThreatScore;
        }
        foreach (Piece piece in Board.Instance.BlackActivePieces)
        {
            PrintPiece(piece);
            value += (piece.PieceValue);
            if (piece.ThreatScore < 0) value -= piece.ThreatScore;
        }

        Debug.Log("Score value is :" + value);
        return value;

    }

    void PrintPiece(Piece p) {

        string s = "PieceName :" + p.playerColor.ToString() + p.pieceName.ToString() +
                   " Index :" + p.Index.ToString() +
                   " ValidMoveCount :" + p.ValidMoves.Count().ToString() +
                   " AttackingValue :" + p.AttackingMovesScore.ToString() +
                   " ThreatValue :" + p.ThreatScore.ToString() +
                   " Defending Value" + p.DefendingMovesScore.ToString() ;

        //Debug.Log(s);

    }

    List<Moves> MoveOrdering(PlayerColor player) {

        List<Moves> moveList = new List<Moves>();

        List<Piece> ActivePieces = (player == PlayerColor.WHITE) ? Board.Instance.WhiteActivePieces : Board.Instance.BlackActivePieces;

        PIECENAME[] promotedPiece = new PIECENAME[4] { PIECENAME.QUEEN, PIECENAME.ROOK, PIECENAME.KNIGHT, PIECENAME.BISHOP };

        foreach (Piece p in ActivePieces) 
        {
            foreach (Moves i in p.ValidMoves.ToArray()) moveList.Add(i);
        }

        List<Moves> orderedMoveList;
        orderedMoveList = moveList.OrderByDescending(x => x.MoveScore).ToList();

        return orderedMoveList;

    }

    public MoveEvaluation GetAIMove(PlayerColor playerTurn, int depth, float alpha, float beta ) 
    {

        List<Moves> Validmoves = new List<Moves>();

        PlayerColor opponentColor = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        Board.Instance.UpdateValidMoves();

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        MoveEvaluation move;
        MoveEvaluation node = new MoveEvaluation (0,0,0);

        if (Board.Instance.IsCheckMateWhitePlayer())
        {
            return new MoveEvaluation(-9999, -1, -1);

        }

        else if (Board.Instance.IsCheckMateBlackPlayer())
        {
            return new MoveEvaluation(9999, -1, -1);
        }

        else if (depth == -1)
        {
            GloabalCount++;
            return new MoveEvaluation(EvaluateBoard(Board.Instance.ChessBoard), -1, -1);

        }

        Validmoves = (playerTurn == PlayerColor.WHITE) ? MoveOrdering(PlayerColor.WHITE) : MoveOrdering(PlayerColor.BLACK);

        if (playerTurn == PlayerColor.WHITE)
        {
            foreach (Moves i in Validmoves)
            {
                ulong prevHash = Board.Instance.GetHashValue();
                i.DoMove();

                move = GetAIMove(opponentColor, depth - 1, alpha, beta);

                alpha = Mathf.Max(alpha, move.EvaluationValue);
                maxValue = Mathf.Max(maxValue, move.EvaluationValue);

                if (maxValue == move.EvaluationValue)
                {
                    if (i.MoveType == MOVETYPE.PAWN_PROMOTION) 
                    { 
                        PawnPromotionMove p = (PawnPromotionMove)i;
                        node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination, p.PromotedPieceName);
                    } 
                    else node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);

                }

                i.UndoMove();

                if (beta <= alpha) break;

            }

        }
        else {

            foreach (Moves i in Validmoves)
            {
                ulong prevHash = Board.Instance.GetHashValue();

                i.DoMove();

                move = GetAIMove(opponentColor, depth - 1, alpha, beta);

                beta = Mathf.Min(beta, move.EvaluationValue);
                minValue = Mathf.Min(minValue, move.EvaluationValue);

                if (minValue == move.EvaluationValue)
                {

                    if (i.MoveType == MOVETYPE.PAWN_PROMOTION)
                    {
                        PawnPromotionMove p = (PawnPromotionMove)i;
                        node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination, p.PromotedPieceName);
                    }
                    else node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);

                }

                i.UndoMove();

                if (beta <= alpha) break;
            }

        }

        return node;

    }

}
