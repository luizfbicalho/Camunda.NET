/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.model.xml.test.assertions
{
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AttributeReferenceAssert : AbstractReferenceAssert<AttributeReferenceAssert, AttributeReference<JavaToDotNetGenericWildcard>>
	{

	  protected internal AttributeReferenceAssert<T1>(AttributeReference<T1> actual) : base(actual, typeof(AttributeReferenceAssert))
	  {
	  }

	  public virtual AttributeReferenceAssert hasSourceAttribute<T1>(Attribute<T1> sourceAttribute)
	  {
		NotNull;

		Attribute<string> actualSourceAttribute = actual.ReferenceSourceAttribute;

		if (!sourceAttribute.Equals(actualSourceAttribute))
		{
		  failWithMessage("Expected reference <%s> to have source attribute <%s> but was <%s>", actual, sourceAttribute, actualSourceAttribute);
		}

		return this;
	  }


	}

}