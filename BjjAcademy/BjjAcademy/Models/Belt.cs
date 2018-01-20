using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace BjjAcademy
{
    public enum BeltColour
    {
        White,
        Blue,
        Purple,
        Brown,
        Black,
    }

    public class Belt
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public BeltColour BeltColour { get; set; }

        public byte Stripes { get; set; }

        public Belt()
        {
            this.BeltColour = BeltColour.White;
            this.Stripes = 0;
        }

        public Belt(BeltColour beltColour, byte stripes)
        {
            this.BeltColour = beltColour;
            this.Stripes = stripes;
        }
    }
}
