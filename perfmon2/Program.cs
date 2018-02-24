using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

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
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }

        private static void CollectSamples(ArrayList samplesList, ArrayList timeList, ArrayList readList, ArrayList writeList)
        {
            //Random r = new Random(DateTime.Now.Millisecond);
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

            //for (int j = 0; j <= 10; j++)
            //{

            //    //int value = r.Next(1, 10);
            //    //Console.Write(j + " = " + value);
            //    //cpu.IncrementBy(value);
            //    samplesList.Add(cpu.NextValue());
            //    timeList.Add(GetTimestamp(DateTime.Now));
            //    readList.Add(read.NextValue());
            //    writeList.Add(write.NextValue());
            //    System.Threading.Thread.Sleep(1000);
            //}

        }

        private static void CalculateResults(ArrayList samplesList, ArrayList timeList, ArrayList readList, ArrayList writeList)
        {
            var result = new StringBuilder();
            result.AppendLine("Time,CPU%,Reads/sec,Writes/sec");

            for (int i = 0; i < (samplesList.Count - 1); i++)
            {
                Console.WriteLine(" time: "+ timeList[i]+" CPU: "+ samplesList[i] + " read: "+ readList[i]+" write: "+ writeList[i]);
                var newLine = string.Format("{0},{1},{2},{3}", timeList[i], samplesList[i], readList[i], writeList[i]);
                result.AppendLine(newLine);
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

            result.AppendLine(string.Format("Total reads={0}, total writes={1}", readSum, writeSum));

            File.WriteAllText("output.csv", result.ToString());

            Console.ReadKey();
        }

    }
}
