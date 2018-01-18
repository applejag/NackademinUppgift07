using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;
using Newtonsoft.Json;

namespace NackademinUppgift07.Models
{
	public struct SavedCart
	{
		public (int foodId, int count)[] orders;
		public int TotalCount => orders?.Sum(o => o.count) ?? 0;

		public SavedCart(HttpContext context)
		{
			string serialized = context.Session.GetString("Cart");

			this = string.IsNullOrEmpty(serialized)
				? new SavedCart()
				: JsonConvert.DeserializeObject<SavedCart>(serialized);
		}

		public SavedCart(Bestallning cart)
		{
			orders = (from b in cart.BestallningMatratt
					select (foodId: b.Matratt.MatrattId, count: b.Antal))
				.ToArray();
		}

		public async Task<Bestallning> ConvertToOrder(TomasosContext context)
		{
			if (orders == null)
				return new Bestallning();

			var matratts = context.Matratt
				.Include(m => m.MatrattTypNavigation)
				.Include(m => m.MatrattProdukt).ThenInclude(p => p.Produkt);

			return new Bestallning
			{
				BestallningMatratt =
					await (from mat in matratts
						join order in orders on mat.MatrattId equals order.foodId
						select new BestallningMatratt
						{
							Matratt = mat,
							MatrattId = mat.MatrattId,
							Antal = order.count
						}).ToListAsync(),
			};
		}

		public void SaveCart(HttpContext context)
		{
			orders = orders?.Where(o => o.count >= 0).ToArray();
			string serialized = JsonConvert.SerializeObject(this);

			context.Session.SetString("Cart", serialized);
		}
	}
}