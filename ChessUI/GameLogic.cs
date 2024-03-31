using Mysqlx.Session;
using System;
using System.Collections;
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

        private ArrayList whitePieces = new ArrayList();
        private ArrayList blackPieces = new ArrayList();
        
        private bool whitesTurn;

        //initalize the board and put the pieces in their correct starting spot
        public BoardLogic() 
        {
            // Initialize pawns for both white and black
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 1] = new Square(new PawnLogic(PieceColor.White, x, 1));
                whitePieces.Add(LogicBoard[x, 1].piece);
                LogicBoard[x, 6] = new Square(new PawnLogic(PieceColor.Black, x, 6));
                blackPieces.Add(LogicBoard[x, 6].piece);
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

            //Initalize the rest as empty and add pieces to arraylist
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 2] = new Square(null);
                LogicBoard[x, 3] = new Square(null);
                LogicBoard[x, 4] = new Square(null);
                LogicBoard[x, 5] = new Square(null);
                
                whitePieces.Add(LogicBoard[x, 0].piece);
                blackPieces.Add(LogicBoard[x, 7].piece);

            }

            whitesTurn = true;
        }

        public bool isSquareFilled(int x, int y)
        {
            return LogicBoard[x, y].isFilled;
        }

        public bool isMoveLegal(int startX, int startY, int endX, int endY)
        {
            // check if not filled then no piece to move
            if (!LogicBoard[startX, startY].isFilled) return false;

            // get piece to be moved and its king
            PieceLogic movedPiece = LogicBoard[startX, startY].piece;
            KingLogic friendlyKing = movedPiece.getIsWhite ? whiteKing : blackKing;

            // can only move piece of the your color
            if (movedPiece.getIsWhite != whitesTurn) return false;

            // cannot make a move that will end with your king being in check 
            if (movedPiece is KingLogic)
            {
                if (isUnderAttack(movedPiece, endX, endY)) return false;
            }

            // check if the move checks friendly king
            PieceLogic attacker = checkIfCheck(endX, endY, movedPiece, friendlyKing);
            if (friendlyKing.isInCheck)
            {
                friendlyKing.removeCheck(attacker); // remove checks if they were placed
                return false;
            }

            if (movedPiece.isMoveValid(this, startX, startY, endX, endY))
            {
                // can't move on a square that has same color piece
                if (!(LogicBoard[endX, endY].piece is null) && (LogicBoard[endX, endY].piece.getIsWhite == movedPiece.getIsWhite))
                {
                    return false;
                }
                
                return true; /***VALID MOVE***/
            }

            // if the move is a king and puts it in check, it is invalid, check if it is a castle
            if (movedPiece is KingLogic)
            {
                //check if it is a castle
                // cant castle while under check and has to be their first move of the king
                if (!friendlyKing.isInCheck && movedPiece.FirstMove)
                {
                    // Check if he is moving the king 2 squares along the x axis
                    if (endY == startY && (Math.Abs(endX - startX) == 2))
                    {
                        // get the approiate rook
                        int rookY = startY;
                        int rookX;
                        if (startY > endY)
                        {
                            rookX = 0;
                            //must be clear inbetween 2 pieces
                            for (int i = startX; i > rookX; i--)
                            {
                                if (isSquareFilled(i, endY)) return false;
                            }
                        }
                        else
                        {
                            rookX = 7;
                            //must be clear inbetween 2 pieces
                            for (int i = startX; i < rookX; i++)
                            {
                                if (isSquareFilled(i, endY)) return false;
                            }
                        }

                        PieceLogic rook = LogicBoard[rookX, rookY].piece;
                        // Check if it is a valid rook and it is their first move
                        if (rook == null) return false;
                        if(!(rook is RookLogic)) return false;
                        if (movedPiece.getIsWhite != rook.getIsWhite) return false;
                        if (!(rook.FirstMove)) return false;

                        return true; /***VALID CASTLE***/
                    }
                }

            }

            // if it is not return false
            return false;
        }

        public bool move(int startX, int startY, int endX, int endY)
        {
            if (!isMoveLegal(startX, startY, endX, endY)) return false;

            PieceLogic movedPiece = LogicBoard[startX, startY].piece;
            KingLogic friendlyKing = whitesTurn ? whiteKing : blackKing;
            KingLogic enemyKing = whitesTurn ? blackKing : whiteKing;

            //move pieces
            //kill pieces it is moving on
            if (!(LogicBoard[endX, endY].piece is null))
            { 
                LogicBoard[endX, endY].piece.isKilled = true;
            }
            LogicBoard[endX, endY].piece = movedPiece;
            movedPiece.col = endX;
            movedPiece.row = endY;
            LogicBoard[endX, endY].isFilled = true;
            LogicBoard[startX, startY].piece = null;
            LogicBoard[startX, startY].isFilled = false;

            // update first move var
            if (movedPiece.FirstMove == false)
            {
                movedPiece.FirstMove = true;
            }

            // if it is a pawn promote if necessary
            if (movedPiece is PawnLogic) promote(endX, endY);
            // if it is a castle move as necessary
            if (movedPiece is KingLogic) 
            {
                bool res = castle(startX, startY, endX, endY);
                if (!res) // if changes were reverted the move was not performed 
                {
                    removeChecks(friendlyKing);
                    return false;
                }
            }

            //remove check (if other pieces are no longer checking friendly  king), and add new ones
            removeChecks(friendlyKing);
            checkIfCheck(endX, endY, movedPiece, enemyKing);

            //flip turn and return
            whitesTurn = !whitesTurn;
            return true;
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
        
        //returns true if move persist (castle or not) returns false if king and rook reverted to original spots due to king being in check from castle move
        private bool castle(int startX, int startY, int endX, int endY)
        {
            PieceLogic king = LogicBoard[endX, endY].piece;
            // not a king
            if (!(king is KingLogic)) return true;

            // Check if he is moving the king 2 squares along the x axis
            if (endY == startY && (Math.Abs(endX - startX) == 2))
            {
                // get the approiate rook
                int rookY = startY;
                int rookX;
                int sign = 1;
                if (startY > endY)
                {
                    rookX = 0;
                }
                else
                {
                    rookX = 7;
                    sign = -1;
                }

                // move rook to the correct spot (king moved in move method)
                LogicBoard[endX + sign, endY].piece = LogicBoard[rookX, rookY].piece;
                LogicBoard[endX + sign, endY].piece.col = endX + sign;
                LogicBoard[endX + sign, endY].isFilled = true;
                LogicBoard[rookX, rookY].piece = null;
                LogicBoard[rookX, rookY].isFilled = false;
                LogicBoard[endX + sign, endY].piece.FirstMove = true;

                //Check that the king is not under attack now
                if (isUnderAttack(LogicBoard[endX, endY].piece, endX, endY))
                {
                    // if he is revert changes
                    LogicBoard[startX, startY].piece = LogicBoard[endX, endY].piece;
                    LogicBoard[startX, startY].piece.col = startX;
                    LogicBoard[startX, startY].piece.row = startY;
                    LogicBoard[startX, startY].isFilled = true;
                    LogicBoard[endX, endY].piece = null;
                    LogicBoard[endX, endY].isFilled = false;
                    LogicBoard[startX, startY].piece.FirstMove = false;
                    /// then rook
                    LogicBoard[rookX, rookY].piece = LogicBoard[endX + sign, endY].piece;
                    LogicBoard[rookX, rookY].piece.col = rookX;
                    LogicBoard[rookX, rookY].isFilled = true;
                    LogicBoard[endX + sign, endY].piece = null;
                    LogicBoard[endX + sign, endY].isFilled = false;
                    LogicBoard[rookX, rookY].piece.FirstMove = false;

                    //return false to indicate that the move was reversed
                    return false;
                }
            }
            
            return true;
        }

        // check if the piece is listed as checking the king is now blocking a checker, remove if necessary
        private void removeChecks(KingLogic king)
        {
            if (king == null) return;

            //get attackers
            HashSet<PieceLogic> checkers = king.getCheckingPieces;
            
            //loop through each piece and see if it can still attack
            foreach(PieceLogic attacker in checkers)
            {
                //if a move to the king is no longer legal then remove it as a checking piece
                if (!isMoveLegal(attacker.col, attacker.row, king.col, king.row)) king.removeCheck(attacker);
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
        //check to see if the king is now under check.
        private PieceLogic checkIfCheck(int endX, int endY, PieceLogic attacker, KingLogic king)
        {
            /* return PieceLogic for checking if move is valid. 
             * if a move places a check on a friendly king then the move is invalid and not executed 
             * BUT... This method places the king under check of that found piece
             * SO... it needs to be returned so it can be removed as a piece checking. 
             */
            PieceLogic res = null;
            
            //make sure pieces are valid
            if(attacker == null) return res; 
            if(king == null) return res;
            //make sure the new location is a valid one
            if(!(endX >= 0 && endX < 8 && endY >= 0 && endY < 8)) return res;
            
            // if pieces are oppsite color and attacker can attack king
            if((king.getIsWhite != attacker.getIsWhite) && attacker.isMoveValid(this, endX, endY, king.col, king.row))
            {
                // add to the attack list and set 
                king.nowChecking(attacker); // won't add if already in 
                res = attacker;
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
                        res = foundPiece;                    
                    }
                    // if the ray is a straight line, needs to be a rook
                    else if ((deltaX == 0 || deltaY == 0) && foundPiece is RookLogic)
                    {
                        king.nowChecking(foundPiece);
                        res = foundPiece;                        
                    }
                    // else if the ray is a diag, needs to be a bishop
                    else if((Math.Abs(deltaX) == Math.Abs(deltaX)) && foundPiece is BishopLogic)
                    {
                        king.nowChecking(foundPiece);
                        res = foundPiece;                        
                    }
                    return res;
                }

                // go further along the ray
                x += xDir;
                y += yDir;
            }

            return res;
        }
    
        // returns if a piece moves there will it be under attack by any piece
        private bool isUnderAttack(PieceLogic piece, int startX, int startY)
        {
            // location vars
            int x = startX;
            int y = startY;

            if (x < 8 || y < 8 || x > 0 || y > 0) {  return true; }

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
    

        public bool isGameOver() //TODO********************************
        {
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
        public bool isKilled {  get; set; }
        
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
            // if not checking 
            if (!checkedBy.Contains(attacker)) { return false; }
            // if you are about to remove the only piece that is checking the king
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
