using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EconomicOnParcs.Classes
{
    public class FileReader
    {
        private List<IResource> resources = new List<IResource>();
        private List<IResource> industries = new List<IResource>();

        private string folderPath = "";

        public FileReader(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public void readResources()
        {
            string filePath = folderPath + "\\Resources.txt";
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(' ');
                int ID = int.Parse(parts[0]);
                string name = parts[1];
                Color color = ColorTranslator.FromHtml(parts[2]);
                BasicResource resource = new BasicResource(ID, name, color);
                resources.Add(resource);
            }
        }

        public void readIndustries()
        {
            string filePath = folderPath + "\\Industries.txt";
            int counter = 0;
            foreach (string line in File.ReadAllLines(filePath))
            {
                int grade = 0;
                Industry industry = new Industry(counter, "Empty", grade);
                IResource resource = new BasicResource(-1, "Name", Color.Black);
                char letter1 = 'a';
                string[] parts = line.Split(' ');
                bool resAddReady = true;
                for (int i = 0; i < parts.Length; i++)
                {
                    string text = parts[i];
                    if (text == "")
                        break;
                    if (i == 0)
                    {
                        grade = int.Parse(text);
                        industry.setGrade(grade);
                    }
                    else if (i == 1)
                        industry.setName(text);
                    else
                    {
                        if (i % 2 == 0)  //even indexes contain resource names
                        {
                            string resName = parts[i];
                            letter1 = resName.ElementAt(0);
                            if (!char.IsLetter(resName, 0))
                                resName = resName.Remove(0, 1);
                            if (resources.Contains(new BasicResource(0, resName, Color.Aqua)))
                            {
                                resource = resources.Where(r => (r as BasicResource).getName().Equals(resName)).First();
                            }
                            else if (industries.Contains(new Industry(0, resName, 1)))
                            {
                                resource = industries.Where(r => (r as Industry).getName().Equals(resName)).First();
                            }
                            else
                            {
                                resAddReady = false;
                            }
                        }
                        else  //odd indexes contain resource ammounts
                        {
                            string ammount = parts[i];
                            string[] words = ammount.Split(',');
                            int item1 = int.Parse(words[0]);
                            int item2 = int.Parse(words[0]);
                            if (words.Length > 1)
                                item2 = int.Parse(words[1]);
                            Tuple<int, int> ammounts = new Tuple<int, int>(item1, item2);
                            if (letter1 == '!' && resAddReady)
                            {
                                letter1 = 'a';
                                industry.addRequired(resource, ammounts);
                                resource = new BasicResource(-1, "Name", Color.Black);
                            }
                            else if (resAddReady)
                            {
                                industry.addNeeded(resource, ammounts);
                                resource = new BasicResource(-1, "Name", Color.Black);
                            }
                            else
                                resAddReady = true;
                        }
                    }
                }
                industry.fillObtained();
                industry.fillObtainedRequired();
                industries.Add(industry);
                counter++;
            }
        }

        public List<IResource> getResources()
        {
            return resources;
        }

        public List<IResource> getIndustries()
        {
            return industries;
        }

        public void run()
        {
            readResources();
            readIndustries();
        }
    }
}
