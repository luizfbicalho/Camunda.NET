using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;


	/// <summary>
	/// Gives access to a deployed process model, e.g., a BPMN 2.0 XML file, through
	/// a stream of bytes.
	/// 
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class GetDeploymentProcessModelCmd : Command<Stream>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;

	  public GetDeploymentProcessModelCmd(string processDefinitionId)
	  {
		if (string.ReferenceEquals(processDefinitionId, null) || processDefinitionId.Length < 1)
		{
		  throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId + "' has been provided.");
		}
		this.processDefinitionId = processDefinitionId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Stream execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String deploymentId = processDefinition.getDeploymentId();
		string deploymentId = processDefinition.DeploymentId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName = processDefinition.getResourceName();
		string resourceName = processDefinition.ResourceName;

		Stream processModelStream = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, deploymentId, resourceName));

		return processModelStream;
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly GetDeploymentProcessModelCmd outerInstance;

		  private CommandContext commandContext;
		  private string deploymentId;
		  private string resourceName;

		  public CallableAnonymousInnerClass(GetDeploymentProcessModelCmd outerInstance, CommandContext commandContext, string deploymentId, string resourceName)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.deploymentId = deploymentId;
			  this.resourceName = resourceName;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream call() throws Exception
		  public Stream call()
		  {
			return (new GetDeploymentResourceCmd(deploymentId, resourceName)).execute(commandContext);
		  }
	  }

	}

}