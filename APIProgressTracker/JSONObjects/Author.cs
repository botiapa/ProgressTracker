using System;
using System.Collections.Generic;
using System.Text;

namespace APIProgressTracker.JSONObjects
{
    public class Author
    {
        public string Name;
        public string Image;

        public Author(string name, string image)
        {
            Name = name;
            Image = image;
        }
    }
}
