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
	using ExtensionElements = org.camunda.bpm.model.bpmn.instance.ExtensionElements;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractTaskBuilder<B, E> : AbstractActivityBuilder<B, E> where B : AbstractTaskBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.Task
	{

	  protected internal AbstractTaskBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
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

	}

}