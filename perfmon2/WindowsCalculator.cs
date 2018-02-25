using System.Collections.Generic;

namespace perfmon2
{
    interface WindowsCalculator
    {
        double CalculateBestPrice();

        List<double> CalculateAllPrices(bool descending = true);
    }
}
