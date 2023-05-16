using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace EconomicOnParcs.Classes
{
    public class Country
    {
        private Dictionary<IResource, int> resourceAmmounts { get; set; }
        private List<Industry> industriesPoints { get; set; }
        private Random rand = new Random();
        private Vector2 beginCoordinates;
        private Color countryColor;


        public Country(FileReader textReader, Vector2 coords)
        {
            resourceAmmounts = new Dictionary<IResource, int>();
            foreach (IResource resource in textReader.getResources())
            {
                resourceAmmounts.Add(resource as BasicResource, 0);
            }
            industriesPoints = new List<Industry>();
            foreach (IResource industry in textReader.getIndustries())
            {
                industriesPoints.Add(industry as Industry);
            }
            beginCoordinates = coords;
        }

        public void AddResourceAmmount(IResource resource, int ammount)
        {
            resourceAmmounts[resource] = ammount;
        }

        public List<BasicResource> CopyResources(List<IResource> iresources)
        {
            List<BasicResource> resources = new List<BasicResource>();
            for (int i = 0; i < iresources.Count(); i++)
            {
                resources.Add(iresources.ElementAt(i) as BasicResource);
            }
            return resources;
        }

        //This is the reading-the-map-and-counting-resources Method
        public void CountResources(List<List<MapPart>> mapToRead)
        {
            List<BasicResource> resources = CopyResources(resourceAmmounts.Keys.ToList());
            List<MapPart> checkedParts = new List<MapPart>();
            List<MapPart> candidates = new List<MapPart>();
            MapPart beginElement = mapToRead.ElementAt((int)beginCoordinates.Y).ElementAt((int)beginCoordinates.X);
            candidates.Add(beginElement);
            countryColor = beginElement.countryColor;

            while (candidates.Count > 0)
            {
                MapPart currentElement = candidates.ElementAt(0);
                if (currentElement.countryColor != countryColor || checkedParts.Contains(currentElement))
                {
                    candidates.RemoveAt(0);
                }
                else
                {
                    BasicResource res = resources.Where(r => r.getColor() == currentElement.onGroundResColor).FirstOrDefault();
                    if (res != null)
                    {
                        resourceAmmounts[res] += 1;  //MAYBE add 2 or more, we'll see when the maps will be ready
                    }
                    res = resources.Where(r => r.getColor() == currentElement.underGroundResColor).FirstOrDefault();
                    if (res != null)
                    {
                        resourceAmmounts[res] += 1;  //Same here
                    }
                    res = resources.Where(r => r.getColor() == currentElement.globalResColor).FirstOrDefault();
                    if (res != null)
                    {
                        resourceAmmounts[res] += 1;  //Same here
                    }

                    //Now detect the neighbors and put them into Candidates array
                    int x = (int)currentElement.coordinates.X;
                    int y = (int)currentElement.coordinates.Y;
                    if (y > 0)
                        candidates.Add(mapToRead.ElementAt(y - 1).ElementAt(x));
                    if (y < mapToRead.Count() - 1)
                        candidates.Add(mapToRead.ElementAt(y + 1).ElementAt(x));
                    if (x > 0)
                        candidates.Add(mapToRead.ElementAt(y).ElementAt(x - 1));
                    if (y < mapToRead.ElementAt(0).Count() - 1)
                        candidates.Add(mapToRead.ElementAt(y).ElementAt(x + 1));
                    //Confirm the checking and remove the element
                    checkedParts.Add(currentElement);
                    candidates.RemoveAt(0);
                }
            }
        }

        //This is the clear-all-the-zero-numbered-resources Method
        public void ClearLists()
        {
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                Industry industry = industriesPoints[i];
                List<IResource> resources = new List<IResource>();
                resources.AddRange(resourceAmmounts.Keys.Where(r => industry.getNeeded().Contains(r)));
                List<IResource> requiredResources = new List<IResource>();
                requiredResources.AddRange(resourceAmmounts.Keys.Where(r => industry.getRequired().Contains(r)));
                bool cont = true;
                //If one Required is missing, the industry is nonAccessable
                for (int j = 0; j < requiredResources.Count; j++)
                {
                    if (resourceAmmounts[requiredResources.ElementAt(j)] == 0)
                    {
                        industriesPoints.Remove(industriesPoints.ElementAt(i));
                        i--;
                        cont = false;
                        break;
                    }
                }
                if (cont)
                {
                    //If one Needed is missing, the industry MAY BE nonAccessable
                    int zeroNeededRes = 0;
                    for (int j = 0; j < resources.Count; j++)
                    {
                        if (resourceAmmounts[resources.ElementAt(j)] == 0)
                        {
                            zeroNeededRes++;
                        }
                    }
                    if ((industry.getGrade() > 1 && zeroNeededRes > 0) || zeroNeededRes == industry.getNeeded().Count)
                    {
                        industriesPoints.Remove(industriesPoints.ElementAt(i));
                        i--;
                    }
                }
            }
            for (int i = 0; i < resourceAmmounts.Count; i++)
            {
                if (resourceAmmounts.ElementAt(i).Value == 0)
                {
                    resourceAmmounts.Remove(resourceAmmounts.ElementAt(i).Key);
                    i--;
                }
            }
        }

        public void aquirePointsInterface()
        {
            foreach (IResource industry in industriesPoints)
            {
                (industry as Industry).aquirePoints();
                //MAYBE return the resources to the "bank" and re-distribute them?
            }
        }

        public void aquirePointsIndustries()
        {
            foreach (Industry industry in industriesPoints)
            {
                industry.aquirePoints();
            }
        }

        public void basicResourcesDistribute()
        {
            for (int i = 0; i < resourceAmmounts.Count; i++)  //distribute basic resources between industries
            {
                List<IResource> industries = new List<IResource>();
                industries.AddRange(industriesPoints.Where(r => r.getNeeded().
                    Contains(resourceAmmounts.ElementAt(i).Key)));
                industries.AddRange(industriesPoints.Where(r => r.getRequired().
                    Contains(resourceAmmounts.ElementAt(i).Key)));
                industries = industries.Distinct().ToList();  //clear the duplicates

                int ammountToCut = 0;
                for (int j = 0; j < resourceAmmounts.ElementAt(i).Value; j++)
                {
                    if (industries.Count > 0)
                    {
                        int chosenIndustry = rand.Next(0, industries.Count);
                        Industry ind = industries.ElementAt(chosenIndustry) as Industry;
                        if (ind.getNeeded().Contains(resourceAmmounts.ElementAt(i).Key))
                            ind.addObtained(resourceAmmounts.ElementAt(i).Key, 1);
                        else if (ind.getRequired().Contains(resourceAmmounts.ElementAt(i).Key))
                            ind.addObtainedRequired(resourceAmmounts.ElementAt(i).Key, 1);
                        ammountToCut++;
                    }
                }
                resourceAmmounts[resourceAmmounts.ElementAt(i).Key] -= ammountToCut;
            }
        }

        public void industriesDistribute()
        {
            //If the resource stays within the facility, the Random gives -1
            //Else - it gives the facility's index in Resources list
            for (int i = 0; i < industriesPoints.Count; i++)  //distribute industries' resources between other industries
            {
                List<IResource> industries = new List<IResource>();
                industries.AddRange(industriesPoints.Where(r => (r as Industry).getNeeded().
                    Contains(industriesPoints.ElementAt(i))));
                industries.AddRange(industriesPoints.Where(r => (r as Industry).getRequired().
                    Contains(industriesPoints.ElementAt(i))));
                industries = industries.Distinct().ToList();

                if (industries.Count > 0)
                {
                    for (int j = 0; j < (industriesPoints.ElementAt(i) as Industry).getPoints(); j++)
                    {
                        int chosenIndustry = rand.Next(-1, industries.Count);
                        if (chosenIndustry >= 0)
                            (industries.ElementAt(chosenIndustry) as Industry).addObtained(industriesPoints.ElementAt(i), 1);
                    }
                }
            }
        }

        //Now - the calculating method
        public void CalculatePotential()
        {
            basicResourcesDistribute();
            //And now each industry gets its total points depending on obtained resources
            aquirePointsInterface();
            industriesDistribute();
            //And now each industry gets its total points depending on obtained resources
            aquirePointsIndustries();
            //And I need ONE MORE run for the third-grade industries to get their points
            industriesDistribute();
            aquirePointsIndustries();
        }

        //I also need the Output method
        public List<Industry> getPotential()
        {
            return industriesPoints;
        }

        public Color getColor()
        {
            return countryColor;
        }

        public Dictionary<IResource, int> getResourceAmounts()
        {
            return resourceAmmounts;
        }

        public void printResources()
        {
            Console.WriteLine("Country");
            for (int i = 0; i < resourceAmmounts.Count; i++)
            {
                BasicResource res = resourceAmmounts.ElementAt(i).Key as BasicResource;
                Console.Write(res.getID() + " ");
                Console.Write(res.getName() + " ");
                Console.Write(resourceAmmounts.ElementAt(i).Value + "\n");
            }
        }

        public void printPotential()
        {
            Console.WriteLine("Country");
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                Console.Write(industriesPoints.ElementAt(i).getGrade() + " ");
                Console.Write(industriesPoints.ElementAt(i).getName() + " ");
                Console.Write(industriesPoints.ElementAt(i).getPoints() + "\n");
            }
        }

        public void makePotentialOneIter(List<Industry> defaultIndustriesPoints, ref int bestIndustriesPotential,
            List<Industry> bestIndustriesPoints)
        {
            CalculatePotential();
            int potential = 0;
            for (int j = 0; j < industriesPoints.Count(); j++)
            {
                //The grade 3 industries are more important, so they must "weigh" more to keep the balance
                if ((industriesPoints.ElementAt(j) as Industry).getGrade() == 3)
                    potential += (industriesPoints.ElementAt(j) as Industry).getPoints() * 5;
                else
                    potential++;
            }
            if (potential > bestIndustriesPotential)
            {
                bestIndustriesPotential = potential;
                bestIndustriesPoints.Clear();
                bestIndustriesPoints.AddRange(industriesPoints);
            }
            industriesPoints.Clear();
            industriesPoints.AddRange(defaultIndustriesPoints);
        }

        public void makePotential()
        {
            List<Industry> defaultIndustriesPoints = new List<Industry>();
            defaultIndustriesPoints.AddRange(industriesPoints);
            List<Industry> bestIndustriesPoints = new List<Industry>();
            bestIndustriesPoints.AddRange(industriesPoints);
            int bestIndustriesPotential = 0;
            //Loop the calculation for some times and keep the best result
            for (int i = 0; i < 10; i++)
            {
                makePotentialOneIter(defaultIndustriesPoints, ref bestIndustriesPotential, bestIndustriesPoints);
            }
            industriesPoints.Clear();
            industriesPoints.AddRange(bestIndustriesPoints);
        }

        //The function is to be run as a Thread
        public void runProcess(List<List<MapPart>> mapToRead)
        {
            if (beginCoordinates != new Vector2(-1, -1))
            {
                CountResources(mapToRead);
                Console.WriteLine("Reading finished");
            }
            ClearLists();
            Console.WriteLine("Clearing finished");
            makePotential();
            Console.WriteLine("Counting finished");
        }
    }
}
