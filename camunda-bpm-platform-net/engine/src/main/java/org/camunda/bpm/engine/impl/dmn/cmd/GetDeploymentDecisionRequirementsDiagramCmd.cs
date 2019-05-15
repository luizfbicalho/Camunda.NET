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
namespace org.camunda.bpm.engine.impl.dmn.cmd
{

	using GetDeploymentResourceCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;


	/// <summary>
	/// Gives access to a deployed decision requirements diagram, e.g., a PNG image, through a stream of bytes.
	/// </summary>
	[Serializable]
	public class GetDeploymentDecisionRequirementsDiagramCmd : Command<Stream>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string decisionRequirementsDefinitionId;

	  public GetDeploymentDecisionRequirementsDiagramCmd(string decisionRequirementsDefinitionId)
	  {
		this.decisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Stream execute(CommandContext commandContext)
	  {
		DecisionRequirementsDefinition decisionRequirementsDefinition = (new GetDeploymentDecisionRequirementsDefinitionCmd(decisionRequirementsDefinitionId)).execute(commandContext);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String deploymentId = decisionRequirementsDefinition.getDeploymentId();
		string deploymentId = decisionRequirementsDefinition.DeploymentId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName = decisionRequirementsDefinition.getDiagramResourceName();
		string resourceName = decisionRequirementsDefinition.DiagramResourceName;

		if (!string.ReferenceEquals(resourceName, null))
		{
		  return commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, deploymentId, resourceName));
		}
		else
		{
		  return null;
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly GetDeploymentDecisionRequirementsDiagramCmd outerInstance;

		  private CommandContext commandContext;
		  private string deploymentId;
		  private string resourceName;

		  public CallableAnonymousInnerClass(GetDeploymentDecisionRequirementsDiagramCmd outerInstance, CommandContext commandContext, string deploymentId, string resourceName)
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