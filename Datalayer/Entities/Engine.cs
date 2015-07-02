using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Datalayer.Entities
{
    public class Engine
    {
        [Key]
        public string Name { get; set; }
        
        public string Author { get; set; }

        public virtual List<EngineOption> Options { get; set; }
    }
}
