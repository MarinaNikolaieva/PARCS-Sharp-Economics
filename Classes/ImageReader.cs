using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EconomicOnParcs.Classes
{
    public class ImageReader
    {
        private List<List<MapPart>> map;
        private Dictionary<Color, Vector2> countriesCoordinates { get; set; }

        private Bitmap politicMap { get; set; }
        private Bitmap ongroundResMap { get; set; }
        private Bitmap undergroundResMap { get; set; }
        private Bitmap globalResMap { get; set; }
        private string folderPath = "";

        public ImageReader(string folderPath)
        {
            map = new List<List<MapPart>>();
            countriesCoordinates = new Dictionary<Color, Vector2>();
            this.folderPath = folderPath;
        }

        public void readMaps()
        {
            string polFilePath = folderPath + "\\PoliticalMap.png";
            politicMap = new Bitmap(Image.FromFile(polFilePath));
            string ongFilePath = folderPath + "\\OnGroundResMap.png";
            ongroundResMap = new Bitmap(Image.FromFile(ongFilePath));
            string ungFilePath = folderPath + "\\UnderGroundResMap.png";
            undergroundResMap = new Bitmap(Image.FromFile(ungFilePath));
            string glFilePath = folderPath + "\\GlobalResMap.png";
            globalResMap = new Bitmap(Image.FromFile(glFilePath));

            for (int i = 0; i < politicMap.Height; i++)
            {
                map.Add(new List<MapPart>());
                for (int j = 0; j < politicMap.Width; j++)
                {
                    Vector2 coords = new Vector2(j, i);
                    if (politicMap.GetPixel(j, i) != ColorTranslator.FromHtml("#ffffff"))
                    {
                        if (!countriesCoordinates.ContainsKey(politicMap.GetPixel(j, i)))
                            countriesCoordinates.Add(politicMap.GetPixel(j, i), coords);
                    }
                    MapPart mapPart = new MapPart();
                    mapPart.coordinates = coords;
                    mapPart.countryColor = politicMap.GetPixel(j, i);
                    mapPart.onGroundResColor = ongroundResMap.GetPixel(j, i);
                    mapPart.underGroundResColor = undergroundResMap.GetPixel(j, i);
                    mapPart.globalResColor = globalResMap.GetPixel(j, i);
                    map.ElementAt(i).Add(mapPart);
                }
            }
        }

        public List<List<MapPart>> getMap()
        {
            return map;
        }

        public Dictionary<Color, Vector2> getCoordinates()
        {
            return countriesCoordinates;
        }
    }
}
