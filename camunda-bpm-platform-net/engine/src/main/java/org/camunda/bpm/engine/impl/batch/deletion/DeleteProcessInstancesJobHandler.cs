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
namespace org.camunda.bpm.engine.impl.batch.deletion
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ByteArrayManager = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayManager;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteProcessInstancesJobHandler : AbstractBatchJobHandler<DeleteProcessInstanceBatchConfiguration>
	{

	  public static readonly BatchJobDeclaration JOB_DECLARATION = new BatchJobDeclaration(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_DELETION);

	  public override string Type
	  {
		  get
		  {
			return org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_DELETION;
		  }
	  }

	  protected internal override DeleteProcessInstanceBatchConfigurationJsonConverter JsonConverterInstance
	  {
		  get
		  {
			return DeleteProcessInstanceBatchConfigurationJsonConverter.INSTANCE;
		  }
	  }

	  public override JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration
	  {
		  get
		  {
			return JOB_DECLARATION;
		  }
	  }

	  protected internal override DeleteProcessInstanceBatchConfiguration createJobConfiguration(DeleteProcessInstanceBatchConfiguration configuration, IList<string> processIdsForJob)
	  {
		return new DeleteProcessInstanceBatchConfiguration(processIdsForJob, configuration.DeleteReason, configuration.SkipCustomListeners, configuration.SkipSubprocesses, configuration.FailIfNotExists);
	  }

	  public override void execute(BatchJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		ByteArrayEntity configurationEntity = commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), configuration.ConfigurationByteArrayId);

		DeleteProcessInstanceBatchConfiguration batchConfiguration = readConfiguration(configurationEntity.Bytes);

		bool initialLegacyRestrictions = commandContext.RestrictUserOperationLogToAuthenticatedUsers;
		commandContext.disableUserOperationLog();
		commandContext.RestrictUserOperationLogToAuthenticatedUsers = true;
		try
		{
		  RuntimeService runtimeService = commandContext.ProcessEngineConfiguration.RuntimeService;
		  if (batchConfiguration.FailIfNotExists)
		  {
			runtimeService.deleteProcessInstances(batchConfiguration.Ids, batchConfiguration.deleteReason, batchConfiguration.SkipCustomListeners, true, batchConfiguration.SkipSubprocesses);
		  }
		  else
		  {
			runtimeService.deleteProcessInstancesIfExists(batchConfiguration.Ids, batchConfiguration.deleteReason, batchConfiguration.SkipCustomListeners, true, batchConfiguration.SkipSubprocesses);
		  }
		}
		finally
		{
		  commandContext.enableUserOperationLog();
		  commandContext.RestrictUserOperationLogToAuthenticatedUsers = initialLegacyRestrictions;
		}

		commandContext.ByteArrayManager.delete(configurationEntity);
	  }

	  public override bool createJobs(BatchEntity batch)
	  {
		DeleteProcessInstanceBatchConfiguration configuration = readConfiguration(batch.ConfigurationBytes);

		IList<string> ids = configuration.Ids;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;

		int batchJobsPerSeed = batch.BatchJobsPerSeed;
		int invocationsPerBatchJob = batch.InvocationsPerBatchJob;

		int numberOfItemsToProcess = Math.Min(invocationsPerBatchJob * batchJobsPerSeed, ids.Count);
		// view of process instances to process
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> processIds = ids.subList(0, numberOfItemsToProcess);
		IList<string> processIds = ids.subList(0, numberOfItemsToProcess);

		IList<string> deploymentIds = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, processIds));

		foreach (String deploymentId in deploymentIds)
		{

		  IList<String> processIdsPerDeployment = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, commandContext, processIds));

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		  processIds.removeAll(processIdsPerDeployment);

		  createJobEntities(batch, configuration, deploymentId, processIdsPerDeployment, invocationsPerBatchJob);
		}

		// when there are non existing process instance ids
		if (processIds.Count > 0)
		{
		  createJobEntities(batch, configuration, null, processIds, invocationsPerBatchJob);
		}

		return ids.Count == 0;
	  }

	  private class CallableAnonymousInnerClass : Callable<IList<string>>
	  {
		  private readonly DeleteProcessInstancesJobHandler outerInstance;

		  private CommandContext commandContext;
		  private IList<string> processIds;

		  public CallableAnonymousInnerClass(DeleteProcessInstancesJobHandler outerInstance, CommandContext commandContext, IList<string> processIds)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.processIds = processIds;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<String> call() throws Exception
		  public override IList<string> call()
		  {
			return commandContext.DeploymentManager.findDeploymentIdsByProcessInstances(processIds);
		  }
	  }

	  private class CallableAnonymousInnerClass2 : Callable<IList<string>>
	  {
		  private readonly DeleteProcessInstancesJobHandler outerInstance;

		  private CommandContext commandContext;
		  private IList<string> processIds;

		  public CallableAnonymousInnerClass2(DeleteProcessInstancesJobHandler outerInstance, CommandContext commandContext, IList<string> processIds)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.processIds = processIds;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<String> call() throws Exception
		  public override IList<string> call()
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQueryToBeProcess = new org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl();
			ProcessInstanceQueryImpl processInstanceQueryToBeProcess = new ProcessInstanceQueryImpl();
			processInstanceQueryToBeProcess.processInstanceIds(new HashSet<string>(processIds)).deploymentId(deploymentId);
			return commandContext.ExecutionManager.findProcessInstancesIdsByQueryCriteria(processInstanceQueryToBeProcess);
		  }
	  }

	  protected internal virtual void createJobEntities(BatchEntity batch, DeleteProcessInstanceBatchConfiguration configuration, string deploymentId, IList<string> processInstancesToHandle, int invocationsPerBatchJob)
	  {


		CommandContext commandContext = Context.CommandContext;
		ByteArrayManager byteArrayManager = commandContext.ByteArrayManager;
		JobManager jobManager = commandContext.JobManager;

		int createdJobs = 0;
		while (processInstancesToHandle.Count > 0)
		{
		  int lastIdIndex = Math.Min(invocationsPerBatchJob, processInstancesToHandle.Count);
		  // view of process instances for this job
		  IList<string> idsForJob = processInstancesToHandle.subList(0, lastIdIndex);

		  DeleteProcessInstanceBatchConfiguration jobConfiguration = createJobConfiguration(configuration, idsForJob);
		  ByteArrayEntity configurationEntity = saveConfiguration(byteArrayManager, jobConfiguration);

		  JobEntity job = createBatchJob(batch, configurationEntity);
		  job.DeploymentId = deploymentId;

		  jobManager.insertAndHintJobExecutor(job);
		  createdJobs++;

		  idsForJob.Clear();
		}

		// update created jobs for batch
		batch.JobsCreated = batch.JobsCreated + createdJobs;

		// update batch configuration
		batch.ConfigurationBytes = writeConfiguration(configuration);
	  }
	}

}