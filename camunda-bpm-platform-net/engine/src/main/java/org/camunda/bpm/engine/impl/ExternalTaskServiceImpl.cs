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
namespace org.camunda.bpm.engine.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using UpdateExternalTaskRetriesSelectBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesSelectBuilder;
	using org.camunda.bpm.engine.impl.cmd;
	using ExternalTaskQueryTopicBuilderImpl = org.camunda.bpm.engine.impl.externaltask.ExternalTaskQueryTopicBuilderImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// @author Askar Akhmerov
	/// </summary>
	public class ExternalTaskServiceImpl : ServiceImpl, ExternalTaskService
	{

	  public virtual ExternalTaskQueryBuilder fetchAndLock(int maxTasks, string workerId)
	  {
		return fetchAndLock(maxTasks, workerId, false);
	  }

	  public virtual ExternalTaskQueryBuilder fetchAndLock(int maxTasks, string workerId, bool usePriority)
	  {
		return new ExternalTaskQueryTopicBuilderImpl(commandExecutor, workerId, maxTasks, usePriority);
	  }

	  public virtual void complete(string externalTaskId, string workerId)
	  {
		complete(externalTaskId, workerId, null, null);
	  }

	  public virtual void complete(string externalTaskId, string workerId, IDictionary<string, object> variables)
	  {
		complete(externalTaskId, workerId, variables, null);
	  }

	  public virtual void complete(string externalTaskId, string workerId, IDictionary<string, object> variables, IDictionary<string, object> localVariables)
	  {
		commandExecutor.execute(new CompleteExternalTaskCmd(externalTaskId, workerId, variables, localVariables));
	  }

	  public virtual void handleFailure(string externalTaskId, string workerId, string errorMessage, int retries, long retryDuration)
	  {
		this.handleFailure(externalTaskId,workerId,errorMessage,null,retries,retryDuration);
	  }

	  public virtual void handleFailure(string externalTaskId, string workerId, string errorMessage, string errorDetails, int retries, long retryDuration)
	  {
		commandExecutor.execute(new HandleExternalTaskFailureCmd(externalTaskId, workerId, errorMessage, errorDetails, retries, retryDuration));
	  }

	  public virtual void handleBpmnError(string externalTaskId, string workerId, string errorCode)
	  {
		handleBpmnError(externalTaskId, workerId, errorCode, null, null);
	  }

	  public virtual void handleBpmnError(string externalTaskId, string workerId, string errorCode, string errorMessage)
	  {
		handleBpmnError(externalTaskId, workerId, errorCode, errorMessage, null);
	  }

	  public virtual void handleBpmnError(string externalTaskId, string workerId, string errorCode, string errorMessage, IDictionary<string, object> variables)
	  {
		commandExecutor.execute(new HandleExternalTaskBpmnErrorCmd(externalTaskId, workerId, errorCode, errorMessage, variables));
	  }

	  public virtual void unlock(string externalTaskId)
	  {
		commandExecutor.execute(new UnlockExternalTaskCmd(externalTaskId));
	  }

	  public virtual void setRetries(string externalTaskId, int retries, bool writeUserOperationLog)
	  {
		commandExecutor.execute(new SetExternalTaskRetriesCmd(externalTaskId, retries, writeUserOperationLog));
	  }

	  public virtual void setPriority(string externalTaskId, long priority)
	  {
		commandExecutor.execute(new SetExternalTaskPriorityCmd(externalTaskId, priority));
	  }

	  public virtual ExternalTaskQuery createExternalTaskQuery()
	  {
		return new ExternalTaskQueryImpl(commandExecutor);
	  }

	  public virtual string getExternalTaskErrorDetails(string externalTaskId)
	  {
		return commandExecutor.execute(new GetExternalTaskErrorDetailsCmd(externalTaskId));
	  }

	  public virtual void setRetries(string externalTaskId, int retries)
	  {
		setRetries(externalTaskId, retries, true);
	  }

	  public virtual void setRetries(IList<string> externalTaskIds, int retries)
	  {
		updateRetries().externalTaskIds(externalTaskIds).set(retries);
	  }

	  public virtual Batch setRetriesAsync(IList<string> externalTaskIds, ExternalTaskQuery externalTaskQuery, int retries)
	  {
		return updateRetries().externalTaskIds(externalTaskIds).externalTaskQuery(externalTaskQuery).setAsync(retries);
	  }

	  public virtual UpdateExternalTaskRetriesSelectBuilder updateRetries()
	  {
		return new UpdateExternalTaskRetriesBuilderImpl(commandExecutor);
	  }

	  public virtual void extendLock(string externalTaskId, string workerId, long lockDuration)
	  {
		commandExecutor.execute(new ExtendLockOnExternalTaskCmd(externalTaskId, workerId, lockDuration));
	  }

	}

}