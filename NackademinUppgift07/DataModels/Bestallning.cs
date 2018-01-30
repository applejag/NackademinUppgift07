using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NackademinUppgift07.Models;
using NackademinUppgift07.Utility;

namespace NackademinUppgift07.DataModels
{
    public partial class Bestallning
    {
		public const int POINTS_FOR_FREE_FOOD = 100;
	    public const int POINTS_PER_MATRATT = 10;
	    public const decimal PREMIUM_DISCOUNT = 0.2m;

        public Bestallning()
        {
            BestallningMatratt = new HashSet<BestallningMatratt>();
        }

        public int BestallningId { get; set; }
        public DateTime BestallningDatum { get; set; }
        public decimal Totalbelopp { get; set; }
		public int OrdinalBelopp { get; set; }
		public int GratisPizzaPris { get; set; }
		public decimal Rabatt { get; set; }
        public bool Levererad { get; set; }
        public string KundId { get; set; }

        public ApplicationUser Kund { get; set; }
        public ICollection<BestallningMatratt> BestallningMatratt { get; set; }

		[NotMapped]
	    public int TotalCount => BestallningMatratt?.Sum(bm => bm.Antal) ?? 0;

	    public void CalculateTotalPrice(bool kundPremium, int kundPoints)
	    {
			if (BestallningMatratt == null)
				throw new ArgumentNullException(nameof(BestallningMatratt));
			if (BestallningMatratt.Any(bm => bm.Matratt == null))
				throw new ArgumentNullException(nameof(BestallningMatratt), "Matratt in BestallningMatratt is null");
			
		    OrdinalBelopp = BestallningMatratt.Sum(bm => bm.Antal * bm.Matratt.Pris);

		    GratisPizzaPris = kundPoints + TotalCount * POINTS_PER_MATRATT >= POINTS_FOR_FREE_FOOD
				? BestallningMatratt.Min(bm => bm.Matratt.Pris)
				: 0;

		    Rabatt = kundPremium ? PREMIUM_DISCOUNT : 0;

		    Totalbelopp = (OrdinalBelopp - GratisPizzaPris) * (1m - Rabatt);
	    }
    }
}
