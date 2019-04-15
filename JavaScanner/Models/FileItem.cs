using System.Collections.Generic;

namespace JavaScanner.Models
{
    public class FileItem
    {
        public string Name { get; set; }

        public IReadOnlyCollection<JavaMethod> Methods { get; set; }
    }

}