using System;
using System.Text;

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


	public class AstFunction : AstRightValue, FunctionNode
	{
		private readonly int index;
		private readonly string name;
		private readonly AstParameters @params;
		private readonly bool varargs;

		public AstFunction(string name, int index, AstParameters @params) : this(name, index, @params, false)
		{
		}

		public AstFunction(string name, int index, AstParameters @params, bool varargs)
		{
			this.name = name;
			this.index = index;
			this.@params = @params;
			this.varargs = varargs;
		}

		/// <summary>
		/// Invoke method. </summary>
		/// <param name="bindings"> </param>
		/// <param name="context"> </param>
		/// <param name="base"> </param>
		/// <param name="method"> </param>
		/// <returns> method result </returns>
		/// <exception cref="InvocationTargetException"> </exception>
		/// <exception cref="IllegalAccessException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object invoke(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context, Object super, Method method) throws InvocationTargetException, IllegalAccessException
		protected internal virtual object invoke(Bindings bindings, ELContext context, object @base, System.Reflection.MethodInfo method)
		{
			Type[] types = method.ParameterTypes;
			object[] @params = null;
			if (types.Length > 0)
			{
				@params = new object[types.Length];
				if (varargs && method.VarArgs)
				{
					for (int i = 0; i < @params.Length - 1; i++)
					{
						object param = getParam(i).eval(bindings, context);
						if (param != null || types[i].IsPrimitive)
						{
							@params[i] = bindings.convert(param, types[i]);
						}
					}
					int varargIndex = types.Length - 1;
					Type varargType = types[varargIndex].GetElementType();
					int length = ParamCount - varargIndex;
					object array = null;
					if (length == 1)
					{ // special: eventually use argument as is
						object param = getParam(varargIndex).eval(bindings, context);
						if (param != null && param.GetType().IsArray)
						{
							if (types[varargIndex].IsInstanceOfType(param))
							{
								array = param;
							}
							else
							{ // coerce array elements
								length = Array.getLength(param);
								array = Array.CreateInstance(varargType, length);
								for (int i = 0; i < length; i++)
								{
									object elem = Array.get(param, i);
									if (elem != null || varargType.IsPrimitive)
									{
										((Array)array).SetValue(bindings.convert(elem, varargType), i);
									}
								}
							}
						}
						else
						{ // single element array
							array = Array.CreateInstance(varargType, 1);
							if (param != null || varargType.IsPrimitive)
							{
								((Array)array).SetValue(bindings.convert(param, varargType), 0);
							}
						}
					}
					else
					{
						array = Array.CreateInstance(varargType, length);
						for (int i = 0; i < length; i++)
						{
							object param = getParam(varargIndex + i).eval(bindings, context);
							if (param != null || varargType.IsPrimitive)
							{
								((Array)array).SetValue(bindings.convert(param, varargType), i);
							}
						}
					}
					@params[varargIndex] = array;
				}
				else
				{
					for (int i = 0; i < @params.Length; i++)
					{
						object param = getParam(i).eval(bindings, context);
						if (param != null || types[i].IsPrimitive)
						{
							@params[i] = bindings.convert(param, types[i]);
						}
					}
				}
			}
			return method.invoke(@base, @params);
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			System.Reflection.MethodInfo method = bindings.getFunction(index);
			try
			{
				return invoke(bindings, context, null, method);
			}
			catch (IllegalAccessException e)
			{
				throw new ELException(LocalMessages.get("error.function.access", name), e);
			}
			catch (InvocationTargetException e)
			{
				throw new ELException(LocalMessages.get("error.function.invocation", name), e.InnerException);
			}
		}

		public override string ToString()
		{
			return name;
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(bindings != null && bindings.isFunctionBound(index) ? "<fn>" : name);
			@params.appendStructure(b, bindings);
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual bool VarArgs
		{
			get
			{
				return varargs;
			}
		}

		public virtual int ParamCount
		{
			get
			{
				return @params.Cardinality;
			}
		}

		protected internal virtual AstNode getParam(int i)
		{
			return @params.getChild(i);
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? @params : null;
		}
	}

}