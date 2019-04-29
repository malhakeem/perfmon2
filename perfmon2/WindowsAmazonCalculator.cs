using System;

namespace perfmon2
{
    /*    
        The calculator for Amazon Web Services DBaaS.
            - MS SQL Server, 'db.m4.xxxx' instances, Standard, Single-AZ Deployment 
                - General purpose SSD 
                - Provisioned IOPS
            - PostgreSQL, 'db.m4.large' instance, Standard plan, Single-AZ Deployment
        Contains methods for calculating all possible prices for a given configuration
    and for determining the lowest possible price.     
    */
    class WindowsAmazonCalculator : WindowsCalculator
    {
        // Postgres, SQL Server
        // Amazon RDS On-Demand DB Instance
        
        // An array that stores a table for the AWS SQL Server DBaaS pricing
        // Each row includes {vCPU, RAM (= 4 * vCPU), Price/hour}
        // for instance types 'db.m4.large' -- 'db.m4.16xlarge'
        private static double[,] awsTable = new double[,] {
                { 2, 8, 1.011}, { 4, 16, 1.291}, { 8, 32, 2.682}, { 16, 64, 5.316}, { 64, 256, 20.656 }
        };
        public int NoOfInstances
        {
            get; set;
        }
        public double Storage
        {
            get; set;
        }
        public double NoOfHours
        {
            get; set;
        }
        public double RAM
        {
            get; set;
        }
        public int vCPUs
        {
            get; set;
        }
        public double IOPS
        {
            get; set;
        }

        public double CalculateBestPrice(DBType db)
        {
            if (db == DBType.PostgreSQL)
                return CalculcateBestPricePostgreSQL();
            if (db == DBType.SQLServer)
                return CalculcateBestPriceSQLServer();

            return -1;
        }

        private double CalculcateBestPricePostgreSQL()
        {
            // 'db.m4.large' (simlar to the IBM PostgreSQL option)
            // 0.217 / h
            // 0.137 / GB
            // Min Storage is 5 GB
            if (Storage < 5)
                Storage = 5;

            double priceStorage = Storage * 0.137;
            
            double priceHours = 0.217 * 30.5 * NoOfHours;

            return (priceHours + priceStorage) * (NoOfInstances);
        }

        private double CalculcateBestPriceSQLServer()
        {
            // 'db.m4.xxxx' instances

            double ph = double.MaxValue;

            // For each DB Instance class, 
            // 20 GB to 16 TB of 
            // General Purpose (SSD): 
            //      charged for storage you provision
            //      notcharged for the I/Os you consume 
            // Baseline is 3 IOPS per GB (i.e. more volume equals better performance)      
            if (Storage < 20)
                Storage = 20;
            if (Storage > 1024 * 16)
                return ph;
            bool canUseGeneral = Storage * 3 >= IOPS;
            double priceStorage = Storage * 0.273; // General purpose SSD 

            // Provisioned IOPS:
            //      For SQL Server Standard Licence, 200 GB - 16TB of storage
            //      1000 - 200000 IOPS 
            //      Charged for the throughput and storage
            //      Not charged for the I/Os you consume
            //      Ratio of IOPS:Storage is between 3:1 and 10:1
            if (Storage < 200)
                Storage = 200;
            if (IOPS < 1000)
                IOPS = 1000;

            if (Storage * 3 > IOPS)
                IOPS = Storage * 3;
            else if (Storage * 10 < IOPS)
                Storage = Math.Ceiling(IOPS / 10);

            if (IOPS > 200000)
                return double.MaxValue;

            double priceIO = Storage * 0.149 + IOPS * 0.119; // Provisioned IO

            // Calculate the price for the instance - does not depend on storage type.
            int i = 0;
            // Find the first instance that satisfies the requirements in terms of RAM/vCPUs
            while (Convert.ToDouble(awsTable[i, 0]) < vCPUs || Convert.ToDouble(awsTable[i, 1]) < RAM)
            {
                i++;
                // If the end of the table is reached then the implemented services do not support higher requirements.
                // Return 'infinity'
                if (i >= awsTable.GetLength(0))
                    return ph;
            }
            ph = Convert.ToDouble(awsTable[i, 2]);
            // The formula used by AWS to calcuate monthly price: hourly * 30.5 days * Number of hours per day
            double priceHours = ph * 30.5 * NoOfHours;
            
            double priceGeneralSSD = (priceHours + priceStorage) * (NoOfInstances);
            double priceProvisionedIO = (priceHours + priceIO) * (NoOfInstances);

            // Return the minimal price for given requirements.
            if (canUseGeneral && priceGeneralSSD <= priceProvisionedIO)
                return priceGeneralSSD;
            return priceProvisionedIO;
        }
    }
}
