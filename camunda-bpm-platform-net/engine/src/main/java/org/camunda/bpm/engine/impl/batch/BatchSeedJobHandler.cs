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
namespace org.camunda.bpm.engine.impl.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using BatchSeedJobConfiguration = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler.BatchSeedJobConfiguration;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// The batch seed job handler is responsible to
	/// create all jobs to be executed by the batch.
	/// 
	/// If all jobs are created a seed monitor job is
	/// created to oversee the completion of the batch
	/// (see <seealso cref="BatchMonitorJobHandler"/>).
	/// </summary>
	public class BatchSeedJobHandler : JobHandler<BatchSeedJobConfiguration>
	{

	  public const string TYPE = "batch-seed-job";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(BatchSeedJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {

		string batchId = configuration.BatchId;
		BatchEntity batch = commandContext.BatchManager.findBatchById(batchId);
		ensureNotNull("Batch with id '" + batchId + "' cannot be found", "batch", batch);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: BatchJobHandler<?> batchJobHandler = commandContext.getProcessEngineConfiguration().getBatchHandlers().get(batch.getType());
		BatchJobHandler<object> batchJobHandler = commandContext.ProcessEngineConfiguration.BatchHandlers[batch.Type];

		bool done = batchJobHandler.createJobs(batch);

		if (!done)
		{
		  batch.createSeedJob();
		}
		else
		{
		  // create monitor job initially without due date to
		  // enable rapid completion of simple batches
		  batch.createMonitorJob(false);
		}
	  }

	  public virtual BatchSeedJobConfiguration newConfiguration(string canonicalString)
	  {
		return new BatchSeedJobConfiguration(canonicalString);
	  }

	  public class BatchSeedJobConfiguration : JobHandlerConfiguration
	  {
		protected internal string batchId;

		public BatchSeedJobConfiguration(string batchId)
		{
		  this.batchId = batchId;
		}

		public virtual string BatchId
		{
			get
			{
			  return batchId;
			}
		}

		public virtual string toCanonicalString()
		{
		  return batchId;
		}
	  }

	  public virtual void onDelete(BatchSeedJobConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}