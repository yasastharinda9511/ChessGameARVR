using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Moves
{
    public int Source { get; set; }
    public int Destination { get; set; }
    public bool MovePieceOneIsFirstMove { get; set; }
    public MOVETYPE MoveType { get; set; }
    public float MoveScore { get; set; }

    public Moves(int source , int destination) 
    {
        this.Source = source;
        this.Destination = destination;
    }

    public abstract void DoMove();

    public abstract void UndoMove();
}
