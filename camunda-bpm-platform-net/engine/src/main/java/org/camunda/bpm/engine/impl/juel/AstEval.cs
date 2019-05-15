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
	using MethodInfo = org.camunda.bpm.engine.impl.javax.el.MethodInfo;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	public sealed class AstEval : AstNode
	{
		private readonly AstNode child;
		private readonly bool deferred;

		public AstEval(AstNode child, bool deferred)
		{
			this.child = child;
			this.deferred = deferred;
		}

		public bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		public override bool LeftValue
		{
			get
			{
				return getChild(0).LeftValue;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return getChild(0).MethodInvocation;
			}
		}

		public override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			return child.getValueReference(bindings, context);
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return child.eval(bindings, context);
		}

		public override string ToString()
		{
			return (deferred ? "#" : "$") + "{...}";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(deferred ? "#{" : "${");
			child.appendStructure(b, bindings);
			b.Append("}");
		}

		public override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return child.getMethodInfo(bindings, context, returnType, paramTypes);
		}

		public override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			return child.invoke(bindings, context, returnType, paramTypes, paramValues);
		}

		public override Type getType(Bindings bindings, ELContext context)
		{
			return child.getType(bindings, context);
		}

		public override bool LiteralText
		{
			get
			{
				return child.LiteralText;
			}
		}

		public override bool isReadOnly(Bindings bindings, ELContext context)
		{
			return child.isReadOnly(bindings, context);
		}

		public override void setValue(Bindings bindings, ELContext context, object value)
		{
			child.setValue(bindings, context, value);
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
			return i == 0 ? child : null;
		}
	}

}