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
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	public class AstMethod : AstNode
	{
		private readonly AstProperty property;
		private readonly AstParameters @params;

		public AstMethod(AstProperty property, AstParameters @params)
		{
			this.property = property;
			this.@params = @params;
		}

		public override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		public override Type getType(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override bool isReadOnly(Bindings bindings, ELContext context)
		{
			return true;
		}

		public override void setValue(Bindings bindings, ELContext context, object value)
		{
			throw new ELException(LocalMessages.get("error.value.set.rvalue", getStructuralId(bindings)));
		}

		public override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public override bool LeftValue
		{
			get
			{
				return false;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return true;
			}
		}

		public sealed override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override void appendStructure(StringBuilder builder, Bindings bindings)
		{
			property.appendStructure(builder, bindings);
			@params.appendStructure(builder, bindings);
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return invoke(bindings, context, null, null, null);
		}

		public override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			object @base = property.Prefix.eval(bindings, context);
			if (@base == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.base.null", property.Prefix));
			}
			object method = property.getProperty(bindings, context);
			if (method == null)
			{
				throw new PropertyNotFoundException(LocalMessages.get("error.property.method.notfound", "null", @base));
			}
			string name = bindings.convert(method, typeof(string));
			paramValues = @params.eval(bindings, context);

			context.PropertyResolved = false;
			object result = context.ELResolver.invoke(context, @base, name, paramTypes, paramValues);
			if (!context.PropertyResolved)
			{
				throw new MethodNotFoundException(LocalMessages.get("error.property.method.notfound", name, @base.GetType()));
			}
	//		if (returnType != null && !returnType.isInstance(result)) { // should we check returnType for method invocations?
	//			throw new MethodNotFoundException(LocalMessages.get("error.property.method.notfound", name, base.getClass()));
	//		}
			return result;
		}

		public override int Cardinality
		{
			get
			{
				return 2;
			}
		}

		public override Node getChild(int i)
		{
			return i == 0 ? property : i == 1 ? @params : null;
		}

		public override string ToString()
		{
			return "<method>";
		}
	}

}