using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BjjAcademy.Converters
{
    public class TypeOfEventToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var TypeOfEvent = (Models.BjjEventType)value;
            if (TypeOfEvent == Models.BjjEventType.Promotion) return Color.FromHex("#fafaaa");
            else return Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
