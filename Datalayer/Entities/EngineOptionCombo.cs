using System.ComponentModel.DataAnnotations;

namespace Datalayer.Entities
{
    public class EngineOptionCombo
    {
        [Key]
        public virtual Engine Engine { get; set; }

        [Key]
        public virtual EngineOption Option { get; set; }

        [Key]
        public string ComboValue { get; set; }
    }
}
