using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Imperialism_2_SGEditor
{
  public static class DB
  {
    //public static int NotifyCounter = 0;

    //ALL CONST VALUES ARE COUNTED FROM ZERO (so 20th byte is 19 in const value for array use)
    public const byte OWNER1 = 4;
    public const byte OWNER2 = 5;
    public const byte SEA = 255;
    public const byte MAP_VISIBILITY = 31;
    public const byte BUILDINGS = 29;//55 capitol
    public const byte PROD_LEVEL = 16;//0-4 levels
    public const byte LAND_TYPE_RES = 21;//resource that can be used
    public const byte LAND_TYPE_GFX = 22;//visual only
    public const byte PROVINCE_ID = 23;//255 is sea, other numbers are province id
    public static Brush[] COUNTRY_COLORS = new Brush[6] { Brushes.Blue, Brushes.Red, Brushes.Teal, Brushes.Orange, Brushes.Green, Brushes.Yellow };
    public static string[] COUNTRY_NAMES = new string[22] { "France", "England", "Sweden", "Holland", "Portugal", "Spain", "Ireland", "Scotland", "Denmark", "Germany", "Italy", "Switzerland", "The Aztecs", "The Incas", "The Mayas", "The Taino", "The Iroquois", "The Sioux", "The Kwakiutl", "The Cherokee", "The Huron", "The Pueblo" };
    public static string[] HEADERS = new string[40] { "0", "1", "2", "3", "Owner1", "Owner2", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "Prod lvl", "Res discovered by", "18", "19", "20", "Type (res)", "Type (vis)", "Sea/ID", "24", "25", "26", "27", "28", "Buildings", "30", "Explored", "32", "33", "34", "35", "36", "37", "38", "39" };

    //CITIES
    public const byte FORT_LEVEL = 3;
    public const byte IS_EXPANDED = 2;
    public const byte CITY_OWNER1 = 0;
    public const byte CITY_OWNER2 = 1;
    public const byte CITY_POS1 = 68;
    public const byte CITY_POS2 = 69;
    //CITIES END

    public static List<Resource> LandTypes = new List<Resource>()
    {
      new Resource("Plains", "plains.jpg", 1),
      new Resource("Cotton Plantation", "cotton.jpg", 2),
      new Resource("Livestock", "cattle.jpg", 3),
      new Resource("Horse Ranch", "horses.jpg", 4),
      new Resource("Grain Farm", "grain.jpg", 5),
      new Resource("Spice Orchard", "spices.jpg", 6),
      new Resource("Fertile Hills", "sheep.jpg", 7),
      new Resource("Barren Hills", "bhills.jpg", 8),
      new Resource("Mountains", "mountains.jpg", 9),
      new Resource("Swamp", "swamp.jpg", 10),
      new Resource("Desert", "desert.jpg", 11),
      new Resource("Tundra", "tundra.jpg", 12),
      new Resource("Hardwood Forest", "forest.jpg", 13),
      new Resource("Scrub Forest", "scrub_forest.jpg", 15),
      new Resource("Sugar Cane", "sugar_cane.jpg", 16),
      new Resource("Tobacco Plantation", "tobacco.jpg", 17),
      new Resource("Furry Tundra", "furs.jpg", 18)
    };

    public static List<Resource> Resources = new List<Resource>()
    {
      new Resource("Wool", "rsheep.jpg", 0),
      new Resource("Timber", "rtimber.jpg", 1),
      new Resource("Tin", "rtin.jpg", 2),
      new Resource("Copper", "rcopper.jpg", 3),
      new Resource("Iron Ore", "riron_ore.jpg", 4),
      new Resource("Coal", "rcoal.jpg", 5),
      new Resource("Cotton", "rcotton.jpg", 6),
      new Resource("Sugar Cane", "rsugar_cane.jpg", 7),
      new Resource("Tobacco", "rtobacco.jpg", 8),
      new Resource("Fur", "rfurs.jpg", 9),
      new Resource("Horses", "rhorses.jpg", 10),
      new Resource("Grain", "rgrain.jpg", 20),
      new Resource("Fish", "rfish.jpg", 21),
      new Resource("Livestock", "rcattle.jpg", 22),
      new Resource("Spices", "rspice.jpg", 23),
      new Resource("Silver", "rsilver.jpg", 24),
      new Resource("Gold", "rgold.jpg", 25),
      new Resource("Gems", "rgems.jpg", 26),
      new Resource("Diamonds", "rdiamonds.jpg", 27),
      new Resource("None", "rnone.png", 255)
    };

    public static Resource GetResourceById(byte id)
    {
      //255 no resource //32,4 res discovered (1/0, 2/1, 4/2, 8/3, 16/4, 32/5, 64/6, 128)
      foreach (var item in Resources) if (item.Id == id) return item;
      return null;
    }

    public static Resource GetLandTypesById(byte id)
    {
      //255 no resource //32,4 res discovered (1/0,2./1,4/2,8/3,16/4,32/5,64/6,128)
      foreach (var item in LandTypes) if (item.Id == id) return item;
      return null;
    }

    internal static byte GetCountryNumber(string name)
    {
      for (int i = 0; i < COUNTRY_NAMES.Length; i++)
      {
        if (COUNTRY_NAMES[i] == name) return (byte)i;
      }
      return 255;
    }
  }
}
