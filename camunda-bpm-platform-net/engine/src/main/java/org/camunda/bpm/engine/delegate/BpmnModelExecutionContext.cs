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
namespace org.camunda.bpm.engine.@delegate
{
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;

	/// <summary>
	/// Implemented by classes which provide access to the <seealso cref="BpmnModelInstance"/>
	/// and the currently executed <seealso cref="FlowElement"/>.
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public interface BpmnModelExecutionContext
	{

	  /// <summary>
	  /// Returns the <seealso cref="BpmnModelInstance"/> for the currently executed Bpmn Model
	  /// </summary>
	  /// <returns> the current <seealso cref="BpmnModelInstance"/> </returns>
	  BpmnModelInstance BpmnModelInstance {get;}

	  /// <summary>
	  /// <para>Returns the currently executed Element in the BPMN Model. This method returns a <seealso cref="FlowElement"/> which may be casted
	  /// to the concrete type of the Bpmn Model Element currently executed.</para>
	  /// 
	  /// <para>If called from a Service <seealso cref="ExecutionListener"/>, the method will return the corresponding <seealso cref="FlowNode"/>
	  /// for <seealso cref="ExecutionListener#EVENTNAME_START"/> and <seealso cref="ExecutionListener#EVENTNAME_END"/> and the corresponding
	  /// <seealso cref="SequenceFlow"/> for <seealso cref="ExecutionListener#EVENTNAME_TAKE"/>.</para>
	  /// </summary>
	  /// <returns> the <seealso cref="FlowElement"/> corresponding to the current Bpmn Model Element </returns>
	  FlowElement BpmnModelElementInstance {get;}

	}

}