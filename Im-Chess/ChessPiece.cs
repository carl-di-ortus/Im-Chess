using CoreUtils;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Windows.Media;

namespace Im_Chess
{
    public class ChessPiece : ViewModelBase
    {
        public int Id;

        private Point _pos;
        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; RaisePropertyChanged(() => Pos); }
        }

        private PieceType _type;
        public PieceType Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged(() => Type); }
        }

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { _player = value; RaisePropertyChanged(() => Player); }
        }

        private PointCollection _move;
        public PointCollection MoveArrow
        {
            get { return _move; }
            set { _move = value; RaisePropertyChanged(() => MoveArrow); }
        }
    }
}
