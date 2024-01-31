using System;
using System.Drawing;
using System.Windows.Forms;

public class ChessboardForm : Form
{
    private const int boardSize = 8;
    private const int squareSize = 80; 

    public ChessboardForm()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
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
            }
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ChessboardForm());
    }
}
