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

        GetMove(this.AIPlayerColor, 1 , float.MinValue, float.MaxValue);

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
    public MoveEvaluation GetMove(PlayerColor playerTurn, int depth, float alpha , float beta)
    {

        int originalIndex;

        List<Moves> Validmoves = new List<Moves>();

        MoveEvaluation node = new MoveEvaluation(0,0,0); 

        PlayerColor opponentColor = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;
        
        Board.Instance.UpdateValidMoves();

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

        PlayerColor p = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        //List<int> opponetValidMoves = GetOppnentAllValidMoves(p);

        Piece originalPiece = null;
        bool IsFirstMove;
        MoveEvaluation move;

        if (playerTurn == PlayerColor.WHITE) {

            Validmoves = moveOrdering(PlayerColor.WHITE);

            float maxValue = float.MinValue;

            foreach (Moves i in Validmoves)
            {

                ulong prevHash = Board.Instance.GetHashValue();

                Board.Instance.UpdateHashValue(i.Source, i.Destination);
                
                if (Board.Instance.ChessBoard[i.Destination] != null) {

                    originalPiece = Board.Instance.ChessBoard[i.Destination];
                    Board.Instance.RemoveActivePiece(originalPiece);
                } 
                else originalPiece = null;

                Board.Instance.ChessBoard[i.Destination] = Board.Instance.ChessBoard[i.Source];
                Board.Instance.ChessBoard[i.Source] = null;

                originalIndex = Board.Instance.ChessBoard[i.Destination].Index;
                IsFirstMove = Board.Instance.ChessBoard[i.Destination].isFirstMove;

                Board.Instance.ChessBoard[i.Destination].Index = i.Destination;
                Board.Instance.ChessBoard[i.Destination].isFirstMove = false;

                move = GetMove(opponentColor, depth - 1, alpha, beta);

                alpha = Mathf.Max(alpha, move.EvaluationValue);
                maxValue = Mathf.Max(maxValue , move.EvaluationValue);
                if ( maxValue == move.EvaluationValue) {

                    node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);
                    
                }

                Board.Instance.ChessBoard[i.Source] = Board.Instance.ChessBoard[i.Destination];
                Board.Instance.ChessBoard[i.Source].Index = i.Source;
                Board.Instance.ChessBoard[i.Source].isFirstMove = IsFirstMove;

                Board.Instance.ChessBoard[i.Destination] = originalPiece;
                if (originalPiece != null) {

                    Board.Instance.AddActivePiece(originalPiece);
                   
                }

                if (move.EvaluationValue >= beta) {

                    break;
                } 

            }

            threadMove = node;
            return threadMove;

        } else {
            

            Validmoves = moveOrdering(PlayerColor.BLACK);


            //Debug.Log("Black valid moves count is :" + Validmoves.Count);


            float minValue = float.MaxValue;
            foreach (Moves i in Validmoves)
            {

                ulong prevHash = Board.Instance.GetHashValue();

                Board.Instance.UpdateHashValue(i.Source, i.Destination);

                if (Board.Instance.ChessBoard[i.Destination] != null) {

                    originalPiece = Board.Instance.ChessBoard[i.Destination];
                    Board.Instance.RemoveActivePiece(originalPiece);

                } 
                else originalPiece = null;

                Board.Instance.ChessBoard[i.Destination] = Board.Instance.ChessBoard[i.Source];
                Board.Instance.ChessBoard[i.Source] = null;

                originalIndex = Board.Instance.ChessBoard[i.Destination].Index;
                IsFirstMove = Board.Instance.ChessBoard[i.Destination].isFirstMove;

                Board.Instance.ChessBoard[i.Destination].Index = i.Destination;
                Board.Instance.ChessBoard[i.Destination].isFirstMove = false;

                move = GetMove(opponentColor, depth - 1, alpha, beta);

                //Debug.Log("white move evaluation value is :" + move.EvaluationValue);
                beta = Mathf.Min(beta, move.EvaluationValue);
                minValue = Mathf.Min(minValue, move.EvaluationValue);

                //Debug.Log("######## Black values :" + move.EvaluationValue);

                if (minValue == move.EvaluationValue)
                {
                    node = new MoveEvaluation(move.EvaluationValue, i.Source, i.Destination);
                }

                Board.Instance.ChessBoard[i.Source] = Board.Instance.ChessBoard[i.Destination];

                Board.Instance.ChessBoard[i.Source].Index = i.Source;
                Board.Instance.ChessBoard[i.Source].isFirstMove = IsFirstMove;

                Board.Instance.ChessBoard[i.Destination] = originalPiece;

                //Debug.Log(Board.Instance.PrintBoard());

                if (originalPiece != null)
                {

                    Board.Instance.AddActivePiece(originalPiece);

                }

                if (move.EvaluationValue <= alpha)
                {

                    break;

                }
            }

            threadMove = node;
            return threadMove;
            
        }
    }
    float EvaluateBoard(Piece[] board) {
        float value = 0;

        foreach (Piece piece in Board.Instance.WhiteActivePieces)
        {
            value += (piece.PieceValue + 0.1f * piece.ValidMoves.Count);

        }
        foreach (Piece piece in Board.Instance.BlackActivePieces)
        {
            
            value += (piece.PieceValue - 0.1f * piece.ValidMoves.Count);
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

    List<Moves> moveOrdering( PlayerColor player) { 
        
        List<Moves> moveList = new List<Moves>();

        int orderingScore;

        if (player == PlayerColor.WHITE)
        {

            foreach (var p in Board.Instance.WhiteActivePieces)
            {

                foreach (Moves  i in p.ValidMoves.ToArray())
                {
                    moveList.Add(i);

                }

            }

        }
        else {

            foreach (var p in Board.Instance.BlackActivePieces)
            {

                foreach (Moves i in p.ValidMoves.ToArray())
                {

                    orderingScore = 0;
                    moveList.Add(i);

                }

            }

        }

        return moveList;

    }

    MoveEvaluation GetAIMove(PlayerColor playerTurn, int depth, float alpha, float beta  ) {

        int originalIndex;

        List<Moves> Validmoves = new List<Moves>();

        MoveEvaluation node = new MoveEvaluation(0, 0, 0);

        PlayerColor opponentColor = (playerTurn == PlayerColor.WHITE) ? PlayerColor.BLACK : PlayerColor.WHITE;

        Board.Instance.UpdateValidMoves();

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

        Validmoves = (playerTurn == PlayerColor.WHITE) ? moveOrdering(PlayerColor.WHITE) : moveOrdering(PlayerColor.BLACK);

        foreach (Moves i in Validmoves)
        {

        }



        }
}
