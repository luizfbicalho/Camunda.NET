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
namespace org.camunda.bpm.model.bpmn.instance
{
	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;



	/// <summary>
	/// The BPMN process element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public interface Process : CallableElement
	{

	  ProcessBuilder builder();

	  ProcessType ProcessType {get;set;}


	  bool Closed {get;set;}


	  bool Executable {get;set;}


	  // TODO: collaboration ref

	  Auditing Auditing {get;set;}


	  Monitoring Monitoring {get;set;}


	  ICollection<Property> Properties {get;}

	  ICollection<LaneSet> LaneSets {get;}

	  ICollection<FlowElement> FlowElements {get;}

	  ICollection<Artifact> Artifacts {get;}

	  ICollection<CorrelationSubscription> CorrelationSubscriptions {get;}

	  ICollection<ResourceRole> ResourceRoles {get;}

	  ICollection<Process> Supports {get;}

	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaCandidateStarterGroups {get;set;}


	  IList<string> CamundaCandidateStarterGroupsList {get;set;}


	  string CamundaCandidateStarterUsers {get;set;}


	  IList<string> CamundaCandidateStarterUsersList {get;set;}


	  string CamundaJobPriority {get;set;}


	  string CamundaTaskPriority {get;set;}


	  [Obsolete]
	  Integer getCamundaHistoryTimeToLive();

	  [Obsolete]
	  void setCamundaHistoryTimeToLive(int? historyTimeToLive);

	  string CamundaHistoryTimeToLiveString {get;set;}


	  bool? CamundaStartableInTasklist {get;}

	  bool? CamundaIsStartableInTasklist {set;}

	  string CamundaVersionTag {get;set;}

	}

}