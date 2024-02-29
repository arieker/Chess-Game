using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class ChessBoardForm : Form
{
    private const int boardSize = 8;
    private const int squareSize = 85;
    private readonly Piece[,] board = new Piece[boardSize, boardSize];
    private readonly Image[,] pieceImages = new Image[6, 2]; // 6 types of pieces x 2 colors

    public ChessBoardForm()
    {
        LoadPieceImages(); // Load piece images
        InitializeBoard();
        this.MouseClick += ChessBoardForm_MouseClick;
    }

    private void LoadPieceImages()
    {
        // Relative directory path where the images are located
        string directoryPath = @"../../Assets";

        // Load images for each piece and color
        for (PieceColor color = PieceColor.White; color <= PieceColor.Black; color++)
        {
            for (PieceType type = PieceType.Pawn; type <= PieceType.King; type++)
            {
                // Construct the file name based on piece type and color
                string fileName = $"{type.ToString().ToLower()}_{color.ToString().ToLower()}.png";

                try
                {
                    // Combine the directory path and file name to form the full file path
                    string filePath = Path.Combine(directoryPath, fileName);

                    // Print out the full file path
                    Console.WriteLine($"Attempting to load image: {filePath}");

                    // Load the image from the file path
                    pieceImages[(int)type, (int)color] = Image.FromFile(filePath);
                }
                catch (FileNotFoundException)
                {
                    // Handle file not found exception
                    MessageBox.Show($"Image file '{fileName}' not found in directory: {directoryPath}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    MessageBox.Show($"An error occurred while loading image '{fileName}': {ex.Message}");
                }
            }
        }
    }



    private void InitializeBoard()
    {
        // Initialize pawns for both white and black
        for (int col = 0; col < boardSize; col++)
        {
            board[1, col] = new Pawn(PieceColor.Black, pieceImages[(int)PieceType.Pawn, (int)PieceColor.Black]);
            board[6, col] = new Pawn(PieceColor.White, pieceImages[(int)PieceType.Pawn, (int)PieceColor.White]);
        }

        // Initialize rooks
        board[0, 0] = new Rook(PieceColor.Black, pieceImages[(int)PieceType.Rook, (int)PieceColor.Black]);
        board[0, 7] = new Rook(PieceColor.Black, pieceImages[(int)PieceType.Rook, (int)PieceColor.Black]);
        board[7, 0] = new Rook(PieceColor.White, pieceImages[(int)PieceType.Rook, (int)PieceColor.White]);
        board[7, 7] = new Rook(PieceColor.White, pieceImages[(int)PieceType.Rook, (int)PieceColor.White]);

        // Initialize knights
        board[0, 1] = new Knight(PieceColor.Black, pieceImages[(int)PieceType.Knight, (int)PieceColor.Black]);
        board[0, 6] = new Knight(PieceColor.Black, pieceImages[(int)PieceType.Knight, (int)PieceColor.Black]);
        board[7, 1] = new Knight(PieceColor.White, pieceImages[(int)PieceType.Knight, (int)PieceColor.White]);
        board[7, 6] = new Knight(PieceColor.White, pieceImages[(int)PieceType.Knight, (int)PieceColor.White]);

        // Initialize bishops
        board[0, 2] = new Bishop(PieceColor.Black, pieceImages[(int)PieceType.Bishop, (int)PieceColor.Black]);
        board[0, 5] = new Bishop(PieceColor.Black, pieceImages[(int)PieceType.Bishop, (int)PieceColor.Black]);
        board[7, 2] = new Bishop(PieceColor.White, pieceImages[(int)PieceType.Bishop, (int)PieceColor.White]);
        board[7, 5] = new Bishop(PieceColor.White, pieceImages[(int)PieceType.Bishop, (int)PieceColor.White]);

        // Initialize queens
        board[0, 3] = new Queen(PieceColor.Black, pieceImages[(int)PieceType.Queen, (int)PieceColor.Black]);
        board[7, 3] = new Queen(PieceColor.White, pieceImages[(int)PieceType.Queen, (int)PieceColor.White]);

        // Initialize kings
        board[0, 4] = new King(PieceColor.Black, pieceImages[(int)PieceType.King, (int)PieceColor.Black]);
        board[7, 4] = new King(PieceColor.White, pieceImages[(int)PieceType.King, (int)PieceColor.White]);

        this.Size = new Size(boardSize * squareSize, boardSize * squareSize);
        this.Paint += new PaintEventHandler(DrawChessboard);
    }

    private void DrawChessboard(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                bool isLightSquare = (row + col) % 2 == 0;
                Brush squareBrush = (isLightSquare) ? Brushes.WhiteSmoke : Brushes.CornflowerBlue;

                int x = col * squareSize;
                int y = row * squareSize;

                g.FillRectangle(squareBrush, x, y, squareSize, squareSize);

                // Draw piece if exists
                if (board[row, col] != null)
                {
                    board[row, col].Draw(g, x, y, squareSize);
                }
            }
        }
    }

    private Piece selectedPiece = null;
    private Point selectedSquare = Point.Empty;

    private void ChessBoardForm_MouseClick(object sender, MouseEventArgs e)
    {
        int row = e.Y / squareSize;
        int col = e.X / squareSize;

        if (selectedPiece == null)
        {
            // If no piece is selected, check if there's a piece at the clicked position
            if (board[row, col] != null)
            {
                // Select the piece at the clicked position
                selectedPiece = board[row, col];
                selectedSquare = new Point(col, row);
            }
        }
        else
        {
            // If a piece is already selected, move the selected piece to the clicked position
            if (IsValidMove(selectedSquare, new Point(col, row)))
            {
                // Move the selected piece to the destination square
                board[row, col] = selectedPiece;
                board[selectedSquare.Y, selectedSquare.X] = null;

                // Reset selected piece and square
                selectedPiece = null;
                selectedSquare = Point.Empty;

                // Redraw the chessboard
                this.Invalidate();
            }
            else
            {
                // Invalid move, deselect the piece
                selectedPiece = null;
                selectedSquare = Point.Empty;
            }
        }
    }

    private bool IsValidMove(Point source, Point destination)
    {
        // Check if the move is within the bounds of the board
        if (destination.X < 0 || destination.X >= boardSize || destination.Y < 0 || destination.Y >= boardSize)
        {
            return false; // Destination square is outside the board
        }

        // Get the piece at the source square
        Piece sourcePiece = board[source.Y, source.X];

        // Checks there's a piece at the source square
        if (sourcePiece == null)
        {
            return false; // no piece to move
        }

        // Checks destination square is empty or contains an opponent's piece
        Piece destinationPiece = board[destination.Y, destination.X];
        if (destinationPiece == null || destinationPiece.Color != sourcePiece.Color)
        {
            // Move valid if  destination square is empty or contains an opponent's piece
            return true;
        }

        // Destination square contains a piece of the same color
        return false;
    }



    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // ChessBoardForm
        // 
        this.ClientSize = new System.Drawing.Size(716, 359);
        this.Name = "ChessBoardForm";
        this.ResumeLayout(false);

    }
}

// Define Piece, Pawn, Rook, Knight, Bishop, Queen, and King classes
public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

public abstract class Piece
{
    public PieceColor Color { get; set; }
    public Image Image { get; set; }
    public abstract void Draw(Graphics g, int x, int y, int size);
}

public class Pawn : Piece
{
    public Pawn(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}

public class Rook : Piece
{
    public Rook(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}

public class Knight : Piece
{
    public Knight(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}

public class Bishop : Piece
{
    public Bishop(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}

public class Queen : Piece
{
    public Queen(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}

public class King : Piece
{
    public King(PieceColor color, Image image)
    {
        Color = color;
        Image = image;
    }

    public override void Draw(Graphics g, int x, int y, int size)
    {
        g.DrawImage(Image, x, y, size, size);
    }
}