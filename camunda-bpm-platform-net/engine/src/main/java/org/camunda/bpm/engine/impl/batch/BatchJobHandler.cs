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
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;

	/// <summary>
	/// A batch job handler manages batch jobs based
	/// on the configuration <seealso cref="T"/>.
	/// 
	/// Used by a seed job to manage lifecycle of execution jobs.
	/// </summary>
	public interface BatchJobHandler<T> : JobHandler<BatchJobConfiguration>
	{

	  /// <summary>
	  /// Converts the configuration of the batch to a byte array.
	  /// </summary>
	  /// <param name="configuration"> the configuration object </param>
	  /// <returns> the serialized configuration </returns>
	  sbyte[] writeConfiguration(T configuration);

	  /// <summary>
	  /// Read the serialized configuration of the batch.
	  /// </summary>
	  /// <param name="serializedConfiguration"> the serialized configuration </param>
	  /// <returns> the deserialized configuration object </returns>
	  T readConfiguration(sbyte[] serializedConfiguration);

	  /// <summary>
	  /// Get the job declaration for batch jobs.
	  /// </summary>
	  /// <returns> the batch job declaration </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, org.camunda.bpm.engine.impl.persistence.entity.MessageEntity> getJobDeclaration();
	  JobDeclaration<object, MessageEntity> JobDeclaration {get;}

	  /// <summary>
	  /// Creates batch jobs for a batch.
	  /// </summary>
	  /// <param name="batch"> the batch to create jobs for </param>
	  /// <returns> true of no more jobs have to be created for this batch, false otherwise </returns>
	  bool createJobs(BatchEntity batch);

	  /// <summary>
	  /// Delete all jobs for a batch.
	  /// </summary>
	  /// <param name="batch"> the batch to delete jobs for </param>
	  void deleteJobs(BatchEntity batch);

	}

}