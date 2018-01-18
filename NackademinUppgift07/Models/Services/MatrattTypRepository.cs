using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;

namespace NackademinUppgift07.Models.Services
{
	public class MatrattTypRepository
	{
		protected readonly TomasosContext dbContext;

		public MatrattTypRepository(TomasosContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<List<MatrattTyp>> GetMatrattTypesAsync()
		{
		    return await dbContext.MatrattTyp.ToListAsync();
		}
	}
}