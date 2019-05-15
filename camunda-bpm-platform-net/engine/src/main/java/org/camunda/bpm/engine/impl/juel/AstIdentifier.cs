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
	using MethodInfo = org.camunda.bpm.engine.impl.javax.el.MethodInfo;
	using MethodNotFoundException = org.camunda.bpm.engine.impl.javax.el.MethodNotFoundException;
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	public class AstIdentifier : AstNode, IdentifierNode
	{
		private readonly string name;
		private readonly int index;

		public AstIdentifier(string name, int index)
		{
			this.name = name;
			this.index = index;
		}

		public override Type getType(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.getVariable(index);
			if (expression != null)
			{
				return expression.getType(context);
			}
			context.PropertyResolved = false;
			Type result = context.ELResolver.getType(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.identifier.property.notfound", name));
			}
			return result;
		}


		public override bool LeftValue
		{
			get
			{
				return true;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return false;
			}
		}

		public override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		public override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.getVariable(index);
			if (expression != null)
			{
				return expression.getValueReference(context);
			}
			return new ValueReference(null, name);
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.getVariable(index);
			if (expression != null)
			{
				return expression.getValue(context);
			}
			context.PropertyResolved = false;
			object result = context.ELResolver.getValue(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.identifier.property.notfound", name));
			}
			return result;
		}

		public override void setValue(Bindings bindings, ELContext context, object value)
		{
			ValueExpression expression = bindings.getVariable(index);
			if (expression != null)
			{
				expression.setValue(context, value);
				return;
			}
			context.PropertyResolved = false;
			context.ELResolver.setValue(context, null, name, value);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.identifier.property.notfound", name));
			}
		}

		public override bool isReadOnly(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.getVariable(index);
			if (expression != null)
			{
				return expression.isReadOnly(context);
			}
			context.PropertyResolved = false;
			bool result = context.ELResolver.isReadOnly(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.identifier.property.notfound", name));
			}
			return result;
		}

		protected internal virtual System.Reflection.MethodInfo getMethod(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			object value = eval(bindings, context);
			if (value == null)
			{
				throw new MethodNotFoundException(LocalMessages.get("error.identifier.method.notfound", name));
			}
			if (value is System.Reflection.MethodInfo)
			{
				System.Reflection.MethodInfo method = (System.Reflection.MethodInfo)value;
				if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType))
				{
					throw new MethodNotFoundException(LocalMessages.get("error.identifier.method.notfound", name));
				}
				if (!Arrays.Equals(method.ParameterTypes, paramTypes))
				{
					throw new MethodNotFoundException(LocalMessages.get("error.identifier.method.notfound", name));
				}
				return method;
			}
			throw new MethodNotFoundException(LocalMessages.get("error.identifier.method.notamethod", name, value.GetType()));
		}

		public override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			System.Reflection.MethodInfo method = getMethod(bindings, context, returnType, paramTypes);
			return new MethodInfo(method.Name, method.ReturnType, paramTypes);
		}

		public override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] @params)
		{
			System.Reflection.MethodInfo method = getMethod(bindings, context, returnType, paramTypes);
			try
			{
				return method.invoke(null, @params);
			}
			catch (IllegalAccessException)
			{
				throw new ELException(LocalMessages.get("error.identifier.method.access", name));
			}
			catch (System.ArgumentException e)
			{
				throw new ELException(LocalMessages.get("error.identifier.method.invocation", name, e));
			}
			catch (InvocationTargetException e)
			{
				throw new ELException(LocalMessages.get("error.identifier.method.invocation", name, e.InnerException));
			}
		}

		public override string ToString()
		{
			return name;
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(bindings != null && bindings.isVariableBound(index) ? "<var>" : name);
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

		public override int Cardinality
		{
			get
			{
				return 0;
			}
		}

		public override AstNode getChild(int i)
		{
			return null;
		}
	}

}