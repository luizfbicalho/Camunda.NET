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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using GetDeploymentResourceCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceCmd;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// Gives access to a deployed case diagram, e.g., a PNG image, through a stream
	/// of bytes.
	/// 
	/// @author Simon Zambrovski
	/// </summary>
	[Serializable]
	public class GetDeploymentCaseDiagramCmd : Command<Stream>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseDefinitionId;

	  public GetDeploymentCaseDiagramCmd(string caseDefinitionId)
	  {
		if (string.ReferenceEquals(caseDefinitionId, null) || caseDefinitionId.Length < 1)
		{
		  throw new ProcessEngineException("The case definition id is mandatory, but '" + caseDefinitionId + "' has been provided.");
		}
		this.caseDefinitionId = caseDefinitionId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public java.io.InputStream execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Stream execute(CommandContext commandContext)
	  {
		CaseDefinitionEntity caseDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadCaseDefinition(caseDefinition);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String deploymentId = caseDefinition.getDeploymentId();
		string deploymentId = caseDefinition.DeploymentId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName = caseDefinition.getDiagramResourceName();
		string resourceName = caseDefinition.DiagramResourceName;

		Stream caseDiagramStream = null;

		if (!string.ReferenceEquals(resourceName, null))
		{

		  caseDiagramStream = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, deploymentId, resourceName));
		}

		return caseDiagramStream;
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly GetDeploymentCaseDiagramCmd outerInstance;

		  private CommandContext commandContext;
		  private string deploymentId;
		  private string resourceName;

		  public CallableAnonymousInnerClass(GetDeploymentCaseDiagramCmd outerInstance, CommandContext commandContext, string deploymentId, string resourceName)
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