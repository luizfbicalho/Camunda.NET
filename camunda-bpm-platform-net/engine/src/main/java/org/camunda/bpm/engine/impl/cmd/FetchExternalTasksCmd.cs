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

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using OptimisticLockingListener = org.camunda.bpm.engine.impl.db.entitymanager.OptimisticLockingListener;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using LockedExternalTaskImpl = org.camunda.bpm.engine.impl.externaltask.LockedExternalTaskImpl;
	using TopicFetchInstruction = org.camunda.bpm.engine.impl.externaltask.TopicFetchInstruction;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// 
	/// </summary>
	public class FetchExternalTasksCmd : Command<IList<LockedExternalTask>>
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string workerId;
	  protected internal int maxResults;
	  protected internal bool usePriority;
	  protected internal IDictionary<string, TopicFetchInstruction> fetchInstructions = new Dictionary<string, TopicFetchInstruction>();

	  public FetchExternalTasksCmd(string workerId, int maxResults, IDictionary<string, TopicFetchInstruction> instructions) : this(workerId, maxResults, instructions, false)
	  {
	  }

	  public FetchExternalTasksCmd(string workerId, int maxResults, IDictionary<string, TopicFetchInstruction> instructions, bool usePriority)
	  {
		this.workerId = workerId;
		this.maxResults = maxResults;
		this.fetchInstructions = instructions;
		this.usePriority = usePriority;
	  }

	  public virtual IList<LockedExternalTask> execute(CommandContext commandContext)
	  {
		validateInput();

		foreach (TopicFetchInstruction instruction in fetchInstructions.Values)
		{
		  instruction.ensureVariablesInitialized();
		}

		IList<ExternalTaskEntity> externalTasks = commandContext.ExternalTaskManager.selectExternalTasksForTopics(fetchInstructions.Values, maxResults, usePriority);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<org.camunda.bpm.engine.externaltask.LockedExternalTask> result = new ArrayList<org.camunda.bpm.engine.externaltask.LockedExternalTask>();
		IList<LockedExternalTask> result = new List<LockedExternalTask>();

		foreach (ExternalTaskEntity entity in externalTasks)
		{

		  TopicFetchInstruction fetchInstruction = fetchInstructions[entity.TopicName];
		  entity.@lock(workerId, fetchInstruction.LockDuration.Value);

		  LockedExternalTaskImpl resultTask = LockedExternalTaskImpl.fromEntity(entity, fetchInstruction.VariablesToFetch, fetchInstruction.LocalVariables, fetchInstruction.DeserializeVariables);

		  result.Add(resultTask);
		}

		filterOnOptimisticLockingFailure(commandContext, result);

		return result;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void filterOnOptimisticLockingFailure(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, final List<org.camunda.bpm.engine.externaltask.LockedExternalTask> tasks)
	  protected internal virtual void filterOnOptimisticLockingFailure(CommandContext commandContext, IList<LockedExternalTask> tasks)
	  {
		commandContext.DbEntityManager.registerOptimisticLockingListener(new OptimisticLockingListenerAnonymousInnerClass(this, tasks));
	  }

	  private class OptimisticLockingListenerAnonymousInnerClass : OptimisticLockingListener
	  {
		  private readonly FetchExternalTasksCmd outerInstance;

		  private IList<LockedExternalTask> tasks;

		  public OptimisticLockingListenerAnonymousInnerClass(FetchExternalTasksCmd outerInstance, IList<LockedExternalTask> tasks)
		  {
			  this.outerInstance = outerInstance;
			  this.tasks = tasks;
		  }


		  public Type EntityType
		  {
			  get
			  {
				return typeof(ExternalTaskEntity);
			  }
		  }

		  public void failedOperation(DbOperation operation)
		  {
			if (operation is DbEntityOperation)
			{
			  DbEntityOperation dbEntityOperation = (DbEntityOperation) operation;
			  DbEntity dbEntity = dbEntityOperation.Entity;

			  bool failedOperationEntityInList = false;

			  IEnumerator<LockedExternalTask> it = tasks.GetEnumerator();
			  while (it.MoveNext())
			  {
				LockedExternalTask resultTask = it.Current;
				if (resultTask.Id.Equals(dbEntity.Id))
				{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
				  it.remove();
				  failedOperationEntityInList = true;
				  break;
				}
			  }

			  if (!failedOperationEntityInList)
			  {
				throw LOG.concurrentUpdateDbEntityException(operation);
			  }
			}
		  }
	  }

	  protected internal virtual void validateInput()
	  {
		EnsureUtil.ensureNotNull("workerId", workerId);
		EnsureUtil.ensureGreaterThanOrEqual("maxResults", maxResults, 0);

		foreach (TopicFetchInstruction instruction in fetchInstructions.Values)
		{
		  EnsureUtil.ensureNotNull("topicName", instruction.TopicName);
		  EnsureUtil.ensurePositive("lockTime", instruction.LockDuration);
		}
	  }
	}

}