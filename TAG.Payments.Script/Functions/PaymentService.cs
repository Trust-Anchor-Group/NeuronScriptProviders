using Paiwise;
using SkiaSharp;
using System;
using System.Collections.Generic;
using Waher.Script;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Exceptions;
using Waher.Script.Graphs;
using Waher.Script.Model;
using Waher.Script.Objects;
using Waher.Script.Operators;

namespace TAG.Payments.Script.Functions
{
	/// <summary>
	/// Defines a service for payments.
	/// </summary>
	public class PaymentService : ServiceDefinition
	{
		/// <summary>
		/// Defines a service for payments.
		/// </summary>
		/// <param name="Argument">Script definition.</param>
		/// <param name="Start">Start position in expression.</param>
		/// <param name="Length">Length of subexpression.</param>
		/// <param name="Expression">Expression containing function.</param>
		public PaymentService(ScriptNode Argument, int Start, int Length, Expression Expression)
			: base(Argument, Start, Length, Expression)
		{
		}

		/// <summary>
		/// Name of function.
		/// </summary>
		public override string FunctionName => nameof(PaymentService);

		/// <summary>
		/// Evaluates function
		/// </summary>
		/// <param name="Argument">Argument</param>
		/// <param name="Variables">Variables</param>
		/// <returns>Result</returns>
		public override IElement Evaluate(Dictionary<string, IElement> Definition, Variables Variables)
		{
			string Id = string.Empty;
			string Name = string.Empty;
			string IconUrl = string.Empty;
			int IconWidth = 0;
			int IconHeight = 0;
			ILambdaExpression Supports = null;
			ILambdaExpression Payment = null;

			foreach (KeyValuePair<string, IElement> P in Definition)
			{
				switch (P.Key.ToLower())
				{
					case "id":
						Id = P.Value.AssociatedObjectValue?.ToString() ?? string.Empty;
						break;

					case "name":
						Name = P.Value.AssociatedObjectValue?.ToString() ?? string.Empty;
						break;

					case "iconurl":
						IconUrl = P.Value.AssociatedObjectValue?.ToString() ?? string.Empty;
						break;

					case "iconwidth":
						IconWidth = (int)Expression.ToDouble(P.Value.AssociatedObjectValue);
						if (IconWidth < 0 || IconWidth > 2048)
							throw new ScriptRuntimeException("Invalid icon width.", this);
						break;

					case "iconheight":
						IconHeight = (int)Expression.ToDouble(P.Value.AssociatedObjectValue);
						if (IconHeight < 0 || IconHeight > 2048)
							throw new ScriptRuntimeException("Invalid icon height.", this);
						break;

					case "icon":
						if (P.Value is SKImage Image)
						{
							using (SKData Data = Image.Encode(SKEncodedImageFormat.Png, 100))
							{
								IconUrl = "data:image/png;base64," + Convert.ToBase64String(Data.ToArray());
								IconWidth = Image.Width;
								IconHeight = Image.Height;
							}
						}
						else if (P.Value is Graph Graph)
						{
							PixelInformation Pixels = Graph.CreatePixels(Variables);

							IconUrl = "data:image/png;base64," + Convert.ToBase64String(Pixels.EncodeAsPng());
							IconWidth = Pixels.Width;
							IconHeight = Pixels.Height;
						}
						else if (P.Value is PixelInformation Pixels)
						{
							IconUrl = "data:image/png;base64," + Convert.ToBase64String(Pixels.EncodeAsPng());
							IconWidth = Pixels.Width;
							IconHeight = Pixels.Height;
						}
						else
							throw new ScriptRuntimeException("Invalid icon.", this);
						break;

					case "support":
					case "supports":
						if (P.Value is ILambdaExpression L)
						{
							if (L.NrArguments != 1)
								throw new ScriptRuntimeException("Support method must only have one argument.", this);
						}
						else
						{
							L = new LambdaDefinition(new string[] { "Currency" }, new ArgumentType[] { ArgumentType.Scalar },
								new ConstantElement(P.Value, this.Start, this.Length, this.Expression),
								this.Start, this.Length, this.Expression);
						}
						Supports = L;
						break;

					case "payment":
					case "pay":
						if (P.Value is ILambdaExpression L4)
						{
							if (L4.NrArguments != 1)
								throw new ScriptRuntimeException("Payment method must only have one argument.", this);
						}
						else
							throw new ScriptRuntimeException("Payment method must be a lambda expression.", this);

						Payment = L4;
						break;

					default:
						throw new ScriptRuntimeException("Unrecognized property: " + P.Key, this);
				}
			}

			if (string.IsNullOrEmpty(Id))
				throw new ScriptRuntimeException("Id property missing.", this);

			if (string.IsNullOrEmpty(Name))
				throw new ScriptRuntimeException("Name property missing.", this);

			if (Payment is null)
				throw new ScriptRuntimeException("Payment method missing.", this);

			IPaymentService Service = new Providers.Payment.PaymentService(Id, Name, IconUrl, IconWidth, IconHeight,
				Supports, Payment, Variables);

			Providers.Payment.PaymentServiceProvider.Register(Service);

			return new ObjectValue(Service);
		}

	}
}
