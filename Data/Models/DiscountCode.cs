using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public bool Used { get; set; }
    }
}
