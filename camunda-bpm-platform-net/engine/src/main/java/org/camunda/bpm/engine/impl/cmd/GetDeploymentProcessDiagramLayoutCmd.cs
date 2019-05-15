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

	using ProcessDiagramLayoutFactory = org.camunda.bpm.engine.impl.bpmn.diagram.ProcessDiagramLayoutFactory;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;


	/// <summary>
	/// Provides positions and dimensions of elements in a process diagram as
	/// provided by <seealso cref="GetDeploymentProcessDiagramCmd"/>.
	/// 
	/// This command requires a process model and a diagram image to be deployed.
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class GetDeploymentProcessDiagramLayoutCmd : Command<DiagramLayout>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;

	  public GetDeploymentProcessDiagramLayoutCmd(string processDefinitionId)
	  {
		if (string.ReferenceEquals(processDefinitionId, null) || processDefinitionId.Length < 1)
		{
		  throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId + "' has been provided.");
		}
		this.processDefinitionId = processDefinitionId;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.repository.DiagramLayout execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual DiagramLayout execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}

		Stream processModelStream = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));

		Stream processDiagramStream = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, commandContext));

		return (new ProcessDiagramLayoutFactory()).getProcessDiagramLayout(processModelStream, processDiagramStream);
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly GetDeploymentProcessDiagramLayoutCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(GetDeploymentProcessDiagramLayoutCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream call() throws Exception
		  public Stream call()
		  {
			return (new GetDeploymentProcessModelCmd(outerInstance.processDefinitionId)).execute(commandContext);
		  }
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Stream>
	  {
		  private readonly GetDeploymentProcessDiagramLayoutCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass2(GetDeploymentProcessDiagramLayoutCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream call() throws Exception
		  public Stream call()
		  {
			return (new GetDeploymentProcessDiagramCmd(outerInstance.processDefinitionId)).execute(commandContext);
		  }
	  }

	}

}