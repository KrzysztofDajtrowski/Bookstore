using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.ViewController
{
    /// <summary>
    /// Biznes logika
    /// Rejestracja nowego usera
    /// wymog wypelnienia wszystkich pol
    /// [EmailAddress] - sprawdza czy faktycznie wprowadzony zostal adres e-mail
    /// haslo typ password, min 6 znakow, max 100, wymagane potwierdzenie
    /// 
    /// </summary>
    public class RegiesterViewModel
    {
       
        [Required]  
        [EmailAddress] 
        [Display(Name = "Adres e-mail")]
        public string Login { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        [Compare("Password", ErrorMessage = "Hasło i jego potwierdzenie są niezgodne.")]
        public string RetypePassword { get; set; }
    }
    /// <summary>
    /// Logowanie, oba pola wymagane, haslo typu password 
    /// </summary>
    public class LoginViewModel
    {
       
        [Required]
        [Display(Name = "Email")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

    }
    /// <summary>
    /// dane potrzebne do wysylki, wszystkie pola wymagane
    /// </summary>
    public class AddressViewModel
    {
       
        [Required]
        [Display(Name ="Imię")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Ulica")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Kraj")]
        public string Country { get; set; }
    }
}