using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    public class OrdersBook
    {
        public virtual int Id { get; set; }
        public virtual int OrderID { get; set; }
        public virtual int BookID { get; set; }
    }
}