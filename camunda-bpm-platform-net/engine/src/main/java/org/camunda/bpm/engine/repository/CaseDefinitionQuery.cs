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
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseDefinitionQuery : Query<CaseDefinitionQuery, CaseDefinition>
	{

	  /// <summary>
	  /// Only select case definition with the given id.
	  /// </summary>
	  /// <param name="caseDefinitionId"> the id of the case definition </param>
	  CaseDefinitionQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select case definitions with the given ids.
	  /// </summary>
	  /// <param name="ids"> list of case definition ids </param>
	  CaseDefinitionQuery caseDefinitionIdIn(params string[] ids);

	  /// <summary>
	  /// Only select case definitions with the given category.
	  /// </summary>
	  /// <param name="caseDefinitionCategory"> the category of the case definition </param>
	  CaseDefinitionQuery caseDefinitionCategory(string caseDefinitionCategory);

	  /// <summary>
	  /// Only select case definitions where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  /// <param name="caseDefinitionCategoryLike"> the pattern to match the case definition category </param>
	  CaseDefinitionQuery caseDefinitionCategoryLike(string caseDefinitionCategoryLike);

	  /// <summary>
	  /// Only select case definitions with the given name.
	  /// </summary>
	  /// <param name="caseDefinitionName"> the name of the case definition </param>
	  CaseDefinitionQuery caseDefinitionName(string caseDefinitionName);

	  /// <summary>
	  /// Only select case definition with the given key.
	  /// </summary>
	  /// <param name="caseDefinitionKey"> the key of the case definition </param>
	  CaseDefinitionQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select case definitions where the key matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  /// <param name="caseDefinitionKeyLike"> the pattern to match the case definition key </param>
	  CaseDefinitionQuery caseDefinitionKeyLike(string caseDefinitionKeyLike);

	  /// <summary>
	  /// Only select case definitions where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  /// <param name="caseDefinitionNameLike"> the pattern to match the case definition name </param>
	  CaseDefinitionQuery caseDefinitionNameLike(string caseDefinitionNameLike);

	  /// <summary>
	  /// Only select case definitions that are deployed in a deployment with the
	  /// given deployment id.
	  /// </summary>
	  /// <param name="deploymentId"> the id of the deployment </param>
	  CaseDefinitionQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select case definition with a certain version.
	  /// Particularly useful when used in combination with <seealso cref="#caseDefinitionKey(String)"/>
	  /// </summary>
	  /// <param name="caseDefinitionVersion"> the version of the case definition </param>
	  CaseDefinitionQuery caseDefinitionVersion(int? caseDefinitionVersion);

	  /// <summary>
	  /// Only select the case definitions which are the latest deployed
	  /// (ie. which have the highest version number for the given key).
	  /// 
	  /// Can only be used in combination with <seealso cref="#caseDefinitionKey(String)"/>
	  /// or <seealso cref="#caseDefinitionKeyLike(String)"/>. Can also be used without any
	  /// other criteria (ie. query.latest().list()), which will then give all the
	  /// latest versions of all the deployed case definitions.
	  /// 
	  /// </summary>
	  CaseDefinitionQuery latestVersion();

	  /// <summary>
	  /// Only select case definition with the given resource name.
	  /// </summary>
	  /// <param name="resourceName"> the name of the resource </param>
	  CaseDefinitionQuery caseDefinitionResourceName(string resourceName);

	  /// <summary>
	  /// Only select case definition with a resource name like the given.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  /// <param name="resourceNameLike"> the pattern to match the resource name </param>
	  CaseDefinitionQuery caseDefinitionResourceNameLike(string resourceNameLike);

	  /// <summary>
	  /// Only select case definitions with one of the given tenant ids. </summary>
	  CaseDefinitionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select case definitions which have no tenant id. </summary>
	  CaseDefinitionQuery withoutTenantId();

	  /// <summary>
	  /// Select case definitions which have no tenant id. Can be used in
	  /// combination with <seealso cref="#tenantIdIn(String...)"/>.
	  /// </summary>
	  CaseDefinitionQuery includeCaseDefinitionsWithoutTenantId();

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the case definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByCaseDefinitionCategory();

	  /// <summary>
	  /// Order by case definition key (needs to be followed by <seealso cref="#asc()"/> or
	  /// <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByCaseDefinitionKey();

	  /// <summary>
	  /// Order by the id of the case definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by the version of the case definitions (needs to be followed
	  /// by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByCaseDefinitionVersion();

	  /// <summary>
	  /// Order by the name of the case definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByCaseDefinitionName();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="#asc()"/>
	  /// or <seealso cref="#desc()"/>). 
	  /// </summary>
	  CaseDefinitionQuery orderByDeploymentId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of case instances without tenant id is database-specific. 
	  /// </summary>
	  CaseDefinitionQuery orderByTenantId();

	}

}