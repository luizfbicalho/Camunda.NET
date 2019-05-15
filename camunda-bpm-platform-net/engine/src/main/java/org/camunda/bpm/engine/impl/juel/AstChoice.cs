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


	public class AstChoice : AstRightValue
	{
		private readonly AstNode question, yes, no;

		public AstChoice(AstNode question, AstNode yes, AstNode no)
		{
			this.question = question;
			this.yes = yes;
			this.no = no;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object eval(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override object eval(Bindings bindings, ELContext context)
		{
			bool? value = bindings.convert(question.eval(bindings, context), typeof(Boolean));
			return value.Value ? yes.eval(bindings, context) : no.eval(bindings, context);
		}

		public override string ToString()
		{
			return "?";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			question.appendStructure(b, bindings);
			b.Append(" ? ");
			yes.appendStructure(b, bindings);
			b.Append(" : ");
			no.appendStructure(b, bindings);
		}

		public override int Cardinality
		{
			get
			{
				return 3;
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? question : i == 1 ? yes : i == 2 ? no : null;
		}
	}

}