using System;
using System.Collections;

namespace perfmon2
{
    /*    
        The calculator for IBM Bluemix DBaaS.
            - IBM DB2, Flex plan, (1 Core)  
            - PostgreSQL, Standard plan (1 GB ~ 1 Unit)
        Contains methods for calculating all available tiers for given storage requirements (PostgreSQL)
        and for calculating the price based on the formula (DB2).     
    */
    class WindowsIBMCalculator : WindowsCalculator
    {   
        // Arrays that store possible prices per GB and size limits for PostgreSQL
        // The price is only based on storage provisioned.
        private static double[] pricetable = { 9.03, 8.12, 7.22, 6.32, 5.42, 4.51, 3.61, 2.70 };
        private static int[] tierLimits = { 9, 24, 49, 99, 499, 999, 4999, int.MaxValue };

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
            }
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
            // IBM DB2 Pricing model:            
            //      €142.00 EUR/Instance
            //      €142.00 EUR/Instance (High Availability)
            //      €9.78 EUR/GB of RAM
            //      €0.75 EUR/GB of Storage
            //      €0.15 EUR/1 million IO operations
            // When using High Availability, price for RAM and Storage doubles
            int m = 1;
            if (HighAvailability)
                m = 2;
            return NoOfInstances * 142 + 
                m * (RAM * 9.78 + Storage * 0.75) + MillionsOfIO * 0.15;  
        }
        private double CalculcateBestPricePostgreSQL()
        {
            // IBM PostgreSQL
            //      Storage: 1GB = 1Unit
            //       102MB RAM 
            int tier = 0;
            while (NoOfUnits > tierLimits[tier])
                tier++;

            return NoOfUnits * pricetable[tier];
        }
    }
}
