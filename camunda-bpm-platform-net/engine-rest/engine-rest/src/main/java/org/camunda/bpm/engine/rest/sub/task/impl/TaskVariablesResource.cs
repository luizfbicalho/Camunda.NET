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
namespace org.camunda.bpm.engine.rest.sub.task.impl
{

	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using AbstractVariablesResource = org.camunda.bpm.engine.rest.sub.impl.AbstractVariablesResource;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TaskVariablesResource : AbstractVariablesResource
	{

	  public TaskVariablesResource(ProcessEngine engine, string resourceId, ObjectMapper objectMapper) : base(engine, resourceId, objectMapper)
	  {
	  }

	  protected internal override string ResourceTypeName
	  {
		  get
		  {
			return "task";
		  }
	  }

	  protected internal override TypedValue getVariableEntity(string variableKey, bool deserializeValue)
	  {
		return engine.TaskService.getVariableTyped(resourceId, variableKey, deserializeValue);
	  }

	  protected internal override VariableMap getVariableEntities(bool deserializeValues)
	  {
		return engine.TaskService.getVariablesTyped(resourceId, deserializeValues);
	  }

	  protected internal override void removeVariableEntity(string variableKey)
	  {
		engine.TaskService.removeVariable(resourceId, variableKey);
	  }

	  protected internal override void updateVariableEntities(VariableMap modifications, IList<string> deletions)
	  {
		TaskServiceImpl taskService = (TaskServiceImpl) engine.TaskService;
		taskService.updateVariables(resourceId, modifications, deletions);
	  }

	  protected internal override void setVariableEntity(string variableKey, TypedValue variableValue)
	  {
		engine.TaskService.setVariable(resourceId, variableKey, variableValue);
	  }

	}

}