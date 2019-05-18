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

	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using CamundaFormData = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormData;
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractUserTaskBuilder<B> : AbstractTaskBuilder<B, UserTask> where B : AbstractUserTaskBuilder<B>
	{

	  protected internal AbstractUserTaskBuilder(BpmnModelInstance modelInstance, UserTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the build user task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B implementation(string implementation)
	  {
		element.Implementation = implementation;
		return myself;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda attribute assignee.
	  /// </summary>
	  /// <param name="camundaAssignee">  the assignee to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaAssignee(string camundaAssignee)
	  {
		element.CamundaAssignee = camundaAssignee;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda candidate groups attribute.
	  /// </summary>
	  /// <param name="camundaCandidateGroups">  the candidate groups to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCandidateGroups(string camundaCandidateGroups)
	  {
		element.CamundaCandidateGroups = camundaCandidateGroups;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda candidate groups attribute.
	  /// </summary>
	  /// <param name="camundaCandidateGroups">  the candidate groups to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCandidateGroups(IList<string> camundaCandidateGroups)
	  {
		element.CamundaCandidateGroupsList = camundaCandidateGroups;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda candidate users attribute.
	  /// </summary>
	  /// <param name="camundaCandidateUsers">  the candidate users to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCandidateUsers(string camundaCandidateUsers)
	  {
		element.CamundaCandidateUsers = camundaCandidateUsers;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda candidate users attribute.
	  /// </summary>
	  /// <param name="camundaCandidateUsers">  the candidate users to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCandidateUsers(IList<string> camundaCandidateUsers)
	  {
		element.CamundaCandidateUsersList = camundaCandidateUsers;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda due date attribute.
	  /// </summary>
	  /// <param name="camundaDueDate">  the due date of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDueDate(string camundaDueDate)
	  {
		element.CamundaDueDate = camundaDueDate;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda follow up date attribute.
	  /// </summary>
	  /// <param name="camundaFollowUpDate">  the follow up date of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaFollowUpDate(string camundaFollowUpDate)
	  {
		element.CamundaFollowUpDate = camundaFollowUpDate;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda form handler class attribute.
	  /// </summary>
	  /// <param name="camundaFormHandlerClass">  the class name of the form handler </param>
	  /// <returns> the builder object </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public B camundaFormHandlerClass(Class camundaFormHandlerClass)
	  public virtual B camundaFormHandlerClass(Type camundaFormHandlerClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaFormHandlerClass(camundaFormHandlerClass.FullName);
	  }

	  /// <summary>
	  /// Sets the camunda form handler class attribute.
	  /// </summary>
	  /// <param name="camundaFormHandlerClass">  the class name of the form handler </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaFormHandlerClass(string fullQualifiedClassName)
	  {
		element.CamundaFormHandlerClass = fullQualifiedClassName;
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
	  /// Sets the camunda priority attribute.
	  /// </summary>
	  /// <param name="camundaPriority">  the priority of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaPriority(string camundaPriority)
	  {
		element.CamundaPriority = camundaPriority;
		return myself;
	  }

	  /// <summary>
	  /// Creates a new camunda form field extension element.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual CamundaUserTaskFormFieldBuilder camundaFormField()
	  {
		CamundaFormData camundaFormData = getCreateSingleExtensionElement(typeof(CamundaFormData));
		CamundaFormField camundaFormField = createChild(camundaFormData, typeof(CamundaFormField));
		return new CamundaUserTaskFormFieldBuilder(modelInstance, element, camundaFormField);
	  }

	  /// <summary>
	  /// Add a class based task listener with specified event name
	  /// </summary>
	  /// <param name="eventName"> - event names to listen to </param>
	  /// <param name="fullQualifiedClassName"> - a string representing a class </param>
	  /// <returns> the builder object </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public B camundaTaskListenerClass(String eventName, Class listenerClass)
	  public virtual B camundaTaskListenerClass(string eventName, Type listenerClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaTaskListenerClass(eventName, listenerClass.FullName);
	  }

	  /// <summary>
	  /// Add a class based task listener with specified event name
	  /// </summary>
	  /// <param name="eventName"> - event names to listen to </param>
	  /// <param name="fullQualifiedClassName"> - a string representing a class </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTaskListenerClass(string eventName, string fullQualifiedClassName)
	  {
		CamundaTaskListener executionListener = createInstance(typeof(CamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaClass = fullQualifiedClassName;

		addExtensionElement(executionListener);

		return myself;
	  }

	  public virtual B camundaTaskListenerExpression(string eventName, string expression)
	  {
		CamundaTaskListener executionListener = createInstance(typeof(CamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaExpression = expression;

		addExtensionElement(executionListener);

		return myself;
	  }

	  public virtual B camundaTaskListenerDelegateExpression(string eventName, string delegateExpression)
	  {
		CamundaTaskListener executionListener = createInstance(typeof(CamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaDelegateExpression = delegateExpression;

		addExtensionElement(executionListener);

		return myself;
	  }
	}

}