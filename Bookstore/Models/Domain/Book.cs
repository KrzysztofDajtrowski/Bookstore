using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    public class Book
    {
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }
        public virtual string Genre { get; set; }
        public virtual decimal Price { get; set; }
    }
}