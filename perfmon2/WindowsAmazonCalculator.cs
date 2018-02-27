using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return 100;
        }
    }
}
