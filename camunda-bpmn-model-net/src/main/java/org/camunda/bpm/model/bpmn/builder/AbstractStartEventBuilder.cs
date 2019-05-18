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
	using CompensateEventDefinition = org.camunda.bpm.model.bpmn.instance.CompensateEventDefinition;
	using ErrorEventDefinition = org.camunda.bpm.model.bpmn.instance.ErrorEventDefinition;
	using EscalationEventDefinition = org.camunda.bpm.model.bpmn.instance.EscalationEventDefinition;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using CamundaFormData = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormData;
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractStartEventBuilder<B> : AbstractCatchEventBuilder<B, StartEvent> where B : AbstractStartEventBuilder<B>
	{

	  protected internal AbstractStartEventBuilder(BpmnModelInstance modelInstance, StartEvent element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use camundaAsyncBefore() instead.
	  /// 
	  /// Sets the camunda async attribute to true.
	  /// 
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore() instead.")]
	  public virtual B camundaAsync()
	  {
		element.CamundaAsyncBefore = true;
		return myself;
	  }

	  /// @deprecated use camundaAsyncBefore(isCamundaAsyncBefore) instead.
	  /// 
	  /// Sets the camunda async attribute.
	  /// 
	  /// <param name="isCamundaAsync">  the async state of the task </param>
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore(isCamundaAsyncBefore) instead.")]
	  public virtual B camundaAsync(bool isCamundaAsync)
	  {
		element.CamundaAsyncBefore = isCamundaAsync;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda form handler class attribute.
	  /// </summary>
	  /// <param name="camundaFormHandlerClass">  the class name of the form handler </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaFormHandlerClass(string camundaFormHandlerClass)
	  {
		element.CamundaFormHandlerClass = camundaFormHandlerClass;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda form key attribute.
	  /// </summary>
	  /// <param name="camundaFormKey">  the form key to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaFormKey(string camundaFormKey)
	  {
		element.CamundaFormKey = camundaFormKey;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda initiator attribute.
	  /// </summary>
	  /// <param name="camundaInitiator">  the initiator to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInitiator(string camundaInitiator)
	  {
		element.CamundaInitiator = camundaInitiator;
		return myself;
	  }

	  /// <summary>
	  /// Creates a new camunda form field extension element.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual CamundaStartEventFormFieldBuilder camundaFormField()
	  {
		CamundaFormData camundaFormData = getCreateSingleExtensionElement(typeof(CamundaFormData));
		CamundaFormField camundaFormField = createChild(camundaFormData, typeof(CamundaFormField));
		return new CamundaStartEventFormFieldBuilder(modelInstance, element, camundaFormField);
	  }

	  /// <summary>
	  /// Sets a catch all error definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B error()
	  {
		ErrorEventDefinition errorEventDefinition = createInstance(typeof(ErrorEventDefinition));
		element.EventDefinitions.add(errorEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual B error(string errorCode)
	  {
		ErrorEventDefinition errorEventDefinition = createErrorEventDefinition(errorCode);
		element.EventDefinitions.add(errorEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Creates an error event definition with an unique id
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition(string id)
	  {
		ErrorEventDefinition errorEventDefinition = createEmptyErrorEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  errorEventDefinition.Id = id;
		}

		element.EventDefinitions.add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Creates an error event definition
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition()
	  {
		ErrorEventDefinition errorEventDefinition = createEmptyErrorEventDefinition();
		element.EventDefinitions.add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Sets a catch all escalation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B escalation()
	  {
		EscalationEventDefinition escalationEventDefinition = createInstance(typeof(EscalationEventDefinition));
		element.EventDefinitions.add(escalationEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an escalation
	  /// with this code exists it will be used, otherwise a new escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual B escalation(string escalationCode)
	  {
		EscalationEventDefinition escalationEventDefinition = createEscalationEventDefinition(escalationCode);
		element.EventDefinitions.add(escalationEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets a catch compensation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B compensation()
	  {
		CompensateEventDefinition compensateEventDefinition = createCompensateEventDefinition();
		element.EventDefinitions.add(compensateEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets whether the start event is interrupting or not.
	  /// </summary>
	  public virtual B interrupting(bool interrupting)
	  {
		element.Interrupting = interrupting;

		return myself;
	  }

	}

}