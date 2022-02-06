using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

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
        if (!aiThreadStart) return;
        else {

            if (!ai.IsAlive) {

                Board.Instance.ChessBoard[threadMove.Index].ChangePosition(threadMove.GotoIndex);

                if (AIPlayer.Instance.AIPlayerColor == PlayerColor.WHITE)
                {
                    GameManagerAI.Instance.IsCheckBlackPlayer();

                } else if (AIPlayer.Instance.AIPlayerColor == PlayerColor.BLACK) 
                {

                    GameManagerAI.Instance.IsCheckWhitePlayer();

                }
                aiThreadStart = false;

            }
        
        }
    }

    public void DelayAIMoveCall() {

        Invoke("AIMove" , 1.5f );
    
    }
    public void AIMove()
    {
        aiThreadStart = true;
        ai = new Thread(() => GetMove(Board.Instance.ChessBoard, this.AIPlayerColor, 2 , float.MinValue, float.MaxValue));
        ai.Start();
    }

    public MoveEvaluation GetMove(Piece[] board, PlayerColor playerTurn, int depth, float alpha , float beta)
    {

        int originalIndex;
        Piece[] cloneBoard = new Piece[64];

        List<Piece> activePiece = new List<Piece>();
         
        List<MoveEvaluation> moveEvaluations = new List<MoveEvaluation>();

        MoveEvaluation node = new MoveEvaluation(0,0,0);

        board.CopyTo(cloneBoard, 0);

        Board.Instance.ChessBoard = cloneBoard;
        Board.Instance.ActivePiecesUpdate();

        PlayerColor opponentColor = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        if (Board.Instance.IsCheckMateWhitePlayer())
        {
            return  new MoveEvaluation(-9999, -1, -1);

        }
        else if (Board.Instance.IsCheckMateBlackPlayer())
        {

            return new MoveEvaluation(9999, -1, -1);
        }

        else if (depth == -1)
        {

            return new MoveEvaluation(EvaluateBoard(cloneBoard), -1, -1);

        }

        if (playerTurn == PlayerColor.WHITE)
        {
            Board.Instance.WhiteActivePieces.ForEach
                (piece =>
                {
                    activePiece.Add((Piece)piece.Clone());

                });
        }

        else if (playerTurn == PlayerColor.BLACK) {

            Board.Instance.BlackActivePieces.ForEach
                (piece =>
                {
                    activePiece.Add((Piece)piece.Clone());

                });

        } 


        Piece originalPiece;

        if (playerTurn == PlayerColor.WHITE) {

            float maxValue = float.MinValue;
            foreach (Piece piece in activePiece)
            {

                foreach (int i in piece.CalculateValidMoves().ToArray())  //  piece.CalculateValidMoves().ToArray()
                {
                    
                    if (cloneBoard[i] != null) originalPiece = (Piece)cloneBoard[i].Clone();
                    else originalPiece = null;

                    if (piece.pieceName == PIECENAME.KING) Board.Instance.BlackKingIndex = i;

                    cloneBoard[i] = (Piece)piece.Clone();
                    cloneBoard[piece.Index] = null;
                    originalIndex = cloneBoard[i].Index;
                    cloneBoard[i].Index = i;

                    MoveEvaluation move = GetMove(cloneBoard, opponentColor, depth - 1, alpha, beta);
                    alpha = Mathf.Max(alpha, move.EvaluationValue);
                    maxValue = Mathf.Max(maxValue , move.EvaluationValue);
                    if ( maxValue == move.EvaluationValue) {

                        node = new MoveEvaluation(move.EvaluationValue, originalIndex, i);
                    
                    }

                    cloneBoard[i] = originalPiece;
                    cloneBoard[originalIndex] = piece;

                    if (piece.pieceName == PIECENAME.KING) Board.Instance.BlackKingIndex = piece.Index;

                    if (move.EvaluationValue >= beta) {

                        Board.Instance.ChessBoard = board;
                        threadMove = node ;
                        return threadMove;
                    } 

                }

            }

            Board.Instance.ChessBoard = board;
            threadMove = node;
            return threadMove;

        } else {

            float minValue = float.MaxValue;
            foreach (Piece piece in activePiece)
            {
                foreach (int i in piece.CalculateValidMoves().ToArray()) //  piece.CalculateValidMoves().ToArray()
                {

                    if (cloneBoard[i] != null) originalPiece = (Piece)cloneBoard[i].Clone();
                    else originalPiece = null;

                    if (piece.pieceName == PIECENAME.KING) Board.Instance.BlackKingIndex = i;

                    cloneBoard[i] = (Piece)piece.Clone();
                    cloneBoard[piece.Index] = null;
                    originalIndex = cloneBoard[i].Index;
                    cloneBoard[i].Index = i;

                    MoveEvaluation move = GetMove(cloneBoard, opponentColor , depth - 1, alpha, beta);
                    beta = Mathf.Min(beta, move.EvaluationValue);
                    minValue = Mathf.Min(minValue, move.EvaluationValue);
                    if (minValue == move.EvaluationValue)
                    {
                        node = new MoveEvaluation(move.EvaluationValue, originalIndex, i);
                    }
                    cloneBoard[i] = originalPiece;
                    cloneBoard[originalIndex] = piece;

                    if (piece.pieceName == PIECENAME.KING) Board.Instance.BlackKingIndex = piece.Index;

                    if (move.EvaluationValue <= alpha)
                    {
                        Board.Instance.ChessBoard = board;
                        threadMove = node;
                        return threadMove;

                    }

                }

            }

            Board.Instance.ChessBoard = board;
            threadMove = node;
            return threadMove;
            
        }
    }
    float EvaluateBoard(Piece[] board) {

        float value = 0;
        foreach (Piece piece in board)
        {
            if (piece != null) {

                if (piece.pieceName == PIECENAME.PAWN && piece.playerColor == PlayerColor.WHITE) value += 10 + pawnPeiceSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.ROOK && piece.playerColor == PlayerColor.WHITE) value += 50 + rookPeiceSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.BISHOP && piece.playerColor == PlayerColor.WHITE) value += 30 + bishopPeiceSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.KNIGHT && piece.playerColor == PlayerColor.WHITE) value += 30 + knightPeiceSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.KING && piece.playerColor == PlayerColor.WHITE) value += 900  + kingSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.QUEEN && piece.playerColor == PlayerColor.WHITE) value += 90 + queenSquareTable[piece.Index];
                else if (piece.pieceName == PIECENAME.PAWN && piece.playerColor == PlayerColor.BLACK) value -= (10 + pawnPeiceSquareTable[63 -piece.Index]) ;
                else if (piece.pieceName == PIECENAME.ROOK && piece.playerColor == PlayerColor.BLACK) value -= (50 + rookPeiceSquareTable[63 - piece.Index]);
                else if (piece.pieceName == PIECENAME.BISHOP && piece.playerColor == PlayerColor.BLACK) value -= (30 + bishopPeiceSquareTable[63- piece.Index]);
                else if (piece.pieceName == PIECENAME.KNIGHT && piece.playerColor == PlayerColor.BLACK) value -= (30 + knightPeiceSquareTable[63 - piece.Index]);
                else if (piece.pieceName == PIECENAME.KING && piece.playerColor == PlayerColor.BLACK) value -= (900 + kingSquareTable[63 - piece.Index]);
                else if (piece.pieceName == PIECENAME.QUEEN && piece.playerColor == PlayerColor.BLACK) value -= (90 + queenSquareTable[63 - piece.Index]);

            }

        }

        return value;
        
    }


    bool CompareBoards(Piece[] Board1 , Piece[] Board2) {

        for (int i =0; i < 64; i ++) 
        {

            if (Board1[i] != null && Board2[i] == null)
            {

                return false;

            }
            else if (Board1[i] == null && Board2[i] != null)
            {

                return false;

            }
            else if (Board1[i] != null && Board2[i] != null &&
               (Board1[i].pieceName != Board2[i].pieceName || Board1[i].Index != Board2[i].Index || Board1[i].playerColor != Board2[i].playerColor))
            {

                return false;
            }
        
        
        }

        return true;
    
    }
}
