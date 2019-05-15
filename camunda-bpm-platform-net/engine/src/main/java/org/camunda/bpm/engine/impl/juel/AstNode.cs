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


	public abstract class AstNode : ExpressionNode
	{
		public abstract Node getChild(int i);
		public abstract int Cardinality {get;}
		public abstract object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues);
		public abstract org.camunda.bpm.engine.impl.javax.el.MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes);
		public abstract void setValue(Bindings bindings, ELContext context, object value);
		public abstract bool isReadOnly(Bindings bindings, ELContext context);
		public abstract Type getType(Bindings bindings, ELContext context);
		public abstract org.camunda.bpm.engine.impl.javax.el.ValueReference getValueReference(Bindings bindings, ELContext context);
		public abstract bool MethodInvocation {get;}
		public abstract bool LeftValue {get;}
		public abstract bool LiteralText {get;}
		/// <summary>
		/// evaluate and return the (optionally coerced) result.
		/// </summary>
		public object getValue(Bindings bindings, ELContext context, Type type)
		{
			object value = eval(bindings, context);
			if (type != null)
			{
				value = bindings.convert(value, type);
			}
			return value;
		}

		public abstract void appendStructure(StringBuilder builder, Bindings bindings);

		public abstract object eval(Bindings bindings, ELContext context);

		public string getStructuralId(Bindings bindings)
		{
			StringBuilder builder = new StringBuilder();
			appendStructure(builder, bindings);
			return builder.ToString();
		}
	}

}