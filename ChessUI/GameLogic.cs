using Mysqlx.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipelines;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public Square[,] LogicBoard = new Square[8, 8];

        private KingLogic whiteKing = new KingLogic(PieceColor.White, 4, 0);
        private KingLogic blackKing = new KingLogic(PieceColor.Black, 4, 7);

        private PieceLogic[] whitePieces = new PieceLogic[16];
        private PieceLogic[] blackPieces = new PieceLogic[16];
        
        public bool whitesTurn;

        //initalize the board and put the pieces in their correct starting spot
        public BoardLogic() 
        {
            int pieceIndex = 0;
            // Initialize pawns for both white and black
            for (int x = 0; x < 8; x++)
            {
                LogicBoard[x, 1] = new Square(new PawnLogic(PieceColor.White, x, 1));
                whitePieces[pieceIndex] = LogicBoard[x, 1].piece;
                LogicBoard[x, 6] = new Square(new PawnLogic(PieceColor.Black, x, 6));
                blackPieces[pieceIndex] = LogicBoard[x, 6].piece;

                pieceIndex += 1;
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

                whitePieces[pieceIndex] = LogicBoard[x, 0].piece;
                blackPieces[pieceIndex] = LogicBoard[x, 7].piece;

                pieceIndex += 1;

            }

            whitesTurn = true;
        }

        public bool isSquareFilled(int x, int y)
        {
            //if out of bounds ret false
            if (x > 7 || y > 7 || x < 0 || y < 0) { return false; }
            //else get piece and see if it is not null
            PieceLogic piece = LogicBoard[x, y].piece;
            if (piece == null) return false;
            return true;
        }

        public bool isMoveLegal(int startX, int startY, int endX, int endY)
        {
            // check if not filled then no piece to move
            if (!LogicBoard[startX, startY].isFilled) return false;
            // can't move to same location
            if(startX == endX && startY == endY) return false;

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
            else
            {
                // check if the move checks friendly king
                if (checkIfCheck(endX, endY, movedPiece))
                {
                    addChecks(friendlyKing);
                    return false;
                }
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
            if (LogicBoard[endX, endY].piece != null)
            {
                PieceLogic foundPiece = LogicBoard[endX, endY].piece;
                if(!foundPiece.isKilled && foundPiece.getIsWhite != movedPiece.getIsWhite)
                {
                    LogicBoard[endX, endY].piece.isKilled = true;
                }

            }
            LogicBoard[endX, endY].piece = movedPiece;
            movedPiece.col = endX;
            movedPiece.row = endY;
            LogicBoard[endX, endY].isFilled = true;
            LogicBoard[startX, startY].piece = null;
            LogicBoard[startX, startY].isFilled = false;

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

            // update first move var
            if (movedPiece.FirstMove == false)
            {
                movedPiece.FirstMove = true;
            }

            //remove check (if other pieces are no longer checking friendly  king), and add new ones
            removeChecks(friendlyKing);
            addChecks(enemyKing);

            //Console.WriteLine("friend King White: " + friendlyKing.getIsWhite);
            //Console.WriteLine("enemy King White: " + enemyKing.getIsWhite);
            //flip turn and return
            //Console.WriteLine("Black King: " + blackKing.isInCheck);
            //Console.WriteLine("White King: " + whiteKing.isInCheck);
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

            }
            else if (!(LogicBoard[x, y].piece.getIsWhite) && y == 0)
            {
                // promote to a black queen
                LogicBoard[x, y].piece = new QueenLogic(PieceColor.Black, x, y);
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
                if (!LogicBoard[endX + sign, endY].piece.FirstMove || !king.FirstMove || isUnderAttack(LogicBoard[endX, endY].piece, endX, endY))
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
                if (!attacker.isMoveValid(this, attacker.col, attacker.row, king.col, king.row)) king.removeCheck(attacker);
            }
            return;
        }

        private void addChecks(KingLogic king)
        {
            //get enemy pieceset
            PieceLogic[] pieces = king.getIsWhite ? blackPieces : whitePieces;

            foreach (PieceLogic piece in pieces)
            {
                //skip dead pieces
                if (piece.isKilled) continue;

                //Console.WriteLine("Piece: " + piece.ToString() + " " + piece.getIsWhite);

                // location vars
                int x = piece.col;
                int y = piece.row;

                bool res = piece.isMoveValid(this, x, y, king.col, king.row);

                //Console.WriteLine(res);

                if (res)// a legal move exist then the piece is under attack
                {
                    king.nowChecking(piece);
                }

            }
        }

        //check to see if the move will cause friendly king to be under check.
        private bool checkIfCheck(int endX, int endY, PieceLogic movedPiece)
        {
            //make sure pieces are valid
            if (movedPiece == null) return true;
            KingLogic king = movedPiece.getIsWhite ? whiteKing : blackKing;
            //make sure the new location is a valid one
            if (endX > 7 || endY > 7 || endX < 0 || endY < 0) return king.isInCheck;

            PieceLogic[] pieces = king.getIsWhite ? blackPieces : whitePieces;

            //Check the ray from the old location. See if it opened up an attack
            int x = king.col; 
            int y = king.row;
            int deltaX = movedPiece.col - x;
            int deltaY = movedPiece.row - y;
            int xDir;
            int yDir;
            if (endX > 7 || endY > 7 || endX < 0 || endY < 0) return king.isInCheck;

            //get direction it came from (the ray)
            if (deltaX == 0) xDir = 0;
            else xDir = deltaX > 0 ? 1 : -1;
            if (deltaY == 0) yDir = 0;
            else yDir = deltaY > 0 ? 1 : -1;

            // take a step away from the king
            x += xDir;
            y += yDir;

            if (((deltaX == 0) != (deltaY == 0)) || (Math.Abs(deltaX) == Math.Abs(deltaY)))
            {
                // travel along the ray until you hit the end of board or reach a piece
                while (x < 8 && y < 8 && x >= 0 && y >= 0)
                {
                    //if you hit a square that is filled
                    if (this.isSquareFilled(x, y))
                    {
                        PieceLogic foundPiece = this.LogicBoard[x, y].piece;

                        if (foundPiece == movedPiece)
                        {
                            x += xDir;
                            y += yDir;
                            continue;
                        }

                        //Console.WriteLine("Piece: " + foundPiece.ToString() + " " + foundPiece.getIsWhite);
                        //Console.WriteLine("Found @ loc: " + x + ", " + y);
                        //Console.WriteLine("End loc: " + endX + ", " + endY);

                        //if same color can't attack
                        if (foundPiece.getIsWhite == king.getIsWhite) break;

                        if (x == endX && y == endY) break;


                        // queen can attack on any ray
                        if (foundPiece is QueenLogic)
                        {
                            return true;
                        }
                        // if the ray is a straight line, needs to be a rook
                        else if (((deltaX == 0) != (deltaY == 0)) && foundPiece is RookLogic)
                        {
                            return true; 
                        }
                        // else if the ray is a diag, needs to be a bishop
                        else if ((Math.Abs(deltaX) == Math.Abs(deltaY)) && foundPiece is BishopLogic)
                        {
                            return true;
                        }
                    }

                    // go further along the ray
                    x += xDir;
                    y += yDir;
                }
            }
            
            //get attackers and see if you take or are blocking
            List<PieceLogic> checking = king.getCheckingPieces.ToList<PieceLogic>();

            //loop through each piece and see if it can still attack
            foreach (PieceLogic attacking in checking)
            {
                //Console.WriteLine("Piece: " + attacking.ToString() + " " + attacking.getIsWhite);
                //Console.WriteLine("end loc: " + endX + ", " + endY);
                int currX = attacking.col;
                int currY = attacking.row;

                if (currX == endX && currY == endY)
                {
                    king.removeCheck(attacking);
                    continue;
                }

                // if one of these 3 see if you are blocking 
                if (attacking is QueenLogic || attacking is RookLogic || attacking is BishopLogic)
                {
                    int kingX = king.col;
                    int kingY = king.row;
                    deltaX = kingX - currX;
                    deltaY = kingY - currY;
                    //int xDir;
                    //int yDir;

                    // if no movement (promotion and castling cases)
                    if (deltaX == 0 && deltaY == 0)
                    {
                        continue;
                    }

                    //get direction it came from (the ray)
                    if (deltaX == 0) xDir = 0;
                    else xDir = deltaX > 0 ? 1 : -1;
                    if (deltaY == 0) yDir = 0;
                    else yDir = deltaY > 0 ? 1 : -1;

                    while(currX != kingX && currY != kingY)
                    {
                        if (currX > 7 || currY > 7 || currX < 0 || currY < 0) break;
                        //Console.WriteLine("curr loc: "+ currX + ", " + currY);

                        if (currX == endX && currY == endY)
                        {
                            king.removeCheck(attacking);
                            break;
                        }

                        currX += xDir;
                        currY += yDir;
                    }
                }
            }

            return king.isInCheck;
        }
    
        // returns if a piece moves there will it be under attack by any piece
        private bool isUnderAttack(PieceLogic movedPiece, int endX, int endY)
        {
            //can't attack off board
            if (endX > 7 || endY > 7 || endX < 0 || endY < 0) {  return false; }
            
            //get enemy pieceset
            PieceLogic[] pieces = movedPiece.getIsWhite ? blackPieces: whitePieces;

            foreach (PieceLogic piece in pieces)
            {
                //skip dead pieces
                if (piece.isKilled) continue;

                // location vars
                int x = piece.row;
                int y = piece.col;

                if (isMoveLegal(x, y, endX, endY))// a legal move exist then the piece is under attack
                {
                    return true;
                }

            }

            //get here no attackers
            return false;
        }
    
        public GameOverType isGameOver() 
        {
            // initialize result to the game not being over
            GameOverType res = GameOverType.NotOver;

            //get all the pieces and the king of whoevers turn it is
            PieceLogic[] pieces = whitesTurn ? whitePieces : blackPieces;
            
            KingLogic king = whitesTurn ? whiteKing : blackKing;
            
            //for each piece get the number of legal moves they have. 
            // if ANY of them have a legal move then the game is not over
            int moves = 0;
            foreach(PieceLogic piece  in pieces)
            {
                //skip dead pieces
                if (piece.isKilled) continue;
                
                moves += piece.numLegalMoves(this);
                if (moves > 0)// a legal move exist, game is not over
                {
                    return res;
                }

            }

            // if there is no legal moves it is a checkmate or stalemate
            // if king check then it's checkmate if not it's stalemate
            res = king.isInCheck ? GameOverType.CheckMate : GameOverType.StaleMate;
            return res; 
        }
        
        public bool isKingUnderCheck(PieceColor color)
        {
            // get the approiate king
            KingLogic king = (color == PieceColor.White) ? whiteKing : blackKing;
            //return if he is under check
            return king.isInCheck;
        }

        public PieceLogic getPieceAtLocation(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7) return null;
            return LogicBoard[x, y].piece;
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

        public abstract int numLegalMoves(BoardLogic board);

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
        
    }

    public class PawnLogic : PieceLogic
    {
        //Use constructor in abstract PieceLogic class
        public PawnLogic(PieceColor color, int x, int y) : base(color, x, y) {}

        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            /*ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }
            */
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
                return !(glBoard.isSquareFilled(endX, endY) || glBoard.isSquareFilled(endX, endY - (1 * signFlipper))) && FirstMove;
            }
            
            // else if diag right 1 pos
            else if (startY + (1 * signFlipper) == endY && startX + (1 * signFlipper) == endX)
            {
                // must be filled and oppsite colors to move diag
                PieceLogic piece = glBoard.getPieceAtLocation(endX, endY);
                if(piece == null)
                {
                    return false;
                }
                return this.getIsWhite != piece.getIsWhite;
                
            }
            
            // else if diag left 1 pos
            else if (startY + (1 * signFlipper) == endY && startX - (1 * signFlipper) == endX)
            {
                // must be filled and oppsite colors to move diag
                PieceLogic piece = glBoard.getPieceAtLocation(endX, endY);
                if (piece == null)
                {
                    return false;
                }
                return this.getIsWhite != piece.getIsWhite;
            }

            // else not a valid move
            return false;

        }

        public override int numLegalMoves(BoardLogic board)
        {
            // return var
            int res = 0;
            
            // direction pawn moves to
            int direction = 1;
            if (!this.getIsWhite)
            {
                direction = -1;
            }

            // all the possiple moves
            int[] deltaX = { 0, 0, 1, -1 };
            int[] deltaY = { 1, 2, 1, 1 };
            int endX;
            int endY;

           // see if any are legal
            for (int i = 0; i < 4; i++)
            {
                endX = this.col + deltaX[i];
                endY = this.row + (deltaY[i] * direction);
                if(board.isMoveLegal(this.col, this.row, endX, endY))
                {
                    res += 1;
                }
            }

            return res;
        }
    }

    public class RookLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class
        public RookLogic(PieceColor color, int x, int y) : base(color, x, y) { }

        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }

            // if they start of end off the board it is not valid
            if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            {
                return false;
            }

            // check if ending position alligns with piece rules 
            // either the x position changes or the y changes, not both and not neither
            if((startX == endX) != (startY == endY))
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

        public override int numLegalMoves(BoardLogic board)
        {
            // initialize return var
            int res = 0;

            // 4 directions Rook can go
            int[] dirX = { 1, -1, 0, 0 };
            int[] dirY = { 0, 0, 1, -1 };

            int tmpX, startX = this.col;
            int tmpY, startY = this.row;

            // go in each dir while not off board, summing legal moves along way
            for (int i = 0; i < 4; i++)
            {
                tmpX = startX + dirX[i];
                tmpY = startY + dirY[i];
                while(IsValidPosition(tmpX, tmpY))
                {
                    if(board.isMoveLegal(startX, startY, tmpX, tmpY))
                    {
                        res += 1;
                    }

                    tmpX += dirX[i];
                    tmpY += dirY[i];
                }
            }

            return res;
        }
    }
    
    public class KnightLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public KnightLogic(PieceColor color, int x, int y) : base(color, x, y) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }

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

        public override int numLegalMoves(BoardLogic board)
        {
            // initialize return var
            int res = 0;

            // 8 directions Knight can go
            int[] dirX = { 1, 2, 2, 1, -1, -2, -2, -1 };
            int[] dirY = { 2, 1, -1, -2, 2, 1, -1, -2 };
            
            // go to each spot on board summing legal moves along way
            for (int i = 0; i < 8; i++)
            {
                if (board.isMoveLegal(this.col, this.row, this.col + dirX[i], this.row + dirY[i]))
                {
                    res += 1;
                }    
            }

            return res;
        }
    }
    
    public class BishopLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public BishopLogic(PieceColor color, int x, int y) : base(color, x, y) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }

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
            if(deltaX == 0 || deltaY == 0)
            {
                return false;
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

        public override int numLegalMoves(BoardLogic board)
        {
            // initialize return var
            int res = 0;

            // 4 directions Bishop can go
            int[] dirX = { 1, -1, 1, -1 };
            int[] dirY = { 1, 1, -1, -1 };

            int tmpX, startX = this.col;
            int tmpY, startY = this.row;

            // go in each dir while not off board, summing legal moves along way
            for (int i = 0; i < 4; i++)
            {
                tmpX = startX + dirX[i];
                tmpY = startY + dirY[i];
                while (IsValidPosition(tmpX, tmpY))
                {
                    if (board.isMoveLegal(startX, startY, tmpX, tmpY))
                    {
                        res += 1;
                    }

                    tmpX += dirX[i];
                    tmpY += dirY[i];
                }
            }

            return res;
        }
    }
    
    public class QueenLogic : PieceLogic 
    {
        //Use constructor in abstract PieceLogic class 
        public QueenLogic(PieceColor color, int x, int y) : base(color, x, y) { }
        public override bool isMoveValid(BoardLogic glBoard, int startX, int startY, int endX, int endY)
        {
            //ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }

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
                if (deltaX == 0 || deltaY == 0)
                {
                    return false;
                }
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

        public override int numLegalMoves(BoardLogic board)
        {
            // initialize return var
            int res = 0;

            // 8 directions Queen can go
            int[] dirX = { 1, -1, 0, 0, 1, -1, 1, -1 };
            int[] dirY = { 0, 0, 1, -1, 1, 1, -1, -1 };

            int tmpX, startX = this.col;
            int tmpY, startY = this.row;

            // go in each dir while not off board, summing legal moves along way
            for (int i = 0; i < 8; i++)
            {
                tmpX = startX + dirX[i];
                tmpY = startY + dirY[i];
                while (IsValidPosition(tmpX, tmpY))
                {
                    if (board.isMoveLegal(startX, startY, tmpX, tmpY))
                    {
                        res += 1;
                    }

                    tmpX += dirX[i];
                    tmpY += dirY[i];
                }
            }

            return res;
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
            //ensure this pieceloc matches starting loc on the board it is trying to move on
            if (startX != this.col || startY != this.row)
            {
                return false;
            }

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
            // else not moving 1 square
            return false;
            
        }

        public override int numLegalMoves(BoardLogic board)
        {
            // initialize return var
            int res = 0;

            // 8 directions king can go
            int[] dirX = { 1, -1, 0, 0, 1, -1, 1, -1 };
            int[] dirY = { 0, 0, 1, -1, 1, 1, -1, -1 };

            // go to each spot on board summing legal moves along way
            for (int i = 0; i < 8; i++)
            {
                if (board.isMoveLegal(this.col, this.row, this.col + dirX[i], this.row + dirY[i]))
                {
                    res += 1;
                }
            }

            return res;
        }
    }

    public enum GameOverType
    {
        NotOver,
        CheckMate,
        StaleMate
    }
}
