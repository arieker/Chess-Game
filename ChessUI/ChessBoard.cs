using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class ChessboardForm : Form
{
    private const int boardSize = 8;
    private const int squareSize = 85;
    private readonly Piece[,] board = new Piece[boardSize, boardSize];
    private readonly Image[,] pieceImages = new Image[6, 2]; // 6 types of pieces x 2 colors

    public ChessboardForm()
    {
        LoadPieceImages(); // Load piece images
        InitializeBoard();
        this.MouseClick += ChessboardForm_MouseClick;
    }

    private void LoadPieceImages()
    {
        // Load images for each piece and color
        for (PieceColor color = PieceColor.White; color <= PieceColor.Black; color++)
        {
            for (PieceType type = PieceType.Pawn; type <= PieceType.King; type++)
            {
                string fileName = $"{type.ToString().ToLower()}_{color.ToString().ToLower()}.png";
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                pieceImages[(int)type, (int)color] = Image.FromFile(filePath);
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
                Brush squareBrush = (isLightSquare) ? Brushes.Cornsilk : Brushes.Green;

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

    private void ChessboardForm_MouseClick(object sender, MouseEventArgs e)
    {
        int row = e.Y / squareSize;
        int col = e.X / squareSize;

        // For demonstration, let's place a piece (e.g., a pawn) at the clicked position
        board[row, col] = new Pawn(PieceColor.White, pieceImages[(int)PieceType.Pawn, (int)PieceColor.White]); // Change the piece type/color as needed
        this.Invalidate(); // Redraw the chessboard
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ChessboardForm());
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
