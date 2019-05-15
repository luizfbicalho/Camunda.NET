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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	public abstract class AbstractSetExternalTaskRetriesCmd<T> : Command<T>
	{
		public abstract T execute(CommandContext commandContext);

	  protected internal UpdateExternalTaskRetriesBuilderImpl builder;

	  public AbstractSetExternalTaskRetriesCmd(UpdateExternalTaskRetriesBuilderImpl builder)
	  {
		this.builder = builder;
	  }

	  protected internal virtual IList<string> collectProcessInstanceIds()
	  {

		ISet<string> collectedProcessInstanceIds = new HashSet<string>();

		IList<string> processInstanceIds = builder.ProcessInstanceIds;
		if (processInstanceIds != null && processInstanceIds.Count > 0)
		{
		  collectedProcessInstanceIds.addAll(processInstanceIds);
		}

		ProcessInstanceQueryImpl processInstanceQuery = (ProcessInstanceQueryImpl) builder.ProcessInstanceQuery;
		if (processInstanceQuery != null)
		{
		  collectedProcessInstanceIds.addAll(processInstanceQuery.listIds());
		}

		HistoricProcessInstanceQueryImpl historicProcessInstanceQuery = (HistoricProcessInstanceQueryImpl) builder.HistoricProcessInstanceQuery;
		if (historicProcessInstanceQuery != null)
		{
		  collectedProcessInstanceIds.addAll(historicProcessInstanceQuery.listIds());
		}

		return new List<string>(collectedProcessInstanceIds);
	  }

	  protected internal virtual IList<string> collectExternalTaskIds()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<String> collectedIds = new java.util.HashSet<String>();
		ISet<string> collectedIds = new HashSet<string>();

		IList<string> externalTaskIds = builder.ExternalTaskIds;
		if (externalTaskIds != null)
		{
		  ensureNotContainsNull(typeof(BadUserRequestException), "External task id cannot be null", "externalTaskIds", externalTaskIds);
		  collectedIds.addAll(externalTaskIds);
		}

		ExternalTaskQueryImpl externalTaskQuery = (ExternalTaskQueryImpl) builder.ExternalTaskQuery;
		if (externalTaskQuery != null)
		{
		  collectedIds.addAll(externalTaskQuery.listIds());
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> collectedProcessInstanceIds = collectProcessInstanceIds();
		IList<string> collectedProcessInstanceIds = collectProcessInstanceIds();
		if (collectedProcessInstanceIds.Count > 0)
		{

		  Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, collectedIds, collectedProcessInstanceIds));
		}

		return new List<string>(collectedIds);
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly AbstractSetExternalTaskRetriesCmd<T> outerInstance;

		  private ISet<string> collectedIds;
		  private IList<string> collectedProcessInstanceIds;

		  public CallableAnonymousInnerClass(AbstractSetExternalTaskRetriesCmd<T> outerInstance, ISet<string> collectedIds, IList<string> collectedProcessInstanceIds)
		  {
			  this.outerInstance = outerInstance;
			  this.collectedIds = collectedIds;
			  this.collectedProcessInstanceIds = collectedProcessInstanceIds;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			ExternalTaskQueryImpl query = new ExternalTaskQueryImpl();
			query.processInstanceIdIn(collectedProcessInstanceIds.ToArray());
			collectedIds.addAll(query.listIds());
			return null;
		  }

	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, int retries, int numInstances, bool async)
	  {

		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, async));
		propertyChanges.Add(new PropertyChange("retries", null, retries));

		commandContext.OperationLogManager.logExternalTaskOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_EXTERNAL_TASK_RETRIES, null, propertyChanges);
	  }
	}

}