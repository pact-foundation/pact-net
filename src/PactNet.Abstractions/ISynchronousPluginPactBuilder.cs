using System;

namespace PactNet;

public interface ISynchronousPluginPactBuilderV4 : IPactBuilder, IDisposable
{
    ISynchronousPluginRequestBuilderV4 UponReceiving(string description);
}
