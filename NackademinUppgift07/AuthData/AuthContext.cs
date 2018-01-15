using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NackademinUppgift07.AuthData
{
	public class AuthContext : IdentityDbContext<User>
	{
		public AuthContext(DbContextOptions options)
			: base(options)
		{}
	}
}