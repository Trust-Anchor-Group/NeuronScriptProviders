using System.Threading.Tasks;
using TAG.Payments.Script.Providers.BuyEDaler;
using TAG.Payments.Script.Providers.Payment;
using TAG.Payments.Script.Providers.SellEDaler;
using Waher.Runtime.Inventory;

namespace TAG.Payments.Script
{
	/// <summary>
	/// Script Provider module.
	/// </summary>
	public class ScriptProviders : IModule
	{
		internal readonly static BuyEDalerServiceProvider buyEDalerServiceProvider = new BuyEDalerServiceProvider();
		internal readonly static SellEDalerServiceProvider sellEDalerServiceProvider = new SellEDalerServiceProvider();
		internal readonly static PaymentServiceProvider paymentServiceProvider = new PaymentServiceProvider();

		/// <summary>
		/// Script Provider module.
		/// </summary>
		public ScriptProviders()
		{
		}

		/// <summary>
		/// Called when module is started.
		/// </summary>
		public Task Start()
		{
			this.Clear();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Called when module is stopped.
		/// </summary>
		public Task Stop()
		{
			this.Clear();
			return Task.CompletedTask;
		}

		private void Clear()
		{
			BuyEDalerServiceProvider.Clear();
			SellEDalerServiceProvider.Clear();
			PaymentServiceProvider.Clear();
		}
	}
}
