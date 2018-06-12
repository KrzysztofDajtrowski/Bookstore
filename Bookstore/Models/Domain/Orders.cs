using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    public class Orders
    {
        public virtual int Id { get; set; }
        public virtual User UserID { get; set; }
        public virtual bool Active { get; set; }
    }
}