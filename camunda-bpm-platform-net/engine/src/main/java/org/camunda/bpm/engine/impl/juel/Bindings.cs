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

	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;


	/// <summary>
	/// Bindings, usually created by a <seealso cref="org.camunda.bpm.engine.impl.juel.Tree"/>.
	/// 
	/// @author Christoph Beck
	/// </summary>
	[Serializable]
	public class Bindings : TypeConverter
	{
		private const long serialVersionUID = 1L;

		private static readonly System.Reflection.MethodInfo[] NO_FUNCTIONS = new System.Reflection.MethodInfo[0];
		private static readonly ValueExpression[] NO_VARIABLES = new ValueExpression[0];

		/// <summary>
		/// Wrap a <seealso cref="Method"/> for serialization.
		/// </summary>
		[Serializable]
		private class MethodWrapper
		{
			internal const long serialVersionUID = 1L;

			[NonSerialized]
			internal System.Reflection.MethodInfo method;
			internal MethodWrapper(System.Reflection.MethodInfo method)
			{
				this.method = method;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException, ClassNotFoundException
			internal virtual void writeObject(ObjectOutputStream @out)
			{
				@out.defaultWriteObject();
				@out.writeObject(method.DeclaringClass);
				@out.writeObject(method.Name);
				@out.writeObject(method.ParameterTypes);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
			internal virtual void readObject(ObjectInputStream @in)
			{
				@in.defaultReadObject();
				Type type = (Type)@in.readObject();
				string name = (string)@in.readObject();
				Type[] args = (Type[])@in.readObject();
				try
				{
					method = type.getDeclaredMethod(name, args);
				}
				catch (NoSuchMethodException e)
				{
					throw new IOException(e.Message);
				}
			}
		}

		[NonSerialized]
		private System.Reflection.MethodInfo[] functions;
		private readonly ValueExpression[] variables;
		private readonly TypeConverter converter;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Bindings(System.Reflection.MethodInfo[] functions, ValueExpression[] variables) : this(functions, variables, TypeConverter_Fields.DEFAULT)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Bindings(System.Reflection.MethodInfo[] functions, ValueExpression[] variables, TypeConverter converter) : base()
		{

			this.functions = functions == null || functions.Length == 0 ? NO_FUNCTIONS : functions;
			this.variables = variables == null || variables.Length == 0 ? NO_VARIABLES : variables;
			this.converter = converter == null ? TypeConverter_Fields.DEFAULT : converter;
		}

		/// <summary>
		/// Get function by index. </summary>
		/// <param name="index"> function index </param>
		/// <returns> method </returns>
		public virtual System.Reflection.MethodInfo getFunction(int index)
		{
			return functions[index];
		}

		/// <summary>
		/// Test if given index is bound to a function.
		/// This method performs an index check. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> <code>true</code> if the given index is bound to a function </returns>
		public virtual bool isFunctionBound(int index)
		{
			return index >= 0 && index < functions.Length;
		}

		/// <summary>
		/// Get variable by index. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> value expression </returns>
		public virtual ValueExpression getVariable(int index)
		{
			return variables[index];
		}

		/// <summary>
		/// Test if given index is bound to a variable.
		/// This method performs an index check. </summary>
		/// <param name="index"> identifier index </param>
		/// <returns> <code>true</code> if the given index is bound to a variable </returns>
		public virtual bool isVariableBound(int index)
		{
			return index >= 0 && index < variables.Length && variables[index] != null;
		}

		/// <summary>
		/// Apply type conversion. </summary>
		/// <param name="value"> value to convert </param>
		/// <param name="type"> target type </param>
		/// <returns> converted value </returns>
		/// <exception cref="ELException"> </exception>
		public virtual T convert<T>(object value, Type<T> type)
		{
			return converter.convert(value, type);
		}

		public override bool Equals(object obj)
		{
			if (obj is Bindings)
			{
				Bindings other = (Bindings)obj;
				return Arrays.Equals(functions, other.functions) && Arrays.Equals(variables, other.variables) && converter.Equals(other.converter);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Arrays.GetHashCode(functions) ^ Arrays.GetHashCode(variables) ^ converter.GetHashCode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException, ClassNotFoundException
		private void writeObject(ObjectOutputStream @out)
		{
			@out.defaultWriteObject();
			MethodWrapper[] wrappers = new MethodWrapper[functions.Length];
			for (int i = 0; i < wrappers.Length; i++)
			{
				wrappers[i] = new MethodWrapper(functions[i]);
			}
			@out.writeObject(wrappers);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void readObject(ObjectInputStream @in)
		{
			@in.defaultReadObject();
			MethodWrapper[] wrappers = (MethodWrapper[])@in.readObject();
			if (wrappers.Length == 0)
			{
				functions = NO_FUNCTIONS;
			}
			else
			{
				functions = new System.Reflection.MethodInfo[wrappers.Length];
				for (int i = 0; i < functions.Length; i++)
				{
					functions[i] = wrappers[i].method;
				}
			}
		}
	}

}