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

	using BatchMonitorJobConfiguration = org.camunda.bpm.engine.impl.batch.BatchMonitorJobHandler.BatchMonitorJobConfiguration;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// Job handler for batch monitor jobs. The batch monitor job
	/// polls for the completion of the batch.
	/// </summary>
	public class BatchMonitorJobHandler : JobHandler<BatchMonitorJobConfiguration>
	{

	  public const string TYPE = "batch-monitor-job";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(BatchMonitorJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {

		string batchId = configuration.BatchId;
		BatchEntity batch = commandContext.BatchManager.findBatchById(configuration.BatchId);
		ensureNotNull("Batch with id '" + batchId + "' cannot be found", "batch", batch);

		bool completed = batch.Completed;

		if (!completed)
		{
		  batch.createMonitorJob(true);
		}
		else
		{
		  batch.delete(false);
		}
	  }

	  public virtual BatchMonitorJobConfiguration newConfiguration(string canonicalString)
	  {
		return new BatchMonitorJobConfiguration(canonicalString);
	  }

	  public class BatchMonitorJobConfiguration : JobHandlerConfiguration
	  {
		protected internal string batchId;

		public BatchMonitorJobConfiguration(string batchId)
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

	  public virtual void onDelete(BatchMonitorJobConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}