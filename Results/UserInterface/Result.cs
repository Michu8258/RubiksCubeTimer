using System.Collections.Generic;

namespace Results.UserInterface
{
    public class Result
    {
        private bool _isFaulted;
        private readonly IList<string> _messages;

        public bool IsFaulted { get { return _isFaulted; } }
        public IEnumerable<string> Messages { get { return _messages; } }
        public object JsonData { get; set; }

        public Result()
        {
            _isFaulted = false;
            _messages = new List<string>();
        }

        public Result(string message, bool fail = true)
        {
            _isFaulted = fail;
            _messages = new List<string>()
            {
                message
            };
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void AddError(string error)
        {
            _isFaulted = true;
            _messages.Add(error);
        }
    }
}
