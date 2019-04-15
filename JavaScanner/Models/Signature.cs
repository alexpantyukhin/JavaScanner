using System.Collections.Generic;

namespace JavaScanner.Models
{
    public class Signature
    {
        public IReadOnlyCollection<string> Arguments { get; set; }

        public string ReturnType { get; set; }
    }
}