using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NackademinUppgift07.DataModels
{
    public partial class Produkt
    {
        public Produkt()
        {
            MatrattProdukt = new HashSet<MatrattProdukt>();
        }

        public int ProduktId { get; set; }

		[Display(Name = "Namn")]
		[Required(ErrorMessage = "Var god ange ett namn för ingrediensen.")]
		[MaxLength(50, ErrorMessage = "Ingrediensens namn får max vara 50 karaktärer.")]
        public string ProduktNamn { get; set; }

        public ICollection<MatrattProdukt> MatrattProdukt { get; set; }
    }
}
