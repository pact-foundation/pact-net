using PactNet.Models;

namespace PactNet.Tests
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public MessageModel()
        {
        }

        internal MessageModel(int id, string description)
        {
            Id = id;
            Description = description;
        }

        internal NativeMessage ToNativeMessage()
        {
            return new NativeMessage
            {
                Contents = this
            };
        }
    }
}
