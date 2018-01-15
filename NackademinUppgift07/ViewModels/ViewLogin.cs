using System.ComponentModel.DataAnnotations;

namespace NackademinUppgift07.ViewModels
{
	public class ViewLogin
	{
		[Display(Name = "Användarnamn")]
		[Required(ErrorMessage = "Var god ange ditt användarnamn.")]
		[MaxLength(20, ErrorMessage = "Användarnamnet får max vara 20 karaktärer.")]
		public string AnvandarNamn { get; set; }

		[Display(Name = "Lösenord")]
		[Required(ErrorMessage = "Var god ange ditt lösenord.")]
		[MaxLength(20, ErrorMessage = "Lösenordet får max vara 20 karaktärer.")]
		[DataType(DataType.Password)]
		public string Losenord { get; set; }
	}
}