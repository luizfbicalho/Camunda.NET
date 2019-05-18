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
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AbstractSubProcessBuilder<B> : AbstractActivityBuilder<B, SubProcess> where B : AbstractSubProcessBuilder<B>
	{

	  protected internal AbstractSubProcessBuilder(BpmnModelInstance modelInstance, SubProcess element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public virtual EmbeddedSubProcessBuilder embeddedSubProcess()
	  {
		return new EmbeddedSubProcessBuilder(this);
	  }

	  /// <summary>
	  /// Sets the sub process to be triggered by an event.
	  /// </summary>
	  /// <returns>  the builder object </returns>
	  public virtual B triggerByEvent()
	  {
		element.TriggeredByEvent = true;
		return myself;
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