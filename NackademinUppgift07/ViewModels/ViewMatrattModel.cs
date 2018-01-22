using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NackademinUppgift07.ViewModels
{
	public class ViewMatrattModel
	{
		[Required(ErrorMessage = "Var god ange maträttens namn.")]
		[MaxLength(50, ErrorMessage = "Namnet får max vara 50 karaktärer.")]
		[Display(Name = "Namn")]
		public string MatrattNamn { get; set; }

		[Required(ErrorMessage = "Var god ange maträttens beskrivning.")]
		[MaxLength(200, ErrorMessage = "Beskrivningen får max vara 200 karaktärer.")]
		[DataType(DataType.MultilineText)]
		public string Beskrivning { get; set; }

		[Required(ErrorMessage = "Var god ange maträttens pris.")]
		[Range(0, int.MaxValue, ErrorMessage = "Var god ange ett giltigt pris.")]
		public int Pris { get; set; }

		[Required(ErrorMessage = "Var god ange maträttens kategori.")]
		[Display(Name = "Kategori")]
		public int MatrattTyp { get; set; }
		public SelectList MatrattTypes { get; set; }

		[Display(Name = "Ingredienser")]
		[Required(ErrorMessage = "Var god ange maträttens ingredienser")]
		public List<SelectListItem> Produkts { get; set; }
	}
}