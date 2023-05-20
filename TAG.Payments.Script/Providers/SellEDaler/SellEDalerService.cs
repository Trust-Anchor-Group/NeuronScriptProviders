using Paiwise;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAG.Payments.Script.Providers.BuyEDaler;
using Waher.Content;
using Waher.Events;
using Waher.Persistence;
using Waher.Runtime.Inventory;
using Waher.Script;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Model;
using Waher.Script.Objects;

namespace TAG.Payments.Script.Providers.SellEDaler
{
	/// <summary>
	/// Represents a service for selling eDaler, based on script.
	/// </summary>
	public class SellEDalerService : ISellEDalerService
	{
		private readonly Variables variables;

		/// <summary>
		/// Represents a service for selling eDaler, based on script.
		/// </summary>
		/// <param name="Id">Identifier of service.</param>
		/// <param name="Name">Name of se+rvice.</param>
		/// <param name="IconUrl">Optional Icon for service.</param>
		/// <param name="IconWidth">Width of icon</param>
		/// <param name="IconHeight">Height of icon</param>
		/// <param name="TemplateContractId">Lambda expression (or null) for defining contract ID template.</param>
		/// <param name="Supports">Supports method.</param>
		/// <param name="CanSellEDaler">CanSellEDaler method.</param>
		/// <param name="GetOptions">GetOptions method.</param>
		/// <param name="Variables">Variables collection.</param>
		public SellEDalerService(string Id, string Name, string IconUrl, int IconWidth, int IconHeight, string TemplateContractId,
			ILambdaExpression Supports, ILambdaExpression CanSellEDaler, ILambdaExpression GetOptions, ILambdaExpression SellEDaler,
			Variables Variables)
		{
			if (!(Supports is null) && Supports.NrArguments != 1)
				throw new ArgumentException("Supports method must take one argument.", nameof(Supports));

			if (!(CanSellEDaler is null) && CanSellEDaler.NrArguments != 1)
				throw new ArgumentException("CanSellEDaler method must take one argument.", nameof(CanSellEDaler));

			if (!(GetOptions is null) && GetOptions.NrArguments != 1)
				throw new ArgumentException("GetOptions method must take one argument.", nameof(GetOptions));

			if (SellEDaler is null)
				throw new ArgumentNullException(nameof(SellEDaler));

			if (SellEDaler.NrArguments != 1)
				throw new ArgumentException("SellEDaler method must take one argument.", nameof(SellEDaler));

			this.Id = Id;
			this.Name = Name;
			this.IconUrl = IconUrl;
			this.IconWidth = IconWidth;
			this.IconHeight = IconHeight;
			this.SellEDalerTemplateContractId = TemplateContractId;
			this.SupportsLambda = Supports;
			this.CanSellEDalerLambda = CanSellEDaler;
			this.GetOptionsLambda = GetOptions;
			this.SellEDalerLambda = SellEDaler;

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
		/// Optional template contract ID for selling eDaler.
		/// </summary>
		public string SellEDalerTemplateContractId { get; }

		/// <summary>
		/// Supports method.
		/// </summary>
		public ILambdaExpression SupportsLambda { get; }

		/// <summary>
		/// CanSellEDaler method.
		/// </summary>
		public ILambdaExpression CanSellEDalerLambda { get; }

		/// <summary>
		/// GetOptions method.
		/// </summary>
		public ILambdaExpression GetOptionsLambda { get; }

		/// <summary>
		/// SellEDaler method.
		/// </summary>
		public ILambdaExpression SellEDalerLambda { get; }

		/// <summary>
		/// Reference to the service provider.
		/// </summary>
		public ISellEDalerServiceProvider SellEDalerServiceProvider => ScriptProviders.sellEDalerServiceProvider;

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
		/// If a given account can sell eDaler.
		/// </summary>
		/// <param name="AccountName">Name of account.</param>
		/// <returns>If the account is allowed to sell eDaler.</returns>
		public async Task<bool> CanSellEDaler(CaseInsensitiveString AccountName)
		{
			if (this.SupportsLambda is null)
				return true;
			else
			{
				try
				{
					IElement E = await this.CanSellEDalerLambda.EvaluateAsync(new IElement[] { new StringValue(AccountName.Value) }, this.variables);
					object Result = E.AssociatedObjectValue;

					if (Result is bool b)
						return b;
					else if (Result is Grade G)
						return G != Grade.NotAtAll;
					else if (Result is string s && CommonTypes.TryParse(s, out b))
						return b;
					else
					{
						Log.Error("Cannot evaluate if an account (" + AccountName.Value + ") is allowed to sell eDaler. Result: " + (Result?.ToString() ?? "null"), this.Id);
						return false;
					}
				}
				catch (Exception ex)
				{
					Log.Critical(ex);
					return false;
				}
			}
		}

		/// <summary>
		/// Called when a user requests to receive payment options available for selling eDaler.
		/// </summary>
		/// <param name="IdentityProperties">Identity properties.</param>
		/// <param name="SuccessUrl">Callback URL if successful.</param>
		/// <param name="FailureUrl">Callback URL if failed</param>
		/// <param name="CancelUrl">Callback URL if cancelled</param>
		/// <param name="ClientUrlCallback">Method to call if URL is needed to be sent to client.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		/// <returns>Array of available options.</returns>
		public async Task<IDictionary<CaseInsensitiveString, object>[]> GetPaymentOptionsForSellingEDaler(
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties, string SuccessUrl,
			string FailureUrl, string CancelUrl, ClientUrlEventHandler ClientUrlCallback, object State)
		{
			if (this.GetOptionsLambda is null)
				return new IDictionary<CaseInsensitiveString, object>[0];
			else
			{
				try
				{
					OptionsRequest Request = new OptionsRequest(IdentityProperties, SuccessUrl, FailureUrl, CancelUrl,
						ClientUrlCallback, State);
					IElement E = await this.GetOptionsLambda.EvaluateAsync(new IElement[] { new ObjectValue(Request) }, this.variables);

					return GetOptions(E.AssociatedObjectValue);
				}
				catch (Exception ex)
				{
					Log.Critical(ex);
					return new IDictionary<CaseInsensitiveString, object>[0];
				}
			}
		}

		/// <summary>
		/// Extracts options from a script result.
		/// </summary>
		/// <param name="Result">Script result.</param>
		/// <returns>Options found.</returns>
		/// <exception cref="Exception">If unable to extract options.</exception>
		internal static IDictionary<CaseInsensitiveString, object>[] GetOptions(object Result)
		{
			if (Result is IDictionary<string, IElement> Obj)
			{
				IDictionary<CaseInsensitiveString, object> Result2 = new Dictionary<CaseInsensitiveString, object>();

				foreach (KeyValuePair<string, IElement> P in Obj)
					Result2[P.Key] = P.Value.AssociatedObjectValue;

				return new IDictionary<CaseInsensitiveString, object>[] { Result2 };
			}
			else if (Result is IDictionary<string, object> Obj2)
			{
				IDictionary<CaseInsensitiveString, object> Result2 = new Dictionary<CaseInsensitiveString, object>();

				foreach (KeyValuePair<string, object> P in Obj2)
					Result2[P.Key] = P.Value;

				return new IDictionary<CaseInsensitiveString, object>[] { Result2 };
			}
			else if (Result is IEnumerable Array)
			{
				List<IDictionary<CaseInsensitiveString, object>> Result2 = new List<IDictionary<CaseInsensitiveString, object>>();
				IEnumerator e = Array.GetEnumerator();

				while (e.MoveNext())
					Result2.AddRange(GetOptions(e.Current));

				return Result2.ToArray();
			}
			else
				throw new Exception("Unable to parse option from object of type " + (Result?.GetType().FullName ?? "null"));
		}

		/// <summary>
		/// Called when a user wants to sell eDaler using the service.
		/// </summary>
		/// <param name="ContractParameters">Contract Parameters.</param>
		/// <param name="IdentityProperties">Identity Properties</param>
		/// <param name="Amount">Amount</param>
		/// <param name="Currency">Currency</param>
		/// <param name="SuccessUrl">Callback URL if successful.</param>
		/// <param name="FailureUrl">Callback URL if failed</param>
		/// <param name="CancelUrl">Callback URL if cancelled</param>
		/// <param name="ClientUrlCallback">Method to call if URL is needed to be sent to client.</param>
		/// <param name="State">State object to pass on to callback method.</param>
		/// <returns>Payment result.</returns>
		public async Task<PaymentResult> SellEDaler(IDictionary<CaseInsensitiveString, object> ContractParameters,
			IDictionary<CaseInsensitiveString, CaseInsensitiveString> IdentityProperties,
			decimal Amount, string Currency, string SuccessUrl, string FailureUrl, string CancelUrl,
			ClientUrlEventHandler ClientUrlCallback, object State)
		{
			try
			{
				PaymentRequest Request = new PaymentRequest(ContractParameters, IdentityProperties, Amount, Currency,
					SuccessUrl, FailureUrl, CancelUrl, ClientUrlCallback, State);
				IElement E = await this.SellEDalerLambda.EvaluateAsync(new IElement[] { new ObjectValue(Request) }, this.variables);
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
					Log.Error("Cannot evaluate script for selling of eDaler. Result: " + (Result?.ToString() ?? "null"), this.Id);
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
