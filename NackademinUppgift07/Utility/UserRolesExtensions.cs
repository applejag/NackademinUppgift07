using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NackademinUppgift07.DataModels;

namespace NackademinUppgift07.Utility
{
	public static class UserRolesExtensions
	{

		public static bool IsInRole<TEnum>(this ClaimsPrincipal user, TEnum role)
		{
			return user.IsInRole(role.ToString());
		}

		public static async Task<bool> IsInRoleAsync<TUser, TEnum>(this UserManager<TUser> userManager, TUser user, TEnum role)
			where TUser : IdentityUser
		{
			return await userManager.IsInRoleAsync(user, role.ToString());
		}

		public static async Task<IdentityResult> AddToRoleAsync<TUser, TEnum>(this UserManager<TUser> userManager, TUser user, TEnum role)
			where TUser : IdentityUser
		{
			return await userManager.AddToRoleAsync(user, role.ToString());
		}

		public static async Task<IdentityResult> AddToRolesAsync<TUser, TEnum>(this UserManager<TUser> userManager, TUser user,
			params TEnum[] roles) where TUser : IdentityUser
		{
			return await userManager.AddToRolesAsync(user, roles.Select(r => r.ToString()));
		}

		public static async Task<IdentityResult> RemoveFromRoleAsync<TUser, TEnum>(this UserManager<TUser> userManager, TUser user, TEnum role)
			where TUser : IdentityUser
		{
			return await userManager.RemoveFromRoleAsync(user, role.ToString());
		}

		public static async Task<IdentityResult> RemoveFromRolesAsync<TUser, TEnum>(this UserManager<TUser> userManager, TUser user,
			params TEnum[] roles) where TUser : IdentityUser
		{
			return await userManager.RemoveFromRolesAsync(user, roles.Select(r => r.ToString()));
		}

		public static async Task<IdentityResult> EnsureAllUsersInRoleAsync<TUser, TEnum>(this UserManager<TUser> userManager, TEnum role)
			where TUser : IdentityUser
		{
			foreach (TUser user in userManager.Users)
			{
				if (await userManager.IsInRoleAsync(user, role)) continue;

				IdentityResult result = await userManager.AddToRoleAsync(user, role);

				if (!result.Succeeded)
					return result;
			}

			return IdentityResult.Success;
		}

		public static async Task EnsureRoleExists<TRole>(this RoleManager<TRole> roleManager, string role)
			where TRole : IdentityRole, new()
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new TRole {Name = role});
			}
		}

		public static async Task EnsureAllRolesExists<TRole, TEnum>(this RoleManager<TRole> roleManager)
			where TRole : IdentityRole, new()
		{
			foreach (string role in Enum.GetNames(typeof(TEnum)))
			{
				await roleManager.EnsureRoleExists(role);
			}
		}
	}
}