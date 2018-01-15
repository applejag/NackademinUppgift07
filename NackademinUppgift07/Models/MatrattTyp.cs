using System;
using System.Collections.Generic;

namespace NackademinUppgift07.Models
{
    public partial class MatrattTyp
    {
        public MatrattTyp()
        {
            Matratt = new HashSet<Matratt>();
        }

        public int MatrattTyp1 { get; set; }
        public string Beskrivning { get; set; }

        public ICollection<Matratt> Matratt { get; set; }
    }
}
