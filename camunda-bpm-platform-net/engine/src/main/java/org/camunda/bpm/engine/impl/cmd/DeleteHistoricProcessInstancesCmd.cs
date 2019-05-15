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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[Serializable]
	public class DeleteHistoricProcessInstancesCmd : Command<Void>
	{

	  protected internal readonly IList<string> processInstanceIds;
	  protected internal readonly bool failIfNotExists;

	  public DeleteHistoricProcessInstancesCmd(IList<string> processInstanceIds, bool failIfNotExists)
	  {
		this.processInstanceIds = processInstanceIds;
		this.failIfNotExists = failIfNotExists;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotEmpty(typeof(BadUserRequestException),"processInstanceIds", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "processInstanceId is null", "processInstanceIds", processInstanceIds);

		// Check if process instance is still running
		IList<HistoricProcessInstance> instances = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this));

		if (failIfNotExists)
		{
		  if (processInstanceIds.Count == 1)
		  {
			ensureNotEmpty(typeof(BadUserRequestException), "No historic process instance found with id: " + processInstanceIds[0], "historicProcessInstanceIds", instances);
		  }
		  else
		  {
			ensureNotEmpty(typeof(BadUserRequestException), "No historic process instances found", "historicProcessInstanceIds", instances);
		  }
		}

		IList<string> existingIds = new List<string>();

		foreach (HistoricProcessInstance historicProcessInstance in instances)
		{
		  existingIds.Add(historicProcessInstance.Id);

		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkDeleteHistoricProcessInstance(historicProcessInstance);
		  }

		  ensureNotNull(typeof(BadUserRequestException), "Process instance is still running, cannot delete historic process instance: " + historicProcessInstance, "instance.getEndTime()", historicProcessInstance.EndTime);
		}

		if (failIfNotExists)
		{
		  List<string> nonExistingIds = new List<string>(processInstanceIds);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		  nonExistingIds.removeAll(existingIds);
		  if (nonExistingIds.Count != 0)
		  {
			throw new BadUserRequestException("No historic process instance found with id: " + nonExistingIds);
		  }
		}

		if (existingIds.Count > 0)
		{
		  commandContext.HistoricProcessInstanceManager.deleteHistoricProcessInstanceByIds(existingIds);
		}
		writeUserOperationLog(commandContext, existingIds.Count);

		return null;
	  }

	  private class CallableAnonymousInnerClass : Callable<IList<HistoricProcessInstance>>
	  {
		  private readonly DeleteHistoricProcessInstancesCmd outerInstance;

		  public CallableAnonymousInnerClass(DeleteHistoricProcessInstancesCmd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<org.camunda.bpm.engine.history.HistoricProcessInstance> call() throws Exception
		  public override IList<HistoricProcessInstance> call()
		  {
			return (new HistoricProcessInstanceQueryImpl()).processInstanceIds(new HashSet<string>(outerInstance.processInstanceIds)).list();
		  }
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int numInstances)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, false));

		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, null, null, null, propertyChanges);
	  }
	}

}