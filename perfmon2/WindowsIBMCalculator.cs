using System;
using System.Collections.Generic;
/*
€142.00 EUR/Instance
€142.00 EUR/Instance (High Availability)
€9.78 EUR/GB of RAM
€0.75 EUR/GB of Storage
€0.15 EUR/1 million IO operations
 */

namespace perfmon2
{
    class WindowsIBMCalculator : WindowsCalculator
    {
        // PostgreSQL, DB2?
        private static double[] pricetable = { 9.03, 8.12, 7.22, 6.32, 5.42, 4.51, 3.61, 2.70 };
        private static int[] tierLimits = { 9, 24, 49, 99, 499, 999, 4999, int.MaxValue };
        

        public WindowsIBMCalculator()
        {  
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

        public double MillionsOfIO
        {
            get; set;
        }
        public bool HighAvailability
        {
            get; set;
        }
        
        public int NoOfUnits
        {
            get
            {
                return (int)Math.Ceiling(Storage);
                //return (int)Math.Ceiling(Math.Max(Storage, RAM / 102));
            }
        }

        public List<double> CalculateAllPrices(bool descending = true)
        {          
            return new List<double>();
        }

        public double CalculateBestPrice(DBType db)
        {
            if (db == DBType.PostgreSQL)
                return CalculcateBestPricePostgreSQL();
            if (db == DBType.DB2)
                return CalculcateBestPriceDB2();

            return -1;
            //throw new ArgumentException();
        }

        private double CalculcateBestPriceDB2()
        {
            int m = 1;
            if (HighAvailability)
                m = 2;
            return NoOfInstances * 142 + 
                m * (RAM * 9.78 + Storage * 0.75) + MillionsOfIO * 0.15;  
        }
        private double CalculcateBestPricePostgreSQL()
        {
            int tier = 0;
            while (NoOfUnits > tierLimits[tier])
                tier++;

            return NoOfUnits * pricetable[tier];
        }
    }
}
