using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BjjAcademy.Converters
{
    public class BeltToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var beltId = (byte)value;
            

            if (beltId <= 5) return Color.White;
            else if (beltId > 5 && beltId <= 10) return Color.DarkBlue;
            else if (beltId > 10 && beltId <= 15) return Color.Purple;
            else if (beltId > 15 && beltId <= 20) return Color.Brown;
            else if (beltId > 20 && beltId <= 25) return Color.Black;
            else return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
