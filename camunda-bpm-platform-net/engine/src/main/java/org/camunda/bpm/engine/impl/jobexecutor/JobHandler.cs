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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public interface JobHandler<T> where T : JobHandlerConfiguration
	{

	  string Type {get;}

	  void execute(T configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId);

	  T newConfiguration(string canonicalString);

	  /// <summary>
	  /// Clean up before job is deleted. Like removing of auxiliary entities specific for this job handler.
	  /// </summary>
	  /// <param name="configuration"> the job handler configuration </param>
	  /// <param name="jobEntity"> the job entity to be deleted </param>
	  void onDelete(T configuration, JobEntity jobEntity);

	}

}