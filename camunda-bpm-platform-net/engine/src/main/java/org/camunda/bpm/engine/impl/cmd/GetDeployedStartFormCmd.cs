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

	using FormData = org.camunda.bpm.engine.form.FormData;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class GetDeployedStartFormCmd : AbstractGetDeployedFormCmd
	{

	  protected internal string processDefinitionId;

	  public GetDeployedStartFormCmd(string processDefinitionId)
	  {
		EnsureUtil.ensureNotNull(typeof(BadUserRequestException), "Process definition id cannot be null", "processDefinitionId", processDefinitionId);
		this.processDefinitionId = processDefinitionId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected org.camunda.bpm.engine.form.FormData getFormData(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal override FormData getFormData(CommandContext commandContext)
	  {
		return commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
	  }

	  private class CallableAnonymousInnerClass : Callable<FormData>
	  {
		  private readonly GetDeployedStartFormCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(GetDeployedStartFormCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.form.FormData call() throws Exception
		  public override FormData call()
		  {
			return (new GetStartFormCmd(outerInstance.processDefinitionId)).execute(commandContext);
		  }
	  }

	  protected internal override void checkAuthorization(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}
	  }
	}

}