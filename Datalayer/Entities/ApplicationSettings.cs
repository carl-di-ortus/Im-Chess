using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Entities
{
    public class ApplicationSetting
    {
        [Key]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
