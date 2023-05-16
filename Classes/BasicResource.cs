using System;
using System.Drawing;

namespace EconomicOnParcs.Classes
{
    public class BasicResource : IResource, IEquatable<IResource>
    {
        private int ID { get; }
        private string Name { get; }
        private Color color { get; }

        public BasicResource(int ID, string Name, Color color)
        {
            this.ID = ID;
            this.Name = Name;
            this.color = color;
        }

        public BasicResource(BasicResource other)  //Sort of a copy constructor
        {
            this.ID = other.getID();
            this.Name = other.getName();
            this.color = other.getColor();
        }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return color;
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
            if (this.GetType() == typeof(Industry)) thisInd = (IResource)this as Industry;
            if (other.GetType() == typeof(Industry)) otherInd = other as Industry;
            if (this.GetType() == typeof(BasicResource)) thisRes = this;
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
