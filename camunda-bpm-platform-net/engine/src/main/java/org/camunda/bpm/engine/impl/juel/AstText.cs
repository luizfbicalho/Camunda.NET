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
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	public sealed class AstText : AstNode
	{
		private readonly string value;

		public AstText(string value)
		{
			this.value = value;
		}

		public override bool LiteralText
		{
			get
			{
				return true;
			}
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

		public override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return value;
		}

		public override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			return returnType == null ? value : bindings.convert(value, returnType);
		}

		public override string ToString()
		{
			return "\"" + value + "\"";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			int end = value.Length - 1;
			for (int i = 0; i < end; i++)
			{
				char c = value[i];
				if ((c == '#' || c == '$') && value[i + 1] == '{')
				{
					b.Append('\\');
				}
				b.Append(c);
			}
			if (end >= 0)
			{
				b.Append(value[end]);
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