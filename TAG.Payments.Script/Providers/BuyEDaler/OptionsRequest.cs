using Paiwise;
using System.Collections.Generic;
using Waher.Events;
using Waher.Persistence;

namespace TAG.Payments.Script.Providers.BuyEDaler
{
	/// <summary>
	/// Class representing a request for options when buying eDaler.
	/// </summary>
	public class OptionsRequest
	{
		/// <summary>
		/// Class representing a request for options when buying eDaler.
		/// </summary>
		/// <param name="IdentityProperties">Identity properties.</param>
		/// <param name="SuccessUrl">Callback URL if successful.</param>
		/// <param name="FailureUrl">Callback URL if failed</param>
		/// <param name="CancelUrl">Callback URL if cancelled</param>
		/// <param name="ClientUrlCallback">Method to call if URL is needed to be sent to client.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		public OptionsRequest(IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties, string SuccessUrl,
			string FailureUrl, string CancelUrl, EventHandlerAsync<ClientUrlEventArgs> ClientUrlCallback, object State)
		{
			this.IdentityProperties = IdentityProperties;
			this.SuccessUrl = SuccessUrl;
			this.FailureUrl = FailureUrl;
			this.CancelUrl = CancelUrl;
			this.ClientUrlCallback = ClientUrlCallback;
			this.State = State;
		}

		/// <summary>
		/// Identity properties.
		/// </summary>
		public IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties { get; }

		/// <summary>
		/// Callback URL if successful.
		/// </summary>
		public string SuccessUrl { get; }

		/// <summary>
		/// Callback URL if failed
		/// </summary>
		public string FailureUrl { get; }

		/// <summary>
		/// Callback URL if cancelled
		/// </summary>
		public string CancelUrl { get; }

		/// <summary>
		/// Method to call if URL is needed to be sent to client.
		/// </summary>
		public EventHandlerAsync<ClientUrlEventArgs> ClientUrlCallback { get; }

		/// <summary>
		/// State object to pass on to callback method.
		/// </summary>
		public object State { get; }


	}
}
