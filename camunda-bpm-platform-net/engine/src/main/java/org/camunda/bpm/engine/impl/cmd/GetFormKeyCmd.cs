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
namespace org.camunda.bpm.engine.impl.cmd
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DefaultStartFormHandler = org.camunda.bpm.engine.impl.form.handler.DefaultStartFormHandler;
	using DelegateStartFormHandler = org.camunda.bpm.engine.impl.form.handler.DelegateStartFormHandler;
	using FormHandler = org.camunda.bpm.engine.impl.form.handler.FormHandler;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;


	/// <summary>
	/// Command for retrieving start or task form keys.
	/// 
	/// @author Falko Menge (camunda)
	/// </summary>
	public class GetFormKeyCmd : Command<string>
	{

	  protected internal string taskDefinitionKey;
	  protected internal string processDefinitionId;

	  /// <summary>
	  /// Retrieves a start form key.
	  /// </summary>
	  public GetFormKeyCmd(string processDefinitionId)
	  {
		ProcessDefinitionId = processDefinitionId;
	  }

	  /// <summary>
	  /// Retrieves a task form key.
	  /// </summary>
	  public GetFormKeyCmd(string processDefinitionId, string taskDefinitionKey)
	  {
		ProcessDefinitionId = processDefinitionId;
		if (string.ReferenceEquals(taskDefinitionKey, null) || taskDefinitionKey.Length < 1)
		{
		  throw new ProcessEngineException("The task definition key is mandatory, but '" + taskDefinitionKey + "' has been provided.");
		}
		this.taskDefinitionKey = taskDefinitionKey;
	  }

	  protected internal virtual string ProcessDefinitionId
	  {
		  set
		  {
			if (string.ReferenceEquals(value, null) || value.Length < 1)
			{
			  throw new ProcessEngineException("The process definition id is mandatory, but '" + value + "' has been provided.");
			}
			this.processDefinitionId = value;
		  }
	  }

	  public virtual string execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}

		Expression formKeyExpression = null;

		if (string.ReferenceEquals(taskDefinitionKey, null))
		{

		  // TODO: Maybe add getFormKey() to FormHandler interface to avoid the following cast
		  FormHandler formHandler = processDefinition.StartFormHandler;

		  if (formHandler is DelegateStartFormHandler)
		  {
			DelegateStartFormHandler delegateFormHandler = (DelegateStartFormHandler) formHandler;
			formHandler = delegateFormHandler.FormHandler;
		  }

		  // Sorry!!! In case of a custom start form handler (which does not extend
		  // the DefaultFormHandler) a formKey would never be returned. So a custom
		  // form handler (for a startForm) has always to extend the DefaultStartFormHandler!
		  if (formHandler is DefaultStartFormHandler)
		  {
			DefaultStartFormHandler startFormHandler = (DefaultStartFormHandler) formHandler;
			formKeyExpression = startFormHandler.FormKey;
		  }

		}
		else
		{
		  TaskDefinition taskDefinition = processDefinition.TaskDefinitions[taskDefinitionKey];
		  formKeyExpression = taskDefinition.FormKey;
		}

		string formKey = null;
		if (formKeyExpression != null)
		{
		  formKey = formKeyExpression.ExpressionText;
		}
		return formKey;
	  }

	}

}