﻿using System;

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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;

	/// <summary>
	/// JobEntity for ever living job, which can be rescheduled and executed again.
	/// 
	/// @author Svetlana Dorokhova
	/// </summary>
	[Serializable]
	public class EverLivingJobEntity : JobEntity
	{

	  private const long serialVersionUID = 1L;

	  private static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public const string TYPE = "ever-living";

	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  protected internal override void postExecute(CommandContext commandContext)
	  {
		LOG.debugJobExecuted(this);
		init(commandContext);
		commandContext.HistoricJobLogManager.fireJobSuccessfulEvent(this);
	  }

	  public override void init(CommandContext commandContext)
	  {
		init(commandContext, false);
	  }

	  public virtual void init(CommandContext commandContext, bool shouldResetLock)
	  {
		// clean additional data related to this job
		JobHandler jobHandler = JobHandler;
		if (jobHandler != null)
		{
		  jobHandler.onDelete(JobHandlerConfiguration, this);
		}

		//cancel the retries -> will resolve job incident if present
		Retries = commandContext.ProcessEngineConfiguration.DefaultNumberOfRetries;

		//delete the job's exception byte array and exception message
		string exceptionByteArrayIdToDelete = null;
		if (!string.ReferenceEquals(exceptionByteArrayId, null))
		{
		  exceptionByteArrayIdToDelete = exceptionByteArrayId;
		  this.exceptionByteArrayId = null;
		  this.exceptionMessage = null;
		}

		//clean the lock information
		if (shouldResetLock)
		{
		  LockOwner = null;
		  LockExpirationTime = null;
		}

		if (!string.ReferenceEquals(exceptionByteArrayIdToDelete, null))
		{
		  ByteArrayEntity byteArray = commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), exceptionByteArrayIdToDelete);
		  commandContext.DbEntityManager.delete(byteArray);
		}
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", duedate=" + duedate + ", lockOwner=" + lockOwner + ", lockExpirationTime=" + lockExpirationTime + ", executionId=" + executionId + ", processInstanceId=" + processInstanceId + ", isExclusive=" + isExclusive + ", retries=" + retries + ", jobHandlerType=" + jobHandlerType + ", jobHandlerConfiguration=" + jobHandlerConfiguration + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + exceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + deploymentId + "]";
	  }

	}

}