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
	/// This class encapsulates a base model object and one of its properties.
	/// 
	/// @since 2.2
	/// </summary>
	[Serializable]
	public class ValueReference
	{
		private const long serialVersionUID = 1L;

		private object @base;
		private object property;

		public ValueReference(object @base, object property)
		{
			this.@base = @base;
			this.property = property;
		}

		public virtual object Base
		{
			get
			{
				return @base;
			}
		}

		public virtual object Property
		{
			get
			{
				return property;
			}
		}
	}
}