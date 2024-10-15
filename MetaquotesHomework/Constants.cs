using System.Text;

namespace MetaquotesHomework
{
    public static class Constants
    {
        public const int HeaderSize = 4 + 32 + 8; //version, name[32], timestamp

        public const int IntSize = 4;

        public const int IpLocationSize = 12;

        public const int LocationSize = 96;
        
        public const int CountryOffset = 0;
        public const int CountrySize = 8;
        
        public const int RegionOffset = 8;
        public const int RegionSize = 12;

        public const int PostalOffset = 20;
        public const int PostalSize = 12;

        public const int OrgOffset = 56;
        public const int OrgSize = 32;

        public const int CityOffset = 32;
        public const int CitySize = 24;

        public const int LatOffset = 88;
        public const int LongOffset = 92;

        public const string ApplicationJsonContent = "application/json; charset=utf-8";
        public static readonly byte[] OpeningBrace = Encoding.UTF8.GetBytes("[");
        public static readonly byte[] Comma = Encoding.UTF8.GetBytes(",");
        public static readonly byte[] ClosingBrace = Encoding.UTF8.GetBytes("]");
    }
}
