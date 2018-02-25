using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perfmon2
{
    class WindowsAzureCalculator : WindowsCalculator
    {
        public WindowsAzureCalculator(double maxCPU, double readSum, double writeSum, double noOfCores, double storage, double noOfHours)
        {
            MaxCPU = maxCPU;
            ReadSum = readSum;
            WriteSum = writeSum;
            NoOfCores = noOfCores;
            Storage = storage;
            NoOfHours = noOfHours;
        }

        double MaxCPU
        {
            get; set;
        }
        double ReadSum
        {
            get; set;
        }
        double WriteSum
        {
            get; set;
        }
        double NoOfCores
        {
            get; set;
        }
        double Storage
        {
            get; set;
        }
        double NoOfHours
        {
            get; set;
        }

        public List<double> CalculateAllPrices(bool descending = true)
        {
            return new List<double>();
        }

        public double CalculateBestPrice()
        {
            return 100;
        }
    }
}
