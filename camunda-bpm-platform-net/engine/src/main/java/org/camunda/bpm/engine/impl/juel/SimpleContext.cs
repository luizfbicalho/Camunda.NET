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

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;

	/// <summary>
	/// Simple context implementation.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class SimpleContext : ELContext
	{
		internal class Functions : FunctionMapper
		{
			internal IDictionary<string, System.Reflection.MethodInfo> map = Collections.emptyMap();

			public override System.Reflection.MethodInfo resolveFunction(string prefix, string localName)
			{
				return map[prefix + ":" + localName];
			}

			public virtual void setFunction(string prefix, string localName, System.Reflection.MethodInfo method)
			{
				if (map.Count == 0)
				{
					map = new Dictionary<string, System.Reflection.MethodInfo>();
				}
				map[prefix + ":" + localName] = method;
			}
		}

		internal class Variables : VariableMapper
		{
			internal IDictionary<string, ValueExpression> map = Collections.emptyMap();

			public override ValueExpression resolveVariable(string variable)
			{
				return map[variable];
			}

			public override ValueExpression setVariable(string variable, ValueExpression expression)
			{
				if (map.Count == 0)
				{
					map = new Dictionary<string, ValueExpression>();
				}
				return map[variable] = expression;
			}
		}

		private Functions functions;
		private Variables variables;
		private ELResolver resolver;

		/// <summary>
		/// Create a context.
		/// </summary>
		public SimpleContext() : this(null)
		{
		}

		/// <summary>
		/// Create a context, use the specified resolver.
		/// </summary>
		public SimpleContext(ELResolver resolver)
		{
			this.resolver = resolver;
		}

		/// <summary>
		/// Define a function.
		/// </summary>
		public virtual void setFunction(string prefix, string localName, System.Reflection.MethodInfo method)
		{
			if (functions == null)
			{
				functions = new Functions();
			}
			functions.setFunction(prefix, localName, method);
		}

		/// <summary>
		/// Define a variable.
		/// </summary>
		public virtual ValueExpression setVariable(string name, ValueExpression expression)
		{
			if (variables == null)
			{
				variables = new Variables();
			}
			return variables.setVariable(name, expression);
		}

		/// <summary>
		/// Get our function mapper.
		/// </summary>
		public override FunctionMapper FunctionMapper
		{
			get
			{
				if (functions == null)
				{
					functions = new Functions();
				}
				return functions;
			}
		}

		/// <summary>
		/// Get our variable mapper.
		/// </summary>
		public override VariableMapper VariableMapper
		{
			get
			{
				if (variables == null)
				{
					variables = new Variables();
				}
				return variables;
			}
		}

		/// <summary>
		/// Get our resolver. Lazy initialize to a <seealso cref="SimpleResolver"/> if necessary.
		/// </summary>
		public override ELResolver ELResolver
		{
			get
			{
				if (resolver == null)
				{
					resolver = new SimpleResolver();
				}
				return resolver;
			}
			set
			{
				this.resolver = value;
			}
		}

	}

}