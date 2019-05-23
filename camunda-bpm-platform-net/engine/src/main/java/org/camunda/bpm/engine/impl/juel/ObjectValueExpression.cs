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


	/// <summary>
	/// Object wrapper expression.
	/// 
	/// @author Christoph Beck
	/// </summary>
	[Serializable]
	public sealed class ObjectValueExpression : org.camunda.bpm.engine.impl.javax.el.ValueExpression
	{
		private const long serialVersionUID = 1L;

		private readonly TypeConverter converter;
		private readonly object @object;
		private readonly Type type;

		/// <summary>
		/// Wrap an object into a value expression. </summary>
		/// <param name="converter"> type converter </param>
		/// <param name="object"> the object to wrap </param>
		/// <param name="type"> the expected type this object will be coerced in <seealso cref="getValue(ELContext)"/>. </param>
		public ObjectValueExpression(TypeConverter converter, object @object, Type type) : base()
		{

			this.converter = converter;
			this.@object = @object;
			this.type = type;

			if (type == null)
			{
				throw new System.NullReferenceException(LocalMessages.get("error.value.notype"));
			}
		}

		/// <summary>
		/// Two object value expressions are equal if and only if their wrapped objects are equal.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == this.GetType())
			{
				ObjectValueExpression other = (ObjectValueExpression)obj;
				if (type != other.type)
				{
					return false;
				}
				return @object == other.@object || @object != null && @object.Equals(other.@object);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return @object == null ? 0 : @object.GetHashCode();
		}

		/// <summary>
		/// Answer the wrapped object, coerced to the expected type.
		/// </summary>
		public override object getValue(ELContext context)
		{
			return converter.convert(@object, type);
		}

		/// <summary>
		/// Answer <code>null</code>.
		/// </summary>
		public override string ExpressionString
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Answer <code>false</code>.
		/// </summary>
		public override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Answer <code>null</code>.
		/// </summary>
		public override Type getType(ELContext context)
		{
			return null;
		}

		/// <summary>
		/// Answer <code>true</code>.
		/// </summary>
		public override bool isReadOnly(ELContext context)
		{
			return true;
		}

		/// <summary>
		/// Throw an exception.
		/// </summary>
		public override void setValue(ELContext context, object value)
		{
			throw new ELException(LocalMessages.get("error.value.set.rvalue", "<object value expression>"));
		}

		public override string ToString()
		{
			return "ValueExpression(" + @object + ")";
		}

		public override Type ExpectedType
		{
			get
			{
				return type;
			}
		}
	}

}