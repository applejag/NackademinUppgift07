using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NackademinUppgift07.Models;

namespace NackademinUppgift07.ViewModels
{
	public class ViewKund
	{

		public ViewKund()
			: this(null)
		{}

		public ViewKund(Kund kund)
		{
			if (kund == null) return;
			CurrentKund = kund;

			Namn = kund.Namn;
			Gatuadress = kund.Gatuadress;
			Postnr = kund.Postnr;
			Postort = kund.Postort;
			Email = kund.Email;
			Telefon = kund.Telefon;
		}

		public Kund CurrentKund { get; }

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

		[Display(Name = "Tidigare lösenord")]
		[Required(ErrorMessage = "Var god ange ditt tidigare lösenord.")]
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[DataType(DataType.Password)]
		public string OldLosenord { get; set; }

		[Display(Name = "Nytt lösenord")]
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[DataType(DataType.Password)]
		public string NewLosenord { get; set; }

		[Display(Name = "Bekräfta nytt lösenord")]
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[Compare(nameof(NewLosenord), ErrorMessage = "Lösenorden måste matcha.")]
		[DataType(DataType.Password)]
		[NotMapped]
		public string NewLosenordConfirm { get; set; }

	}
}