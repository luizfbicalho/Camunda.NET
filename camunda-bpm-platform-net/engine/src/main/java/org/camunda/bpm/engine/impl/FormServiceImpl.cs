using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.impl
{

	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using GetDeployedStartFormCmd = org.camunda.bpm.engine.impl.cmd.GetDeployedStartFormCmd;
	using GetFormKeyCmd = org.camunda.bpm.engine.impl.cmd.GetFormKeyCmd;
	using GetRenderedStartFormCmd = org.camunda.bpm.engine.impl.cmd.GetRenderedStartFormCmd;
	using GetRenderedTaskFormCmd = org.camunda.bpm.engine.impl.cmd.GetRenderedTaskFormCmd;
	using GetStartFormCmd = org.camunda.bpm.engine.impl.cmd.GetStartFormCmd;
	using GetStartFormVariablesCmd = org.camunda.bpm.engine.impl.cmd.GetStartFormVariablesCmd;
	using GetTaskFormCmd = org.camunda.bpm.engine.impl.cmd.GetTaskFormCmd;
	using GetTaskFormVariablesCmd = org.camunda.bpm.engine.impl.cmd.GetTaskFormVariablesCmd;
	using SubmitStartFormCmd = org.camunda.bpm.engine.impl.cmd.SubmitStartFormCmd;
	using SubmitTaskFormCmd = org.camunda.bpm.engine.impl.cmd.SubmitTaskFormCmd;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge (camunda)
	/// </summary>
	public class FormServiceImpl : ServiceImpl, FormService
	{

	  public virtual object getRenderedStartForm(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetRenderedStartFormCmd(processDefinitionId, null));
	  }

	  public virtual object getRenderedStartForm(string processDefinitionId, string engineName)
	  {
		return commandExecutor.execute(new GetRenderedStartFormCmd(processDefinitionId, engineName));
	  }

	  public virtual object getRenderedTaskForm(string taskId)
	  {
		return commandExecutor.execute(new GetRenderedTaskFormCmd(taskId, null));
	  }

	  public virtual object getRenderedTaskForm(string taskId, string engineName)
	  {
		return commandExecutor.execute(new GetRenderedTaskFormCmd(taskId, engineName));
	  }

	  public virtual StartFormData getStartFormData(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetStartFormCmd(processDefinitionId));
	  }

	  public virtual TaskFormData getTaskFormData(string taskId)
	  {
		return commandExecutor.execute(new GetTaskFormCmd(taskId));
	  }

	  public virtual ProcessInstance submitStartFormData(string processDefinitionId, IDictionary<string, string> properties)
	  {
		return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, null, (System.Collections.IDictionary) properties));
	  }

	  public virtual ProcessInstance submitStartFormData(string processDefinitionId, string businessKey, IDictionary<string, string> properties)
	  {
		  return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, businessKey, (System.Collections.IDictionary) properties));
	  }

	  public virtual ProcessInstance submitStartForm(string processDefinitionId, IDictionary<string, object> properties)
	  {
		return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, null, properties));
	  }

	  public virtual ProcessInstance submitStartForm(string processDefinitionId, string businessKey, IDictionary<string, object> properties)
	  {
		return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, businessKey, properties));
	  }

	  public virtual void submitTaskFormData(string taskId, IDictionary<string, string> properties)
	  {
		submitTaskForm(taskId, (System.Collections.IDictionary) properties);
	  }

	  public virtual void submitTaskForm(string taskId, IDictionary<string, object> properties)
	  {
		commandExecutor.execute(new SubmitTaskFormCmd(taskId, properties, false, false));
	  }

	  public virtual VariableMap submitTaskFormWithVariablesInReturn(string taskId, IDictionary<string, object> properties, bool deserializeValues)
	  {
		return commandExecutor.execute(new SubmitTaskFormCmd(taskId, properties, true, deserializeValues));
	  }

	  public virtual string getStartFormKey(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetFormKeyCmd(processDefinitionId));
	  }

	  public virtual string getTaskFormKey(string processDefinitionId, string taskDefinitionKey)
	  {
		return commandExecutor.execute(new GetFormKeyCmd(processDefinitionId, taskDefinitionKey));
	  }

	  public virtual VariableMap getStartFormVariables(string processDefinitionId)
	  {
		return getStartFormVariables(processDefinitionId, null, true);
	  }

	  public virtual VariableMap getStartFormVariables(string processDefinitionId, ICollection<string> formVariables, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetStartFormVariablesCmd(processDefinitionId, formVariables, deserializeObjectValues));
	  }

	  public virtual VariableMap getTaskFormVariables(string taskId)
	  {
		return getTaskFormVariables(taskId, null, true);
	  }

	  public virtual VariableMap getTaskFormVariables(string taskId, ICollection<string> formVariables, bool deserializeObjectValues)
	  {
		return commandExecutor.execute(new GetTaskFormVariablesCmd(taskId, formVariables, deserializeObjectValues));
	  }

	  public virtual Stream getDeployedStartForm(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeployedStartFormCmd(processDefinitionId));
	  }

	  public virtual Stream getDeployedTaskForm(string taskId)
	  {
		return commandExecutor.execute(new GetDeployedTaskFormCmd(taskId));
	  }

	}

}