using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perfmon2
{
    class WindowsIBMCalculator : WindowsCalculator
    {
        public WindowsIBMCalculator(double cpu, double noOfHours, int noOfInstances)
        {
            Cpu = cpu;
            NoOfHours = noOfHours;
            NoOfInstances = noOfInstances;
        }

        public double Cpu
        {
            get; set;
        }

        public double NoOfHours
        {
            get; set;
        }

        public int NoOfInstances
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
