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
namespace org.camunda.bpm.engine.test.api.runtime.migration.models.builder
{
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	public class DefaultExternalTaskModelBuilder
	{

	  public const string DEFAULT_PROCESS_KEY = "Process";
	  public const string DEFAULT_EXTERNAL_TASK_NAME = "externalTask";
	  public const string DEFAULT_EXTERNAL_TASK_TYPE = "external";
	  public const string DEFAULT_TOPIC = "foo";
	  public const int? DEFAULT_PRIORITY = 1;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processKey_Renamed = DEFAULT_PROCESS_KEY;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string externalTaskName_Renamed = DEFAULT_EXTERNAL_TASK_NAME;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string externalTaskType_Renamed = DEFAULT_EXTERNAL_TASK_TYPE;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string topic_Renamed = DEFAULT_TOPIC;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? priority_Renamed = DEFAULT_PRIORITY;

	  public static DefaultExternalTaskModelBuilder createDefaultExternalTaskModel()
	  {
		return new DefaultExternalTaskModelBuilder();
	  }

	  public virtual DefaultExternalTaskModelBuilder processKey(string processKey)
	  {
		this.processKey_Renamed = processKey;
		return this;
	  }

	  public virtual DefaultExternalTaskModelBuilder externalTaskName(string externalTaskName)
	  {
		this.externalTaskName_Renamed = externalTaskName;
		return this;
	  }

	  public virtual DefaultExternalTaskModelBuilder externalTaskType(string externalTaskType)
	  {
		this.externalTaskType_Renamed = externalTaskType;
		return this;
	  }

	  public virtual DefaultExternalTaskModelBuilder topic(string topic)
	  {
		this.topic_Renamed = topic;
		return this;
	  }

	  public virtual DefaultExternalTaskModelBuilder priority(int? priority)
	  {
		this.priority_Renamed = priority;
		return this;
	  }

	  public virtual BpmnModelInstance build()
	  {
		return ProcessModels.newModel(processKey_Renamed).startEvent().serviceTask(externalTaskName_Renamed).camundaType(externalTaskType_Renamed).camundaTopic(topic_Renamed).camundaTaskPriority(priority_Renamed.ToString()).endEvent().done();
	  }

	}

}