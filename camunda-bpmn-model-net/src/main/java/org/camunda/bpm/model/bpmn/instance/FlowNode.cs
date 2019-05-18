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
namespace org.camunda.bpm.model.bpmn.instance
{
	using AbstractFlowNodeBuilder = org.camunda.bpm.model.bpmn.builder.AbstractFlowNodeBuilder;

	/// <summary>
	/// The BPMN flowNode element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface FlowNode : FlowElement
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") org.camunda.bpm.model.bpmn.builder.AbstractFlowNodeBuilder builder();
	  AbstractFlowNodeBuilder builder();

	  ICollection<SequenceFlow> Incoming {get;}

	  ICollection<SequenceFlow> Outgoing {get;}

	  Query<FlowNode> PreviousNodes {get;}

	  Query<FlowNode> SucceedingNodes {get;}

	  bool CamundaAsyncBefore {get;set;}


	  bool CamundaAsyncAfter {get;set;}


	  bool CamundaExclusive {get;set;}


	  string CamundaJobPriority {get;set;}


	}

}