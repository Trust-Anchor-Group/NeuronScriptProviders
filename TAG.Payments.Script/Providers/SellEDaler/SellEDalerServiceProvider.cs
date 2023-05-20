using Paiwise;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Persistence;

namespace TAG.Payments.Script.Providers.SellEDaler
{
	/// <summary>
	/// Publishes service provider integrations made in script.
	/// </summary>
	public class SellEDalerServiceProvider : ISellEDalerServiceProvider
	{
		private static readonly Dictionary<string, ISellEDalerService> services = new Dictionary<string, ISellEDalerService>();

		/// <summary>
		/// Publishes service provider integrations made in script.
		/// </summary>
		public SellEDalerServiceProvider()
		{
		}

		/// <summary>
		/// ID of service provider.
		/// </summary>
		public string Id => typeof(SellEDalerServiceProvider).FullName;

		/// <summary>
		/// Name of service provider.
		/// </summary>
		public string Name => "Script-based services";

		/// <summary>
		/// Icon URL
		/// </summary>
		public string IconUrl => string.Empty;

		/// <summary>
		/// Width of icon
		/// </summary>
		public int IconWidth => 0;

		/// <summary>
		/// Height of icon
		/// </summary>
		public int IconHeight => 0;

		/// <summary>
		/// Gets available services supporting selling eDaler for a given currency and country.
		/// </summary>
		/// <param name="Currency">Currency</param>
		/// <param name="Country">Country</param>
		/// <returns>Array of services.</returns>
		public Task<ISellEDalerService[]> GetServicesForSellingEDaler(CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			lock (services)
			{
				ISellEDalerService[] Result = new ISellEDalerService[services.Count];
				services.Values.CopyTo(Result, 0);
				return Task.FromResult(Result);
			}
		}

		/// <summary>
		/// Gets a service for selling eDaler, given its ID, currency and country.
		/// </summary>
		/// <param name="ServiceId">Service ID</param>
		/// <param name="Currency">Currency</param>
		/// <param name="Country">Country</param>
		/// <returns>Reference to service, if found.</returns>
		public Task<ISellEDalerService> GetServiceForSellingEDaler(string ServiceId, CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			lock (services)
			{
				if (services.TryGetValue(ServiceId, out ISellEDalerService Service))
					return Task.FromResult(Service);
			}

			return Task.FromResult<ISellEDalerService>(null);
		}

		/// <summary>
		/// Clears the collection of registered services.
		/// </summary>
		internal static void Clear()
		{
			lock (services)
			{
				services.Clear();
			}
		}

		/// <summary>
		/// Registers a service.
		/// </summary>
		/// <param name="Service">Service to register.</param>
		internal static void Register(ISellEDalerService Service)
		{
			lock (services)
			{
				services[Service.Id] = Service;
			}
		}
	}
}
