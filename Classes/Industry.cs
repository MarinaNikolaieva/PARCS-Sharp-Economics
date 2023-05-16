using System;
using System.Collections.Generic;
using System.Linq;

namespace EconomicOnParcs.Classes
{
    public class Industry : IResource, IEquatable<IResource>
    {
        private int ID { get; }
        private string Name { get; set; }
        private int Grade { get; set; }
        private int Points { get; set; }
        //Now for the needed dictionaries
        //IResource is the resource/industry, first int is min number needed, second is the max number needed
        private Dictionary<IResource, Tuple<int, int>> Required { get; }  //NO CHANGES, the resources required to aquire a point
        private Dictionary<IResource, int> ObtainedRequired { get; set; }  //the ammount of required resources we have
        private Dictionary<IResource, Tuple<int, int>> Needed { get; }  //NO CHANGES, the resources needed to aquire a point
        private Dictionary<IResource, int> Obtained { get; set; }  //the ammount of needed resources we have

        private Random rand = new Random();

        public Industry(int ID, string Name, int Grade)
        {
            this.ID = ID;
            this.Name = Name;
            this.Grade = Grade;
            Points = 0;

            Required = new Dictionary<IResource, Tuple<int, int>>();
            ObtainedRequired = new Dictionary<IResource, int>();
            Needed = new Dictionary<IResource, Tuple<int, int>>();
            Obtained = new Dictionary<IResource, int>();
        }

        public string getName()
        {
            return Name;
        }

        public void setName(string name)
        {
            Name = name;
        }

        public int getPoints()
        {
            return Points;
        }

        public int getID()
        {
            return ID;
        }

        public int getGrade()
        {
            return Grade;
        }

        public void setGrade(int grade)
        {
            if (grade > 0)
                Grade = grade;
        }

        public List<IResource> getRequired()
        {
            List<IResource> resources = new List<IResource>();
            foreach (var resource in Required)
            {
                resources.Add(resource.Key);
            }
            return resources;
        }

        public List<IResource> getNeeded()
        {
            List<IResource> resources = new List<IResource>();
            foreach (var resource in Needed)
            {
                resources.Add(resource.Key);
            }
            return resources;
        }

        public Dictionary<IResource, int> getObtainedRequired()
        {
            Dictionary<IResource, int> resources = new Dictionary<IResource, int>();
            foreach (var resource in ObtainedRequired)
            {
                resources.Add(resource.Key, resource.Value);
            }
            return resources;
        }

        public Dictionary<IResource, int> getObtained()
        {
            Dictionary<IResource, int> resources = new Dictionary<IResource, int>();
            foreach (var resource in Obtained)
            {
                resources.Add(resource.Key, resource.Value);
            }
            return resources;
        }

        public void aquirePointsGradeOne()
        {
            bool cont = true;
            while (cont)
            {
                for (int i = 0; i < Obtained.Count; i++)
                {
                    int curRate = rand.Next(Needed[Obtained.ElementAt(i).Key].Item1, Needed[Obtained.ElementAt(i).Key].Item2 + 1);
                    if (Obtained.ElementAt(i).Value >= curRate)
                    {
                        if (Required.Count == 0)  //If there are no required resources, then just add a point
                        {
                            Points++;
                            Obtained[Obtained.ElementAt(i).Key] -= curRate;
                            cont = true;
                        }
                        else  //If there are, check if we have enough too
                        {
                            bool add = true;
                            List<int> reqRates = new List<int>();
                            for (int j = 0; j < Required.Count; j++)
                            {
                                int reqRate = rand.Next(Required.ElementAt(j).Value.Item1, Required.ElementAt(j).Value.Item2 + 1);
                                reqRates.Add(reqRate);
                                if (ObtainedRequired.ElementAt(j).Value < reqRate)
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add)
                            {
                                Points++;
                                Obtained[Obtained.ElementAt(i).Key] -= curRate;
                                for (int j = 0; j < Required.Count; j++)
                                {
                                    ObtainedRequired[ObtainedRequired.ElementAt(j).Key] -= reqRates.ElementAt(j);
                                }
                                cont = true;
                            }
                        }
                    }
                    else
                        cont = false;
                }
            }
        }

        public void aquirePointsGradesUp()
        {
            bool cont = true;
            while (cont)
            {
                cont = false;
                bool add = true;
                List<int> reqCurRates = new List<int>();
                List<int> needCurRates = new List<int>();
                for (int i = 0; i < Obtained.Count; i++)
                {
                    int curRate = rand.Next(Needed.ElementAt(i).Value.Item1, Needed.ElementAt(i).Value.Item2 + 1);
                    needCurRates.Add(curRate);
                    if (Obtained.ElementAt(i).Value < curRate)
                    {
                        add = false;
                        break;
                    }
                }
                for (int j = 0; j < Required.Count; j++)
                {
                    int reqRate = rand.Next(Required.ElementAt(j).Value.Item1, Required.ElementAt(j).Value.Item2 + 1);
                    reqCurRates.Add(reqRate);
                    if (ObtainedRequired.ElementAt(j).Value < reqRate)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    Points++;
                    for (int i = 0; i < Needed.Count; i++)
                    {
                        Obtained[Obtained.ElementAt(i).Key] -= needCurRates.ElementAt(i);
                    }
                    for (int j = 0; j < Required.Count; j++)
                    {
                        ObtainedRequired[ObtainedRequired.ElementAt(j).Key] -= reqCurRates.ElementAt(j);
                    }
                    cont = true;
                }
            }
        }

        public void aquirePoints()
        {
            if (Grade == 1)  //the 1-grade industries need one resource + required to get a point
            {
                aquirePointsGradeOne();
            }
            else  //for the higher-level industries, ALL resources + required are needed to get a point
            {
                aquirePointsGradesUp();
            }
        }

        public void addRequired(IResource resource, Tuple<int, int> frequencies)
        {
            if (!Required.ContainsKey(resource))
            {
                Required.Add(resource, frequencies);
            }
        }

        public void addNeeded(IResource resource, Tuple<int, int> frequencies)
        {
            if (!Needed.ContainsKey(resource))
            {
                Needed.Add(resource, frequencies);
            }
        }
        public void fillObtainedRequired()  //must be called AFTER we get all the Required values
        {
            for (int i = 0; i < Required.Count; i++)
            {
                ObtainedRequired.Add(Required.ElementAt(i).Key, 0);
            }
        }

        public void addObtainedRequired(IResource resource, int count)
        {
            ObtainedRequired[resource] += count;
        }

        public void fillObtained()  //must be called AFTER we get all the Needed values
        {
            for (int i = 0; i < Needed.Count; i++)
            {
                Obtained.Add(Needed.ElementAt(i).Key, 0);
            }
        }

        public void addObtained(IResource resource, int count)
        {
            Obtained[resource] += count;
        }

        public bool SameType(IResource other)
        {
            if (this.GetType() == other.GetType())
            {
                return true;
            }
            return false;
        }

        public bool SameName(IResource other)
        {
            Industry thisInd = null;
            Industry otherInd = null;
            BasicResource thisRes = null;
            BasicResource otherRes = null;
            if (this.GetType() == typeof(Industry)) thisInd = this as Industry;
            if (other.GetType() == typeof(Industry)) otherInd = other as Industry;
            if (this.GetType() == typeof(BasicResource)) thisRes = (IResource)this as BasicResource;
            if (other.GetType() == typeof(BasicResource)) otherRes = other as BasicResource;

            if (thisInd != null && otherInd != null)
                return thisInd.getName().Equals(otherInd.getName());
            if (thisInd != null && otherRes != null)
                return thisInd.getName().Equals(otherRes.getName());
            if (thisRes != null && otherInd != null)
                return thisRes.getName().Equals(otherInd.getName());
            if (thisRes != null && otherRes != null)
                return thisRes.getName().Equals(otherRes.getName());

            return false;
        }

        public bool Equals(IResource other)
        {
            return (this as IResource).SameType(other) && (this as IResource).SameName(other);
        }
    }
}
