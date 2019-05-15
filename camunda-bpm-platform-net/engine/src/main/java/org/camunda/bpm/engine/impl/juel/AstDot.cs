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


	public class AstDot : AstProperty
	{
		protected internal readonly string property;

		public AstDot(AstNode @base, string property, bool lvalue) : base(@base, lvalue, true)
		{
			this.property = property;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected String getProperty(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		protected internal override string getProperty(Bindings bindings, ELContext context)
		{
			return property;
		}

		public override string ToString()
		{
			return ". " + property;
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			getChild(0).appendStructure(b, bindings);
			b.Append(".");
			b.Append(property);
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}
	}

}