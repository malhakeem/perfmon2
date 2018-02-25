using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perfmon2
{
    class WindowsIBMCalculator : WindowsCalculator
    {
        private static double[] pricetable = { 9.03, 8.12, 7.22, 6.32, 5.42, 4.51, 3.61, 2.70 };
        private static int[] tierLimits = { 9, 24, 49, 99, 499, 999, 4999, int.MaxValue };

        public WindowsIBMCalculator(double ram = 0, double storage = 0, int noOfInstances = 1)
        {
            RAM = ram;
            Storage = storage;
            NoOfInstances = noOfInstances;
        }

        public double RAM
        {
            get; set;
        }

        public double Storage
        {
            get; set;
        }

        public int NoOfInstances
        {
            get; set;
        }
        
        public int NoOfUnits
        {
            get
            {
                return (int)Math.Ceiling(Math.Max(Storage, RAM / 102));
            }
        }

        public List<double> CalculateAllPrices(bool descending = true)
        {          
            return new List<double>();
        }

        public double CalculateBestPrice()
        {
            int tier = 0;
            while (NoOfUnits > tierLimits[tier])
                tier++;

            return NoOfUnits * pricetable[tier];
        }
    }
}
