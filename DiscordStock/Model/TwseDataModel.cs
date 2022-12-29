using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIscordTest
{
    public class TwseDataModel
    {
        public DateTime Date { get; set; }

        public Decimal TransactionPrice { get; set; }

        public Decimal Turnover { get; set; }

        public Decimal OpeningPrice { get; set; }

        public Decimal HighestPrice { get; set; }
        public Decimal LowestPrice { get; set; }

        public Decimal CloseingPrice { get; set; }

        public Decimal PriceDifference { get; set; }

        public int TransCount { get; set; }

        public TwseDataModel(DateTime _date, Decimal _transactionPrice, Decimal _turnover, Decimal _openingPrice, Decimal _highestPrice, Decimal _lowestPrice,
            Decimal _closeingPrice, Decimal _priceDifference, int _transCount)

        {
            this.Date = _date;
            this.TransactionPrice = _transactionPrice;
            this.Turnover = _turnover;
            this.OpeningPrice = _openingPrice;
            this.HighestPrice = _highestPrice;
            this.LowestPrice = _lowestPrice;
            this.CloseingPrice = _closeingPrice;
            this.PriceDifference = _priceDifference;
            this.TransCount = _transCount;
        }


    }
}
