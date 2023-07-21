using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    private int [] pieceValues = {
        0,      // Null
        100,    // Pawn
        325,    // Knight
        325,    // Bishop
        500,    // Rook
        975,    // Queem
        10000   // King
    };

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();

        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
        int highestValueCapture = 0;

        foreach (Move move in allMoves)
        {
            // Check wheter the current move would result in a checkmate or not
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);

            // Always play checkmate in one
            if (isMate)
            {
                moveToPlay = move;
                break;
            }

            // Find highest value capture
            int currentPieceValue = pieceValues[(int)move.MovePieceType];
            int capturedPieceValue = pieceValues[(int)move.CapturePieceType];

            int promotionPieceValue = 0;
            if (move.IsPromotion) {
                promotionPieceValue = pieceValues[(int)move.PromotionPieceType] - currentPieceValue;
            }

            int lossOnCapture = 0;
            if (move.IsEnPassant) {
                lossOnCapture = currentPieceValue;
            }

            int totalMoveValue = capturedPieceValue + promotionPieceValue - lossOnCapture;

            if (totalMoveValue > highestValueCapture)
            {
                moveToPlay = move;
                highestValueCapture = totalMoveValue;
            }
        }
        return moveToPlay;
    }
}
