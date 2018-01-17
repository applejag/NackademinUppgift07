using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NackademinUppgift07.Models
{
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser(string username)
			: base(username)
		{
			Bestallning = new HashSet<Bestallning>();
		}

		public ApplicationUser()
			: this(null)
		{ }

		public string DisplayName { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }

		public ICollection<Bestallning> Bestallning { get; set; }

	}
}