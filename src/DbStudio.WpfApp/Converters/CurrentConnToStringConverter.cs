using DbStudio.WpfApp.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DbStudio.WpfApp.Converters
{
    public class CurrentConnToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DbConnection connection
                ? $"{connection.DataSource} - {connection.UserId} - {connection.InitialCatalog}"
                : "空";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}