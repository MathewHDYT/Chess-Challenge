using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        int [] pieceValues = {
            0,      // Null
            100,    // Pawn
            325,    // Knight
            325,    // Bishop
            500,    // Rook
            975,    // Queem
            10000   // King
        };
        Move[] allMoves = board.GetLegalMoves();

        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
        int highestValueCapture = 0;

        foreach (Move move in allMoves)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);

            // Use the current move if it would result in a mate, even if another move would result in a higher value
            if (isMate)
            {
                moveToPlay = move;
                break;
            }

            // Check if the move would expose the attacking piece to a possible capture,
            // capturing a rook with a queen is not worth it even if it might be the best possible legal move
            bool targetProtected = board.SquareIsAttackedByOpponent(move.TargetSquare);
            // Gets the value of the piece we are currently evaluating
            int currentPieceValue = pieceValues[(int)move.MovePieceType];
            // Value of the piece the current move could capture
            int capturedPieceValue = pieceValues[(int)move.CapturePieceType];

            // Value of a promotion is the difference between the previous piece and the changed piece
            int promotionPieceValue = 0;
            if (move.IsPromotion) {
                promotionPieceValue = pieceValues[(int)move.PromotionPieceType] - currentPieceValue;
            }

            // Subtract points if the piece can be captured if the move is executed
            int lossOnCapture = 0;
            if (move.IsEnPassant || targetProtected) {
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
