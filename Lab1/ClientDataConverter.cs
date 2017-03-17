using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab1
{
    class ClientDataConverter : IClientDataConverter
    {
        #region Fields
        private readonly static object _instanceLocker = new object();
        private readonly Dictionary<ClientCommand, string> _commandPatternDictionary;

        private static IClientDataConverter _instance;

        #endregion

        #region Constructors

        private ClientDataConverter(string messageEnd)
        {
            _commandPatternDictionary = new Dictionary<ClientCommand, string>
            {
                { ClientCommand.Time, string.Format(@"^TIME{0}$", messageEnd) },
                { ClientCommand.Close, string.Format(@"CLOSE{0}$", messageEnd) },
            };
        }

        #endregion

        #region Public Methods

        public static IClientDataConverter GetInstance(string messageEnd)
        {
            lock (_instanceLocker)
            {
                return _instance ??
                       (_instance = new ClientDataConverter(messageEnd));
            }
        }

        #endregion

        #region IClientDataConverter Members

        public ClientMessage ConvertDataToClientMessage(string data)
        {
            foreach (var kvp in _commandPatternDictionary)
            {
                var match = Regex.Match(data, kvp.Value, RegexOptions.IgnoreCase);
                if (match.Success)
                    return new ClientMessage(data, kvp.Key);
            }

            return new ClientMessage(data, ClientCommand.Error);
        }

        #endregion
    }
}
