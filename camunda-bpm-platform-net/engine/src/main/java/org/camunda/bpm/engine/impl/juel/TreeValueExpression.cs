using System;

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
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;


	/// <summary>
	/// A value expression is ready to be evaluated (by calling either
	/// <seealso cref="#getType(ELContext)"/>, <seealso cref="#getValue(ELContext)"/>, <seealso cref="#isReadOnly(ELContext)"/>
	/// or <seealso cref="#setValue(ELContext, Object)"/>.
	/// 
	/// Instances of this class are usually created using an <seealso cref="ExpressionFactoryImpl"/>.
	/// 
	/// @author Christoph Beck
	/// </summary>
	[Serializable]
	public sealed class TreeValueExpression : org.camunda.bpm.engine.impl.javax.el.ValueExpression
	{
		private const long serialVersionUID = 1L;

		private readonly TreeBuilder builder;
		private readonly Bindings bindings;
		private readonly string expr;
		private readonly Type type;
		private readonly bool deferred;

		[NonSerialized]
		private ExpressionNode node;

		private string structure;

		/// <summary>
		/// Create a new value expression. </summary>
		/// <param name="store"> used to get the parse tree from. </param>
		/// <param name="functions"> the function mapper used to bind functions </param>
		/// <param name="variables"> the variable mapper used to bind variables </param>
		/// <param name="expr"> the expression string </param>
		/// <param name="type"> the expected type (may be <code>null</code>) </param>
		public TreeValueExpression(TreeStore store, FunctionMapper functions, VariableMapper variables, TypeConverter converter, string expr, Type type) : base()
		{

			Tree tree = store.get(expr);

			this.builder = store.Builder;
			this.bindings = tree.bind(functions, variables, converter);
			this.expr = expr;
			this.type = type;
			this.node = tree.Root;
			this.deferred = tree.Deferred;

			if (type == null)
			{
				throw new System.NullReferenceException(LocalMessages.get("error.value.notype"));
			}
		}

		private string StructuralId
		{
			get
			{
				if (string.ReferenceEquals(structure, null))
				{
					structure = node.getStructuralId(bindings);
				}
				return structure;
			}
		}

		public override Type ExpectedType
		{
			get
			{
				return type;
			}
		}

		public override string ExpressionString
		{
			get
			{
				return expr;
			}
		}

	  /// <summary>
	  /// Evaluates the expression as an lvalue and answers the result type. </summary>
	  /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
	  /// and to determine the result from the last base/property pair </param>
	  /// <returns> lvalue evaluation type or <code>null</code> for rvalue expressions </returns>
	  /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Class getType(org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override Type getType(ELContext context)
		{
			return node.getType(bindings, context);
		}

	  /// <summary>
	  /// Evaluates the expression as an rvalue and answers the result. </summary>
	  /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
	  /// and to determine the result from the last base/property pair </param>
	  /// <returns> rvalue evaluation result </returns>
	  /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object getValue(org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override object getValue(ELContext context)
		{
			return node.getValue(bindings, context, type);
		}

		/// <summary>
		/// Evaluates the expression as an lvalue and determines if <seealso cref="#setValue(ELContext, Object)"/>
		/// will always fail. </summary>
		/// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
		/// and to determine the result from the last base/property pair </param>
		/// <returns> <code>true</code> if <seealso cref="#setValue(ELContext, Object)"/> always fails. </returns>
		/// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean isReadOnly(org.camunda.bpm.engine.impl.javax.el.ELContext context) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override bool isReadOnly(ELContext context)
		{
			return node.isReadOnly(bindings, context);
		}

		/// <summary>
		/// Evaluates the expression as an lvalue and assigns the given value. </summary>
		/// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
		/// and to perform the assignment to the last base/property pair </param>
		/// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, assignment failed...) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setValue(org.camunda.bpm.engine.impl.javax.el.ELContext context, Object value) throws org.camunda.bpm.engine.impl.javax.el.ELException
		public override void setValue(ELContext context, object value)
		{
			node.setValue(bindings, context, value);
		}

		/// <returns> <code>true</code> if this is a literal text expression </returns>
		public override bool LiteralText
		{
			get
			{
				return node.LiteralText;
			}
		}

		public override ValueReference getValueReference(ELContext context)
		{
			return node.getValueReference(bindings, context);
		}

		/// <summary>
		/// Answer <code>true</code> if this could be used as an lvalue.
		/// This is the case for eval expressions consisting of a simple identifier or
		/// a nonliteral prefix, followed by a sequence of property operators (<code>.</code> or <code>[]</code>)
		/// </summary>
		public bool LeftValue
		{
			get
			{
				return node.LeftValue;
			}
		}

		/// <summary>
		/// Answer <code>true</code> if this is a deferred expression (containing
		/// sub-expressions starting with <code>#{</code>)
		/// </summary>
		public bool Deferred
		{
			get
			{
				return deferred;
			}
		}

		/// <summary>
		/// Expressions are compared using the concept of a <em>structural id</em>:
		/// variable and function names are anonymized such that two expressions with
		/// same tree structure will also have the same structural id and vice versa.
		/// Two value expressions are equal if
		/// <ol>
		/// <li>their structural id's are equal</li>
		/// <li>their bindings are equal</li>
		/// <li>their expected types are equal</li>
		/// </ol>
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == this.GetType())
			{
				TreeValueExpression other = (TreeValueExpression)obj;
				if (!builder.Equals(other.builder))
				{
					return false;
				}
				if (type != other.type)
				{
					return false;
				}
				return StructuralId.Equals(other.StructuralId) && bindings.Equals(other.bindings);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return StructuralId.GetHashCode();
		}

		public override string ToString()
		{
			return "TreeValueExpression(" + expr + ")";
		}

		/// <summary>
		/// Print the parse tree. </summary>
		/// <param name="writer"> </param>
		public void dump(PrintWriter writer)
		{
			NodePrinter.dump(writer, node);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void readObject(ObjectInputStream @in)
		{
			@in.defaultReadObject();
			try
			{
				node = builder.build(expr).Root;
			}
			catch (ELException e)
			{
				throw new IOException(e.Message);
			}
		}
	}

}