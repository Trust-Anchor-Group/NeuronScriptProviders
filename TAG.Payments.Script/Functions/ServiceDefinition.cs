using System.Collections.Generic;
using Waher.Script;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Exceptions;
using Waher.Script.Model;

namespace TAG.Payments.Script.Functions
{
	/// <summary>
	/// Abstract base class for service definition functions.
	/// </summary>
	public abstract class ServiceDefinition : FunctionOneVariable
	{
		/// <summary>
		/// Abstract base class for service definition functions.
		/// </summary>
		/// <param name="Argument">Script definition.</param>
		/// <param name="Start">Start position in expression.</param>
		/// <param name="Length">Length of subexpression.</param>
		/// <param name="Expression">Expression containing function.</param>
		public ServiceDefinition(ScriptNode Argument, int Start, int Length, Expression Expression)
			: base(Argument, Start, Length, Expression)
		{
		}

		/// <summary>
		/// Default argument names.
		/// </summary>
		public override string[] DefaultArgumentNames => new string[] { "Definition" };

		/// <summary>
		/// Evaluates function
		/// </summary>
		/// <param name="Argument">Argument</param>
		/// <param name="Variables">Variables</param>
		/// <returns>Result</returns>
		public override sealed IElement Evaluate(IElement Argument, Variables Variables)
		{
			if (!(Argument.AssociatedObjectValue is Dictionary<string, IElement> Definition))
				throw new ScriptRuntimeException("Expected an object script definition.", this);

			return this.Evaluate(Definition, Variables);
		}

		/// <summary>
		/// Evaluates function
		/// </summary>
		/// <param name="Argument">Argument</param>
		/// <param name="Variables">Variables</param>
		/// <returns>Result</returns>
		public abstract IElement Evaluate(Dictionary<string, IElement> Definition, Variables Variables);
	}
}
