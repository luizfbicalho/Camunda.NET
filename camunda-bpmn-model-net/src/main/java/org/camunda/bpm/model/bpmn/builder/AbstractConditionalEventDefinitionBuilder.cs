using System;
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
namespace org.camunda.bpm.model.bpmn.builder
{
	using Condition = org.camunda.bpm.model.bpmn.instance.Condition;
	using ConditionalEventDefinition = org.camunda.bpm.model.bpmn.instance.ConditionalEventDefinition;
	using Event = org.camunda.bpm.model.bpmn.instance.Event;

	/// <summary>
	/// Represents the abstract conditional event definition builder.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com> </summary>
	/// @param <B> </param>
	public class AbstractConditionalEventDefinitionBuilder<B> : AbstractRootElementBuilder<B, ConditionalEventDefinition> where B : AbstractConditionalEventDefinitionBuilder<B>
	{

	  public AbstractConditionalEventDefinitionBuilder(BpmnModelInstance modelInstance, ConditionalEventDefinition element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the condition of the conditional event definition.
	  /// </summary>
	  /// <param name="conditionText"> the condition which should be evaluate to true or false </param>
	  /// <returns> the builder object </returns>
	  public virtual B condition(string conditionText)
	  {
		Condition condition = createInstance(typeof(Condition));
		condition.TextContent = conditionText;
		element.Condition = condition;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda variable name attribute, that defines on
	  /// which variable the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableName"> the variable on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaVariableName(string variableName)
	  {
		element.CamundaVariableName = variableName;
		return myself;
	  }

	  /// <summary>
	  /// Set the camunda variable events attribute, that defines the variable
	  /// event on which the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableEvents"> the events on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaVariableEvents(string variableEvents)
	  {
		element.CamundaVariableEvents = variableEvents;
		return myself;
	  }

	  /// <summary>
	  /// Set the camunda variable events attribute, that defines the variable
	  /// event on which the condition should be evaluated.
	  /// </summary>
	  /// <param name="variableEvents"> the events on which the condition should be evaluated </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaVariableEvents(IList<string> variableEvents)
	  {
		element.CamundaVariableEventsList = variableEvents;
		return myself;
	  }

	  /// <summary>
	  /// Finishes the building of a conditional event definition.
	  /// </summary>
	  /// @param <T> </param>
	  /// <returns> the parent event builder </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractFlowNodeBuilder> T conditionalEventDefinitionDone()
	  public virtual T conditionalEventDefinitionDone<T>() where T : AbstractFlowNodeBuilder
	  {
		return (T)((Event) element.ParentElement).builder();
	  }

	}
}