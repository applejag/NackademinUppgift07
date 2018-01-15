using System;
using System.Collections.Generic;

namespace NackademinUppgift07.Models
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
        public int KundId { get; set; }

        public Kund Kund { get; set; }
        public ICollection<BestallningMatratt> BestallningMatratt { get; set; }
    }
}
