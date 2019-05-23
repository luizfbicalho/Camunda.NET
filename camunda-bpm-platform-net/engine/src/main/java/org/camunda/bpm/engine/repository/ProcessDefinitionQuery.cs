using System;

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
namespace org.camunda.bpm.engine.repository
{
	using ProcessDefinitionQueryImpl = org.camunda.bpm.engine.impl.ProcessDefinitionQueryImpl;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="ProcessDefinition"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Saeid Mirzaei
	/// </summary>
	public interface ProcessDefinitionQuery : Query<ProcessDefinitionQuery, ProcessDefinition>
	{

	  /// <summary>
	  /// Only select process definiton with the given id. </summary>
	  ProcessDefinitionQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select process definiton with the given id. </summary>
	  ProcessDefinitionQuery processDefinitionIdIn(params string[] ids);

	  /// <summary>
	  /// Only select process definitions with the given category. </summary>
	  ProcessDefinitionQuery processDefinitionCategory(string processDefinitionCategory);

	  /// <summary>
	  /// Only select process definitions where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionCategoryLike(string processDefinitionCategoryLike);

	  /// <summary>
	  /// Only select process definitions with the given name. </summary>
	  ProcessDefinitionQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select process definitions where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionNameLike(string processDefinitionNameLike);

	  /// <summary>
	  /// Only select process definitions that are deployed in a deployment with the
	  /// given deployment id
	  /// </summary>
	  ProcessDefinitionQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select process definition with the given key.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select process definitions with the given keys
	  /// </summary>
	  ProcessDefinitionQueryImpl processDefinitionKeysIn(params string[] processDefinitionKeys);

	  /// <summary>
	  /// Only select process definitions where the key matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionKeyLike(string processDefinitionKeyLike);

	  /// <summary>
	  /// Only select process definition with a certain version.
	  /// Particulary useful when used in combination with <seealso cref="processDefinitionKey(string)"/>
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersion(int? processDefinitionVersion);

	  /// <summary>
	  /// <para>
	  /// Only select the process definitions which are the latest deployed (ie.
	  /// which have the highest version number for the given key).
	  /// </para>
	  /// 
	  /// <para>
	  /// Can only be used in combination with <seealso cref="processDefinitionKey(string)"/>
	  /// of <seealso cref="processDefinitionKeyLike(string)"/>. Can also be used without any
	  /// other criteria (ie. query.latest().list()), which will then give all the
	  /// latest versions of all the deployed process definitions.
	  /// </para>
	  /// 
	  /// <para>For multi-tenancy: select the latest deployed process definitions for each
	  /// tenant. If a process definition is deployed for multiple tenants then all
	  /// process definitions are selected.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///           if used in combination with <seealso cref="groupId(string)"/>,
	  ///           <seealso cref="processDefinitionVersion(int)"/> or
	  ///           <seealso cref="deploymentId(string)"/> </exception>
	  ProcessDefinitionQuery latestVersion();

	  /// <summary>
	  /// Only select process definition with the given resource name. </summary>
	  ProcessDefinitionQuery processDefinitionResourceName(string resourceName);

	  /// <summary>
	  /// Only select process definition with a resource name like the given . </summary>
	  ProcessDefinitionQuery processDefinitionResourceNameLike(string resourceNameLike);

	  /// <summary>
	  /// Only selects process definitions which given userId is authorized to start
	  /// </summary>
	  ProcessDefinitionQuery startableByUser(string userId);

	  /// <summary>
	  /// Only selects process definitions which are suspended
	  /// </summary>
	  ProcessDefinitionQuery suspended();

	  /// <summary>
	  /// Only selects process definitions which are active
	  /// </summary>
	  ProcessDefinitionQuery active();

	  /// <summary>
	  /// Only selects process definitions with the given incident type.
	  /// </summary>
	  ProcessDefinitionQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only selects process definitions with the given incident id.
	  /// </summary>
	  ProcessDefinitionQuery incidentId(string incidentId);

	  /// <summary>
	  /// Only selects process definitions with the given incident message.
	  /// </summary>
	  ProcessDefinitionQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only selects process definitions with an incident message like the given.
	  /// </summary>
	  ProcessDefinitionQuery incidentMessageLike(string incidentMessageLike);

	  /// <summary>
	  /// Only selects process definitions with a specific version tag
	  /// </summary>
	  ProcessDefinitionQuery versionTag(string versionTag);

	  /// <summary>
	  /// Only selects process definitions with a version tag like the given
	  /// </summary>
	  ProcessDefinitionQuery versionTagLike(string versionTagLike);

	  // Support for event subscriptions /////////////////////////////////////

	  /// <seealso cref= #messageEventSubscriptionName(String) </seealso>
	  [Obsolete]
	  ProcessDefinitionQuery messageEventSubscription(string messageName);

	  /// <summary>
	  /// Selects the single process definition which has a start message event
	  /// with the messageName.
	  /// </summary>
	  ProcessDefinitionQuery messageEventSubscriptionName(string messageName);

	  /// <summary>
	  /// Only select process definitions with one of the given tenant ids. </summary>
	  ProcessDefinitionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select process definitions which have no tenant id. </summary>
	  ProcessDefinitionQuery withoutTenantId();

	  /// <summary>
	  /// Select process definitions which have no tenant id. Can be used in
	  /// combination with <seealso cref="tenantIdIn(string...)"/>.
	  /// </summary>
	  ProcessDefinitionQuery includeProcessDefinitionsWithoutTenantId();

	  /// <summary>
	  /// Select process definitions which could be started in Tasklist.
	  /// </summary>
	  ProcessDefinitionQuery startableInTasklist();

	  /// <summary>
	  /// Select process definitions which could not be started in Tasklist.
	  /// </summary>
	  ProcessDefinitionQuery notStartableInTasklist();

	  ProcessDefinitionQuery startablePermissionCheck();

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the process definitions (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionCategory();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by the id of the process definitions (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by the version of the process definitions (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionVersion();

	  /// <summary>
	  /// Order by the name of the process definitions (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionName();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  ProcessDefinitionQuery orderByDeploymentId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of process instances without tenant id is database-specific. 
	  /// </summary>
	  ProcessDefinitionQuery orderByTenantId();

	  /// <summary>
	  /// Order by version tag (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// 
	  /// <strong>Note:</strong> sorting by versionTag is a string based sort.
	  /// There is no interpretation of the version which can lead to a sorting like:
	  /// v0.1.0 v0.10.0 v0.2.0.
	  /// </summary>
	  ProcessDefinitionQuery orderByVersionTag();

	}

}