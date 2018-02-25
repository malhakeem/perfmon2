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
    class Program
    {
        private static PerformanceCounter cpu;
        private static PerformanceCounter read;
        private static PerformanceCounter write;

        public static double maxCPU;
        public static double maxRead;
        public static double maxWrite;
        public static double noOfCores = 4;
        public static double storage = 100;
        public static double noOfHours = 528;


        static void Main(string[] args)
        {
            ArrayList sampleList = new ArrayList();
            ArrayList timeList = new ArrayList();
            ArrayList readList = new ArrayList();
            ArrayList writeList = new ArrayList();

            
            CreateCounters();

            CollectSamples(sampleList, timeList, readList, writeList);
            CalculateResults(sampleList, timeList, readList, writeList);

            Console.WriteLine("max reads= " + maxRead);
            Console.WriteLine("max writes= " + maxWrite);
            Console.WriteLine("max CPU % = " + maxCPU);

            double minMonthlyPrice = WindowsAzureCalculator.AzureCalculator(maxCPU, maxRead, maxWrite, noOfCores, storage, noOfHours);

            Console.WriteLine("minimum price for running this workload monthly is = " + minMonthlyPrice);

            Console.ReadKey();

        }

        private static void CreateCounters()
        {
            
            cpu = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);
            read = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", "_Total", true);
            write = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", "_Total");
            //cpu.CategoryName = "Processor";
            //cpu.CounterName = "% Processor Time";
            //cpu.InstanceName = "_Total";
            //cpu.ReadOnly = false;
            //cpu.RawValue = 0;

        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }

        private static void CollectSamples(ArrayList samplesList, ArrayList timeList, ArrayList readList, ArrayList writeList)
        {
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    samplesList.Add(cpu.NextValue());
                    timeList.Add(GetTimestamp(DateTime.Now));
                    readList.Add(read.NextValue());
                    writeList.Add(write.NextValue());
                    System.Threading.Thread.Sleep(1000);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
/*
            for (int j = 0; j <= 10; j++)
            {

                //int value = r.Next(1, 10);
                //Console.Write(j + " = " + value);
                //cpu.IncrementBy(value);
                samplesList.Add(cpu.NextValue());
                timeList.Add(GetTimestamp(DateTime.Now));
                readList.Add(read.NextValue());
                writeList.Add(write.NextValue());
                System.Threading.Thread.Sleep(1000);
            }
            */

           

        }

        private static void CalculateResults(ArrayList samplesList, ArrayList timeList, ArrayList readList, ArrayList writeList)
        {
            maxRead = 0;
            maxWrite = 0;
            maxCPU = 0;
            for (int i = 0; i < (samplesList.Count - 1); i++)
            {
                Console.WriteLine(" time: " + timeList[i] + " CPU: " + samplesList[i] + " read: " + readList[i] + " write: " + writeList[i]);
                //readSum = readSum + Convert.ToDouble(readList[i]);
                //writeSum = writeSum + Convert.ToDouble(writeList[i]);

                if (Convert.ToDouble(readList[i]) > maxRead)
                {
                    maxRead = Convert.ToDouble(readList[i]);
                }

                if (Convert.ToDouble(writeList[i]) > maxWrite)
                {
                    maxWrite = Convert.ToDouble(writeList[i]);
                }

                if (Convert.ToDouble(samplesList[i])> maxCPU)
                {
                    maxCPU = Convert.ToDouble(samplesList[i]);
                }
            }

            /*
            Double[] readArray = (Double[])readList.ToArray(typeof(Double));
            Double[] writeArray = (Double[])writeList.ToArray(typeof(Double));
            for (int j=0;j< (samplesList.Count - 1); j++)
            {
                readSum = readSum + readArray[j];
                writeSum = writeSum + writeArray[j];
            }
            

            Console.WriteLine("total reads= " + readSum);
            Console.WriteLine("total writes= " + writeSum);
            Console.WriteLine("max CPU % = " + maxCPU);

            */
        }

    }
}
