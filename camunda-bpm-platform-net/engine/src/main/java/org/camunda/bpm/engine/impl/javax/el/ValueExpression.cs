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
namespace org.camunda.bpm.engine.impl.javax.el
{
	/// <summary>
	/// An Expression that can get or set a value. In previous incarnations of this API, expressions
	/// could only be read. ValueExpression objects can now be used both to retrieve a value and to set a
	/// value. Expressions that can have a value set on them are referred to as l-value expressions.
	/// Those that cannot are referred to as r-value expressions. Not all r-value expressions can be used
	/// as l-value expressions (e.g. "${1+1}" or "${firstName} ${lastName}"). See the EL Specification
	/// for details. Expressions that cannot be used as l-values must always return true from
	/// isReadOnly(). The <seealso cref="ExpressionFactory#createValueExpression(ELContext, String, Class)"/>
	/// method can be used to parse an expression string and return a concrete instance of
	/// ValueExpression that encapsulates the parsed expression. The <seealso cref="FunctionMapper"/> is used at
	/// parse time, not evaluation time, so one is not needed to evaluate an expression using this class.
	/// However, the <seealso cref="ELContext"/> is needed at evaluation time. The <seealso cref="#getValue(ELContext)"/>,
	/// <seealso cref="#setValue(ELContext, Object)"/>, <seealso cref="#isReadOnly(ELContext)"/> and
	/// <seealso cref="#getType(ELContext)"/> methods will evaluate the expression each time they are called. The
	/// <seealso cref="ELResolver"/> in the ELContext is used to resolve the top-level variables and to determine
	/// the behavior of the . and [] operators. For any of the four methods, the
	/// <seealso cref="ELResolver#getValue(ELContext, Object, Object)"/> method is used to resolve all properties
	/// up to but excluding the last one. This provides the base object. At the last resolution, the
	/// ValueExpression will call the corresponding
	/// <seealso cref="ELResolver#getValue(ELContext, Object, Object)"/>,
	/// <seealso cref="ELResolver#setValue(ELContext, Object, Object, Object)"/>,
	/// <seealso cref="ELResolver#isReadOnly(ELContext, Object, Object)"/> or
	/// <seealso cref="ELResolver#getType(ELContext, Object, Object)"/> method, depending on which was called on
	/// the ValueExpression. See the notes about comparison, serialization and immutability in the
	/// <seealso cref="Expression"/> javadocs.
	/// </summary>
	/// <seealso cref= ELResolver </seealso>
	/// <seealso cref= Expression </seealso>
	/// <seealso cref= ExpressionFactory </seealso>
	[Serializable]
	public abstract class ValueExpression : Expression
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Returns the type the result of the expression will be coerced to after evaluation.
		/// </summary>
		/// <returns> the expectedType passed to the ExpressionFactory.createValueExpression method that
		///         created this ValueExpression. </returns>
		public abstract Type ExpectedType {get;}

		/// <summary>
		/// Evaluates the expression relative to the provided context, and returns the most general type
		/// that is acceptable for an object to be passed as the value parameter in a future call to the
		/// <seealso cref="#setValue(ELContext, Object)"/> method. This is not always the same as
		/// getValue().getClass(). For example, in the case of an expression that references an array
		/// element, the getType method will return the element type of the array, which might be a
		/// superclass of the type of the actual element that is currently in the specified array
		/// element.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <returns> the most general acceptable type; otherwise undefined. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if one of the property resolutions failed because a specified variable or
		///             property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing property or variable resolution. The
		///             thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public abstract Type getType(ELContext context);

		/// <summary>
		/// Evaluates the expression relative to the provided context, and returns the resulting value.
		/// The resulting value is automatically coerced to the type returned by getExpectedType(), which
		/// was provided to the ExpressionFactory when this expression was created.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <returns> The result of the expression evaluation. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if one of the property resolutions failed because a specified variable or
		///             property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing property or variable resolution. The
		///             thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public abstract object getValue(ELContext context);

		/// <summary>
		/// Evaluates the expression relative to the provided context, and returns true if a call to
		/// <seealso cref="#setValue(ELContext, Object)"/> will always fail.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <returns> true if the expression is read-only or false if not. </returns>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if one of the property resolutions failed because a specified variable or
		///             property does not exist or is not readable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while performing property or variable resolution. The
		///             thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public abstract bool isReadOnly(ELContext context);

		/// <summary>
		/// Evaluates the expression relative to the provided context, and sets the result to the
		/// provided value.
		/// </summary>
		/// <param name="context">
		///            The context of this evaluation. </param>
		/// <param name="value">
		///            The new value to be set. </param>
		/// <exception cref="NullPointerException">
		///             if context is null. </exception>
		/// <exception cref="PropertyNotFoundException">
		///             if one of the property resolutions failed because a specified variable or
		///             property does not exist or is not readable. </exception>
		/// <exception cref="PropertyNotWritableException">
		///             if the final variable or property resolution failed because the specified
		///             variable or property is not writable. </exception>
		/// <exception cref="ELException">
		///             if an exception was thrown while attempting to set the property or variable. The
		///             thrown exception must be included as the cause property of this exception, if
		///             available. </exception>
		public abstract void setValue(ELContext context, object value);

		/// <summary>
		/// Returns a <seealso cref="ValueReference"/> for this expression instance.
		/// </summary>
		/// <param name="context">
		///            the context of this evaluation </param>
		/// <returns> the <code>ValueReference</code> for this <code>ValueExpression</code>, or
		///         <code>null</code> if this <code>ValueExpression</code> is not a reference to a base
		///         (null or non-null) and a property. If the base is null, and the property is a EL
		///         variable, return the <code>ValueReference</code> for the <code>ValueExpression</code>
		///         associated with this EL variable.
		/// 
		/// @since 2.2 </returns>
		public virtual ValueReference getValueReference(ELContext context)
		{
			return null;
		}
	}

}