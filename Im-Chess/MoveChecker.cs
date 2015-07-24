using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CoreUtils;

namespace Im_Chess
{
    public class MoveChecker
    {
        private int _enPassante;
        private Dictionary<Player, bool> _canCastleShort;
        private Dictionary<Player, bool> _canCastleLong;
        private Dictionary<Player, bool> _hasCastled;

        public MoveChecker()
        {
            Reset();
        }

        public bool Check { get; set; }

        public bool EnPassante { get; set; }
        public bool EnPassanteRemove { get; set; }

        public Player SideToMove { get; set; }
        
        public bool IsLegalMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            if (piece.Player != SideToMove || (x.IsEqual(position.X) && y.IsEqual(position.Y)) || IsKingInCheck(pieces, piece, position, x, y))
            {
                return false;
            }
            piece.Pos = position;

            switch (piece.Type)
            {
                case PieceType.Bishop:
                    return IsLegalBishopMove(pieces, piece, position, x, y);
                case PieceType.King:
                    return IsLegalKingMove(pieces, piece, position, x, y);
                case PieceType.Knight:
                    return IsLegalKnightMove(pieces, piece, position, x, y);
                case PieceType.Pawn:
                    return IsLegalPawnMove(pieces, piece, position, x, y);
                case PieceType.Queen:
                    return IsLegalQueenMove(pieces, piece, position, x, y);
                case PieceType.Rook:
                    return IsLegalRookMove(pieces, piece, position, x, y);
            }
            return false;
        }

        public void Reset()
        {
            Check = false;
            EnPassante = false;
            SideToMove = Player.White;
            _canCastleShort = new Dictionary<Player, bool>
            {
                { Player.White, true },
                { Player.Black, true }
            };
            _canCastleLong = new Dictionary<Player, bool>
            {
                { Player.White, true },
                { Player.Black, true }
            };
            _hasCastled = new Dictionary<Player, bool>
            {
                { Player.White, false },
                { Player.Black, false }
            };
        }

        public void ToggleSideToMove()
        {
            SideToMove = SideToMove == Player.Black ? Player.White : Player.Black;
        }

        private bool IsKingInCheck(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            // temporary piece if move was to capture another piece
            var removable = pieces.FirstOrDefault(p => p.Id != piece.Id && x.IsEqual(p.Pos.X) && y.IsEqual(p.Pos.Y));
            if (removable != null)
            {
                pieces.Remove(removable);
            }
            else if (IsLegalPawnMove(pieces, piece, position, x, y))
            {
                if (EnPassanteRemove)
                {
                    EnPassante = true;
                    EnPassanteRemove = false;
                    removable = y == 5 ? pieces.FirstOrDefault(p => x.IsEqual(p.Pos.X) && 4.IsEqual(p.Pos.Y)) : pieces.FirstOrDefault(p => x.IsEqual(p.Pos.X) && 3.IsEqual(p.Pos.Y));
                    pieces.Remove(removable);
                }
            }

            piece.Pos = new Point(x, y);
            
            var king = pieces.FirstOrDefault(p => p.Player == piece.Player && p.Type == PieceType.King);
            if (king == null)
            {
                if (removable != null)
                {
                    pieces.Add(removable);
                }
                return true;
            }

            if (KingInCheckByRook(pieces, king, removable) || KingInCheckByBishop(pieces, king, removable))
            {
                return true;
            }

            var kingx = Convert.ToInt32(king.Pos.X);
            var kingy = Convert.ToInt32(king.Pos.Y);


            if (
                pieces.Any(
                    p =>
                        p.Player != king.Player && p.Type == PieceType.Knight &&
                        ((1.IsEqual(Math.Abs(kingx - p.Pos.X)) && 2.IsEqual(Math.Abs(kingy - p.Pos.Y))) || (2.IsEqual(Math.Abs(kingx - p.Pos.X)) && 1.IsEqual(Math.Abs(kingy - p.Pos.Y))))))
            {
                if (removable != null)
                {
                    pieces.Add(removable);
                }
                return true;
            }

            if ((piece.Player == Player.Black && !Converters.Invert) || (piece.Player == Player.White && Converters.Invert))
            {
                if (pieces.Any(p => p.Player != king.Player && p.Type == PieceType.Pawn && 1.IsEqual(Math.Abs(kingx - p.Pos.X)) && 1.IsEqual(p.Pos.Y - kingy)))
                {
                    if (removable != null)
                    {
                        pieces.Add(removable);
                    }
                    return true;
                }
            }
            else
            {
                if (pieces.Any(p => p.Player != king.Player && p.Type == PieceType.Pawn && 1.IsEqual(Math.Abs(kingx - p.Pos.X)) && 1.IsEqual(kingy - p.Pos.Y)))
                {
                    if (removable != null)
                    {
                        pieces.Add(removable);
                    }
                    return true;
                }
            }

            if (removable != null)
            {
                pieces.Add(removable);
            }
            return false;
        }

        private bool IsLegalBishopMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            var oldX = Convert.ToInt32(position.X);
            var oldY = Convert.ToInt32(position.Y);

            if (Math.Abs(x - oldX) != Math.Abs(y - oldY))
            {
                return false;
            }

            var squares = oldX.To(x).Zip(oldY.To(y), (xx, yy) => new { X = xx, Y = yy });
            foreach (var sq in squares)
            {
                if (pieces.Any(p => p.Id != piece.Id && p.Player == piece.Player && p.Pos.X.IsEqual(sq.X) && p.Pos.Y.IsEqual(sq.Y)))
                {
                    return false;
                }
                if (pieces.Any(p => p.Id != piece.Id && p.Player != piece.Player && p.Pos.X.IsEqual(sq.X) && p.Pos.Y.IsEqual(sq.Y)) && sq.Y != y)
                {
                    return false;
                }
            }

            EnPassante = false;
            EnPassanteRemove = false;
            return true;
        }

        private bool IsLegalKingMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            var oldX = Convert.ToInt32(position.X);
            var oldY = Convert.ToInt32(position.Y);

            var otherKing = pieces.First(p => p.Type == PieceType.King && p.Id != piece.Id);
            if (Math.Abs(x - otherKing.Pos.X) <= 1 && Math.Abs(y - otherKing.Pos.Y) <= 1)
            {
                return false;
            }

            if (Math.Abs(x - oldX) <= 1 && Math.Abs(y - oldY) <= 1)
            {
                if (!IsLegalRookMove(pieces, piece, position, x, y) && !IsLegalBishopMove(pieces, piece, position, x, y))
                {
                    return false;
                }
                _canCastleShort[piece.Player] = false;
                _canCastleLong[piece.Player] = false;
                return true;
            }

            // castling
            if (IsKingInCheck(pieces, piece, position, oldX, oldY) || IsKingInCheck(pieces, piece, position, (oldX + x) / 2, (oldY + y) / 2))
            {
                return false;
            }

            if (x - oldX == -2 && y == oldY && !_hasCastled[piece.Player] && _canCastleLong[piece.Player])
            {
                var rook = pieces.First(p => p.Type == PieceType.Rook && y.IsEqual(p.Pos.Y) && 0.IsEqual(p.Pos.X));
                if (IsLegalRookMove(pieces.Where(p => p.Id != piece.Id && p.Id != rook.Id).ToList(), rook, rook.Pos, x + 1, y))
                {
                    _hasCastled[piece.Player] = true;
                    _canCastleLong[piece.Player] = false;
                    _canCastleShort[piece.Player] = false;
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
            }
            if (x - oldX == 2 && y == oldY && !_hasCastled[piece.Player] && _canCastleShort[piece.Player])
            {
                var rook = pieces.First(p => p.Type == PieceType.Rook && y.IsEqual(p.Pos.Y) && 7.IsEqual(p.Pos.X));
                if (IsLegalRookMove(pieces.Where(p => p.Id != piece.Id && p.Id != rook.Id).ToList(), rook, rook.Pos, x - 1, y))
                {
                    _hasCastled[piece.Player] = true;
                    _canCastleLong[piece.Player] = false;
                    _canCastleShort[piece.Player] = false;
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
            }
            return false;
        }

        private bool IsLegalKnightMove(IEnumerable<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            var oldX = Convert.ToInt32(position.X);
            var oldY = Convert.ToInt32(position.Y);

            if ((Math.Abs(x - oldX) != 1 || Math.Abs(y - oldY) != 2) &&
                (Math.Abs(x - oldX) != 2 || Math.Abs(y - oldY) != 1))
            {
                return false;
            }

            if (pieces.Any(p => p.Id != piece.Id && p.Player == piece.Player && x.IsEqual(p.Pos.X) && y.IsEqual(p.Pos.Y)))
            {
                return false;
            }

            EnPassante = false;
            EnPassanteRemove = false;
            return true;
        }

        private bool IsLegalPawnMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            var oldX = Convert.ToInt32(position.X);
            var oldY = Convert.ToInt32(position.Y);
            if ((piece.Player == Player.Black && !Converters.Invert) || (piece.Player == Player.White && Converters.Invert))
            {
                if (oldX == x && oldY == y - 1 && !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y))
                {
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
                if (oldX == x && oldY == y - 2 && oldY == 1 &&
                    !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y) &&
                    !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y - 1))
                {
                    EnPassante = true;
                    EnPassanteRemove = false;
                    _enPassante = x;
                    return true;
                }
                if (Math.Abs(oldX - x) == 1 && oldY == y - 1 && pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y))
                {
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
                if (Math.Abs(oldX - x) == 1 && oldY == y - 1 && EnPassante && x == _enPassante && y == 5)
                {
                    EnPassante = false;
                    EnPassanteRemove = true;
                    return true;
                }
            }
            else
            {
                if (oldX == x && oldY == y + 1 && !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y))
                {
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
                if (oldX == x && oldY == y + 2 && oldY == 6 &&
                    !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y) &&
                    !pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y + 1))
                {
                    EnPassante = true;
                    EnPassanteRemove = false;
                    _enPassante = x;
                    return true;
                }
                if (Math.Abs(oldX - x) == 1 && oldY == y + 1 && pieces.Any(p => p.Id != piece.Id && Convert.ToInt32(p.Pos.X) == x && Convert.ToInt32(p.Pos.Y) == y))
                {
                    EnPassante = false;
                    EnPassanteRemove = false;
                    return true;
                }
                if (Math.Abs(oldX - x) == 1 && oldY == y + 1 && EnPassante && x == _enPassante && y == 2)
                {

                    EnPassante = false;
                    EnPassanteRemove = true;
                    return true;
                }
            }
            return false;
        }

        private bool IsLegalQueenMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            return IsLegalRookMove(pieces, piece, position, x, y) || IsLegalBishopMove(pieces, piece, position, x, y);
        }

        private bool IsLegalRookMove(IList<ChessPiece> pieces, ChessPiece piece, Point position, int x, int y)
        {
            var oldX = Convert.ToInt32(position.X);
            var oldY = Convert.ToInt32(position.Y);
            if (x == oldX)
            {
                foreach (var i in oldY.To(y))
                {
                    if (pieces.Any(p => p.Id != piece.Id && p.Player == piece.Player && x.IsEqual(p.Pos.X) && i.IsEqual(p.Pos.Y)))
                    {
                        return false;
                    }
                    if (pieces.Any(p => p.Id != piece.Id && p.Player != piece.Player && x.IsEqual(p.Pos.X) && i.IsEqual(p.Pos.Y)) && i != y)
                    {
                        return false;
                    }
                }
                EnPassante = false;
                EnPassanteRemove = false;
                if ((!Converters.Invert && oldX == 7) || (Converters.Invert && oldX == 0))
                {
                    _canCastleShort[piece.Player] = false;
                }
                else if ((!Converters.Invert && oldX == 0) || (Converters.Invert && oldX == 7))
                {
                    _canCastleLong[piece.Player] = false;
                }
                return true;
            }
            if (y == oldY)
            {
                foreach (var i in oldX.To(x))
                {
                    if (pieces.Any(p => p.Id != piece.Id && p.Player == piece.Player && i.IsEqual(p.Pos.X) && y.IsEqual(p.Pos.Y)))
                    {
                        return false;
                    }
                    if (pieces.Any(p => p.Id != piece.Id && p.Player != piece.Player && i.IsEqual(p.Pos.X) && y.IsEqual(p.Pos.Y)) && i != x)
                    {
                        return false;
                    }
                }
                EnPassante = false;
                EnPassanteRemove = false;
                if ((!Converters.Invert && oldX == 7) || (Converters.Invert && oldX == 0))
                {
                    _canCastleShort[piece.Player] = false;
                }
                else if ((!Converters.Invert && oldX == 0) || (Converters.Invert && oldX == 7))
                {
                    _canCastleLong[piece.Player] = false;
                }
                return true;
            }
            return false;
        }

        private bool KingInCheckByBishop(ICollection<ChessPiece> pieces, ChessPiece king, ChessPiece removable)
        {
            var kingx = Convert.ToInt32(king.Pos.X);
            var kingy = Convert.ToInt32(king.Pos.Y);

            var nx = kingx + 1;
            var ny = kingy + 1;
            while (nx < 8 && ny < 8)
            {
                var sq = pieces.FirstOrDefault(p => p.Id != king.Id && nx.IsEqual(p.Pos.X) && ny.IsEqual(p.Pos.Y));
                if (sq != null)
                {
                    if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Bishop))
                    {
                        return true;
                    }
                    break;
                }
                nx++;
                ny++;
            }

            nx = kingx + 1;
            ny = kingy - 1;
            while (nx < 8 && ny >= 0)
            {
                var sq = pieces.FirstOrDefault(p => p.Id != king.Id && nx.IsEqual(p.Pos.X) && ny.IsEqual(p.Pos.Y));
                if (sq != null)
                {
                    if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Bishop))
                    {
                        return true;
                    }
                    break;
                }
                nx++;
                ny--;
            }

            nx = kingx - 1;
            ny = kingy - 1;
            while (nx >= 0 && ny >= 0)
            {
                var sq = pieces.FirstOrDefault(p => p.Id != king.Id && nx.IsEqual(p.Pos.X) && ny.IsEqual(p.Pos.Y));
                if (sq != null)
                {
                    if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Bishop))
                    {
                        return true;
                    }
                    break;
                }
                nx--;
                ny--;
            }

            nx = kingx - 1;
            ny = kingy + 1;
            while (nx >= 0 && ny < 8)
            {
                var sq = pieces.FirstOrDefault(p => p.Id != king.Id && nx.IsEqual(p.Pos.X) && ny.IsEqual(p.Pos.Y));
                if (sq != null)
                {
                    if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Bishop))
                    {
                        return true;
                    }
                    break;
                }
                nx--;
                ny++;
            }
            return false;
        }

        private bool KingInCheckByRook(ICollection<ChessPiece> pieces, ChessPiece king, ChessPiece removable)
        {
            var kingx = Convert.ToInt32(king.Pos.X);
            var kingy = Convert.ToInt32(king.Pos.Y);

            foreach (var sq in kingx.To(0).Select(i => pieces.FirstOrDefault(p => p.Id != king.Id && i.IsEqual(p.Pos.X) && kingy.IsEqual(p.Pos.Y))).Where(sq => sq != null))
            {
                if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Rook))
                {
                    return true;
                }
                break;
            }

            foreach (var sq in kingx.To(7).Select(i => pieces.FirstOrDefault(p => p.Id != king.Id && i.IsEqual(p.Pos.X) && kingy.IsEqual(p.Pos.Y))).Where(sq => sq != null))
            {
                if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Rook))
                {
                    return true;
                }
                break;
            }

            foreach (var sq in kingy.To(0).Select(i => pieces.FirstOrDefault(p => p.Id != king.Id && kingx.IsEqual(p.Pos.X) && i.IsEqual(p.Pos.Y))).Where(sq => sq != null))
            {
                if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Rook))
                {
                    return true;
                }
                break;
            }

            foreach (var sq in kingy.To(7).Select(i => pieces.FirstOrDefault(p => p.Id != king.Id && kingx.IsEqual(p.Pos.X) && i.IsEqual(p.Pos.Y))).Where(sq => sq != null))
            {
                if (KingOctagonalCheck(pieces, sq, removable, king.Player, PieceType.Rook))
                {
                    return true;
                }
                break;
            }
            return false;
        }

        private bool KingOctagonalCheck(ICollection<ChessPiece> pieces, ChessPiece square, ChessPiece removable, Player kingPlayer, PieceType pieceType)
        {
            if (square.Player == kingPlayer || (square.Type != pieceType && square.Type != PieceType.Queen))
            {
                return false;
            }

            if (removable != null)
            {
                pieces.Add(removable);
            }
            return true;
        }
    }
}
