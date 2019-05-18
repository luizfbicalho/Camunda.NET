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
namespace org.camunda.bpm.model.bpmn.builder
{
	using ErrorEventDefinition = org.camunda.bpm.model.bpmn.instance.ErrorEventDefinition;
	using Event = org.camunda.bpm.model.bpmn.instance.Event;


	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// </summary>

	public abstract class AbstractErrorEventDefinitionBuilder<B> : AbstractRootElementBuilder<B, ErrorEventDefinition> where B : AbstractErrorEventDefinitionBuilder<B>
	{

	  public AbstractErrorEventDefinitionBuilder(BpmnModelInstance modelInstance, ErrorEventDefinition element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public override B id(string identifier)
	  {
		return base.id(identifier);
	  }

	  /// <summary>
	  /// Sets the error code variable attribute.
	  /// </summary>
	  public virtual B errorCodeVariable(string errorCodeVariable)
	  {
		element.CamundaErrorCodeVariable = errorCodeVariable;
		return myself;
	  }

	  /// <summary>
	  /// Sets the error message variable attribute.
	  /// </summary>
	  public virtual B errorMessageVariable(string errorMessageVariable)
	  {
		element.CamundaErrorMessageVariable = errorMessageVariable;
		return myself;
	  }

	  /// <summary>
	  /// Sets the error attribute with errorCode.
	  /// </summary>
	  public virtual B error(string errorCode)
	  {
		element.Error = findErrorForNameAndCode(errorCode);
		return myself;
	  }

	  /// <summary>
	  /// Finishes the building of a error event definition.
	  /// </summary>
	  /// @param <T> </param>
	  /// <returns> the parent event builder </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractFlowNodeBuilder> T errorEventDefinitionDone()
	  public virtual T errorEventDefinitionDone<T>() where T : AbstractFlowNodeBuilder
	  {
		return (T)((Event) element.ParentElement).builder();
	  }
	}

}