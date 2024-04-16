using Paiwise;
using System.Collections.Generic;
using System.Threading.Tasks;
using Waher.Persistence;

namespace TAG.Payments.Script.Providers.Payment
{
	/// <summary>
	/// Publishes service provider integrations made in script.
	/// </summary>
	public class PaymentServiceProvider : IPaymentServiceProvider
	{
		private static readonly Dictionary<string, IPaymentService> services = new Dictionary<string, IPaymentService>();

		/// <summary>
		/// Publishes service provider integrations made in script.
		/// </summary>
		public PaymentServiceProvider()
		{
		}

		/// <summary>
		/// ID of service provider.
		/// </summary>
		public string Id => typeof(PaymentServiceProvider).FullName;

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
		/// Gets available services supporting buying eDaler for a given currency and country.
		/// </summary>
		/// <param name="Currency">Currency</param>
		/// <param name="Country">Country</param>
		/// <returns>Array of services.</returns>
		public Task<IPaymentService[]> GetServicesForPayment(CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			lock (services)
			{
				IPaymentService[] Result = new IPaymentService[services.Count];
				services.Values.CopyTo(Result, 0);
				return Task.FromResult(Result);
			}
		}

		/// <summary>
		/// Gets a service for buying eDaler, given its ID, currency and country.
		/// </summary>
		/// <param name="ServiceId">Service ID</param>
		/// <param name="Currency">Currency</param>
		/// <param name="Country">Country</param>
		/// <returns>Reference to service, if found.</returns>
		public Task<IPaymentService> GetServiceForPayment(string ServiceId, CaseInsensitiveString Currency, CaseInsensitiveString Country)
		{
			lock (services)
			{
				if (services.TryGetValue(ServiceId, out IPaymentService Service))
					return Task.FromResult(Service);
			}

			return Task.FromResult<IPaymentService>(null);
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
		internal static void Register(IPaymentService Service)
		{
			lock (services)
			{
				services[Service.Id] = Service;
			}
		}
	}
}
