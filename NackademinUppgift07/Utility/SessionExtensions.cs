using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NackademinUppgift07.Utility
{
	public static class SessionExtensions
	{
		public static void SetObject<T>(this ISession session, string key, T value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetObject<T>(this ISession session, string key)
		{
			string value = session.GetString(key);
			return string.IsNullOrEmpty(value)
				? default(T)
				: JsonConvert.DeserializeObject<T>(value);
		}
	}
}