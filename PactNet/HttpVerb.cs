using System.ComponentModel;

namespace PactNet
{
    public enum HttpVerb
    {
        [Description("get")]
        Get,
        Post,
        Put,
        Delete,
        Head,
        Patch
    }
}