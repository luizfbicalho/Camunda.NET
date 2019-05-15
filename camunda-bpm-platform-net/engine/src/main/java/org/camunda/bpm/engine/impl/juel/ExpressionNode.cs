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
	using MethodInfo = org.camunda.bpm.engine.impl.javax.el.MethodInfo;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;

	/// <summary>
	/// Expression node interface. This interface provides all the methods needed for value expressions
	/// and method expressions.
	/// </summary>
	/// <seealso cref= org.camunda.bpm.engine.impl.juel.Tree
	/// @author Christoph Beck </seealso>
	public interface ExpressionNode : Node
	{
		/// <returns> <code>true</code> if this node represents literal text </returns>
		bool LiteralText {get;}

		/// <returns> <code>true</code> if the subtree rooted at this node could be used as an lvalue
		///         expression (identifier or property sequence with non-literal prefix). </returns>
		bool LeftValue {get;}

		/// <returns> <code>true</code> if the subtree rooted at this node is a method invocation. </returns>
		bool MethodInvocation {get;}

		/// <summary>
		/// Evaluate node.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <param name="expectedType">
		///            result type </param>
		/// <returns> evaluated node, coerced to the expected type </returns>
		object getValue(Bindings bindings, ELContext context, Type expectedType);

		/// <summary>
		/// Get value reference.
		/// </summary>
		/// <param name="bindings"> </param>
		/// <param name="context"> </param>
		/// <returns> value reference </returns>
		ValueReference getValueReference(Bindings bindings, ELContext context);

		/// <summary>
		/// Get the value type accepted in <seealso cref="#setValue(Bindings, ELContext, Object)"/>.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <returns> accepted type or <code>null</code> for non-lvalue nodes </returns>
		Type getType(Bindings bindings, ELContext context);

		/// <summary>
		/// Determine whether <seealso cref="#setValue(Bindings, ELContext, Object)"/> will throw a
		/// <seealso cref="org.camunda.bpm.engine.impl.javax.el.PropertyNotWritableException"/>.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <returns> <code>true</code> if this a read-only expression node </returns>
		bool isReadOnly(Bindings bindings, ELContext context);

		/// <summary>
		/// Assign value.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <param name="value">
		///            value to set </param>
		void setValue(Bindings bindings, ELContext context, object value);

		/// <summary>
		/// Get method information. If this is a non-lvalue node, answer <code>null</code>.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <param name="returnType">
		///            expected method return type (may be <code>null</code> meaning don't care) </param>
		/// <param name="paramTypes">
		///            expected method argument types </param>
		/// <returns> method information or <code>null</code> </returns>
		MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes);

		/// <summary>
		/// Invoke method.
		/// </summary>
		/// <param name="bindings">
		///            bindings containing variables and functions </param>
		/// <param name="context">
		///            evaluation context </param>
		/// <param name="returnType">
		///            expected method return type (may be <code>null</code> meaning don't care) </param>
		/// <param name="paramTypes">
		///            expected method argument types </param>
		/// <param name="paramValues">
		///            parameter values </param>
		/// <returns> result of the method invocation </returns>
		object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues);

		/// <summary>
		/// Get the canonical expression string for this node. Variable and funtion names will be
		/// replaced in a way such that two expression nodes that have the same node structure and
		/// bindings will also answer the same value here.
		/// <p/>
		/// For example, <code>"${foo:bar()+2*foobar}"</code> may lead to
		/// <code>"${&lt;fn>() + 2 * &lt;var>}"</code> if <code>foobar</code> is a bound variable.
		/// Otherwise, the structural id would be <code>"${&lt;fn>() + 2 * foobar}"</code>.
		/// <p/>
		/// If the bindings is <code>null</code>, the full canonical subexpression is returned.
		/// </summary>
		string getStructuralId(Bindings bindings);
	}

}