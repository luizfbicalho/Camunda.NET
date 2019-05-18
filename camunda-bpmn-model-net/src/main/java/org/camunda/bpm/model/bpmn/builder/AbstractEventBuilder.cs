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
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractEventBuilder<B, E> : AbstractFlowNodeBuilder<B, E> where B : AbstractEventBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.Event
	{

	  protected internal AbstractEventBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Creates a new camunda input parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the input parameter </param>
	  /// <param name="value"> the value of the input parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInputParameter(string name, string value)
	  {
		CamundaInputOutput camundaInputOutput = getCreateSingleExtensionElement(typeof(CamundaInputOutput));

		CamundaInputParameter camundaInputParameter = createChild(camundaInputOutput, typeof(CamundaInputParameter));
		camundaInputParameter.CamundaName = name;
		camundaInputParameter.TextContent = value;

		return myself;
	  }

	  /// <summary>
	  /// Creates a new camunda output parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the output parameter </param>
	  /// <param name="value"> the value of the output parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaOutputParameter(string name, string value)
	  {
		CamundaInputOutput camundaInputOutput = getCreateSingleExtensionElement(typeof(CamundaInputOutput));

		CamundaOutputParameter camundaOutputParameter = createChild(camundaInputOutput, typeof(CamundaOutputParameter));
		camundaOutputParameter.CamundaName = name;
		camundaOutputParameter.TextContent = value;

		return myself;
	  }

	}

}