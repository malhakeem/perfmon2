﻿using System;
using System.Collections.Generic;

namespace perfmon2
{
    class WindowsAmazonCalculator : WindowsCalculator
    {
        // Postgres, SQL Server
        // Amazon RDS On-Demand DB Instance

        private static double Ratio = 0.81;
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
            // db.m4.large
            // $1.011 / h
            // $0.137 / GB
            // Min Storage is 20
            if (Storage < 20)
                Storage = 20;

            double priceStorage = 2.74 + (Storage - 20) * 0.137;
            double priceHours = 1.011 * (NoOfHours % 2 + 30) * NoOfHours;
            // not always exactly the same 
            Console.WriteLine("Storage " + priceStorage * NoOfInstances);
            Console.WriteLine("Hours " + priceHours * NoOfInstances);

            return (priceHours + priceStorage) * (NoOfInstances);
        }
    }
}
