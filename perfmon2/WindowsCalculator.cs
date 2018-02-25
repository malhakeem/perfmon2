using System.Collections.Generic;

namespace perfmon2
{
    interface WindowsCalculator
    {

        double CalculateBestPrice(DBType db);

        List<double> CalculateAllPrices(bool descending = true);
    }
}
