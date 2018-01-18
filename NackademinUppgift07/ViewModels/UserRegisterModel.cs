using System.ComponentModel.DataAnnotations;
using NackademinUppgift07.DataModels;

namespace NackademinUppgift07.Models
{
	public class UserRegisterModel
	{
		public UserRegisterModel(ApplicationUser source)
		{
			UserName = source.UserName;
			DisplayName = source.DisplayName;
			Email = source.Email;
			PhoneNumber = source.PhoneNumber;
			Address = source.Address;
			PostalCode = source.PostalCode;
			City = source.City;
		}

		public UserRegisterModel()
		{ }

		public void ApplyToApplicationUser(ApplicationUser user)
		{
			user.UserName = UserName;
			user.DisplayName = DisplayName;
			user.Email = Email;
			user.PhoneNumber = PhoneNumber;
			user.Address = Address;
			user.PostalCode = PostalCode;
			user.City = City;
		}

		[Required]
		[MaxLength(127)]
		public string UserName { get; set; }

		[Required]
		[MaxLength(255)]
		public string DisplayName { get; set; }

		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		[MaxLength(255)]
		public string Email { get; set; }

		[Phone]
		[DataType(DataType.PhoneNumber)]
		[MaxLength(127)]
		public string PhoneNumber { get; set; }

		[Required]
		[MaxLength(255)]
		public string Address { get; set; }

		[Required]
		[DataType(DataType.PostalCode)]
		[MaxLength(127)]
		public string PostalCode { get; set; }

		[Required]
		[MaxLength(255)]
		public string City { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[MinLength(6)]
		[MaxLength(127)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password))]
		public string PasswordConfirm { get; set; }
	}
}