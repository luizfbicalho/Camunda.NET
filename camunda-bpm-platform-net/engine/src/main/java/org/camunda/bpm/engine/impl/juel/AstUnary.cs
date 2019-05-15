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


	public class AstUnary : AstRightValue
	{
		public interface Operator
		{
			object eval(Bindings bindings, ELContext context, AstNode node);
		}
		public abstract class SimpleOperator : Operator
		{
			public virtual object eval(Bindings bindings, ELContext context, AstNode node)
			{
				return apply(bindings, node.eval(bindings, context));
			}

			protected internal abstract object apply(TypeConverter converter, object o);
		}
		public static readonly Operator EMPTY = new SimpleOperatorAnonymousInnerClass();

		private class SimpleOperatorAnonymousInnerClass : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o)
			{
				return BooleanOperations.empty(converter, o);
			}
			public override string ToString()
			{
				return "empty";
			}
		}
		public static readonly Operator NEG = new SimpleOperatorAnonymousInnerClass2();

		private class SimpleOperatorAnonymousInnerClass2 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o)
			{
				return NumberOperations.neg(converter, o);
			}
			public override string ToString()
			{
				return "-";
			}
		}
		public static readonly Operator NOT = new SimpleOperatorAnonymousInnerClass3();

		private class SimpleOperatorAnonymousInnerClass3 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o)
			{
				return !converter.convert(o, typeof(Boolean));
			}
			public override string ToString()
			{
				return "!";
			}
		}

		private readonly Operator @operator;
		private readonly AstNode child;

		public AstUnary(AstNode child, AstUnary.Operator @operator)
		{
			this.child = child;
			this.@operator = @operator;
		}

		public virtual Operator getOperator()
		{
			return @operator;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object eval(Bindings bindings, org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override object eval(Bindings bindings, ELContext context)
		{
			return @operator.eval(bindings, context, child);
		}

		public override string ToString()
		{
			return "'" + @operator.ToString() + "'";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(@operator);
			b.Append(' ');
			child.appendStructure(b, bindings);
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