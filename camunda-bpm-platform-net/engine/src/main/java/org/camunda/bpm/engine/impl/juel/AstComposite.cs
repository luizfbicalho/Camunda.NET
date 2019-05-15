using System.Collections.Generic;
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


	public class AstComposite : AstRightValue
	{
		private readonly IList<AstNode> nodes;

		public AstComposite(IList<AstNode> nodes)
		{
			this.nodes = nodes;
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			StringBuilder b = new StringBuilder(16);
			for (int i = 0; i < Cardinality; i++)
			{
				b.Append(bindings.convert(nodes[i].eval(bindings, context), typeof(string)));
			}
			return b.ToString();
		}

		public override string ToString()
		{
			return "composite";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			for (int i = 0; i < Cardinality; i++)
			{
				nodes[i].appendStructure(b, bindings);
			}
		}

		public override int Cardinality
		{
			get
			{
				return nodes.Count;
			}
		}

		public override AstNode getChild(int i)
		{
			return nodes[i];
		}
	}

}