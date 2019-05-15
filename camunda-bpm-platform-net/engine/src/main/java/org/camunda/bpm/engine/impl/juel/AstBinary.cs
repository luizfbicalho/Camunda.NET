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


	public class AstBinary : AstRightValue
	{
		public interface Operator
		{
			object eval(Bindings bindings, ELContext context, AstNode left, AstNode right);
		}
		public abstract class SimpleOperator : Operator
		{
			public virtual object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				return apply(bindings, left.eval(bindings, context), right.eval(bindings, context));
			}

			protected internal abstract object apply(TypeConverter converter, object o1, object o2);
		}
		public static readonly Operator ADD = new SimpleOperatorAnonymousInnerClass();

		private class SimpleOperatorAnonymousInnerClass : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.add(converter, o1, o2);
			}
			public override string ToString()
			{
				return "+";
			}
		}
		public static readonly Operator AND = new OperatorAnonymousInnerClass();

		private class OperatorAnonymousInnerClass : Operator
		{
			public object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.convert(left.eval(bindings, context), typeof(Boolean));
				return true.Equals(l) ? bindings.convert(right.eval(bindings, context), typeof(Boolean)) : false;
			}
			public override string ToString()
			{
				return "&&";
			}
		}
		public static readonly Operator DIV = new SimpleOperatorAnonymousInnerClass2();

		private class SimpleOperatorAnonymousInnerClass2 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.div(converter, o1, o2);
			}
			public override string ToString()
			{
				return "/";
			}
		}
		public static readonly Operator EQ = new SimpleOperatorAnonymousInnerClass3();

		private class SimpleOperatorAnonymousInnerClass3 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.eq(converter, o1, o2);
			}
			public override string ToString()
			{
				return "==";
			}
		}
		public static readonly Operator GE = new SimpleOperatorAnonymousInnerClass4();

		private class SimpleOperatorAnonymousInnerClass4 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.ge(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">=";
			}
		}
		public static readonly Operator GT = new SimpleOperatorAnonymousInnerClass5();

		private class SimpleOperatorAnonymousInnerClass5 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.gt(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">";
			}
		}
		public static readonly Operator LE = new SimpleOperatorAnonymousInnerClass6();

		private class SimpleOperatorAnonymousInnerClass6 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.le(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<=";
			}
		}
		public static readonly Operator LT = new SimpleOperatorAnonymousInnerClass7();

		private class SimpleOperatorAnonymousInnerClass7 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.lt(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<";
			}
		}
		public static readonly Operator MOD = new SimpleOperatorAnonymousInnerClass8();

		private class SimpleOperatorAnonymousInnerClass8 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.mod(converter, o1, o2);
			}
			public override string ToString()
			{
				return "%";
			}
		}
		public static readonly Operator MUL = new SimpleOperatorAnonymousInnerClass9();

		private class SimpleOperatorAnonymousInnerClass9 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.mul(converter, o1, o2);
			}
			public override string ToString()
			{
				return "*";
			}
		}
		public static readonly Operator NE = new SimpleOperatorAnonymousInnerClass10();

		private class SimpleOperatorAnonymousInnerClass10 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.ne(converter, o1, o2);
			}
			public override string ToString()
			{
				return "!=";
			}
		}
		public static readonly Operator OR = new OperatorAnonymousInnerClass2();

		private class OperatorAnonymousInnerClass2 : Operator
		{
			public object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.convert(left.eval(bindings, context), typeof(Boolean));
				return true.Equals(l) ? true : bindings.convert(right.eval(bindings, context), typeof(Boolean));
			}
			public override string ToString()
			{
				return "||";
			}
		}
		public static readonly Operator SUB = new SimpleOperatorAnonymousInnerClass11();

		private class SimpleOperatorAnonymousInnerClass11 : SimpleOperator
		{
			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.sub(converter, o1, o2);
			}
			public override string ToString()
			{
				return "-";
			}
		}

		private readonly Operator @operator;
		private readonly AstNode left, right;

		public AstBinary(AstNode left, AstNode right, Operator @operator)
		{
			this.left = left;
			this.right = right;
			this.@operator = @operator;
		}

		public virtual Operator getOperator()
		{
			return @operator;
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return @operator.eval(bindings, context, left, right);
		}

		public override string ToString()
		{
			return "'" + @operator.ToString() + "'";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			left.appendStructure(b, bindings);
			b.Append(' ');
			b.Append(@operator);
			b.Append(' ');
			right.appendStructure(b, bindings);
		}

		public override int Cardinality
		{
			get
			{
				return 2;
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? left : i == 1 ? right : null;
		}
	}

}