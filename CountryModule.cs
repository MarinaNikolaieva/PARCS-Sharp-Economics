using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using EconomicOnParcs.Classes;
using Parcs;

namespace EconomicOnParcs
{
    public class CountryModule : IModule
    {
        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            //Each Daemon must get:
            //The address of the data files;
            //The list of the countries' coordinates it must process;
            //The map it must read
            string folderAddress = info.Parent.ReadString();
            Console.WriteLine("Work address: " + folderAddress);
            FileReader textReader = new FileReader(folderAddress);
            textReader.run();
            ImageReader imageReader = new ImageReader(folderAddress);
            imageReader.readMaps();
            List<int> xs = (List<int>)info.Parent.ReadObject(typeof(List<int>));
            List<int> ys = (List<int>)info.Parent.ReadObject(typeof(List<int>));
            //List<Vector2> coordinates = (List<Vector2>)info.Parent.ReadObject(typeof(List<Vector2>));
            List<List<MapPart>> map = imageReader.getMap();
            Console.WriteLine("Data read");

            List<Country> countries = new List<Country>();
            List<Thread> threads = new List<Thread>();
            int index = 0;
            foreach(var coordinate in xs)
            {
                Vector2 coord = new Vector2(xs.ElementAt(index), ys.ElementAt(index));
                index++;
                Country country = new Country(textReader, coord);
                countries.Add(country);
                threads.Add(new Thread(() => country.runProcess(map)));
            }
            Console.WriteLine("Threads created");
            for (int i = 0; i < threads.Count; i++)
            {
                threads.ElementAt(i).Start();
                Console.WriteLine("Thread " + i + " started");
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads.ElementAt(i).Join();
                Console.WriteLine("Thread " + i + " finished");
            }
            for (int i = 0; i < countries.Count; i++)
            {
                Console.WriteLine("Country " + ColorTranslator.ToHtml(Color.FromArgb(countries.ElementAt(i).getColor().ToArgb())));
                countries.ElementAt(i).printPotential();
                Console.WriteLine();
            }
            info.Parent.WriteData(1);
        }
    }
}
