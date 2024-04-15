using ChessUI;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


public class ChessBoardForm : Form
{
    private const int boardSize = 8;
    private const int squareSize = 80;
    public BoardLogic boardLogic;
    private readonly Piece[,] board = new Piece[boardSize, boardSize];
    private GroupBox groupBox1;
    private Label theirTimeLabelLabel;
    private Label yourTimeLabelLabel;
    private Label theirTimerLabel;
    private Label yourTimerLabel;
    private readonly Image[,] pieceImages = new Image[6, 2]; // 6 types of pieces x 2 colors

    public ChessBoardForm()
    {
        InitializeComponent();
        LoadPieceImages(); // Load piece images
        InitializeBoard();
        this.MouseClick += ChessBoardForm_MouseClick;
        boardLogic = new BoardLogic();
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

       
        int formWidth = boardSize * squareSize;
        int formHeight = boardSize * squareSize;

        // Set the size of the form
        this.ClientSize = new Size(formWidth, formHeight+100);

        //Move the winform box to the bottom
        groupBox1.Top = this.ClientSize.Height - groupBox1.Height;
        groupBox1.Text = ""; // make it prettier by removing the text on it

        // Register the DrawChessboard method to handle painting events
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
                board[row, col]?.Draw(g, x, y, squareSize);
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
            
            if (boardLogic.move(selectedSquare.X,7-selectedSquare.Y,col,7-row)) 
            {
               
                // Move the selected piece to the destination square
                board[row, col] = selectedPiece;
                board[selectedSquare.Y, selectedSquare.X] = null;

                // Invalidate the starting cell and the destination cell
                InvalidateCell(selectedSquare);
                InvalidateCell(new Point(col, row));

              

                // Reset selected piece and square
                selectedPiece = null;
                selectedSquare = Point.Empty;

                
            }
            else
            {
                // Invalid move, deselect the piece
                selectedPiece = null;
                selectedSquare = Point.Empty;
            }
        }
    }

    private void InvalidateCell(Point cell)
    {
        Rectangle cellRect = new Rectangle(cell.X * squareSize, cell.Y * squareSize, squareSize, squareSize);
        Invalidate(cellRect);
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.theirTimeLabelLabel = new System.Windows.Forms.Label();
            this.yourTimeLabelLabel = new System.Windows.Forms.Label();
            this.theirTimerLabel = new System.Windows.Forms.Label();
            this.yourTimerLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.theirTimeLabelLabel);
            this.groupBox1.Controls.Add(this.yourTimeLabelLabel);
            this.groupBox1.Controls.Add(this.theirTimerLabel);
            this.groupBox1.Controls.Add(this.yourTimerLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // theirTimeLabelLabel
            // 
            this.theirTimeLabelLabel.AutoSize = true;
            this.theirTimeLabelLabel.Location = new System.Drawing.Point(355, 56);
            this.theirTimeLabelLabel.Name = "theirTimeLabelLabel";
            this.theirTimeLabelLabel.Size = new System.Drawing.Size(90, 13);
            this.theirTimeLabelLabel.TabIndex = 3;
            this.theirTimeLabelLabel.Text = "Opponent\'s Time:";
            // 
            // yourTimeLabelLabel
            // 
            this.yourTimeLabelLabel.AutoSize = true;
            this.yourTimeLabelLabel.Location = new System.Drawing.Point(387, 16);
            this.yourTimeLabelLabel.Name = "yourTimeLabelLabel";
            this.yourTimeLabelLabel.Size = new System.Drawing.Size(58, 13);
            this.yourTimeLabelLabel.TabIndex = 2;
            this.yourTimeLabelLabel.Text = "Your Time:";
            // 
            // theirTimerLabel
            // 
            this.theirTimerLabel.AutoSize = true;
            this.theirTimerLabel.Location = new System.Drawing.Point(451, 56);
            this.theirTimerLabel.Name = "theirTimerLabel";
            this.theirTimerLabel.Size = new System.Drawing.Size(129, 13);
            this.theirTimerLabel.TabIndex = 1;
            this.theirTimerLabel.Text = "OPPONENT TIME HERE";
            // 
            // yourTimerLabel
            // 
            this.yourTimerLabel.AutoSize = true;
            this.yourTimerLabel.Location = new System.Drawing.Point(451, 16);
            this.yourTimerLabel.Name = "yourTimerLabel";
            this.yourTimerLabel.Size = new System.Drawing.Size(99, 13);
            this.yourTimerLabel.TabIndex = 0;
            this.yourTimerLabel.Text = "USER TIME HERE";
            // 
            // ChessBoardForm
            // 
            this.AccessibleDescription = "";
            this.AccessibleName = "";
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(624, 601);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ChessBoardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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