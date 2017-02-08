using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Validators
{
    interface IProviderMessageValidator
    {
        void Validate(MessagingPactFile pactFile);
    }
}
