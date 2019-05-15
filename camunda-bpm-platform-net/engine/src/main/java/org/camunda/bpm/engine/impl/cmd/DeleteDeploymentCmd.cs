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


	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using TransactionLogger = org.camunda.bpm.engine.impl.cfg.TransactionLogger;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeleteDeploymentFailListener = org.camunda.bpm.engine.impl.persistence.deploy.DeleteDeploymentFailListener;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;

	/// <summary>
	/// @author Joram Barrez
	/// @author Thorben Lindhauer
	/// </summary>
	[Serializable]
	public class DeleteDeploymentCmd : Command<Void>
	{

	  private static readonly TransactionLogger TX_LOG = ProcessEngineLogger.TX_LOGGER;

	  private const long serialVersionUID = 1L;

	  protected internal string deploymentId;
	  protected internal bool cascade;

	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;

	  public DeleteDeploymentCmd(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		this.deploymentId = deploymentId;
		this.cascade = cascade;
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("deploymentId", deploymentId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteDeployment(deploymentId);
		}

		UserOperationLogManager logManager = commandContext.OperationLogManager;
		IList<PropertyChange> propertyChanges = Arrays.asList(new PropertyChange("cascade", null, cascade));
		logManager.logDeploymentOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, deploymentId, propertyChanges);

		commandContext.DeploymentManager.deleteDeployment(deploymentId, cascade, skipCustomListeners, skipIoMappings);

		ProcessApplicationReference processApplicationReference = Context.ProcessEngineConfiguration.ProcessApplicationManager.getProcessApplicationForDeployment(deploymentId);

		DeleteDeploymentFailListener listener = new DeleteDeploymentFailListener(deploymentId, processApplicationReference, Context.ProcessEngineConfiguration.CommandExecutorTxRequiresNew);

		try
		{
		  commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
		}
		finally
		{
		  try
		  {
			commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, listener);
		  }
		  catch (Exception)
		  {
			TX_LOG.debugTransactionOperation("Could not register transaction synchronization. Probably the TX has already been rolled back by application code.");
			listener.execute(commandContext);
		  }
		}


		return null;
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DeleteDeploymentCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(DeleteDeploymentCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			(new UnregisterProcessApplicationCmd(outerInstance.deploymentId, false)).execute(commandContext);
			(new UnregisterDeploymentCmd(Collections.singleton(outerInstance.deploymentId))).execute(commandContext);
			return null;
		  }
	  }
	}

}