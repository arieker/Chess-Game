﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace ChessUI
{
    internal class GameLogic
    {
        static void Main()
        {
            GameLoop chessGame = new GameLoop();
            chessGame.Play();
        }

    }

    public class GameLoop
    {
        private BoardLogic gameboard;

        public GameLoop()
        {
            gameboard = new BoardLogic();
        }

        public void Play()
        {
            Console.WriteLine("Compiled");
        }

    }

    // Holds a bool if it is filled and if so with what piece. 
    public class Square
    {
        public PieceLogic piece;
        public bool isFilled;

        public Square(PieceLogic piece) 
        {
            if (piece == null)
            {
                this.piece = null;
                this.isFilled = false;
            }
            else
            {
                this.piece = piece;
                this.isFilled = true;
            }
        }
    }
    
    public class BoardLogic
    {
        // 2d array of the spotsto represent the board for the logic
        private Square[,] LogicBoard = new Square[8, 8];

        //initalize the board and put the pieces in their correct starting spot
        public BoardLogic() 
        {
            // Initialize pawns for both white and black
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 1] = new Square(new PawnLogic(PieceColor.White));
                LogicBoard[x, 6] = new Square(new PawnLogic(PieceColor.Black));
            }

            // Initialize rooks
            LogicBoard[0, 0] = new Square(new RookLogic(PieceColor.White));
            LogicBoard[7, 0] = new Square(new RookLogic(PieceColor.White));
            LogicBoard[0, 7] = new Square(new RookLogic(PieceColor.Black));
            LogicBoard[7, 7] = new Square(new RookLogic(PieceColor.Black));

            // Initialize knights
            LogicBoard[1, 0] = new Square(new KnightLogic(PieceColor.White));
            LogicBoard[6, 0] = new Square(new KnightLogic(PieceColor.White));
            LogicBoard[1, 7] = new Square(new KnightLogic(PieceColor.Black));
            LogicBoard[6, 7] = new Square(new KnightLogic(PieceColor.Black));

            // Initialize bishops
            LogicBoard[2, 0] = new Square(new BishopLogic(PieceColor.White));
            LogicBoard[5, 0] = new Square(new BishopLogic(PieceColor.White));
            LogicBoard[2, 7] = new Square(new BishopLogic(PieceColor.Black));
            LogicBoard[5, 7] = new Square(new BishopLogic(PieceColor.Black));

            // Initialize queens
            LogicBoard[3, 0] = new Square(new QueenLogic(PieceColor.White));
            LogicBoard[3, 7] = new Square(new QueenLogic(PieceColor.Black));

            // Initialize kings
            LogicBoard[4, 0] = new Square(new KingLogic(PieceColor.White));
            LogicBoard[4, 7] = new Square(new KingLogic(PieceColor.Black));

            //Initalize the rest as empty
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 2] = new Square(null);
                LogicBoard[x, 3] = new Square(null);
                LogicBoard[x, 4] = new Square(null);
                LogicBoard[x, 5] = new Square(null);
            }
        }

        public bool isSquareFilled(int x, int y)
        {
            return LogicBoard[x, y].isFilled;
        }

        public void DisplayBoard()
        {
            // Display the current state of the chessboard
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Console.Write($" | {LogicBoard[row, col].piece.ToString()} | \t");
                }
                Console.WriteLine();
            }
        }
    }


    public abstract class PieceLogic
    {
        // if the piece is killed
        private bool isKilled;
        // to determine if white or black
        private bool isWhite;
        // getter
        public bool getIsWhite
        {
            get { return isWhite; }
        }
        // used for pawn first move
        // used for rook and king for castle
        private bool isFirstMove;
        public bool FirstMove
        {
            get { return isFirstMove; }
            set { isFirstMove = value; }
        }

        public PieceLogic(PieceColor color)
        {
            // initialize it to not have moved
            // used for potential extra square move in the first move
            isFirstMove = true;
            // initalize piece to be not killed
            isKilled = false;

            // set it's color
            if (color == PieceColor.White)
            {
                this.isWhite = true;
            }
            else
            {
                this.isWhite = false;
            }

            
        }

        // used by all children pieces to see if they make a vilid move
        /*
            1. check if start off board
            2. check if end off board
            3. check if end pos allignes with piece rules
            4. check if obstructed
            5. special cases (castling, pawn 1st move, knight jumps,...)
         
         */
        public abstract bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY);
        // method to see if it goes off board.
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        public override string ToString()
        {
            char color = 'w';
            if (!this.isWhite)
            {
                color = 'b';
            }
            
            return $"{color} {this.GetType()}";
        }
    }

    //TODO UPDATE FIRSTMOVE var
    public class PawnLogic : PieceLogic
    {
        //Use constructor in abstract PieceLogic class
        public PawnLogic(PieceColor color) : base(color) {}

        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            // if they start of end off the board it is not valid
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // if it is a black pawn moving we need the signFlipper to be neg 
            int signFlipper = 1;
            if(!this.getIsWhite)
            {
                signFlipper = -1;
            }

            // if one up check that it is not filled
            if (startY + (1 * signFlipper) == endY && startX == endX)
            {
                return !(glBoard.isSquareFilled(endX, endY));
            }
            
            // else if two foreward check that both are not filled
            else if (startY + (2 * signFlipper) == endY && startX == endX)
            {
                // if the square 1 foreward is not filled AND the square 2 foreward isn not filled AND it is their first move
                // then and only then can a pawn move 2 spaces foreward
                return !(glBoard.isSquareFilled(endX, endY) && glBoard.isSquareFilled(endX, endY - (1 * signFlipper))) && FirstMove;
            }
            
            // else if diag right 1 pos
            else if (startY + (1 * signFlipper) == endY && startX + (1 * signFlipper) == endX)
            {
                // must be filled to move diag
                return glBoard.isSquareFilled(endX, endY);
            }
            
            // else if diag left 1 pos
            else if (startY + (1 * signFlipper) == endY && startX - (1 * signFlipper) == endX)
            {
                // must be filled to move diag
                return glBoard.isSquareFilled(endX, endY);
            }

            // else not a valid move
            return false;

        }

    }
    public class RookLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class
        public RookLogic(PieceColor color) : base(color) { }

        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            // if they start of end off the board it is not valid
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // check if ending position alligns with piece rules 
            // either the x position does not change or the y does not change
            if(startX != endX || startY != endY) 
            {
                return false;
            }

            //check if obstructed 

            /// vertical
            if(startX == endX)
            {
                // 1 if up, -1 if down
                int direction = (endY - startY) / Math.Abs(endY - startY);
                // how long to loop
                int deltaY = Math.Abs(endY - startY);

                // check if the spaces inbetween start and finish squares are filled (not inclusive)
                for (int i = 1; i < deltaY; i++)
                {
                    // if the spot i squares away is filled then it is an invalid move
                    if (glBoard.isSquareFilled(startX, startY + (i * direction)))
                    {
                        return false;
                    }
                }

                return true;
            }

            /// horz.
            else
            {
                 // 1 if right, -1 if left
                 int direction = (endX - startX) / Math.Abs(endX - startX);
                 // how long to loop
                 int deltaX = Math.Abs(endX - startX);

                 // check if the spaces inbetween start and finish squares are filled (not inclusive)
                 for (int i = 1; i < deltaX; i++)
                 {
                    // if the spot i squares away is filled then it is an invalid move
                    if (glBoard.isSquareFilled(startX + (i * direction), startY))
                    {
                        return false;
                    }
                 }

                 return true;
            }
        }
    }
    public class KnightLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public KnightLogic(PieceColor color) : base(color) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //check to see if they start or end off the board
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // check if L shaped i.e. vertical moved 2, horz. moved 1 or vice versa
            
            /// get change in x and y
            int deltaX = Math.Abs(endX - startX);
            int deltaY = Math.Abs(endY - startY);

            ///make sure 1 of them has the value 2 and the other has the value 1
            return ((deltaX == 2 && deltaY == 1) || (deltaX == 1 && deltaY == 2));
        }
    }
    public class BishopLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public BishopLogic(PieceColor color) : base(color) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //check to see if they start or end off the board
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // verify they are moving diagonally
            // the change in x and y should bo the same
            int deltaX = Math.Abs(endX - startX);
            int deltaY = Math.Abs(endY - startY);
            if (deltaX != deltaY)
            {
                return false; // Not a diagonal move
            }

            // get the direction for the x and y in the form of 1 or -1
            int directionX = (endX - startX)/Math.Abs(endX - startX);
            int directionY = (endY - startY)/Math.Abs(endY - startY);
            
            // Check for obstructions along the diagonal path
            for (int i = 1; i < deltaX; i++)
            {
                int intermediateX = startX + i * directionX;
                int intermediateY = startY + i * directionY;

                if (glBoard.isSquareFilled(intermediateX, intermediateY))
                {
                    return false; // Path is obstructed
                }
            }

            // along a valid path, not obstructed, not off the board 
            return true;
        }
    }
    public class QueenLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public QueenLogic(PieceColor color) : base(color) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //check to see if they start or end off the board
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // used to verify they are moving diagonally, horz. or vertically
            int deltaX = Math.Abs(endX - startX);
            int deltaY = Math.Abs(endY - startY);

            // Check if the path valid and if it is obstructed
            /// if moving diagonally, check if obstructed
            if (deltaX == deltaY) 
            {
                // get the direction for the x and y in the form of 1 or -1
                int directionX = (endX - startX) / Math.Abs(endX - startX);
                int directionY = (endY - startY) / Math.Abs(endY - startY);

                // Check for obstructions along the diagonal path
                for (int i = 1; i < deltaX; i++)
                {
                    int intermediateX = startX + i * directionX;
                    int intermediateY = startY + i * directionY;

                    if (glBoard.isSquareFilled(intermediateX, intermediateY))
                    {
                        return false; // Path is obstructed
                    }
                }

                // along a valid path, not obstructed, not off the board 
                return true;

            }

            /// if vertical movement, check if obstructed
            else if(deltaX == 0 && deltaY != 0) 
            {
                // 1 if up, -1 if down
                int direction = (endY - startY) / Math.Abs(endY - startY);           

                // check if the spaces inbetween start and finish squares are filled (not inclusive)
                for (int i = 1; i < deltaY; i++)
                {
                    // if the spot i squares away is filled then it is an invalid move
                    if (glBoard.isSquareFilled(startX, startY + (i * direction)))
                    {
                        return false;
                    }
                }

                return true;
            } 

            /// horz. movment, check if obstructed
            else if (deltaX != 0 && deltaY == 0)
            {
                // 1 if right, -1 if left
                int direction = (endX - startX) / Math.Abs(endX - startX);

                // check if the spaces inbetween start and finish squares are filled (not inclusive)
                for (int i = 1; i < deltaX; i++)
                {
                    // if the spot i squares away is filled then it is an invalid move
                    if (glBoard.isSquareFilled(startX + (i * direction), startY))
                    {
                        return false;
                    }
                }

                return true;
            }

            ///else it is not a valid queen move
            else
            {
                return false;
            }

        }
    }
    public class KingLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public KingLogic(PieceColor color) : base(color) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //check to see if they start or end off the board
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // the change in x and y 
            int deltaX = Math.Abs(endX - startX);
            int deltaY = Math.Abs(endY - startY);

            // if mover 1 vert. or horz. it is valid
            if ((deltaX + deltaY) == 1)
            {
                return true;
            }
            // if they move diag. 1 square
            else if ((deltaX == 1) && (deltaY == 1))
            {
                return true;
            }
            // else moving 1 square
            else
            {
                return false;
            }
        }
    }
}
