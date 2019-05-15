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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using FormEngine = org.camunda.bpm.engine.impl.form.engine.FormEngine;
	using StartFormHandler = org.camunda.bpm.engine.impl.form.handler.StartFormHandler;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetRenderedStartFormCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;
	  protected internal string formEngineName;
	  private static CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public GetRenderedStartFormCmd(string processDefinitionId, string formEngineName)
	  {
		this.processDefinitionId = processDefinitionId;
		this.formEngineName = formEngineName;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		ensureNotNull("Process Definition '" + processDefinitionId + "' not found", "processDefinition", processDefinition);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}

		StartFormHandler startFormHandler = processDefinition.StartFormHandler;
		if (startFormHandler == null)
		{
		  return null;
		}

		FormEngine formEngine = Context.ProcessEngineConfiguration.FormEngines[formEngineName];

		ensureNotNull("No formEngine '" + formEngineName + "' defined process engine configuration", "formEngine", formEngine);

		StartFormData startForm = startFormHandler.createStartFormData(processDefinition);

		object renderedStartForm = null;
		try
		{
		  renderedStartForm = formEngine.renderStartForm(startForm);
		}
		catch (ScriptEvaluationException e)
		{
		  LOG.exceptionWhenStartFormScriptEvaluation(processDefinitionId, e);
		}
		return renderedStartForm;
	  }
	}

}