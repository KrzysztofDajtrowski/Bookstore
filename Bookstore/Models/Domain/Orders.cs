using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    /// <summary>
    /// Klasa Orders, potrzebna do mapowania bazy danych
    /// </summary>
    public class Orders
    {
        public virtual int Id { get; set; }
        public virtual User UserID { get; set; }
        public virtual bool Active { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string Country { get; set; }
    }
}