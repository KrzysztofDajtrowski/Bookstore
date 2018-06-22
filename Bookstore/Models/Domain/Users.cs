using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    /// <summary>
    /// klasa Users, potrzebna do mapowania bazy danych
    /// </summary>
    public class User : IUser<int>
    {
    
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
    }
}