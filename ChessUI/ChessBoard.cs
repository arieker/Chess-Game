using ChessUI;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using ZstdSharp.Unsafe;


public class ChessBoardForm : Form
{
    private const int boardSize = 8;
    private const int squareSize = 90;
    // for timer in seconds
    private const int duration = 60;
    public BoardLogic boardLogic;
    private readonly Piece[,] board = new Piece[boardSize, boardSize];
    private GroupBox groupBox1;
    private Label theirTimeLabelLabel;
    private Label yourTimeLabelLabel;
    private Label theirTimerLabel;
    private Label yourTimerLabel;
    private readonly Image[,] pieceImages = new Image[6, 2]; // 6 types of pieces x 2 colors

    private Point prevMoveStartSquare = Point.Empty;
    private Point prevMoveEndSquare = Point.Empty;

    private Timer gameTimer;
   
    
    private int whiteTime = duration; 
    private int blackTime = duration; 

    private Label lblWhiteTime;
    private Label lblBlackTime;

    private Button btnDraw;
    private Button btnForfeit;

    private bool gameOver = false;


    public ChessBoardForm()
    {
        InitializeComponent();
        LoadPieceImages(); // Load piece images
        InitializeBoard();
        InitializeTimer();
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
        this.ClientSize = new Size(formWidth+squareSize, formHeight);

        //Move the winform box to the bottom
        groupBox1.Top = this.ClientSize.Height - groupBox1.Height;
        groupBox1.Text = ""; // make it prettier by removing the text on it

        // Register the DrawChessboard method to handle painting events
        this.Paint += new PaintEventHandler(DrawChessboard);
    }

    private void InitializeTimer()
    {
        
        gameTimer = new Timer();
        gameTimer.Interval = 1000;
        gameTimer.Tick += GameTimer_Tick;
        
    }

    private void GameTimer_Tick(object sender, EventArgs e)
    {
        // Update timer for current player
        if (boardLogic.whitesTurn)
        {
            whiteTime--;
            UpdateTimeDisplay(lblWhiteTime, whiteTime);
        }
        else if (!boardLogic.whitesTurn)
        {
            blackTime--;
            UpdateTimeDisplay(lblBlackTime, blackTime);
        }

        // Check for timeouts or end of game conditions
        if (whiteTime <= 0 || blackTime <= 0)
        {
            // Game over due to timeout
           
            EndGame();
            // Handle game over condition
            if(whiteTime == 0)
            {
                MessageBox.Show("Black Wins");

            }
            else
            {
                MessageBox.Show("White Wins");
            }
        }
    }

    private void UpdateTimeDisplay(Label lblTime, int timeInSeconds)
    {
        // Convert timeInSeconds to minutes and seconds
        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;

        // Update the label text with the formatted time
        lblTime.Text = $"{minutes:00}:{seconds:00}";
    }

    private void DrawChessboard(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                bool isLightSquare = (row + col) % 2 == 0;
                Brush squareBrush = (isLightSquare) ? Brushes.GhostWhite : Brushes.CornflowerBlue;

                int x = col * squareSize;
                int y = row * squareSize;

                g.FillRectangle(squareBrush, x, y, squareSize, squareSize);

                // Draw piece if exists
                board[row, col]?.Draw(g, x, y, squareSize);
            }
        }

        // Highlight previous move
        if (prevMoveStartSquare != Point.Empty && prevMoveEndSquare != Point.Empty)
        {
            int thickness = 6;
            using (Pen pen = new Pen(Color.Yellow, thickness))
            {

                int startX = prevMoveStartSquare.X * squareSize;
                int startY = prevMoveStartSquare.Y * squareSize;
                int endX = prevMoveEndSquare.X * squareSize;
                int endY = prevMoveEndSquare.Y * squareSize;
                

                g.DrawRectangle(pen, startX + thickness/2, startY + thickness / 2, squareSize - thickness, squareSize - thickness);
                g.DrawRectangle(pen, endX + thickness/2, endY + thickness / 2, squareSize - thickness, squareSize - thickness);
                
            }
        }

       

    }

    private Piece selectedPiece = null;
    private Point selectedSquare = Point.Empty;

    public void movePiece_visual(int x1, int y1, int x2, int y2) 
    {
        string currentUserforValidation = Program.currentUser.getUsername();
        selectedPiece = board[x1, y1];
        selectedSquare = new Point(y2, x2);
        int row = x1;
        int col = y1;

        // Move the selected piece to the destination square
        board[x2, y2] = selectedPiece;
        board[x1,y1] = null;

        InvalidateCell(prevMoveStartSquare);
        InvalidateCell(prevMoveEndSquare);

        // Record the previous move
        prevMoveStartSquare = selectedSquare;
        prevMoveEndSquare = new Point(col, row);

        // Invalidate the starting cell, the destination cell, and the previous move cells
        InvalidateCell(selectedSquare);
        InvalidateCell(new Point(col, row));

        selectedPiece = null;
        selectedSquare = Point.Empty;
    }
    
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
                gameTimer.Start();

            }
        }
        else
        {
            // If a piece is already selected, move the selected piece to the clicked position

            if (boardLogic.move(selectedSquare.X, 7 - selectedSquare.Y, col, 7 - row))
            {

                string usernameforvalidation = Program.currentUser.getUsername();
                // Move the selected piece to the destination square
                board[row, col] = selectedPiece;
                int x1 = selectedSquare.X; 
                int y1 = selectedSquare.Y;
                board[selectedSquare.Y, selectedSquare.X] = null;

                InvalidateCell(prevMoveStartSquare);
                InvalidateCell(prevMoveEndSquare);

                // Record the previous move
                prevMoveStartSquare = selectedSquare;
                prevMoveEndSquare = new Point(col, row);


                // Invalidate the starting cell, the destination cell, and the previous move cells
                InvalidateCell(selectedSquare);
                InvalidateCell(new Point(col, row));

                selectedPiece = null;
                selectedSquare = Point.Empty;

                int port = 31415;
                string ip = "127.0.0.1";
                Socket cs = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                cs.Connect(ep);
                string request = "move " + Program.currentUser.getUsername() + " " + y1 + " " + x1 + " " + row + " " + col;
                cs.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);
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


    private void BtnDraw_Click(object sender, EventArgs e)
    {
        if (!gameOver)
        {
            MessageBox.Show("Draw offered. Waiting for opponent's response.");
            // Implement logic to notify opponent about draw offer
        }
        else
        {
            MessageBox.Show("Draw accepted. Game ends in a draw.");
            EndGame();
        }
    }

    private void BtnForfeit_Click(object sender, EventArgs e)
    {
        //need to keep track of what player clicked to forfeit
        MessageBox.Show("You forfeit the game. Your opponent wins.");
        
        EndGame();
    }

    private void EndGame()
    {
        // Stop the game timer
        gameOver = true;
        gameTimer.Stop();

        // Disable the mouse click event
        this.MouseClick -= ChessBoardForm_MouseClick;

        // Disable the draw and forfeit buttons
        btnDraw.Enabled = false;
        btnForfeit.Enabled = false;
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
        // ChessBoardForm
        // 
        this.AccessibleDescription = "";
        this.AccessibleName = "";
        this.BackColor = System.Drawing.SystemColors.ControlLight;
        this.ClientSize = new System.Drawing.Size(624, 601);
        this.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Name = "ChessBoardForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.ResumeLayout(false);
        this.SuspendLayout();
        // 
        // ChessBoardForm
        // 
        this.AccessibleDescription = "";
        this.AccessibleName = "";
        this.BackColor = System.Drawing.SystemColors.ControlLight;
        this.ClientSize = new System.Drawing.Size(496, 473);
        this.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Name = "ChessBoardForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.ResumeLayout(false);

        // Initialize lblWhiteTime
        lblWhiteTime = new Label();
            
        lblWhiteTime.Font = new Font(lblWhiteTime.Font.FontFamily, 16); // Change 16 to the desired font size
        lblWhiteTime.ForeColor = Color.Black; // Change Color.Red to the desired color
        lblWhiteTime.BackColor = Color.White;

        UpdateTimeDisplay(lblWhiteTime, duration); // Initial time for White player

        lblWhiteTime.Location = new Point(squareSize * 8 + 10, squareSize * 7 - 10); // Adjust the location as needed
        lblWhiteTime.AutoSize = true;

        Controls.Add(lblWhiteTime);




        // Initialize lblBlackTime
        lblBlackTime = new Label();
        lblBlackTime.ForeColor = Color.White; // Change Color.Red to the desired color
        lblBlackTime.BackColor = Color.Black;
        lblBlackTime.Font = new Font(lblBlackTime.Font.FontFamily, 16); // Change 16 to the desired font size

        UpdateTimeDisplay(lblBlackTime, duration); // Initial time for Black player

        lblBlackTime.Location = new Point(squareSize * 8 + 10 , squareSize - 10); // Adjust the location as needed
        lblBlackTime.AutoSize = true;

        Controls.Add(lblBlackTime);


        int buttonSize = 30;
        // Initialize draw button
        btnDraw = new Button();
        btnDraw.Text = "Draw";
        btnDraw.Font = new Font(btnDraw.Font.FontFamily, 12);
        btnDraw.BackColor = Color.DeepSkyBlue;
        btnDraw.Location = new Point(squareSize * 8, squareSize * 3);
        btnDraw.Size = new Size(squareSize, buttonSize);
        btnDraw.Click += BtnDraw_Click;
        Controls.Add(btnDraw);

        // Initialize forfeit button
        btnForfeit = new Button();
        btnForfeit.Text = "Forfeit";
        btnForfeit.Font = new Font(btnForfeit.Font.FontFamily, 12);
        btnForfeit.BackColor = Color.Red;
        btnForfeit.Location = new Point(squareSize * 8, squareSize * 5 - buttonSize);
        btnForfeit.Size = new Size(squareSize, buttonSize);
        btnForfeit.Click += BtnForfeit_Click;
        Controls.Add(btnForfeit);



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