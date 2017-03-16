using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    class DataConverter : IDataConverter
    {
        #region Fields

        private readonly Encoding _encoding;

        #endregion

        #region Constructors

        public DataConverter(Encoding encoding)
        {
            _encoding = encoding;
        }

        #endregion

        #region IDataConverter Members

        public byte[] GetBytes(string str)
        {
            return _encoding.GetBytes(str);
        }

        public string GetString(byte[] data)
        {
            return _encoding.GetString(data);
        }

        #endregion
    }
}
