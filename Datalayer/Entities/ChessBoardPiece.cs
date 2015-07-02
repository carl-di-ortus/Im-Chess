using CoreUtils;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Datalayer.Entities
{
    public class ChessBoardPiece
    {
        [Key]
        public int Id { get; set; }

        public string Position { get; set; }
        public PieceType Type { get; set; }
        public Player Player { get; set; }
    }
}
