using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace perfmon2
{
    class WindowsAzureCalculator
    {
        //array that stores a table for the dbaas pricing
        //each row includes {DTU, Storage, Price/hour}
        public static double[,] azureTable = new double[,] {
                {50,5,0.0851},{100,10,0.1701},{200,20,0.3402},{300,29,0.5102},{400,39,0.6803},
                {800, 78, 1.3606},{1200, 117, 2.0408},{1600,156,2.7211},{50,50, 0.1274},
                {100,100,0.2548}, {200,200,0.5095}, {300,300, 0.7643}, {400,400,1.019},
                {800,800,2.038}, {1200, 1170, 3.057}, {1600, 1560, 4.076}, {2000, 1950, 5.095},
                {2500,2440, 6.3687}, {3000,2930, 7.6425}, {125, 250, 0.7906}, {250, 500, 1.5812},
                {500,750, 3.1624}, {1000,1000, 6.3248}, {1500, 1500, 9.4872}, {2000,2000,12.65},
                {2500,2500, 15.82}, {3000,3000, 18.98}, {3500, 3500, 22.14}, {4000, 4000, 25.30}
        };

        public static ArrayList priceList;
        public static double AzureCalculator(double maxCPU, double readSum, double writeSum, double noOfCores, double storage, double noOfHours)
        {
            priceList = new ArrayList();
            double cpuDTU = 1.25 * noOfCores * maxCPU;
            double ioDTU = (readSum + writeSum) / 12.84;
            double maxDTU;
            if (cpuDTU > ioDTU)
                maxDTU = cpuDTU;
            else
                maxDTU = ioDTU;

            Console.WriteLine("io DTU = " + ioDTU);
            Console.WriteLine("cpu DTU = " + cpuDTU);
            Console.WriteLine("Max DTU = " + maxDTU);


            for (int i =0; i< azureTable.GetLength(0); i++)
            {
                if (Convert.ToDouble(azureTable[i,0]) > maxDTU)
                {
                    if (Convert.ToDouble(azureTable[i, 1]) > storage)
                    {
                        priceList.Add(azureTable[i, 2]);
                    }
                }
            }

            Console.WriteLine("*********************");
            Console.WriteLine("possible prices per hour");
            for (int j = 0; j < (priceList.Count - 1); j++)
            {
                Console.WriteLine(priceList[j]);
            }
            Console.WriteLine("*********************");


            double min = findMin(priceList);

            Console.WriteLine("*****************************");
            Console.WriteLine("optimum price = "+ min);

            return min*noOfHours;

            

        }

        private static double findMin(ArrayList priceList)
        {
            double min = double.MaxValue;
            for (int j = 0; j < (priceList.Count - 1); j++)
            {
                if (Convert.ToDouble(priceList[j]) < min)
                    min = Convert.ToDouble(priceList[j]);
            }
            return min;
        }
    }
}
