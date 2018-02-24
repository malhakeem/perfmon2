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
        static void Main(string[] args)
        {
            ArrayList sampleList = new ArrayList();
            ArrayList timeList = new ArrayList();
            ArrayList readList = new ArrayList();
            ArrayList writeList = new ArrayList();
            CreateCounters();

            CollectSamples(sampleList, timeList, readList, writeList);
            CalculateResults(sampleList, timeList, readList, writeList);
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
            //Random r = new Random(DateTime.Now.Millisecond);

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

        }

        private static void CalculateResults(ArrayList samplesList, ArrayList timeList, ArrayList readList, ArrayList writeList)
        {
            for (int i = 0; i < (samplesList.Count - 1); i++)
            {
                Console.WriteLine(" time: "+ timeList[i]+" CPU: "+ samplesList[i] + " read: "+ readList[i]+" write: "+ writeList[i]);
                
            }
            double readSum = 0;
            double writeSum = 0;
            /*
            Double[] readArray = (Double[])readList.ToArray(typeof(Double));
            Double[] writeArray = (Double[])writeList.ToArray(typeof(Double));
            for (int j=0;j< (samplesList.Count - 1); j++)
            {
                readSum = readSum + readArray[j];
                writeSum = writeSum + writeArray[j];
            }
            */

            Console.WriteLine("total reads= " + readSum);
            Console.WriteLine("total writes= " + writeSum);

            Console.ReadKey();
        }

    }
}
