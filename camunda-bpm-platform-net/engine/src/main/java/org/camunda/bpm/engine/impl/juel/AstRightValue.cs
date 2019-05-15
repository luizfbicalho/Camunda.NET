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
	using MethodInfo = org.camunda.bpm.engine.impl.javax.el.MethodInfo;
	using ValueReference = org.camunda.bpm.engine.impl.javax.el.ValueReference;


	/// <summary>
	/// @author Christoph Beck
	/// </summary>
	public abstract class AstRightValue : AstNode
	{
		/// <summary>
		/// Answer <code>false</code>
		/// </summary>
		public sealed override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// according to the spec, the result is undefined for rvalues, so answer <code>null</code>
		/// </summary>
		public sealed override Type getType(Bindings bindings, ELContext context)
		{
			return null;
		}

		/// <summary>
		/// non-lvalues are always readonly, so answer <code>true</code>
		/// </summary>
		public sealed override bool isReadOnly(Bindings bindings, ELContext context)
		{
			return true;
		}

		/// <summary>
		/// non-lvalues are always readonly, so throw an exception
		/// </summary>
		public sealed override void setValue(Bindings bindings, ELContext context, object value)
		{
			throw new ELException(LocalMessages.get("error.value.set.rvalue", getStructuralId(bindings)));
		}

		public sealed override MethodInfo getMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			return null;
		}

		public sealed override object invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
		{
			throw new ELException(LocalMessages.get("error.method.invalid", getStructuralId(bindings)));
		}

		public sealed override bool LeftValue
		{
			get
			{
				return false;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return false;
			}
		}

		public sealed override ValueReference getValueReference(Bindings bindings, ELContext context)
		{
			return null;
		}
	}

}