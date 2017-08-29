using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab1.Server
{
    class ClientDataConverter : IClientDataConverter
    {
        #region Fields

        private readonly static object _instanceLocker = new object();
        private readonly Dictionary<ClientCommand, string> _commandPatternDictionary;

        private static IClientDataConverter _instance;

        private readonly List<string> _groupParamsCollection;

        #endregion

        #region Constructors

        private ClientDataConverter(string messageEnd)
        {
            _groupParamsCollection = new List<string>
            {
                "param1", "param2"
            };
            _commandPatternDictionary = new Dictionary<ClientCommand, string>
            {
                { ClientCommand.Time, string.Format(@"^TIME{0}$", messageEnd) },
                { ClientCommand.Close, string.Format(@"^CLOSE{0}$", messageEnd) },
                { ClientCommand.Echo, string.Format(@"^ECHO(\s'(?<{0}>.*)?')?{1}$", _groupParamsCollection[0], messageEnd) },
                { ClientCommand.BeginUpload, string.Format(@"^UPLOAD(\s'(?<{0}>.*)?'),(\s'(?<{1}>\d+)?'){2}$", _groupParamsCollection[0], _groupParamsCollection[1], messageEnd) },
                { ClientCommand.ContinueUpload, string.Format(@"^CONTINUE_UPLOAD(\s'(?<{0}>.*)?')?{1}$", _groupParamsCollection[0], messageEnd) },
                { ClientCommand.FinishUpload, string.Format(@"^FINISH_UPLOAD{0}$", messageEnd) },
                { ClientCommand.Download, string.Format(@"^DOWNLOAD(\s'(?<{0}>.*)?'),(\s?'(?<{1}>\d+)?'){2}$", _groupParamsCollection[0], _groupParamsCollection[1], messageEnd) }
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
                {
                    var paramCollection = new List<string>();
                    _groupParamsCollection.ForEach(p =>
                    {
                        if (match.Groups[p].Success)
                            paramCollection.Add(match.Groups[p].Value);
                    });
                    // TODO: временно используем List<string>, вдруг может понадобится формирование более сложных параметров
                    return new ClientMessage(data, kvp.Key, paramCollection.Cast<object>().ToList());
                }
            }

            return new ClientMessage(data, ClientCommand.Error);
        }

        #endregion
    }
}
