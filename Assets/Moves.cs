using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moves
{
    public int Source { get; set; }
    public int Destination { get; set; }
    public MOVETYPE MoveType { get; set; }
    public PIECENAME AttackedPiece { get; set; }

    public Moves(int source , int destination , MOVETYPE moveType , PIECENAME attackedPiece ) 
    {
        this.Source = source;
        this.Destination = destination;
        this.MoveType = moveType;
        this.AttackedPiece = AttackedPiece;
    }

}
