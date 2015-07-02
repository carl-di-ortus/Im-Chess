using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Datalayer.Entities
{
    public class GameHistory
    {
        [Key]
        public int Id { get; set; }

        public bool LastGame { get; set; }
        public string Notation { get; set; }

        public virtual List<ChessBoardPiece> Pieces { get; set; }
    }
}
