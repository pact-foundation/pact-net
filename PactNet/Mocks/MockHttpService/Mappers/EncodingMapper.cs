using System;
using System.Collections.Generic;
using System.Text;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class EncodingMapper : IEncodingMapper
    {
        private readonly static Dictionary<string, Encoding> Map = new Dictionary<string, Encoding>
        {
            { "utf-8", Encoding.UTF8 },
            { "utf-7", Encoding.UTF7 },
            { "utf-16", Encoding.Unicode },
            { "utf-32", Encoding.UTF32 },
            { "utf-16BE", Encoding.BigEndianUnicode },
            { "utf-16be", Encoding.BigEndianUnicode },
            { "us-ascii", Encoding.ASCII }
        }; 

        public Encoding Convert(string from)
        {
            if (!Map.ContainsKey(from))
            {
                throw new ArgumentException(String.Format("Cannot map {0} to an Encoding, no matching item has been registered.", from));
            }

            return Map[from];
        }
    }
}