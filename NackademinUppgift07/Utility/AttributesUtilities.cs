using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NackademinUppgift07.Utility
{
	public static class AttributesUtilities
	{

		public static TAttr GetPropertyAttribute<TClass, TAttr>(string propertyName)
		{
			return (TAttr)typeof(TClass).GetProperty(propertyName).GetCustomAttributes(typeof(TAttr), true)
				.FirstOrDefault();
		}

		public static TAttr[] GetPropertyAttributes<TClass, TAttr>(string propertyName)
		{
			return typeof(TClass).GetProperty(propertyName).GetCustomAttributes(typeof(TAttr), true)
				.Cast<TAttr>().ToArray();
		}

		public static class Presets
		{
			public static int? GetMinLength<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, MinLengthAttribute>(propertyName)?.Length;
			}

			public static int? GetMaxLength<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, MaxLengthAttribute>(propertyName)?.Length;
			}

			public static string GetDisplayName<TClass>(string propertyName)
			{
				return GetPropertyAttribute<TClass, DisplayAttribute>(propertyName)?.Name;
			}
		}
	}
}