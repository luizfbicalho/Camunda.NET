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

	public interface DecisionDefinitionQuery : Query<DecisionDefinitionQuery, DecisionDefinition>
	{

	  /// <summary>
	  /// Only select decision definition with the given id.
	  /// </summary>
	  /// <param name="decisionDefinitionId"> the id of the decision definition </param>
	  DecisionDefinitionQuery decisionDefinitionId(string decisionDefinitionId);

	  /// <summary>
	  /// Only select decision definitions with the given ids.
	  /// </summary>
	  /// <param name="ids"> list of decision definition ids </param>
	  DecisionDefinitionQuery decisionDefinitionIdIn(params string[] ids);

	  /// <summary>
	  /// Only select decision definitions with the given category.
	  /// </summary>
	  /// <param name="decisionDefinitionCategory"> the category of the decision definition </param>
	  DecisionDefinitionQuery decisionDefinitionCategory(string decisionDefinitionCategory);

	  /// <summary>
	  /// Only select decision definitions where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %category%
	  /// </summary>
	  /// <param name="decisionDefinitionCategoryLike"> the pattern to match the decision definition category </param>
	  DecisionDefinitionQuery decisionDefinitionCategoryLike(string decisionDefinitionCategoryLike);

	  /// <summary>
	  /// Only select decision definitions with the given name.
	  /// </summary>
	  /// <param name="decisionDefinitionName"> the name of the decision definition </param>
	  DecisionDefinitionQuery decisionDefinitionName(string decisionDefinitionName);

	  /// <summary>
	  /// Only select decision definition with the given key.
	  /// </summary>
	  /// <param name="decisionDefinitionKey"> the key of the decision definition </param>
	  DecisionDefinitionQuery decisionDefinitionKey(string decisionDefinitionKey);

	  /// <summary>
	  /// Only select decision definitions where the key matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %key%
	  /// </summary>
	  /// <param name="decisionDefinitionKeyLike"> the pattern to match the decision definition key </param>
	  DecisionDefinitionQuery decisionDefinitionKeyLike(string decisionDefinitionKeyLike);

	  /// <summary>
	  /// Only select decision definitions where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %name%
	  /// </summary>
	  /// <param name="decisionDefinitionNameLike"> the pattern to match the decision definition name </param>
	  DecisionDefinitionQuery decisionDefinitionNameLike(string decisionDefinitionNameLike);

	  /// <summary>
	  /// Only select decision definitions that are deployed in a deployment with the
	  /// given deployment id.
	  /// </summary>
	  /// <param name="deploymentId"> the id of the deployment </param>
	  DecisionDefinitionQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select decision definition with a certain version.
	  /// Particularly useful when used in combination with <seealso cref="#decisionDefinitionKey(String)"/>
	  /// </summary>
	  /// <param name="decisionDefinitionVersion"> the version of the decision definition </param>
	  DecisionDefinitionQuery decisionDefinitionVersion(int? decisionDefinitionVersion);

	  /// <summary>
	  /// Only select the decision definitions which are the latest deployed
	  /// (ie. which have the highest version number for the given key).
	  /// 
	  /// Can only be used in combination with <seealso cref="#decisionDefinitionKey(String)"/>
	  /// or <seealso cref="#decisionDefinitionKeyLike(String)"/>. Can also be used without any
	  /// other criteria (ie. query.latest().list()), which will then give all the
	  /// latest versions of all the deployed decision definitions.
	  /// 
	  /// </summary>
	  DecisionDefinitionQuery latestVersion();

	  /// <summary>
	  /// Only select decision definition with the given resource name.
	  /// </summary>
	  /// <param name="resourceName"> the name of the resource </param>
	  DecisionDefinitionQuery decisionDefinitionResourceName(string resourceName);

	  /// <summary>
	  /// Only select decision definition with a resource name like the given.
	  /// The syntax that should be used is the same as in SQL, eg. %resourceName%
	  /// </summary>
	  /// <param name="resourceNameLike"> the pattern to match the resource name </param>
	  DecisionDefinitionQuery decisionDefinitionResourceNameLike(string resourceNameLike);

	  /// <summary>
	  /// Only select decision definitions which belongs to a decision requirements definition with the given id.
	  /// </summary>
	  /// <param name="decisionRequirementsDefinitionId"> id of the related decision requirements definition </param>
	  DecisionDefinitionQuery decisionRequirementsDefinitionId(string decisionRequirementsDefinitionId);

	  /// <summary>
	  /// Only select decision definitions which belongs to a decision requirements definition with the given key.
	  /// </summary>
	  /// <param name="decisionRequirementsDefinitionKey"> key of the related decision requirements definition </param>
	  DecisionDefinitionQuery decisionRequirementsDefinitionKey(string decisionRequirementsDefinitionKey);

	  /// <summary>
	  /// Only select decision definitions which belongs to no decision requirements definition.
	  /// </summary>
	  DecisionDefinitionQuery withoutDecisionRequirementsDefinition();

	  /// <summary>
	  /// Only select decision definitions with one of the given tenant ids. </summary>
	  DecisionDefinitionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select decision definitions which have no tenant id. </summary>
	  DecisionDefinitionQuery withoutTenantId();

	  /// <summary>
	  /// Select decision definitions which have no tenant id. Can be used in
	  /// combination with <seealso cref="#tenantIdIn(String...)"/>.
	  /// </summary>
	  DecisionDefinitionQuery includeDecisionDefinitionsWithoutTenantId();

	  /// <summary>
	  /// Only selects decision definitions with a specific version tag
	  /// </summary>
	  DecisionDefinitionQuery versionTag(string versionTag);

	  /// <summary>
	  /// Only selects decision definitions with a version tag like the given
	  /// </summary>
	  DecisionDefinitionQuery versionTagLike(string versionTagLike);

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the decision definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDecisionDefinitionCategory();

	  /// <summary>
	  /// Order by decision definition key (needs to be followed by <seealso cref="#asc()"/> or
	  /// <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDecisionDefinitionKey();

	  /// <summary>
	  /// Order by the id of the decision definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDecisionDefinitionId();

	  /// <summary>
	  /// Order by the version of the decision definitions (needs to be followed
	  /// by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDecisionDefinitionVersion();

	  /// <summary>
	  /// Order by the name of the decision definitions (needs to be followed by
	  /// <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDecisionDefinitionName();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="#asc()"/>
	  /// or <seealso cref="#desc()"/>). 
	  /// </summary>
	  DecisionDefinitionQuery orderByDeploymentId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of decision definitions without tenant id is database-specific. 
	  /// </summary>
	  DecisionDefinitionQuery orderByTenantId();

	  /// <summary>
	  /// Order by version tag (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// 
	  /// <strong>Note:</strong> sorting by versionTag is a string based sort.
	  /// There is no interpretation of the version which can lead to a sorting like:
	  /// v0.1.0 v0.10.0 v0.2.0.
	  /// </summary>
	  DecisionDefinitionQuery orderByVersionTag();

	}

}