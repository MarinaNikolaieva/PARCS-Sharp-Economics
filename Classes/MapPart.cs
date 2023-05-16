using System;
using System.Drawing;
using System.Numerics;

namespace EconomicOnParcs.Classes
{
    public class MapPart
    {
        public Vector2 coordinates;
        public Color countryColor;
        public Color onGroundResColor;
        public Color underGroundResColor;
        public Color globalResColor;

        public MapPart()
        {
            coordinates = new Vector2();
            countryColor = new Color();
            onGroundResColor = new Color();
            underGroundResColor = new Color();
            globalResColor = new Color();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (!(obj is MapPart))
            {
                return false;
            }
            MapPart other = obj as MapPart;
            return this.coordinates.Equals(other.coordinates);
        }

        public override int GetHashCode()
        {
            return this.coordinates.X.GetHashCode() ^ this.coordinates.Y.GetHashCode();
        }
    }
}
