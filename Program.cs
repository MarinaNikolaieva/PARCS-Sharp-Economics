using EconomicOnParcs.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Reflection;
using System.IO;
using Parcs;

namespace EconomicOnParcs
{
    internal class Program : IModule
    {
        public static void Main(string[] args)
        {
            var job = new Job();
            if (!File.Exists(Assembly.GetExecutingAssembly().Location))
            {
                Console.WriteLine("File doesn't exist");
                return;
            }
            job.AddFile(Assembly.GetExecutingAssembly().Location);
            new Program().Run(new ModuleInfo(job, null));
            Console.ReadKey();
        }

        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            string strExeFilePath = Assembly.GetExecutingAssembly().Location;
            string strWorkPath = Path.GetDirectoryName(strExeFilePath);
            string readingPath = strWorkPath + "\\DataFiles";
            Console.WriteLine("Read path: " + readingPath);

            ImageReader imageReader = new ImageReader(readingPath);
            imageReader.readMaps();
            Dictionary<Color, Vector2> coordinates = imageReader.getCoordinates();

            //We'll use 2 virtual machines
            const int pointsNum = 2;
            var points = new IPoint[pointsNum];
            var channels = new IChannel[pointsNum];
            for (int i = 0; i < pointsNum; ++i)
            {
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("EconomicOnParcs.CountryModule");
            }
            List<List<int>> xsForMachines = new List<List<int>>();
            List<List<int>> ysForMachines = new List<List<int>>();
            //List<List<Vector2>> coordsForMachines = new List<List<Vector2>>();
            int numPerMachine = coordinates.Count / pointsNum;
            int remainedNumber = coordinates.Count - (numPerMachine * pointsNum);
            int index = 0;
            for (int i = 0; i < pointsNum; i++)
            {
                xsForMachines.Add(new List<int>());
                ysForMachines.Add(new List<int>());
                for (int j = 0; j < numPerMachine; j++)
                {
                    xsForMachines.ElementAt(i).Add((int)coordinates.Values.ElementAt(index).X);
                    ysForMachines.ElementAt(i).Add((int)coordinates.Values.ElementAt(index).Y);
                    index++;
                }
            }
            for (int i = 0; i < remainedNumber; i++)
            {
                xsForMachines.ElementAt(i).Add((int)coordinates.Values.ElementAt(index).X);
                ysForMachines.ElementAt(i).Add((int)coordinates.Values.ElementAt(index).Y);
                index++;
            }

            for (int i = 0; i < pointsNum; ++i)
            {
                channels[i].WriteData(readingPath);
                channels[i].WriteObject(xsForMachines.ElementAt(i));
                channels[i].WriteObject(ysForMachines.ElementAt(i));
                //channels[i].WriteObject(imageReader.getMap());
                Console.WriteLine("Data sent to channel " + i);
            }

            DateTime time = DateTime.Now;
            Console.WriteLine("Waiting for result...");

            int res = 0;
            for (int i = 0; i < pointsNum; i++)
            {
                res += channels[i].ReadInt();
            }

            if (res == pointsNum)
                Console.WriteLine("Success! Finished in time = {0}", Math.Round((DateTime.Now - time).TotalSeconds, 3));
            else
                Console.WriteLine("Error! Finished in time = {0}", Math.Round((DateTime.Now - time).TotalSeconds, 3));
        }
    }
}
