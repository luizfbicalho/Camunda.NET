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


	public sealed class AstString : AstLiteral
	{
		private readonly string value;

		public AstString(string value)
		{
			this.value = value;
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return value;
		}

		public override string ToString()
		{
			return "\"" + value + "\"";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append("'");
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c == '\\' || c == '\'')
				{
					b.Append('\\');
				}
				b.Append(c);
			}
			b.Append("'");
		}
	}

}