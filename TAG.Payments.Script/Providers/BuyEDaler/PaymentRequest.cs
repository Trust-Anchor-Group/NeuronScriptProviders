using Paiwise;
using System.Collections.Generic;
using Waher.Events;
using Waher.Persistence;

namespace TAG.Payments.Script.Providers.BuyEDaler
{
	/// <summary>
	/// Class representing a request to buy eDaler.
	/// </summary>
	public class PaymentRequest : OptionsRequest
	{
		/// <summary>
		/// Class representing a request to buy eDaler.
		/// </summary>
		/// <param name="IdentityProperties">Identity properties.</param>
		/// <param name="SuccessUrl">Callback URL if successful.</param>
		/// <param name="FailureUrl">Callback URL if failed</param>
		/// <param name="CancelUrl">Callback URL if cancelled</param>
		/// <param name="ClientUrlCallback">Method to call if URL is needed to be sent to client.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		public PaymentRequest(IDictionary<CaseInsensitiveString, object> ContractParameters,
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties, decimal Amount, string Currency,
			string SuccessUrl, string FailureUrl, string CancelUrl, EventHandlerAsync<ClientUrlEventArgs> ClientUrlCallback, object State)
			: base(IdentityProperties, SuccessUrl, FailureUrl, CancelUrl, ClientUrlCallback, State)
		{
			this.ContractParameters = ContractParameters;
			this.Amount = Amount;
			this.Currency = Currency;
		}

		/// <summary>
		/// Contract properties.
		/// </summary>
		public IDictionary<CaseInsensitiveString, object> ContractParameters { get; }

		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { get; }

		/// <summary>
		/// Currency
		/// </summary>
		public string Currency { get; }
	}
}
