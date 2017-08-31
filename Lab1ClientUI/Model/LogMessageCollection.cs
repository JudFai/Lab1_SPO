using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Lab1ClientUI.Model
{
    public class LogMessageCollection : ObservableCollection<LogMessage>
    {
        public LogMessageCollection(IEnumerable<LogMessage> collection) : base(collection)
        {
        }

        public LogMessageCollection()
        {
        }
    }
}
