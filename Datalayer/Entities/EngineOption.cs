using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Datalayer.Entities
{
    public enum EngineOptionType
    {
        Check,  //bool
        Spin,   //int in range, ex: 10..1000
        Combo,  //list<string>
        Button, //void command
        String, //string
    }

    public class EngineOption
    {
        [Key]
        public string Name { get; set; }
        
        public EngineOptionType Type { get; set; }
        public string Value { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public virtual List<EngineOptionCombo> ComboOption { get; set; } 
    }
}
