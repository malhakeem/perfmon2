using System;
using System.Collections.Generic;

namespace perfmon2
{
    class WindowsAmazonCalculator : WindowsCalculator
    {
        // Postgres, SQL Server
        // Amazon RDS On-Demand DB Instance

        private static double Ratio = 0.81;

        //array that stores a table for the dbaas pricing
        //each row includes {vCPU, RAM (= 4 * vCPU), Price/hour}
        // only db.m4....
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

        public int NoOfCores
        {
            get; set;
        }
        public List<double> CalculateAllPrices(bool descending = true)
        {
            throw new NotImplementedException();
        }

        public double CalculateBestPrice(DBType db)
        {
            if (db == DBType.PostgreSQL)
                return CalculcateBestPricePostgreSQL();
            if (db == DBType.SQLServer)
                return CalculcateBestPriceSQLServer();

            return -1;
            //throw new ArgumentException();
        }

        private double CalculcateBestPricePostgreSQL()
        {
            // db.m4.large -- close to IBM option
            // $0.217 / h
            // $0.137 / GB
            // Min Storage is 5
            if (Storage < 5)
                Storage = 5;

            double priceStorage = 5.65 + (Storage - 5) * 0.137;
            double priceHours = 0.217 * (NoOfHours % 2 + 30) * NoOfHours;
            // not always exactly the same 
            Console.WriteLine("Storage " + priceStorage * NoOfInstances);
            Console.WriteLine("Hours " + priceHours * NoOfInstances);

            return (priceHours + priceStorage) * (NoOfInstances) * Ratio;
        }

        private double CalculcateBestPriceSQLServer()
        {
            // db.m4.xxx
            // Min Storage is 20
            if (Storage < 20)
                Storage = 20;

            double ph = double.MaxValue;
            int i = 0;
            while (Convert.ToDouble(awsTable[i, 0]) < NoOfCores * 2 || Convert.ToDouble(awsTable[i, 1]) < RAM)
            {
                i++;
                if (i >= awsTable.GetLength(0))
                    return ph;
            }
            ph = Convert.ToDouble(awsTable[i, 2]);

            double priceStorage = 2.74 + (Storage - 20) * 0.137;
            double priceHours = ph * 30.5 * NoOfHours;
            // not always exactly the same 
            Console.WriteLine("Storage " + priceStorage * NoOfInstances);
            Console.WriteLine("Hours " + priceHours * NoOfInstances);

            return (priceHours + priceStorage) * (NoOfInstances);
        }
    }
}
