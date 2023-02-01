using System.Collections;
using System.Collections.Generic;
using System;

public class ZobristHashTable
{
    // Start is called before the first frame update
    public ulong[,] Table { get; set; }
    public ulong HashValue { get; set; }

    public ZobristHashTable() 
    {
        Random rand = new Random();
        Table = new ulong[64 ,12];

        for (int row = 0; row < 64; row++) {
            for (int column = 0; column < 12; column++) {

                Table[row, column] = NextInt64(rand);

            }
        
        }
    
    }

    // Update is called once per frame
    public ulong NextInt64(Random rnd)
    {
        var buffer = new byte[sizeof(ulong)];
        rnd.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }

    public ulong ComputeHash() {

        HashValue = 0;
        foreach (var i in Board.Instance.ChessBoard) {

            if (i != null) HashValue ^= Table[i.Index, GetPieceIndex(i)];
        
        }

        return HashValue;
    
    }

    private int GetPieceIndex(Piece p) {

        int offset = (p.playerColor == PlayerColor.WHITE) ? 0 : 6;

        return offset + (int)p.pieceName;
    
    }

    public void UpdateHashValue(int from, int to) {

        Piece pieceTo = Board.Instance.ChessBoard[to];
        Piece pieceFrom = Board.Instance.ChessBoard[from];

        HashValue ^= Table[pieceFrom.Index, GetPieceIndex(pieceFrom)];
        HashValue ^= Table[to, GetPieceIndex(pieceFrom)];
        if (pieceTo != null) {

            HashValue ^= Table[pieceTo.Index, GetPieceIndex(pieceTo)];

        } 
    }

    public void AddPiece(int index , Piece piece) 
    {

        if (piece != null) {

            HashValue ^= Table[index, GetPieceIndex(piece)];

        }
        

    }
}
