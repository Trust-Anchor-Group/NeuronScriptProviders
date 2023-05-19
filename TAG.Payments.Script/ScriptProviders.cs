using System.Threading.Tasks;
using Waher.Runtime.Inventory;

namespace TAG.Payments.Script
{
	/// <summary>
	/// Script Provider module.
	/// </summary>
	public class ScriptProviders : IModule
	{
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
			return Task.CompletedTask;
		}

		/// <summary>
		/// Called when module is stopped.
		/// </summary>
		public Task Stop()
		{
			return Task.CompletedTask;
		}
	}
}
