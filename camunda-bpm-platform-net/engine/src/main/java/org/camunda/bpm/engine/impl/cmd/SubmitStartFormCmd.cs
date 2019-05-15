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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using FormPropertyHelper = org.camunda.bpm.engine.impl.form.FormPropertyHelper;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstanceStartContext = org.camunda.bpm.engine.impl.pvm.runtime.ProcessInstanceStartContext;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SubmitStartFormCmd : Command<ProcessInstance>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string processDefinitionId;
	  protected internal readonly string businessKey;
	  protected internal VariableMap variables;

	  public SubmitStartFormCmd(string processDefinitionId, string businessKey, IDictionary<string, object> properties)
	  {
		this.processDefinitionId = processDefinitionId;
		this.businessKey = businessKey;
		this.variables = Variables.fromMap(properties);
	  }

	  public virtual ProcessInstance execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		ensureNotNull("No process definition found for id = '" + processDefinitionId + "'", "processDefinition", processDefinition);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateProcessInstance(processDefinition);
		}

		ExecutionEntity processInstance = null;
		if (!string.ReferenceEquals(businessKey, null))
		{
		  processInstance = processDefinition.createProcessInstance(businessKey);
		}
		else
		{
		  processInstance = processDefinition.createProcessInstance();
		}

		// if the start event is async, we have to set the variables already here
		// since they are lost after the async continuation otherwise
		// see CAM-2828
		if (processDefinition.Initial.AsyncBefore)
		{
		  // avoid firing history events
		  processInstance.StartContext = new ProcessInstanceStartContext(processInstance.getActivity());
		  FormPropertyHelper.initFormPropertiesOnScope(variables, processInstance);
		  processInstance.start();

		}
		else
		{
		  processInstance.startWithFormProperties(variables);

		}


		return processInstance;
	  }
	}

}