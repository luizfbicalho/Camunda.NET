using System;

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
	using AbstractAssert = org.assertj.core.api.AbstractAssert;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractReferenceAssert<S, T> : AbstractAssert<S, T> where S : AbstractReferenceAssert<S, T>
	{

	  protected internal AbstractReferenceAssert(T actual, Type selfType) : base(actual, selfType)
	  {
	  }

	  public virtual S hasIdentifier(ModelElementInstance instance, string identifier)
	  {
		NotNull;

		string actualIdentifier = actual.getReferenceIdentifier(instance);

		if (!identifier.Equals(actualIdentifier))
		{
		  failWithMessage("Expected reference <%s> to have identifier <%s> but was <%s>", actual, identifier, actualIdentifier);
		}

		return myself;
	  }

	  public virtual S hasTargetElement(ModelElementInstance instance, ModelElementInstance targetElement)
	  {
		NotNull;

		ModelElementInstance actualTargetElement = actual.getReferenceTargetElement(instance);

		if (!targetElement.Equals(actualTargetElement))
		{
		  failWithMessage("Expected reference <%s> to have target element <%s> but was <%s>", actual, targetElement, actualTargetElement);
		}

		return myself;
	  }

	  public virtual S hasNoTargetElement(ModelElementInstance instance)
	  {
		NotNull;

		ModelElementInstance actualTargetElement = actual.getReferenceTargetElement(instance);

		if (actualTargetElement != null)
		{
		  failWithMessage("Expected reference <%s> to have no target element but has <%s>", actualTargetElement, actualTargetElement);
		}

		return myself;
	  }

	  public virtual S hasTargetAttribute<T1>(Attribute<T1> targetAttribute)
	  {
		NotNull;

		Attribute<string> actualTargetAttribute = actual.ReferenceTargetAttribute;

		if (!targetAttribute.Equals(actualTargetAttribute))
		{
		  failWithMessage("Expected reference <%s> to have target attribute <%s> but was <%s>", actual, targetAttribute, actualTargetAttribute);
		}

		return myself;
	  }
	}

}