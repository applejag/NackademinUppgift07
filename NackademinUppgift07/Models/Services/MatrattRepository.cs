using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;

namespace NackademinUppgift07.Models.Services
{
	public class MatrattRepository
	{
		protected readonly TomasosContext dbContext;

		public MatrattRepository(TomasosContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<List<MatrattTyp>> GetMatrattTypesAsync()
		{
		    return await dbContext.MatrattTyp.ToListAsync();
		}

		public async Task<List<Produkt>> GetProductsAsync()
		{
			return await dbContext.Produkt.ToListAsync();
		}

		public async Task<List<Produkt>> GetProductsWithMatrattsAsync()
		{
			return await dbContext.Produkt
				.Include(p => p.MatrattProdukt).ThenInclude(mp => mp.Matratt)
				.ToListAsync();
		}

		public async Task<SelectList> GetProductSelectListAsync()
		{
			return new SelectList(await GetProductsAsync(), nameof(Produkt.ProduktId), nameof(Produkt.ProduktNamn));
		}
	}
}