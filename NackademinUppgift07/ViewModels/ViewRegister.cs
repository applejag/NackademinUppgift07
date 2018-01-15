using System.ComponentModel.DataAnnotations;

namespace NackademinUppgift07.ViewModels
{
    public class ViewRegister
	{
		[Display(Name = "Användarnamn")]
		[Required(ErrorMessage = "Var god ange ditt användarnamn.")]
		[MaxLength(20, ErrorMessage = "Användarnamnet får max vara 20 karaktärer.")]
		public string AnvandarNamn { get; set; }

		[Required(ErrorMessage = "Var god ange ditt namn.")]
		[MaxLength(100, ErrorMessage = "Namnet får max vara 100 karaktärer.")]
		public string Namn { get; set; }

		[MaxLength(50, ErrorMessage = "Gatu adressen får max vara 50 karaktärer.")]
		public string Gatuadress { get; set; }

		[MaxLength(20, ErrorMessage = "Post nummret får max vara 20 karaktärer.")]
		[DataType(DataType.PostalCode)]
		public string Postnr { get; set; }

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
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[DataType(DataType.Password)]
		public string Losenord { get; set; }

		[Display(Name = "Bekräfta lösenord")]
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[Compare(nameof(Losenord), ErrorMessage = "Lösenorden måste matcha.")]
		[DataType(DataType.Password)]
		public string LosenordConfirm { get; set; }
	}
}
