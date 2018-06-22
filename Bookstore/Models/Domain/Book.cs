using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookstore.Models.Domain
{
    /// <summary>
    /// Klasa Book, potrzebna do mapowania bazy danych
    /// </summary>
    public class Book
    {
        public virtual long Id { get; set; }

        [Display(Name = "Tytuł")]
        public virtual string Title { get; set; }

        [Display(Name = "Autor")]
        public virtual string Author { get; set; }

        [Display(Name = "Gatunek")]
        public virtual string Genre { get; set; }

        [Display(Name = "Cena")]
        public virtual float Price { get; set; }
    }
}