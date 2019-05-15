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

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// 
	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class RegisterProcessApplicationCmd : Command<ProcessApplicationRegistration>
	{

	  protected internal ProcessApplicationReference reference;
	  protected internal ISet<string> deploymentsToRegister;

	  public RegisterProcessApplicationCmd(string deploymentId, ProcessApplicationReference reference) : this(Collections.singleton(deploymentId), reference)
	  {
	  }

	  public RegisterProcessApplicationCmd(ISet<string> deploymentsToRegister, ProcessApplicationReference appReference)
	  {
		this.deploymentsToRegister = deploymentsToRegister;
		reference = appReference;

	  }

	  public virtual ProcessApplicationRegistration execute(CommandContext commandContext)
	  {
		commandContext.AuthorizationManager.checkCamundaAdmin();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.application.ProcessApplicationManager processApplicationManager = processEngineConfiguration.getProcessApplicationManager();
		ProcessApplicationManager processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

		return processApplicationManager.registerProcessApplicationForDeployments(deploymentsToRegister, reference);
	  }

	}

}