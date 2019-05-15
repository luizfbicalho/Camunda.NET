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
namespace org.camunda.bpm.engine.management
{
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="JobDefinition"/>s.
	/// 
	/// @author roman.smirnov
	/// </summary>
	public interface JobDefinitionQuery : Query<JobDefinitionQuery, JobDefinition>
	{

	  /// <summary>
	  /// Only select job definitions with the given id </summary>
	  JobDefinitionQuery jobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// Only select job definitions which exist for the listed activity ids </summary>
	  JobDefinitionQuery activityIdIn(params string[] activityIds);

	  /// <summary>
	  /// Only select job definitions which exist for the given process definition id. * </summary>
	  JobDefinitionQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select job definitions which exist for the given process definition key. * </summary>
	  JobDefinitionQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select job definitions which have the given job type. * </summary>
	  JobDefinitionQuery jobType(string jobType);

	  /// <summary>
	  /// Only select job definitions which contain the configuration. * </summary>
	  JobDefinitionQuery jobConfiguration(string jobConfiguration);

	  /// <summary>
	  /// Only selects job definitions which are active * </summary>
	  JobDefinitionQuery active();

	  /// <summary>
	  /// Only selects job definitions which are suspended * </summary>
	  JobDefinitionQuery suspended();

	  /// <summary>
	  /// Only selects job definitions which have a job priority defined.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  JobDefinitionQuery withOverridingJobPriority();

	  /// <summary>
	  /// Only select job definitions that belong to one of the given tenant ids. </summary>
	  JobDefinitionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select job definitions which have no tenant id. </summary>
	  JobDefinitionQuery withoutTenantId();

	  /// <summary>
	  /// Select job definitions which have no tenant id. Can be used in combination
	  /// with <seealso cref="#tenantIdIn(String...)"/>.
	  /// </summary>
	  JobDefinitionQuery includeJobDefinitionsWithoutTenantId();

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByJobDefinitionId();

	  /// <summary>
	  /// Order by activty id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByActivityId();

	  /// <summary>
	  /// Order by process defintion id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by job type (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByJobType();

	  /// <summary>
	  /// Order by job configuration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobDefinitionQuery orderByJobConfiguration();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of job definitions without tenant id is database-specific.
	  /// </summary>
	  JobDefinitionQuery orderByTenantId();

	}

}