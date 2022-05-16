using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Text;

enum AIGameState { 
    
    BeginGame ,
    Middle ,
    End 
}
public struct MoveEvaluation {

    public float EvaluationValue { get; set; }
    public int Index { get; set; }
    public int GotoIndex { get; set; }

    public MoveEvaluation(float evaluationValue , int index , int gotoindex) { 
    
        this.EvaluationValue = evaluationValue;
        this.Index = index;
        this.GotoIndex = gotoindex;
    
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
        // the code that you want to measure comes here
        ulong hash = Board.Instance.GetHashValue();

        threadMove = GetAIMove(this.AIPlayerColor, 1 , float.MinValue, float.MaxValue);

        Debug.Log("Thread Move " + threadMove.Index + " Goto Index is " + threadMove.GotoIndex);
        watch.Stop();

        if (hash == Board.Instance.GetHashValue()) Debug.Log("Same Hash value value");

        Debug.Log("@@@@@ Time taken is :" + (watch.ElapsedMilliseconds));


        Board.Instance.ActivePiecesUpdate();

        Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);

        Debug.Log("permutation count is  :" + GloabalCount);
        Debug.Log("repeat permutations count is  :" + findInsideCount);

        if (AIPlayer.Instance.AIPlayerColor == PlayerColor.WHITE)
        {
            GameManagerAI.Instance.IsCheckBlackPlayer();

        }
        else if (AIPlayer.Instance.AIPlayerColor == PlayerColor.BLACK)
        {

            GameManagerAI.Instance.IsCheckWhitePlayer();

        }

        Debug.Log("Final Board is :" + Board.Instance.PrintBoard());

        Board.Instance.UpdateValidMoves();
        Debug.Log(Board.Instance.PrintBoardNow());

        DebugEvaluateBoard(Board.Instance.ChessBoard);

    }
    float EvaluateBoard(Piece[] board) {
        float value = 0;

        foreach (Piece piece in Board.Instance.WhiteActivePieces)
        {
            value += (piece.PieceValue);

        }
        foreach (Piece piece in Board.Instance.BlackActivePieces)
        {
            
            value += (piece.PieceValue);
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

        Debug.Log(s);

    }

    List<Moves> MoveOrdering(PlayerColor player) {

        List<Moves> moveList = new List<Moves>();

        List<Piece> ActivePieces = (player == PlayerColor.WHITE) ? Board.Instance.WhiteActivePieces : Board.Instance.BlackActivePieces;

        PIECENAME[] promotedPiece = new PIECENAME[4] { PIECENAME.QUEEN, PIECENAME.ROOK, PIECENAME.KNIGHT, PIECENAME.BISHOP };

        foreach (Piece p in ActivePieces) {

            foreach (Moves i in p.ValidMoves.ToArray())
            {
                if (i.MoveType == MOVETYPE.PAWN_PROMOTION)
                {
                    foreach (PIECENAME pro in promotedPiece) {

                        moveList.Add(new Moves(source : i.Source , destination: i.Destination , moveType : i.MoveType , attackedPiece: PIECENAME.NOPIECE , promotedPiece: pro));

                    }

                }
                moveList.Add(i);

            }

        }

        List<Moves> orderedMoveList = moveList.OrderByDescending(x => x.MoveScore).ToList();

        return orderedMoveList;

    }

    MoveEvaluation GetAIMove(PlayerColor playerTurn, int depth, float alpha, float beta  ) {

        BoardUndoNode undoMove ;

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
            return new MoveEvaluation(+9999, -1, -1);
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

                undoMove = DoMove(i, playerTurn);

                move = GetAIMove(opponentColor, depth - 1, alpha, beta);

                alpha = Mathf.Max(alpha, move.EvaluationValue);
                maxValue = Mathf.Max(maxValue, move.EvaluationValue);

                if (maxValue == move.EvaluationValue)
                {

                    node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);

                }

                UndoMove(i, undoMove);

                if (beta <= move.EvaluationValue)
                {

                    break;

                }

            }

        }
        else {

            foreach (Moves i in Validmoves)
            {
                ulong prevHash = Board.Instance.GetHashValue();

                undoMove = DoMove(i, playerTurn);

                move = GetAIMove(opponentColor, depth - 1, alpha, beta);

                beta = Mathf.Min(alpha, move.EvaluationValue);
                minValue = Mathf.Min(minValue, move.EvaluationValue);

                if (minValue == move.EvaluationValue)
                {

                    node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);

                }

                UndoMove(i, undoMove);

                if (move.EvaluationValue <= alpha)
                {

                    break;

                }

            }

        }

        return node;

    }

    BoardUndoNode DoMove (Moves move , PlayerColor playerTurn)
    {

        Piece eliminatedPiece = null;
        Piece promotedPawn = null ;
        Piece promotedPiece = null;
        bool movePieceOneIsFirstMove = false;
        bool movePieceTwoIsFirstMove = false;

        PlayerColor opponent = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        if (move.MoveType == MOVETYPE.PAWN_PROMOTION)
        {
            if (Board.Instance.ChessBoard[move.Destination] != null)
            {

                eliminatedPiece = Board.Instance.ChessBoard[move.Destination];
                Board.Instance.RemoveActivePiece(eliminatedPiece);

            }

            promotedPawn = Board.Instance.ChessBoard[move.Source];
            Board.Instance.RemoveActivePiece(promotedPawn);

            if (move.PromotedPiece == PIECENAME.QUEEN)
            {

                promotedPiece = new Queen(playerTurn, move.Destination);

            }
            else if (move.PromotedPiece == PIECENAME.ROOK)
            {

                promotedPiece = new Rook(playerTurn, move.Destination);

            }
            else if (move.PromotedPiece == PIECENAME.KNIGHT)
            {

                promotedPiece = new Knight(playerTurn, move.Destination);

            } else if (move.PromotedPiece == PIECENAME.BISHOP)
            {

                promotedPiece = new Bishop(playerTurn , move.Destination);
            
            }

            Board.Instance.ChessBoard[move.Destination] = promotedPiece;
            Board.Instance.AddActivePiece(promotedPiece);

        }
        else if (move.MoveType == MOVETYPE.FREE)
        {

            Board.Instance.ChessBoard[move.Destination] = Board.Instance.ChessBoard[move.Source];
            movePieceOneIsFirstMove = Board.Instance.ChessBoard[move.Destination].isFirstMove;

            Board.Instance.ChessBoard[move.Destination].isFirstMove = false;

            Board.Instance.ChessBoard[move.Destination].Index = move.Destination;
            Board.Instance.ChessBoard[move.Source] = null;

        }
        else if (move.MoveType == MOVETYPE.ATTACKING)
        {
            eliminatedPiece = Board.Instance.ChessBoard[move.Destination];
            Board.Instance.RemoveActivePiece(eliminatedPiece);

            Board.Instance.ChessBoard[move.Destination] = Board.Instance.ChessBoard[move.Source];

            movePieceOneIsFirstMove = Board.Instance.ChessBoard[move.Destination].isFirstMove; // First Piece queen

            Board.Instance.ChessBoard[move.Destination].isFirstMove = false;


            Board.Instance.ChessBoard[move.Destination].Index = move.Destination;
            Board.Instance.ChessBoard[move.Source] = null;


        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_BLACK_LEFT)
        {
            Board.Instance.ChessBoard[58] = Board.Instance.ChessBoard[60];
            Board.Instance.ChessBoard[58].Index = 58;
            movePieceOneIsFirstMove = Board.Instance.ChessBoard[58].isFirstMove;
            Board.Instance.ChessBoard[58].isFirstMove = false;

            Board.Instance.ChessBoard[60] = null;

            Board.Instance.ChessBoard[59] = Board.Instance.ChessBoard[56];
            Board.Instance.ChessBoard[59].Index = 59;
            movePieceTwoIsFirstMove = Board.Instance.ChessBoard[59].isFirstMove;
            Board.Instance.ChessBoard[59].isFirstMove = false;

            Board.Instance.ChessBoard[56] = null;
        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_BLACK_RIGHT)
        {
            Board.Instance.ChessBoard[62] = Board.Instance.ChessBoard[60];
            Board.Instance.ChessBoard[62].Index = 62;
            movePieceOneIsFirstMove = Board.Instance.ChessBoard[62].isFirstMove;
            Board.Instance.ChessBoard[62].isFirstMove = false;

            Board.Instance.ChessBoard[60] = null;

            Board.Instance.ChessBoard[61] = Board.Instance.ChessBoard[63];
            Board.Instance.ChessBoard[61].Index = 61;
            movePieceTwoIsFirstMove = Board.Instance.ChessBoard[61].isFirstMove;
            Board.Instance.ChessBoard[61].isFirstMove = false;

            Board.Instance.ChessBoard[61] = null;

        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_WHITE_LEFT)
        {
            Board.Instance.ChessBoard[2] = Board.Instance.ChessBoard[4];
            Board.Instance.ChessBoard[2].Index = 2;
            movePieceOneIsFirstMove = Board.Instance.ChessBoard[2].isFirstMove;
            Board.Instance.ChessBoard[2].isFirstMove = false;

            Board.Instance.ChessBoard[4] = null;

            Board.Instance.ChessBoard[3] = Board.Instance.ChessBoard[0];
            Board.Instance.ChessBoard[3].Index = 3;
            movePieceTwoIsFirstMove = Board.Instance.ChessBoard[3].isFirstMove;
            Board.Instance.ChessBoard[3].isFirstMove = false;

            Board.Instance.ChessBoard[0] = null;

        } else if (move.MoveType == MOVETYPE.KING_CASTLING_WHITE_RIGHT) 
        {

            Board.Instance.ChessBoard[6] = Board.Instance.ChessBoard[4];
            Board.Instance.ChessBoard[6].Index = 6;
            movePieceOneIsFirstMove = Board.Instance.ChessBoard[6].isFirstMove;
            Board.Instance.ChessBoard[6].isFirstMove = false;

            Board.Instance.ChessBoard[4] = null;

            Board.Instance.ChessBoard[5] = Board.Instance.ChessBoard[7];
            Board.Instance.ChessBoard[5].Index = 5;
            movePieceTwoIsFirstMove = Board.Instance.ChessBoard[5].isFirstMove;
            Board.Instance.ChessBoard[5].isFirstMove = false;

            Board.Instance.ChessBoard[7] = null;
        }

        return new BoardUndoNode(undoMove : move , eliminatedPiece : eliminatedPiece , promotedPawnPiece : promotedPawn, movePieceOneIsFirstMove : movePieceOneIsFirstMove , movePieceTwoIsFirstMove : movePieceTwoIsFirstMove );

    }

    void UndoMove(Moves move , BoardUndoNode undoNode) 
    {

        if (move.MoveType == MOVETYPE.PAWN_PROMOTION)
        {

            Board.Instance.ChessBoard[move.Source] = undoNode.PromotedPawnPiece;
            if (undoNode.EliminatedPiece != null)
            {

                Board.Instance.AddActivePiece(undoNode.EliminatedPiece);
                Board.Instance.RemoveActivePiece(Board.Instance.ChessBoard[move.Destination]);

            }

            Board.Instance.ChessBoard[move.Source] = undoNode.PromotedPawnPiece;
            Board.Instance.ChessBoard[move.Destination] = undoNode.EliminatedPiece;

        }
        else if (move.MoveType == MOVETYPE.FREE)
        {

            Board.Instance.ChessBoard[move.Source] = Board.Instance.ChessBoard[move.Destination];
            Board.Instance.ChessBoard[move.Source].Index = move.Source;
            Board.Instance.ChessBoard[move.Destination] = undoNode.EliminatedPiece;
            Board.Instance.ChessBoard[move.Source].isFirstMove = undoNode.MovePieceOneIsFirstMove;

        }
        else if (move.MoveType == MOVETYPE.ATTACKING)
        {

            Board.Instance.AddActivePiece(undoNode.EliminatedPiece);

            Board.Instance.ChessBoard[move.Source] = Board.Instance.ChessBoard[move.Destination];
            Board.Instance.ChessBoard[move.Source].Index = move.Source;
            Board.Instance.ChessBoard[move.Source].isFirstMove = undoNode.MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[move.Destination] = undoNode.EliminatedPiece;


        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_BLACK_LEFT)
        {

            Board.Instance.ChessBoard[60] = Board.Instance.ChessBoard[58];
            Board.Instance.ChessBoard[60].Index = 60;
            Board.Instance.ChessBoard[60].isFirstMove = undoNode.MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[58] = null;

            Board.Instance.ChessBoard[56] = Board.Instance.ChessBoard[59];
            Board.Instance.ChessBoard[56].Index = 60;
            Board.Instance.ChessBoard[56].isFirstMove = undoNode.MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[59] = null;

        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_BLACK_RIGHT)
        {

            Board.Instance.ChessBoard[60] = Board.Instance.ChessBoard[62];
            Board.Instance.ChessBoard[60].Index = 60;
            Board.Instance.ChessBoard[60].isFirstMove = undoNode.MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[62] = null;

            Board.Instance.ChessBoard[63] = Board.Instance.ChessBoard[61];
            Board.Instance.ChessBoard[63].Index = 63;
            Board.Instance.ChessBoard[63].isFirstMove = undoNode.MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[61] = null;

        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_WHITE_LEFT)
        {

            Board.Instance.ChessBoard[4] = Board.Instance.ChessBoard[2];
            Board.Instance.ChessBoard[4].Index = 4;
            Board.Instance.ChessBoard[4].isFirstMove = undoNode.MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[2] = null;

            Board.Instance.ChessBoard[0] = Board.Instance.ChessBoard[3];
            Board.Instance.ChessBoard[0].Index = 0;
            Board.Instance.ChessBoard[0].isFirstMove = undoNode.MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[3] = null;

        }
        else if (move.MoveType == MOVETYPE.KING_CASTLING_WHITE_RIGHT) 
        {

            Board.Instance.ChessBoard[4] = Board.Instance.ChessBoard[6];
            Board.Instance.ChessBoard[4].Index = 4;
            Board.Instance.ChessBoard[4].isFirstMove = undoNode.MovePieceOneIsFirstMove;

            Board.Instance.ChessBoard[6] = null; 

            Board.Instance.ChessBoard[7] = Board.Instance.ChessBoard[5];
            Board.Instance.ChessBoard[7].Index = 7;
            Board.Instance.ChessBoard[7].isFirstMove = undoNode.MovePieceTwoIsFirstMove;

            Board.Instance.ChessBoard[5] = null;
        
        }

    }

}
