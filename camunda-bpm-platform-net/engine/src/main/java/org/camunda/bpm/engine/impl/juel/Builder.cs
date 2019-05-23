using System;

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
	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;
	using ParseException = org.camunda.bpm.engine.impl.juel.Parser.ParseException;
	using ScanException = org.camunda.bpm.engine.impl.juel.Scanner.ScanException;


	/// <summary>
	/// Tree builder.
	/// 
	/// @author Christoph Beck
	/// </summary>
	[Serializable]
	public class Builder : TreeBuilder
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Feature enumeration type.
		/// </summary>
		public enum Feature
		{
			/// <summary>
			/// Method invocations as in <code>${foo.bar(1)}</code> as specified in JSR 245,
			/// maintenance release 2.
			/// The method to be invoked is resolved at evaluation time by calling
			/// <seealso cref="ELResolver.invoke(javax.el.ELContext, object, object, Class[], Object[])"/>.
			/// </summary>
			METHOD_INVOCATIONS,
			/// <summary>
			/// For some reason we don't understand, the specification does not allow to resolve
			/// <code>null</code> property values. E.g. <code>${map[key]}</code> will always
			/// return <code>null</code> if <code>key</code> evaluates to <code>null</code>.
			/// Enabling this feature will allow <em>JUEL</em> to pass <code>null</code> to
			/// the property resolvers just like any other property value.
			/// </summary>
			NULL_PROPERTIES,
			/// <summary>
			/// Allow for use of Java 5 varargs in function calls.
			/// </summary>
			VARARGS
		}

		protected internal readonly EnumSet<Feature> features;

		public Builder()
		{
			this.features = EnumSet.noneOf(typeof(Feature));
		}

		public Builder(params Feature[] features)
		{
			if (features == null || features.Length == 0)
			{
				this.features = EnumSet.noneOf(typeof(Feature));
			}
			else if (features.Length == 1)
			{
				this.features = EnumSet.of(features[0]);
			}
			else
			{
				Feature[] rest = new Feature[features.Length - 1];
				for (int i = 1; i < features.Length; i++)
				{
					rest[i - 1] = features[i];
				}
				this.features = EnumSet.of(features[0], rest);
			}
		}

		/// <returns> <code>true</code> iff the specified feature is supported. </returns>
		public virtual bool isEnabled(Feature feature)
		{
			return features.contains(feature);
		}

		/// <summary>
		/// Parse expression.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tree build(String expression) throws TreeBuilderException
		public virtual Tree build(string expression)
		{
			try
			{
				return createParser(expression).tree();
			}
			catch (ScanException e)
			{
				throw new TreeBuilderException(expression, e.position, e.encountered, e.expected, e.Message);
			}
			catch (ParseException e)
			{
				throw new TreeBuilderException(expression, e.position, e.encountered, e.expected, e.Message);
			}
		}

		protected internal virtual Parser createParser(string expression)
		{
			return new Parser(this, expression);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			return features.Equals(((Builder)obj).features);
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		/// <summary>
		/// Dump out abstract syntax tree for a given expression
		/// </summary>
		/// <param name="args"> array with one element, containing the expression string </param>
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.Error.WriteLine("usage: java " + typeof(Builder).FullName + " <expression string>");
				Environment.Exit(1);
			}
			PrintWriter @out = new PrintWriter(System.out);
			Tree tree = null;
			try
			{
				tree = (new Builder(Feature.METHOD_INVOCATIONS)).build(args[0]);
			}
			catch (TreeBuilderException e)
			{
				Console.WriteLine(e.Message);
				Environment.Exit(0);
			}
			NodePrinter.dump(@out, tree.Root);
			if (!tree.FunctionNodes.GetEnumerator().hasNext() && !tree.IdentifierNodes.GetEnumerator().hasNext())
			{
				ELContext context = new ELContextAnonymousInnerClass();
				@out.print(">> ");
				try
				{
					@out.println(tree.Root.getValue(new Bindings(null, null), context, null));
				}
				catch (ELException e)
				{
					@out.println(e.Message);
				}
			}
			@out.flush();
		}

		private class ELContextAnonymousInnerClass : ELContext
		{
			public override VariableMapper VariableMapper
			{
				get
				{
					return null;
				}
			}
			public override FunctionMapper FunctionMapper
			{
				get
				{
					return null;
				}
			}
			public override ELResolver ELResolver
			{
				get
				{
					return null;
				}
			}
		}
	}

}