using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;

namespace Lab1ClientUI.Converters
{
    public class StringToIPEndPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IPEndPoint point = null;
            try
            {
                var str = value.ToString().Split(':');
                var ipStr = str[0];
                var portStr = str[1];
                var ip = IPAddress.Parse(ipStr);
                var port = int.Parse(portStr);
                if (port > ushort.MaxValue)
                    port = ushort.MaxValue;
                else if (port < ushort.MinValue)
                    port = ushort.MinValue;

                point = new IPEndPoint(ip, port);
            }
            catch { }

            return point;
        }
    }
}
