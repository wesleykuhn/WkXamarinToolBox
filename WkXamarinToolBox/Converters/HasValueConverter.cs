using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using WkXamarinToolBox.Extensions;
using Xamarin.Forms;

namespace WkXamarinToolBox.Converters
{
    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return false;

            switch (value)
            {
                case string str:
                    return !str.IsNullEmptyOrWhiteSpace();

                case long l:
                    return l != 0;

                case int i:
                    return i != 0;

                case double d:
                    return d != 0;

                case float f:
                    return f != 0;

                case DateTime dt:
                    return dt != default(DateTime) && dt != DateTime.MinValue;

                case TimeSpan ts:
                    return ts != default(TimeSpan) && ts != TimeSpan.Zero;

                case IEnumerable<object> enumarable:
                    return enumarable.Any();

                case ICommand cmd:
                    return cmd is not null;

                case byte[] bytesArray:
                    return bytesArray is not null && bytesArray.Length > 0;

                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
