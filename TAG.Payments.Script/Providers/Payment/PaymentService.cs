using Paiwise;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Payments.Script.Providers.BuyEDaler;
using Waher.Events;
using Waher.Persistence;
using Waher.Runtime.Inventory;
using Waher.Script;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Model;
using Waher.Script.Objects;

namespace TAG.Payments.Script.Providers.Payment
{
	/// <summary>
	/// Represents a service for payments, based on script.
	/// </summary>
	public class PaymentService : IPaymentService
	{
		private readonly Variables variables;

		/// <summary>
		/// Represents a service for payments, based on script.
		/// </summary>
		/// <param name="Id">Identifier of service.</param>
		/// <param name="Name">Name of se+rvice.</param>
		/// <param name="IconUrl">Optional Icon for service.</param>
		/// <param name="IconWidth">Width of icon</param>
		/// <param name="IconHeight">Height of icon</param>
		/// <param name="Supports">Supports method.</param>
		/// <param name="Pay">Payment method.</param>
		/// <param name="Variables">Variables collection.</param>
		public PaymentService(string Id, string Name, string IconUrl, int IconWidth, int IconHeight, ILambdaExpression Supports, 
			ILambdaExpression Pay, Variables Variables)
		{
			if (!(Supports is null) && Supports.NrArguments != 1)
				throw new ArgumentException("Supports method must take one argument.", nameof(Supports));

			if (Pay is null)
				throw new ArgumentNullException(nameof(Payment));

			if (Pay.NrArguments != 1)
				throw new ArgumentException("Payment method must take one argument.", nameof(Payment));

			this.Id = Id;
			this.Name = Name;
			this.IconUrl = IconUrl;
			this.IconWidth = IconWidth;
			this.IconHeight = IconHeight;
			this.SupportsLambda = Supports;
			this.PayLambda = Pay;

			this.variables = new Variables();
			Variables.CopyTo(this.variables);
		}

		/// <summary>
		/// Identifier of service.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Name of se+rvice.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Optional Icon for service.
		/// </summary>
		public string IconUrl { get; }

		/// <summary>
		/// Width of icon
		/// </summary>
		public int IconWidth { get; }

		/// <summary>
		/// Height of icon
		/// </summary>
		public int IconHeight { get; }

		/// <summary>
		/// Supports method.
		/// </summary>
		public ILambdaExpression SupportsLambda { get; }

		/// <summary>
		/// Payment method.
		/// </summary>
		public ILambdaExpression PayLambda { get; }

		/// <summary>
		/// Reference to the service provider.
		/// </summary>
		public IPaymentServiceProvider PaymentServiceProvider => ScriptProviders.paymentServiceProvider;

		/// <summary>
		/// If a given currency is supported.
		/// </summary>
		/// <param name="Currency">Currency</param>
		/// <returns>How well the currency is supported.</returns>
		public Grade Supports(CaseInsensitiveString Currency)
		{
			if (this.SupportsLambda is null)
				return Grade.Ok;
			else
			{
				try
				{
					IElement E = this.SupportsLambda.Evaluate(new IElement[] { new StringValue(Currency.Value.ToUpper()) }, this.variables);
					object Result = E.AssociatedObjectValue;

					if (Result is Grade G)
						return G;
					else if (Result is bool b)
						return b ? Grade.Ok : Grade.NotAtAll;
					else if (Result is string s && Enum.TryParse(s, out G))
						return G;
					else
					{
						Log.Error("Cannot evaluate support grade. Result: " + (Result?.ToString() ?? "null"), this.Id);
						return Grade.NotAtAll;
					}
				}
				catch (Exception ex)
				{
					Log.Critical(ex);
					return Grade.NotAtAll;
				}
			}
		}

		/// <summary>
		/// Called when a user wants to buy eDaler using the service.
		/// </summary>
		/// <param name="Amount">Amount</param>
		/// <param name="Currency">Currency</param>
		/// <param name="Description">Description of payment.</param>
		/// <param name="SuccessUrl">Callback URL if successful.</param>
		/// <param name="FailureUrl">Callback URL if failed</param>
		/// <param name="CancelUrl">Callback URL if cancelled</param>
		/// <param name="ClientUrlCallback">Method to call if URL is needed to be sent to client.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		/// <returns>Payment result.</returns>
		public async Task<PaymentResult> Pay(decimal Amount, string Currency, string Description, string SuccessUrl, string FailureUrl, 
			string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			try
			{
				PaymentRequest Request = new PaymentRequest(new Dictionary<CaseInsensitiveString, object>(),
					new Dictionary<CaseInsensitiveString, CaseInsensitiveString>(), Amount, Currency, SuccessUrl, FailureUrl, CancelUrl, 
					ClientUrlCallback, State);
				IElement E = await this.PayLambda.EvaluateAsync(new IElement[] { new ObjectValue(Request) }, this.variables);
				object Result = E.AssociatedObjectValue;

				if (Result is decimal d)
					return new PaymentResult(d, Currency);
				else if (Result is double d2)
					return new PaymentResult((decimal)d2, Currency);
				else if (Result is PhysicalQuantity Q)
					return new PaymentResult((decimal)Q.Magnitude, Q.Unit.ToString());
				else if (Result is StringValue s)
					return new PaymentResult(s.Value);
				else
				{
					Log.Error("Cannot evaluate script for payment. Result: " + (Result?.ToString() ?? "null"), this.Id);
					return new PaymentResult("Unexpected result received.");
				}
			}
			catch (Exception ex)
			{
				Log.Critical(ex);
				return new PaymentResult(ex.Message);
			}
		}
	}
}
