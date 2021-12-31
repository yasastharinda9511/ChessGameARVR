using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection { 
    
    LSHAPE, // For knight piece
    SQUARE_INFINITE, // For rook piece
    CROSS_INFINITE, // For bishops
    SINGLE_ALL_DIRECTIONS, // For King
    INFINITE_ALL_DIRECTION, // For Quen
    SINGLE_UP_AND_DOUBLE_UP, //For Pawns
    ELEMINATE_CROSS_MOVE
}

public enum PlayerColor { 
    
    WHITE,
    BLACK

}

public enum BOARDSTATUS {

    IS_CHECK_WHITE_PLAYER,
    IS_CHECK_MATE_WHITE_PLAYER,
    WHITE_PLAYER_CHECK,
    WHITE_PLAYER_TURN,
    WHITE_PLAYER_PIECE_SELECT,
    WHITE_CHECKMATE,
    IS_WHITE_PAWN_TO_QUEEN,

    IS_CHECK_BLACK_PLAYER,
    IS_CHECK_MATE_BLACK_PLAYER,
    BLACK_PLAYER_CHECK,
    BLACK_CHECKMATE,
    BLACK_PLAYER_TURN,
    BLACK_PLAYER_PIECE_SELECT,
    IS_BLACK_PAWN_TO_QUEEN,
    
    BLACK_PAWN_TO_QUEEN,
    WHITE_PAWN_TO_QUEEN
}

public enum PIECENAME { 

    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING
  
}
