﻿/*
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
namespace org.camunda.bpm.engine.test.api.runtime.migration.models
{
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskModels
	{

	  public const string EXTERNAL_TASK_TYPE = "external";
	  public const string TOPIC = "foo";
	  public const int? PRIORITY = 100;

	  public static readonly BpmnModelInstance ONE_EXTERNAL_TASK_PROCESS = ProcessModels.newModel().startEvent().serviceTask("externalTask").camundaType(EXTERNAL_TASK_TYPE).camundaTopic(TOPIC).camundaTaskPriority(PRIORITY.ToString()).endEvent().done();

	  public static readonly BpmnModelInstance SUBPROCESS_PROCESS = ProcessModels.newModel().startEvent().subProcess().embeddedSubProcess().startEvent().serviceTask("externalTask").camundaType(EXTERNAL_TASK_TYPE).camundaTopic(TOPIC).camundaTaskPriority(PRIORITY.ToString()).endEvent().subProcessDone().endEvent().done();


	}

}