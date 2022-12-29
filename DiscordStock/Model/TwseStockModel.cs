using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIscordTest
{
    public class TwseStockModel
    {
        public string stat { get; set; }

        public string date { get; set; }

        public string title { get; set; }

        public string[] fields { get; set; }

        public string[][] data { get; set; }

        public string[] note { get; set; }
    }
}
