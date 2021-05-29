using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_manager
{
    public class Books
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Price { get; set; }
        public string Rating { get; set; }
        public string Link { get; set; }
        public Books() { }
        public Books(string name, string author, string price, string rating, string link)
        {
            Name = name;
            Author = author;
            Price = price;
            Rating = rating;
            Link = link;
        }

    }
}
