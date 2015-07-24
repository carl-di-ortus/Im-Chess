using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bizlayer;
using CoreUtils;
using Datalayer.Entities;
using Datalayer.Repositories;

namespace Im_Chess
{
    public partial class Board : UserControl
    {
        private Image _draggedPiece;
        private Point _mousePosition;
        private Point _position;
        private EngineProcess _engine;
        private string _currentPosition;
        private string _gameNotation;
        private readonly MoveChecker _moveChecker;

        public Board()
        {
            InitializeComponent();

            _moveChecker = new MoveChecker();
            Converters.Invert = false;
            AutoFlip = false;
        }

        private ObservableCollection<ChessPiece> Pieces { get; set; }

        public bool AutoFlip { get; set; }

        public bool TwoPlayer { get; set; }

        public List<EngineOption> GetEngineOptions()
        {
            return _engine.Options;
        }

        public string SetEngine(string path)
        {
            _engine = new EngineProcess(path);
            return _engine.Engine.Name;
        }

        public void MakeEngineMove()
        {
            if (_engine == null)
            {
                return;
            }

            var handler = (MainWindow)Application.Current.MainWindow;
            handler.ClearEngineOutput();
            var bestmove = "(none)";
            var response = _engine.Go(_gameNotation);
            foreach (var line in response)
            {
                if (line.StartsWith("bestmove"))
                {
                    bestmove = line.Split(' ')[1];
                }
                handler.AppendEngineOutput(line + "\n");
            }
            _position = Converters.ConvertFromCoord(bestmove);

            if (bestmove == "(none)")
            {
                return;
            }

            var piece = Pieces.First(p => p.Pos.X.IsEqual(_position.X) && p.Pos.Y.IsEqual(_position.Y));
            if (piece == null)
            {
                return;
            }

            var position = Converters.ConvertFromCoord(bestmove.Substring(2));

            if (!_moveChecker.IsLegalMove(Pieces, piece, piece.Pos, Convert.ToInt32(position.X), Convert.ToInt32(position.Y)))
            {
                return;
            }

            DrawMoveArrow(piece, _position, new Point(position.X, position.Y));
            DropPiece(piece, position.X, position.Y);

            if (piece.Type == PieceType.King)
            {
                if ((-2).IsEqual(position.X - _position.X))
                {
                    var rook = Pieces.First(p => p.Type == PieceType.Rook && p.Pos.Y.IsEqual(_position.Y) && p.Pos.X.IsEqual(0));
                    DropPiece(rook, position.X + 1, position.Y);
                }
                if (2.IsEqual(position.X - _position.X))
                {
                    var rook = Pieces.First(p => p.Type == PieceType.Rook && p.Pos.Y.IsEqual(_position.Y) && p.Pos.X.IsEqual(7));
                    DropPiece(rook, position.X - 1, position.Y);
                }
            }
            
            if (piece.Type == PieceType.Pawn && (position.Y.IsEqual(0) || position.Y.IsEqual(7)) && bestmove.Length == 5)
            {
                switch (bestmove.ToLower()[4])
                {
                    case 'n':
                        piece.Type = PieceType.Knight;
                        break;
                    case 'b':
                        piece.Type = PieceType.Bishop;
                        break;
                    case 'r':
                        piece.Type = PieceType.Rook;
                        break;
                    case 'q':
                        piece.Type = PieceType.Queen;
                        break;
                }
            }

            _moveChecker.ToggleSideToMove();
            if (AutoFlip)
            {
                FlipBoard();
            }
            _gameNotation = _gameNotation + bestmove + ' ';
        }

        public void NewGame()
        {
            if (Converters.Invert)
            {
                Pieces = new ObservableCollection<ChessPiece>() {
                new ChessPiece{Pos = new Point(7, 1), Type = PieceType.Pawn, Player = Player.White, Id = 1},
                new ChessPiece{Pos = new Point(6, 1), Type = PieceType.Pawn, Player = Player.White, Id = 2},
                new ChessPiece{Pos = new Point(5, 1), Type = PieceType.Pawn, Player = Player.White, Id = 3},
                new ChessPiece{Pos = new Point(4, 1), Type = PieceType.Pawn, Player = Player.White, Id = 4},
                new ChessPiece{Pos = new Point(3, 1), Type = PieceType.Pawn, Player = Player.White, Id = 5},
                new ChessPiece{Pos = new Point(2, 1), Type = PieceType.Pawn, Player = Player.White, Id = 6},
                new ChessPiece{Pos = new Point(1, 1), Type = PieceType.Pawn, Player = Player.White, Id = 7},
                new ChessPiece{Pos = new Point(0, 1), Type = PieceType.Pawn, Player = Player.White, Id = 8},
                new ChessPiece{Pos = new Point(7, 0), Type = PieceType.Rook, Player = Player.White, Id = 9},
                new ChessPiece{Pos = new Point(6, 0), Type = PieceType.Knight, Player = Player.White, Id = 10},
                new ChessPiece{Pos = new Point(5, 0), Type = PieceType.Bishop, Player = Player.White, Id = 11},
                new ChessPiece{Pos = new Point(4, 0), Type = PieceType.Queen, Player = Player.White, Id = 12},
                new ChessPiece{Pos = new Point(3, 0), Type = PieceType.King, Player = Player.White, Id = 13},
                new ChessPiece{Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.White, Id = 14},
                new ChessPiece{Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.White, Id = 15},
                new ChessPiece{Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.White, Id = 16},
                new ChessPiece{Pos = new Point(7, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 17},
                new ChessPiece{Pos = new Point(6, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 18},
                new ChessPiece{Pos = new Point(5, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 19},
                new ChessPiece{Pos = new Point(4, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 20},
                new ChessPiece{Pos = new Point(3, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 21},
                new ChessPiece{Pos = new Point(2, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 22},
                new ChessPiece{Pos = new Point(1, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 23},
                new ChessPiece{Pos = new Point(0, 6), Type = PieceType.Pawn, Player = Player.Black, Id = 24},
                new ChessPiece{Pos = new Point(7, 7), Type = PieceType.Rook, Player = Player.Black, Id = 25},
                new ChessPiece{Pos = new Point(6, 7), Type = PieceType.Knight, Player = Player.Black, Id = 26},
                new ChessPiece{Pos = new Point(5, 7), Type = PieceType.Bishop, Player = Player.Black, Id = 27},
                new ChessPiece{Pos = new Point(4, 7), Type = PieceType.Queen, Player = Player.Black, Id = 28},
                new ChessPiece{Pos = new Point(3, 7), Type = PieceType.King, Player = Player.Black, Id = 29},
                new ChessPiece{Pos = new Point(2, 7), Type = PieceType.Bishop, Player = Player.Black, Id = 30},
                new ChessPiece{Pos = new Point(1, 7), Type = PieceType.Knight, Player = Player.Black, Id = 31},
                new ChessPiece{Pos = new Point(0, 7), Type = PieceType.Rook, Player = Player.Black, Id = 32}
            };
            }
            else
            {
                Pieces = new ObservableCollection<ChessPiece>() {
                new ChessPiece{Pos = new Point(0, 6), Type = PieceType.Pawn, Player = Player.White, Id = 1},
                new ChessPiece{Pos = new Point(1, 6), Type = PieceType.Pawn, Player = Player.White, Id = 2},
                new ChessPiece{Pos = new Point(2, 6), Type = PieceType.Pawn, Player = Player.White, Id = 3},
                new ChessPiece{Pos = new Point(3, 6), Type = PieceType.Pawn, Player = Player.White, Id = 4},
                new ChessPiece{Pos = new Point(4, 6), Type = PieceType.Pawn, Player = Player.White, Id = 5},
                new ChessPiece{Pos = new Point(5, 6), Type = PieceType.Pawn, Player = Player.White, Id = 6},
                new ChessPiece{Pos = new Point(6, 6), Type = PieceType.Pawn, Player = Player.White, Id = 7},
                new ChessPiece{Pos = new Point(7, 6), Type = PieceType.Pawn, Player = Player.White, Id = 8},
                new ChessPiece{Pos = new Point(0, 7), Type = PieceType.Rook, Player = Player.White, Id = 9},
                new ChessPiece{Pos = new Point(1, 7), Type = PieceType.Knight, Player = Player.White, Id = 10},
                new ChessPiece{Pos = new Point(2, 7), Type = PieceType.Bishop, Player = Player.White, Id = 11},
                new ChessPiece{Pos = new Point(3, 7), Type = PieceType.Queen, Player = Player.White, Id = 12},
                new ChessPiece{Pos = new Point(4, 7), Type = PieceType.King, Player = Player.White, Id = 13},
                new ChessPiece{Pos = new Point(5, 7), Type = PieceType.Bishop, Player = Player.White, Id = 14},
                new ChessPiece{Pos = new Point(6, 7), Type = PieceType.Knight, Player = Player.White, Id = 15},
                new ChessPiece{Pos = new Point(7, 7), Type = PieceType.Rook, Player = Player.White, Id = 16},
                new ChessPiece{Pos = new Point(0, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 17},
                new ChessPiece{Pos = new Point(1, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 18},
                new ChessPiece{Pos = new Point(2, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 19},
                new ChessPiece{Pos = new Point(3, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 20},
                new ChessPiece{Pos = new Point(4, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 21},
                new ChessPiece{Pos = new Point(5, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 22},
                new ChessPiece{Pos = new Point(6, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 23},
                new ChessPiece{Pos = new Point(7, 1), Type = PieceType.Pawn, Player = Player.Black, Id = 24},
                new ChessPiece{Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.Black, Id = 25},
                new ChessPiece{Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.Black, Id = 26},
                new ChessPiece{Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.Black, Id = 27},
                new ChessPiece{Pos = new Point(3, 0), Type = PieceType.Queen, Player = Player.Black, Id = 28},
                new ChessPiece{Pos = new Point(4, 0), Type = PieceType.King, Player = Player.Black, Id = 29},
                new ChessPiece{Pos = new Point(5, 0), Type = PieceType.Bishop, Player = Player.Black, Id = 30},
                new ChessPiece{Pos = new Point(6, 0), Type = PieceType.Knight, Player = Player.Black, Id = 31},
                new ChessPiece{Pos = new Point(7, 0), Type = PieceType.Rook, Player = Player.Black, Id = 32}
            };
            }
            
            _gameNotation = "position startpos moves ";
            ChessBoard.ItemsSource = Pieces;
            _moveChecker.Reset();
        }

        public void SaveGame()
        {
            var game = new GameHistory() { Notation = _gameNotation, Pieces = new List<ChessBoardPiece>() };
            foreach (var boardPiece in Pieces.Select(piece => new ChessBoardPiece() { Player = piece.Player, Type = piece.Type, Position = Converters.ConvertToCoord(piece.Pos.X, piece.Pos.Y) }))
            {
                game.Pieces.Add(boardPiece);
            }
            var repo = new GameHistoryRepo();
            repo.AddGame(game);
        }

        public event EventHandler EngineOutput;

        public void FlipBoard()
        {
            foreach (var piece in Pieces)
            {
                piece.Pos = new Point(7 - piece.Pos.X, 7 - piece.Pos.Y);
            }
            Converters.ToggleInvert();
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var piece = e.Source as Image;
            if (piece == null)
            {
                return;
            }

            _mousePosition = e.GetPosition(ChessBoard);
            _draggedPiece = piece;
            Panel.SetZIndex(_draggedPiece, 100);

            var currentPiece = _draggedPiece.DataContext as ChessPiece;
            if (currentPiece == null)
            {
                return;
            }

            _position = new Point(Convert.ToInt32(currentPiece.Pos.X), Convert.ToInt32(currentPiece.Pos.Y));
            _currentPosition = Converters.ConvertToCoord(currentPiece.Pos.X, currentPiece.Pos.Y);
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedPiece == null)
            {
                return;
            }

            Panel.SetZIndex(_draggedPiece, 0);
            var piece = _draggedPiece.DataContext as ChessPiece;
            if (piece == null)
            {
                return;
            }

            var newX = Convert.ToInt32(piece.Pos.X);
            var newY = Convert.ToInt32(piece.Pos.Y);
            if (!_moveChecker.IsLegalMove(Pieces, piece, _position, newX, newY))
            {
                _draggedPiece = null;
                piece.Pos = _position;
                return;
            }

            // castling - drop king first
            DrawMoveArrow(piece, _position, new Point(newX, newY));
            DropPiece(piece, newX, newY);
            _gameNotation = _gameNotation + _currentPosition + Converters.ConvertToCoord(piece.Pos.X, piece.Pos.Y) + ' ';

            if (piece.Type == PieceType.King && newX - _position.X == -2)
            {
                var rook = Pieces.First(p => p.Type == PieceType.Rook && p.Pos.Y == _position.Y && p.Pos.X == 0);
                DropPiece(rook, newX + 1, newY);
            }
            if (piece.Type == PieceType.King && newX - _position.X == 2)
            {
                var rook = Pieces.First(p => p.Type == PieceType.Rook && p.Pos.Y == _position.Y && p.Pos.X == 7);
                DropPiece(rook, newX - 1, newY);
            }

            // promotion
            if (piece.Type == PieceType.Pawn && (newY == 0 || newY == 7))
            {
                var promotion = new PromoteWindow(piece.Player);
                var window = ((MainWindow)(Application.Current.MainWindow)).MainBoard;
                promotion.Width = window.ActualWidth / 2 + 6;
                promotion.Height = window.ActualHeight / 8 + 6;
                promotion.Left = Application.Current.MainWindow.Left + window.VisualOffset.X + window.ActualWidth / 4;
                promotion.Top = Application.Current.MainWindow.Top + window.VisualOffset.Y + 7 * window.ActualHeight / 16;
                promotion.ShowDialog();

                piece.Type = promotion.PromotePieceType;
                switch (piece.Type)
                {
                    case PieceType.Knight:
                        _gameNotation = _gameNotation.Insert(_gameNotation.Length - 1, "n");
                        break;
                    case PieceType.Bishop:
                        _gameNotation = _gameNotation.Insert(_gameNotation.Length - 1, "b");
                        break;
                    case PieceType.Rook:
                        _gameNotation = _gameNotation.Insert(_gameNotation.Length - 1, "r");
                        break;
                    case PieceType.Queen:
                        _gameNotation = _gameNotation.Insert(_gameNotation.Length - 1, "q");
                        break;
                }
            }

            _moveChecker.ToggleSideToMove();
            if (AutoFlip)
            {
                FlipBoard();
            }
            if (!TwoPlayer && _engine != null)
            {
                MakeEngineMove();
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedPiece == null)
            {
                return;
            }

            var position = e.GetPosition(ChessBoard);
            var offset = position - _mousePosition;
            _mousePosition = position;
            var piece = _draggedPiece.DataContext as ChessPiece;

            if (piece == null)
            {
                return;
            }

            var newPosX = piece.Pos.X + offset.X;
            var newPosY = piece.Pos.Y + offset.Y;

            if (newPosX < -0.49)
            {
                newPosX = -0.49;
            }
            else if (newPosX > 7.49)
            {
                newPosX = 7.49;
            }

            if (newPosY < -0.49)
            {
                newPosY = -0.49;
            }
            else if (newPosY > 7.49)
            {
                newPosY = 7.49;
            }

            piece.Pos = new Point(newPosX, newPosY);
        }

        //todo: fix this method
        private void DrawMoveArrow(ChessPiece piece, Point source, Point destination)
        {
            var removable = Pieces.FirstOrDefault(p => p.MoveArrow != null);
            if (removable != null)
            {
                removable.MoveArrow = null;
            }

            piece.MoveArrow = Types.CreateArrowPointCollection(source, destination, 0.1);

        }

        private void DropPiece(ChessPiece piece, double x, double y)
        {
            var occupied = Pieces.FirstOrDefault(p => x.IsEqual(p.Pos.X) && y.IsEqual(p.Pos.Y) && p.Id != piece.Id);
            if (occupied != null)
            {
                Pieces.Remove(occupied);
            }
            piece.Pos = new Point(x, y);
            _draggedPiece = null;

            if (_moveChecker.EnPassanteRemove)
            {
                var removable = 2.IsEqual(y) ? Pieces.First(p => x.IsEqual(p.Pos.X) && 3.IsEqual(p.Pos.Y)) : Pieces.First(p => x.IsEqual(p.Pos.X) && 4.IsEqual(p.Pos.Y));
                Pieces.Remove(removable);
            }
        }
    }
}
