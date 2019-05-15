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
	using MethodInfo = org.camunda.bpm.engine.impl.javax.el.MethodInfo;
	using MethodNotFoundException = org.camunda.bpm.engine.impl.javax.el.MethodNotFoundException;
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	public abstract class AstProperty : AstNode
	{
		protected internal readonly AstNode prefix;
		protected internal readonly bool lvalue;
		protected internal readonly bool strict; // allow null as property value?

		public AstProperty(AstNode prefix, bool lvalue, bool strict)
		{
			this.prefix = prefix;
			this.lvalue = lvalue;
			this.strict = strict;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract Object getProperty(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException;
		protected internal abstract object getProperty(Bindings bindings, ELContext context);

		protected internal virtual AstNode Prefix
		{
			get
			{
				return prefix;
			}
		}

		public override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			return new ValueReference(prefix.eval(bindings, context), getProperty(bindings, context));
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				return null;
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				return null;
			}
			context.PropertyResolved = false;
			object result = context.ELResolver.getValue(context, @base, property);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", property, @base));
			}
			return result;
		}

		public sealed override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		public sealed override bool LeftValue
		{
			get
			{
				return lvalue;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return false;
			}
		}

		public override Type getType(Bindings bindings, ELContext context)
		{
			if (!lvalue)
			{
				return null;
			}
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", prefix));
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", "null", @base));
			}
			context.PropertyResolved = false;
			Type result = context.ELResolver.getType(context, @base, property);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", property, @base));
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isReadOnly(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override bool isReadOnly(Bindings bindings, ELContext context)
		{
			if (!lvalue)
			{
				return true;
			}
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", prefix));
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", "null", @base));
			}
			context.PropertyResolved = false;
			bool result = context.ELResolver.isReadOnly(context, @base, property);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", property, @base));
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setValue(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context, Object value) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override void setValue(Bindings bindings, ELContext context, object value)
		{
			if (!lvalue)
			{
				throw new ELException(LocalMessages.get("error.value.set.rvalue", getStructuralId(bindings)));
			}
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", prefix));
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", "null", @base));
			}
			context.PropertyResolved = false;
			context.ELResolver.setValue(context, @base, property, value);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.property.notfound", property, @base));
			}
		}

		protected internal virtual System.Reflection.MethodInfo findMethod(string name, Type clazz, Type returnType, Type[] paramTypes)
		{
			System.Reflection.MethodInfo method = null;
			try
			{
				method = clazz.GetMethod(name, paramTypes);
			}
			catch (NoSuchMethodException)
			{
				throw new MethodNotFoundException(LocalMessages.get("error.property.method.notfound", name, clazz));
			}
			if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType))
			{
				throw new MethodNotFoundException(LocalMessages.get("error.property.method.notfound", name, clazz));
			}
			return method;
		}

		public override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", prefix));
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.method.notfound", "null", @base));
			}
			string name = bindings.convert(property, typeof(string));
			System.Reflection.MethodInfo method = findMethod(name, @base.GetType(), returnType, paramTypes);
			return new MethodInfo(method.Name, method.ReturnType, paramTypes);
		}

		public override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			object @base = prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", prefix));
			}
			object property = getProperty(bindings, context);
			if (property == null && strict)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.method.notfound", "null", @base));
			}
			string name = bindings.convert(property, typeof(string));
			System.Reflection.MethodInfo method = findMethod(name, @base.GetType(), returnType, paramTypes);
			try
			{
				return method.invoke(@base, paramValues);
			}
			catch (IllegalAccessException)
			{
				throw new ELException(LocalMessages.get("error.property.method.access", name, @base.GetType()));
			}
			catch (System.ArgumentException e)
			{
				throw new ELException(LocalMessages.get("error.property.method.invocation", name, @base.GetType()), e);
			}
			catch (InvocationTargetException e)
			{
				throw new ELException(LocalMessages.get("error.property.method.invocation", name, @base.GetType()), e.InnerException);
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? prefix : null;
		}
	}

}