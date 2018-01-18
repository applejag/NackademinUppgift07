using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NackademinUppgift07.DataModels
{
    public partial class Bestallning
    {
        public Bestallning()
        {
            BestallningMatratt = new HashSet<BestallningMatratt>();
        }

        public int BestallningId { get; set; }
        public DateTime BestallningDatum { get; set; }
        public int Totalbelopp { get; set; }
        public bool Levererad { get; set; }
        public string KundId { get; set; }

        public ApplicationUser Kund { get; set; }
        public ICollection<BestallningMatratt> BestallningMatratt { get; set; }

		[NotMapped]
	    public int TotalCount => BestallningMatratt?.Sum(bm => bm.Antal) ?? 0;
    }
}
