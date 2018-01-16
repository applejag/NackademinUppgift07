using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NackademinUppgift07.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }

        public int KundId { get; set; }

	    [Display(Name = "Användarnamn")]
	    [Required(ErrorMessage = "Var god ange ditt användarnamn.")]
	    [MaxLength(20, ErrorMessage = "Användarnamnet får max vara 20 karaktärer.")]
	    public string AnvandarNamn { get; set; }

	    [Required(ErrorMessage = "Var god ange ditt namn.")]
	    [MaxLength(100, ErrorMessage = "Namnet får max vara 100 karaktärer.")]
	    public string Namn { get; set; }

	    [Required(ErrorMessage = "Var god ange din gatuadress.")]
	    [MaxLength(50, ErrorMessage = "Gatuadressen får max vara 50 karaktärer.")]
	    public string Gatuadress { get; set; }

	    [Required(ErrorMessage = "Var god ange ditt postnummer.")]
		[MaxLength(20, ErrorMessage = "Postnummret får max vara 20 karaktärer.")]
	    [DataType(DataType.PostalCode)]
	    public string Postnr { get; set; }

	    [Required(ErrorMessage = "Var god ange din post ort.")]
	    [MaxLength(100, ErrorMessage = "Post orten får max vara 100 karaktärer.")]
	    public string Postort { get; set; }

	    [Required(ErrorMessage = "Var god ange din epost adress.")]
	    [EmailAddress(ErrorMessage = "Var god ange en giltig epost adress.")]
	    [MaxLength(50, ErrorMessage = "Epost adressen får max vara 50 karaktärer.")]
	    public string Email { get; set; }

	    [MaxLength(50, ErrorMessage = "Telefon nummret får max vara 50 karaktärer.")]
	    [DataType(DataType.PhoneNumber)]
	    public string Telefon { get; set; }

	    [Display(Name = "Lösenord")]
		[Required(ErrorMessage = "Var god ange ditt lösenord.")]
	    [MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
	    [DataType(DataType.Password)]
	    public string Losenord { get; set; }

	    [Display(Name = "Bekräfta lösenord")]
	    [MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
	    [Compare(nameof(Losenord), ErrorMessage = "Lösenorden måste matcha.")]
	    [DataType(DataType.Password)]
		[NotMapped]
	    public string LosenordConfirm { get; set; }

		public ICollection<Bestallning> Bestallning { get; set; }
    }
}
