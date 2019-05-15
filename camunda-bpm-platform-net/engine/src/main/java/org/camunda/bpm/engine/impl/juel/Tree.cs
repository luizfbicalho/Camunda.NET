using System.Collections.Generic;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */ 
namespace org.camunda.bpm.engine.impl.juel
{

	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;


	/// <summary>
	/// Parsed expression, usually created by a <seealso cref="org.camunda.bpm.engine.impl.juel.TreeBuilder"/>.
	/// The <seealso cref="#bind(FunctionMapper, VariableMapper)"/> method is used to create
	/// <seealso cref="org.camunda.bpm.engine.impl.juel.Bindings"/>, which are needed at evaluation time to
	/// lookup functions and variables. The tree itself does not contain such information,
	/// because it would make the tree depend on the function/variable mapper supplied at
	/// parse time.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class Tree
	{
		private readonly ExpressionNode root;
		private readonly ICollection<FunctionNode> functions;
		private readonly ICollection<IdentifierNode> identifiers;
		private readonly bool deferred;

		/// 
		/// <summary>
		/// Constructor. </summary>
		/// <param name="root"> root node </param>
		/// <param name="functions"> collection of function nodes </param>
		/// <param name="identifiers"> collection of identifier nodes </param>
		public Tree(ExpressionNode root, ICollection<FunctionNode> functions, ICollection<IdentifierNode> identifiers, bool deferred) : base()
		{
			this.root = root;
			this.functions = functions;
			this.identifiers = identifiers;
			this.deferred = deferred;
		}

		/// <summary>
		/// Get function nodes (in no particular order)
		/// </summary>
		public virtual IEnumerable<FunctionNode> FunctionNodes
		{
			get
			{
				return functions;
			}
		}

		/// <summary>
		/// Get identifier nodes (in no particular order)
		/// </summary>
		public virtual IEnumerable<IdentifierNode> IdentifierNodes
		{
			get
			{
				return identifiers;
			}
		}

		/// <returns> root node </returns>
		public virtual ExpressionNode Root
		{
			get
			{
				return root;
			}
		}

		public virtual bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		public override string ToString()
		{
			return Root.getStructuralId(null);
		}

		/// <summary>
		/// Create a bindings. </summary>
		/// <param name="fnMapper"> the function mapper to use </param>
		/// <param name="varMapper"> the variable mapper to use </param>
		/// <returns> tree bindings </returns>
		public virtual Bindings bind(FunctionMapper fnMapper, VariableMapper varMapper)
		{
			return bind(fnMapper, varMapper, null);
		}

		/// <summary>
		/// Create a bindings. </summary>
		/// <param name="fnMapper"> the function mapper to use </param>
		/// <param name="varMapper"> the variable mapper to use </param>
		/// <param name="converter"> custom type converter </param>
		/// <returns> tree bindings </returns>
		public virtual Bindings bind(FunctionMapper fnMapper, VariableMapper varMapper, TypeConverter converter)
		{
			System.Reflection.MethodInfo[] methods = null;
			if (functions.Count > 0)
			{
				if (fnMapper == null)
				{
					throw new ELException(LocalMessages.get("error.function.nomapper"));
				}
				methods = new System.Reflection.MethodInfo[functions.Count];
				foreach (FunctionNode node in functions)
				{
					string image = node.Name;
					System.Reflection.MethodInfo method = null;
					int colon = image.IndexOf(':');
					if (colon < 0)
					{
						method = fnMapper.resolveFunction("", image);
					}
					else
					{
						method = fnMapper.resolveFunction(image.Substring(0, colon), image.Substring(colon + 1));
					}
					if (method == null)
					{
						throw new ELException(LocalMessages.get("error.function.notfound", image));
					}
					if (node.VarArgs && method.VarArgs)
					{
						if (method.ParameterTypes.length > node.ParamCount + 1)
						{
							throw new ELException(LocalMessages.get("error.function.params", image));
						}
					}
					else
					{
						if (method.ParameterTypes.length != node.ParamCount)
						{
							throw new ELException(LocalMessages.get("error.function.params", image));
						}
					}
					methods[node.Index] = method;
				}
			}
			ValueExpression[] expressions = null;
			if (identifiers.Count > 0)
			{
				expressions = new ValueExpression[identifiers.Count];
				foreach (IdentifierNode node in identifiers)
				{
					ValueExpression expression = null;
					if (varMapper != null)
					{
						expression = varMapper.resolveVariable(node.Name);
					}
					expressions[node.Index] = expression;
				}
			}
			return new Bindings(methods, expressions, converter);
		}
	}

}