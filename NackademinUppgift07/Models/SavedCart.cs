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
	}
}