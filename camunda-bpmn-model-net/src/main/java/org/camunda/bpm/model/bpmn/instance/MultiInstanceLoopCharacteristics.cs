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

	using MultiInstanceLoopCharacteristicsBuilder = org.camunda.bpm.model.bpmn.builder.MultiInstanceLoopCharacteristicsBuilder;

	/// <summary>
	/// The BPMN 2.0 multiInstanceLoopCharacteristics element type
	/// 
	/// @author Filip Hrisafov
	/// 
	/// </summary>
	public interface MultiInstanceLoopCharacteristics : LoopCharacteristics
	{

	  LoopCardinality LoopCardinality {get;set;}


	  DataInput LoopDataInputRef {get;set;}


	  DataOutput LoopDataOutputRef {get;set;}


	  InputDataItem InputDataItem {get;set;}


	  OutputDataItem OutputDataItem {get;set;}


	  ICollection<ComplexBehaviorDefinition> ComplexBehaviorDefinitions {get;}

	  CompletionCondition CompletionCondition {get;set;}


	  bool Sequential {get;set;}


	  MultiInstanceFlowCondition Behavior {get;set;}


	  EventDefinition OneBehaviorEventRef {get;set;}


	  EventDefinition NoneBehaviorEventRef {get;set;}


	  string CamundaCollection {get;set;}


	  string CamundaElementVariable {get;set;}


	  bool CamundaAsyncBefore {get;set;}


	  bool CamundaAsyncAfter {get;set;}


	  bool CamundaExclusive {get;set;}


	  MultiInstanceLoopCharacteristicsBuilder builder();

	}

}