using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace ChessUI
{

    // Holds a bool if it is filled and if so with what piece. 
    public class Square
    {
        public PieceLogic piece { get; set; }
        public bool isFilled { get; set; }

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
    
    /*
     *  Y
     * 7 |
     * 6 |
     * 5 |
     * 4 |
     * 3 |
     * 2 |
     * 1 | P | P | P | P | P | ...
     * 0 | R | N | B | Q | K | ...   White
     *     0   1   2   3   4   ...
     *    ________________________________ X
     * 
     */
    
    
    public class BoardLogic
    {
        // 2d array of the spotsto represent the board for the logic
        private Square[,] LogicBoard = new Square[8, 8];
        private KingLogic whiteKing = new KingLogic(PieceColor.White, 4, 0);
        private KingLogic blackKing = new KingLogic(PieceColor.Black, 4, 7);
        private bool whitesTurn;

        //initalize the board and put the pieces in their correct starting spot
        public BoardLogic() 
        {
            // Initialize pawns for both white and black
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 1] = new Square(new PawnLogic(PieceColor.White, x, 1));
                LogicBoard[x, 6] = new Square(new PawnLogic(PieceColor.Black, x, 6));
            }

            // Initialize rooks
            LogicBoard[0, 0] = new Square(new RookLogic(PieceColor.White, 0, 0));
            LogicBoard[7, 0] = new Square(new RookLogic(PieceColor.White, 7, 0));
            LogicBoard[0, 7] = new Square(new RookLogic(PieceColor.Black, 0, 7));
            LogicBoard[7, 7] = new Square(new RookLogic(PieceColor.Black, 7, 7));

            // Initialize knights
            LogicBoard[1, 0] = new Square(new KnightLogic(PieceColor.White, 1, 0));
            LogicBoard[6, 0] = new Square(new KnightLogic(PieceColor.White, 6, 0));
            LogicBoard[1, 7] = new Square(new KnightLogic(PieceColor.Black, 1, 7));
            LogicBoard[6, 7] = new Square(new KnightLogic(PieceColor.Black, 6, 7));

            // Initialize bishops
            LogicBoard[2, 0] = new Square(new BishopLogic(PieceColor.White, 2, 0));
            LogicBoard[5, 0] = new Square(new BishopLogic(PieceColor.White, 5, 0));
            LogicBoard[2, 7] = new Square(new BishopLogic(PieceColor.Black, 2, 7));
            LogicBoard[5, 7] = new Square(new BishopLogic(PieceColor.Black, 5, 7));

            // Initialize queens
            LogicBoard[3, 0] = new Square(new QueenLogic(PieceColor.White, 3, 0));
            LogicBoard[3, 7] = new Square(new QueenLogic(PieceColor.Black, 3, 7));

            // Initialize kings
            LogicBoard[4, 0] = new Square(whiteKing);
            LogicBoard[4, 7] = new Square(blackKing);

            //Initalize the rest as empty
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 2] = new Square(null);
                LogicBoard[x, 3] = new Square(null);
                LogicBoard[x, 4] = new Square(null);
                LogicBoard[x, 5] = new Square(null);
            }

            whitesTurn = true;
        }

        public bool isSquareFilled(int x, int y)
        {
            return LogicBoard[x, y].isFilled;
        }

        public bool move(int startX, int startY, int endX, int endY)
        {
            // check if not filled then no piece to move
            if (!LogicBoard[startX, startY].isFilled) return false;

            // get piece to be moved
            PieceLogic movedPiece = LogicBoard[startX, startY].piece;

            //can only move piece of the your color
            if (movedPiece.getIsWhite != whitesTurn) return false;

            // if the move is a knig and puts it in check, it is invalid
            if(movedPiece is KingLogic)
            {
                if (isUnderAttack(movedPiece, endX, endY)) return false;
            }

            // check if valid move
            if (movedPiece.isMoveValid(this, startX, startY, endX, endY)) 
            {
                // check if the move checks either king
                checkIfCheck(endX, endY, movedPiece, blackKing); 
                checkIfCheck(endX, endY, movedPiece, whiteKing);

                // cannot make a move that will end with your king being in check 
                if(whiteKing.isInCheck && movedPiece.getIsWhite)
                {
                    return false;
                }
                if(blackKing.isInCheck && !movedPiece.getIsWhite)
                {
                    return false;
                }

                //move pieces
                LogicBoard[endX, endY].piece = movedPiece;
                movedPiece.col = endX;
                movedPiece.row = endY;
                LogicBoard[endX, endY].isFilled = true;
                LogicBoard[startX, startY].piece = null;
                LogicBoard[startX, startY].isFilled = false;

                //remove check and flip turn
                whitesTurn = !whitesTurn;
                KingLogic enemyKing = movedPiece.getIsWhite ? whiteKing : blackKing;
                removeCheck(movedPiece, enemyKing);

                
                // update first move var
                if(movedPiece.FirstMove == false)
                {
                    movedPiece.FirstMove = true;
                }

                // if it is a pawn promote if necessary
                if (movedPiece is PawnLogic) promote(endX, endY);
                checkIfCheck(endX, endY, LogicBoard[startX, startY].piece, enemyKing);

                //return true
                return true;
            }
            // check if it is a  castle 
            if(castle(startX, startY, endX, endY)) return true;

            // if it is not return false
            return false;
        }

        // sees if the move that just happend promotes a pawn and performs the promotion
        private void promote(int x, int y)
        {
            // only pawns can promote
            if (!(LogicBoard[x, y].piece is PawnLogic)) return;

            // check if it is black at end or white at end 
            if (LogicBoard[x, y].piece.getIsWhite && y == 7)
            {
                // promote to a white queen
                LogicBoard[x, y].piece = new QueenLogic(PieceColor.White, x, y);
                checkIfCheck(x, y, LogicBoard[x, y].piece, blackKing);

            }
            else if (!(LogicBoard[x, y].piece.getIsWhite) && y == 0)
            {
                // promote to a black queen
                LogicBoard[x, y].piece = new QueenLogic(PieceColor.Black, x, y);
                checkIfCheck(x, y, LogicBoard[x, y].piece, whiteKing);
            }
            return;
        }
        
        private bool castle(int startX, int startY, int endX, int endY)
        {
            // Check if they are trying to move the king
            if(! (LogicBoard[startX, startY].piece is KingLogic)) return false;
            
            // Check if it is his first move
            if (!(LogicBoard[startX, startY].piece.FirstMove)) return false;

            // Check if he is moving the king 2 squares along the x axis
            if( endY == startY && (Math.Abs(endX - startX) == 2))
            {
                // get the approiate rook
                int rookY = startY;
                int rookX;
                int sign = 1;
                if(startY > endY)
                {
                    rookX = 0;
                }
                else
                {
                    rookX = 7;
                    sign = -1;
                }

                // check that there is a rook there
                if (! (LogicBoard[rookX, rookY].piece is RookLogic)) return false;
                // check that he hasn't moved
                if (!(LogicBoard[rookX, rookY].piece.FirstMove)) return false;

                // check if the move checks either king
                checkIfCheck(endX, endY, LogicBoard[rookX, rookY].piece, blackKing);
                checkIfCheck(endX, endY, LogicBoard[rookX, rookY].piece, whiteKing);
                // cannot end with your king being in check 
                // cannot make a move that will end with your king being in check 
                if (whiteKing.isInCheck && LogicBoard[rookX, rookY].piece.getIsWhite)
                {
                    return false;
                }
                if (blackKing.isInCheck && !LogicBoard[rookX, rookY].piece.getIsWhite)
                {
                    return false;
                }


                // move both pieces to the correct spot
                /// king first
                LogicBoard[endX, endY].piece = LogicBoard[startX, startY].piece;
                LogicBoard[endX, endY].piece.col = endX;
                LogicBoard[endX, endY].piece.row = endY;
                LogicBoard[endX, endY].isFilled = true;
                LogicBoard[startX, startY].piece = null;
                LogicBoard[startX, startY].isFilled = false;
                LogicBoard[endX, endY].piece.FirstMove = true;
                /// then rook
                LogicBoard[endX + sign, endY].piece = LogicBoard[rookX, rookY].piece;
                LogicBoard[endX + sign, endY].piece.col = endX + sign;
                LogicBoard[endX + sign, endY].isFilled = true;
                LogicBoard[rookX, rookY].piece = null;
                LogicBoard[rookX, rookY].isFilled = false;
                LogicBoard[endX + sign, endY].piece.FirstMove = true;

                //remove check and flip turn
                whitesTurn = !whitesTurn;
                KingLogic enemyKing = LogicBoard[rookX, rookY].piece.getIsWhite ? whiteKing : blackKing;
                removeCheck(LogicBoard[rookX, rookY].piece, enemyKing);

                return true;
            }

            return false;
        }

        // check if the piece is listed as checking the king, remove if necessary
        private void removeCheck(PieceLogic attacker, KingLogic king)
        {
            if (attacker == null) return;
            if (king == null) return;
            if (king.getIsWhite == attacker.getIsWhite) return; // cant be same color

            //if it is listed as checking the king AND  it is now not attacking the king
            if (king.beingCheckedBy(attacker) && !(attacker.isMoveValid(this, attacker.col, attacker.row, king.col, king.row)))
            {
                king.removeCheck(attacker); // remove it as a checker of the king
            }

            return;
        }

        /* rays: 
         * 
         * 
         *      \  |  /
         *       \ | /
         *    -----P---------    
         *       / | \
         *      /  |  \
         * 
         *  diags are up 1 over 1 exactly
         */


        //check to see if the enemy king is now under check.
        private bool checkIfCheck(int endX, int endY, PieceLogic attacker, KingLogic king)
        {
            bool res = false;
            //make sure pieces are valid
            if(attacker == null) return false;
            if(king == null) return false;
            //make sure the new location is a valid one
            if(!(endX >= 0 && endX < 8 && endY >= 0 && endY < 8)) return false;
            
            // if pieces are oppsite color and attacker can attack king
            if(!(king.getIsWhite == attacker.getIsWhite) && attacker.isMoveValid(this, endX, endY, king.col, king.row))
            {
                // add to the attack list and set 
                king.nowChecking(attacker); // won't add if already in 
                res = true;
            }

            //Check the ray from the old location. See if it opened up an attack
            int x = king.col; int y = king.row;
            int deltaX = endX - x;
            int deltaY = endY - y;
            int xDir;
            int yDir;
            
            // in not a striaght line or an attackable diagnoal then not attackable
            if(!(deltaX == 0 || deltaY == 0) && !(Math.Abs(deltaX) == Math.Abs(deltaX)))
            {
                return res;
            }
            // if no movement (promotion and castling cases)
            if (deltaX == 0 && deltaY == 0 )
            {
                return res;
            }

            //get direction it came from (the ray)
            if (deltaX == 0) xDir = 0;
            else xDir = deltaX > 0 ? 1 : -1;
            if (deltaY == 0) yDir = 0;
            else yDir = deltaY > 0 ? 1 : -1;

            // take a step away from the king
            x += xDir;
            y += yDir;

            // travel along the ray until you hit the end of board or reach a piece
            while (x < 8 && y < 8 && x > 0 && y > 0)
            {
                //if you hit a square that is filled
                if (this.isSquareFilled(x, y))
                {
                    PieceLogic foundPiece = this.LogicBoard[x, y].piece;
                    
                    //if same color can't attack
                    if (foundPiece.getIsWhite == king.getIsWhite) return res;

                    // queen can attack on any ray
                    if (foundPiece is QueenLogic)
                    {
                        king.nowChecking(foundPiece);
                        res = true;                    
                    }
                    // if the ray is a straight line, needs to be a rook
                    else if ((deltaX == 0 || deltaY == 0) && foundPiece is RookLogic)
                    {
                        king.nowChecking(foundPiece);
                        res = true;                        
                    }
                    // else if the ray is a diag, needs to be a bishop
                    else if((Math.Abs(deltaX) == Math.Abs(deltaX)) && foundPiece is BishopLogic)
                    {
                        king.nowChecking(foundPiece);
                        res = true;                        
                    }
                    return res;
                }

                // go further along the ray
                x += xDir;
                y += yDir;
            }

            return res;
        }
    
        private bool isUnderAttack(PieceLogic piece, int startX, int startY)
        {
            // location vars
            int x = startX;
            int y = startY;
            
            //pawns first
            /// pawns attack from... north for white, south for black
            int pawnY = piece.getIsWhite ? 1 : -1;
            /// check east and west
            if(this.isSquareFilled(x + 1, y + pawnY))
            {
                //if pawn and colors dont match then it is attacking
                if (LogicBoard[x + 1, y + pawnY].piece is PawnLogic && (LogicBoard[x + 1, y + pawnY].piece.getIsWhite != piece.getIsWhite)) return true;
            }
            if (this.isSquareFilled(x + -1, y + pawnY))
            {
                if (LogicBoard[x + 1, y + pawnY].piece is PawnLogic && (LogicBoard[x + 1, y + pawnY].piece.getIsWhite != piece.getIsWhite)) return true;
            }

            // go in each rays direction (n, ne, e, se, s, sw, w, nw)
            int[] xDir = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] yDir = { 1, 1, 0, -1, -1, -1, 0, 1 };

            ///for each ray
            for (int i = 0; i < 8; i++)
            {
                // reset x and y
                x = startX + xDir[i];
                y = startY + yDir[i];
                
                // travel along the ray until you hit the end of board or reach a piece
                while (x < 8 && y < 8 && x > 0 && y > 0)
                {
                    //if you hit a square that is filled
                    if (this.isSquareFilled(x, y))
                    {
                        PieceLogic foundPiece = this.LogicBoard[x, y].piece;

                        //if same color can't attack
                        if (foundPiece.getIsWhite == piece.getIsWhite) break;

                        // queen can attack on any ray
                        if (foundPiece is QueenLogic)
                        {
                            return true;
                        }
                        // if the ray is a straight line, needs to be a rook
                        else if ((xDir[i] == 0 || yDir[i] == 0) && foundPiece is RookLogic)
                        {
                            return true;
                        }
                        // else if the ray is a diag, needs to be a bishop
                        else if ((Math.Abs(xDir[i]) == Math.Abs(yDir[i])) && foundPiece is BishopLogic)
                        {
                            return true;
                        }
                        else if(foundPiece != piece) break; //if you hit the piece's current location you need to keep going along the ray, so if not don't
                    }

                    // go further along the ray
                    x += xDir[i];
                    y += yDir[i];
                }
            }

            //do for king too but do not travel along ray
            ///for each offest
            for (int i = 0; i < 8; i++)
            {
                // set location
                x = startX + xDir[i];
                y = startY + yDir[i];
                //if king and colors dont match then it is attacking
                if (this.isSquareFilled(x, y))
                    if (LogicBoard[x, y].piece is KingLogic && (LogicBoard[x, y].piece.getIsWhite != piece.getIsWhite)) return true;
            }

            // check positions knight can attack from 
            /// Can only be attacked by a enemy knight from a position (P) that, if a knight (N) was at startX and startY, N could move to P legally on an empty board.
            int[] xKnight = { 1, 2, 2, 1, -1, -2, -2, -1 };
            int[] yKnight = { 2, 1, -1, -2, -2, -1, 1, 2 };
            ///for each offest
            for (int i = 0; i < 8; i++)
            {
                // set location
                x = startX + xKnight[i];
                y = startY + yKnight[i];
                //if knight and colors dont match then it is attacking
                if (this.isSquareFilled(x, y))
                    if (LogicBoard[x, y].piece is KnightLogic && (LogicBoard[x, y].piece.getIsWhite != piece.getIsWhite)) return true;
            }

            // if get here no attackers
            return false;
        }
    
    }

    public abstract class PieceLogic
    {
        //store where the piece is on board
        private int x;
        public int col
        {
            get { return x; } 
            set { x = value; }
        }
        private int y;
        public int row
        {
            get { return y; }
            set { y = value; }
        }


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

        public PieceLogic(PieceColor color, int x, int y)
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

            this.x = x;
            this.y = y;
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

    public class PawnLogic : PieceLogic
    {
        //Use constructor in abstract PieceLogic class
        public PawnLogic(PieceColor color, int x, int y) : base(color, x, y) {}

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
        public RookLogic(PieceColor color, int x, int y) : base(color, x, y) { }

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
        public KnightLogic(PieceColor color, int x, int y) : base(color, x, y) { }
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
        public BishopLogic(PieceColor color, int x, int y) : base(color, x, y) { }
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
        public QueenLogic(PieceColor color, int x, int y) : base(color, x, y) { }
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
        // store pieces that are checking the king. 
        private HashSet<PieceLogic> checkedBy = new HashSet<PieceLogic>();
        private bool inCheck;
        public bool isInCheck
        {
            get { return inCheck; }
        }
        public HashSet<PieceLogic> getCheckingPieces
        {
            get { return checkedBy; }
        }

        public bool beingCheckedBy(PieceLogic attacker)
        {
            return checkedBy.Contains(attacker);
        }

        //add a piece that is checking the king
        public bool nowChecking(PieceLogic attacker)
        {
            // null check
            if(attacker == null) {  return false; }
            //if both are same color then attacker can't attack king
            if(attacker.getIsWhite == this.getIsWhite) {  return false; }
            // don't add again
            if(checkedBy.Contains(attacker)) { return false; }
            // if you are about to add a piece that is the first to check the king
            if(checkedBy.Count == 0) { inCheck = true; }  
            //add to list
            checkedBy.Add(attacker);
            return true;
        }

        //remove one that is not checking the king anymore
        public bool removeCheck(PieceLogic attacker)
        {
            // null check
            if (attacker == null) { return false; }
            //if both are same color then attacker can't attack king
            if (attacker.getIsWhite == this.getIsWhite) { return false; }
            // if already in
            if (!checkedBy.Contains(attacker)) { return false; }
            // if you are about to removethe only piece that is checking the king
            if (checkedBy.Count == 1) { inCheck = false; }
            //remove
            checkedBy.Remove(attacker);
            return true;
        }

        //Use constructor in abstract PieceLogic class + initialize king to not be in check
        public KingLogic(PieceColor color, int x, int y) : base(color, x, y) 
        {
            inCheck = false;
        }
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
