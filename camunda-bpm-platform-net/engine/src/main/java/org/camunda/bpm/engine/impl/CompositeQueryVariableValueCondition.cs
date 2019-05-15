using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompositeQueryVariableValueCondition : AbstractQueryVariableValueCondition
	{

	  protected internal IList<SingleQueryVariableValueCondition> aggregatedValues = new List<SingleQueryVariableValueCondition>();

	  public CompositeQueryVariableValueCondition(QueryVariableValue variableValue) : base(variableValue)
	  {
	  }

	  public override void initializeValue(VariableSerializers serializers)
	  {
		TypedValue typedValue = wrappedQueryValue.TypedValue;

		ValueTypeResolver resolver = Context.ProcessEngineConfiguration.ValueTypeResolver;
		ICollection<ValueType> concreteTypes = resolver.getSubTypes(typedValue.Type);

		foreach (ValueType type in concreteTypes)
		{
		  if (type.canConvertFromTypedValue(typedValue))
		  {
			TypedValue convertedValue = type.convertFromTypedValue(typedValue);
			SingleQueryVariableValueCondition aggregatedValue = new SingleQueryVariableValueCondition(wrappedQueryValue);
			aggregatedValue.initializeValue(serializers, convertedValue);
			aggregatedValues.Add(aggregatedValue);
		  }
		}
	  }

	  public override IList<SingleQueryVariableValueCondition> DisjunctiveConditions
	  {
		  get
		  {
			return aggregatedValues;
		  }
	  }

	}

}