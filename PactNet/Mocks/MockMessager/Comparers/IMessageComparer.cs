using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Comparers;
using PactNet.Models.Messaging;

namespace PactNet.Mocks.MockMessager.Comparers
{
    internal interface IMessageComparer
    {
        ComparisonResult Compare(Message expected, dynamic actual);
    }
}
