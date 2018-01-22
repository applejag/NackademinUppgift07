using System.Linq;
using NackademinUppgift07.Models;

namespace NackademinUppgift07.Models
{
	public enum UserRole
	{
		RegularUser,
		PremiumUser,
		Admin
	}
}

namespace Microsoft.AspNetCore.Authorization
{
	public class AuthorizeRolesAttribute : AuthorizeAttribute
	{
		public AuthorizeRolesAttribute(params UserRole[] roles)
		{
			Roles = string.Join(",", roles.Select(r => r.ToString()));
		}
	}
}