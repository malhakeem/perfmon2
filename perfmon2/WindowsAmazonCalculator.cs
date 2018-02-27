using System;
using System.Collections.Generic;

namespace perfmon2
{
    class WindowsAmazonCalculator : WindowsCalculator
    {
        // Postgres, SQL Server
        public int NoOfInstances
        {
            get; set;
        }
        public List<double> CalculateAllPrices(bool descending = true)
        {
            throw new NotImplementedException();
        }

        public double CalculateBestPrice(DBType db)
        {
            throw new NotImplementedException();
        }
    }
}
