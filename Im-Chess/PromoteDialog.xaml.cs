using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoreUtils;

namespace Im_Chess
{
    public partial class PromoteDialog : UserControl
    {
        public PromoteDialog()
        {
            InitializeComponent();
        }

        private ObservableCollection<ChessPiece> Pieces { get; set; }

        public PieceType PromotedType { get; set; }

        public void ShowForPlayer(Player player)
        {
            if (player == Player.White)
            {
                Pieces = new ObservableCollection<ChessPiece>()
                {
                    new ChessPiece { Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.White, Id = 9 },
                    new ChessPiece { Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.White, Id = 10 },
                    new ChessPiece { Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.White, Id = 11 },
                    new ChessPiece { Pos = new Point(3, 0), Type = PieceType.Queen, Player = Player.White, Id = 12 }
                };
            }
            else
            {
                Pieces = new ObservableCollection<ChessPiece>()
                {
                    new ChessPiece { Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.Black, Id = 25 },
                    new ChessPiece { Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.Black, Id = 26 },
                    new ChessPiece { Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.Black, Id = 27 },
                    new ChessPiece { Pos = new Point(3, 0), Type = PieceType.Queen, Player = Player.Black, Id = 28 }
                };
            }
            ChessBoard.ItemsSource = Pieces;
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var piece = e.Source as Image;

            if (piece != null)
            {
                var currentPiece = piece.DataContext as ChessPiece;
                PromotedType = currentPiece.Type;

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }
    }
}
