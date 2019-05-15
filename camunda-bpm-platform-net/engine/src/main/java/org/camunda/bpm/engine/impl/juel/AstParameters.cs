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


	public class AstParameters : AstRightValue
	{
		private readonly IList<AstNode> nodes;

		public AstParameters(IList<AstNode> nodes)
		{
			this.nodes = nodes;
		}

		public override object[] eval(Bindings bindings, ELContext context)
		{
			object[] result = new object[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				result[i] = nodes[i].eval(bindings, context);
			}
			return result;
		}

		public override string ToString()
		{
			return "(...)";
		}

		public override void appendStructure(StringBuilder builder, Bindings bindings)
		{
			builder.Append("(");
			for (int i = 0; i < nodes.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(", ");
				}
				nodes[i].appendStructure(builder, bindings);
			}
			builder.Append(")");
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