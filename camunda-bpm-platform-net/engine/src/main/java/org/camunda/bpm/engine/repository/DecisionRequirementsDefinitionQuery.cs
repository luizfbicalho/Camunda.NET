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

	public interface DecisionRequirementsDefinitionQuery : Query<DecisionRequirementsDefinitionQuery, DecisionRequirementsDefinition>
	{

	  /// <summary>
	  /// Only select decision requirements definition with the given id.
	  /// </summary>
	  /// <param name="id"> the id of the decision requirements definition </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionId(string id);

	  /// <summary>
	  /// Only select decision requirements definition with the given ids.
	  /// </summary>
	  /// <param name="ids"> list of decision requirements definition ids </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionIdIn(params string[] ids);

	  /// <summary>
	  /// Only select decision requirements definition with the given category.
	  /// </summary>
	  /// <param name="category"> the category of the decision requirements definition </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionCategory(string category);

	  /// <summary>
	  /// Only select decision requirements definition where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, e.g., %category%.
	  /// </summary>
	  /// <param name="categoryLike"> the pattern to match the decision requirements definition category </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionCategoryLike(string categoryLike);

	  /// <summary>
	  /// Only select decision requirements definition with the given name.
	  /// </summary>
	  /// <param name="name"> the name of the decision requirements definition </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionName(string name);

	  /// <summary>
	  /// Only select decision requirements definition where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, e.g., %name%.
	  /// </summary>
	  /// <param name="nameLike"> the pattern to match the decision requirements definition name </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionNameLike(string nameLike);

	  /// <summary>
	  /// Only select decision requirements definition with the given key.
	  /// </summary>
	  /// <param name="key"> the key of the decision definition </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionKey(string key);

	  /// <summary>
	  /// Only select decision requirements definition where the key matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, e.g., %key%.
	  /// </summary>
	  /// <param name="keyLike"> the pattern to match the decision requirements definition key </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionKeyLike(string keyLike);

	  /// <summary>
	  /// Only select decision requirements definition that are deployed in a deployment with the
	  /// given deployment id.
	  /// </summary>
	  /// <param name="deploymentId"> the id of the deployment </param>
	  DecisionRequirementsDefinitionQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select decision requirements definition with a certain version.
	  /// Particularly useful when used in combination with <seealso cref="decisionRequirementsDefinitionKey(string)"/>
	  /// </summary>
	  /// <param name="version"> the version of the decision requirements definition </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionVersion(int? version);

	  /// <summary>
	  /// Only select the decision requirements definition which are the latest deployed
	  /// (i.e. which have the highest version number for the given key).
	  /// 
	  /// Can only be used in combination with <seealso cref="decisionRequirementsDefinitionKey(string)"/>
	  /// or <seealso cref="decisionRequirementsDefinitionKeyLike(string)"/>. Can also be used without any
	  /// other criteria (i.e. query.latest().list()), which will then give all the
	  /// latest versions of all the deployed decision requirements definition.
	  /// 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery latestVersion();

	  /// <summary>
	  /// Only select decision requirements definition with the given resource name.
	  /// </summary>
	  /// <param name="resourceName"> the name of the resource </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionResourceName(string resourceName);

	  /// <summary>
	  /// Only select decision requirements definition with a resource name like the given.
	  /// The syntax that should be used is the same as in SQL, e.g., %resourceName%.
	  /// </summary>
	  /// <param name="resourceNameLike"> the pattern to match the resource name </param>
	  DecisionRequirementsDefinitionQuery decisionRequirementsDefinitionResourceNameLike(string resourceNameLike);

	  /// <summary>
	  /// Only select decision requirements definition with one of the given tenant ids. </summary>
	  DecisionRequirementsDefinitionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select decision requirements definition which have no tenant id. </summary>
	  DecisionRequirementsDefinitionQuery withoutTenantId();

	  /// <summary>
	  /// Select decision requirements definition which have no tenant id. Can be used in
	  /// combination with <seealso cref="tenantIdIn(string...)"/>.
	  /// </summary>
	  DecisionRequirementsDefinitionQuery includeDecisionRequirementsDefinitionsWithoutTenantId();

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the decision requirements definition (needs to be followed by
	  /// <seealso cref="asc()"/> or <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionCategory();

	  /// <summary>
	  /// Order by decision requirements definition key (needs to be followed by <seealso cref="asc()"/> or
	  /// <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionKey();

	  /// <summary>
	  /// Order by the id of the decision requirements definition (needs to be followed by
	  /// <seealso cref="asc()"/> or <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionId();

	  /// <summary>
	  /// Order by the version of the decision requirements definition (needs to be followed
	  /// by <seealso cref="asc()"/> or <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionVersion();

	  /// <summary>
	  /// Order by the name of the decision requirements definition (needs to be followed by
	  /// <seealso cref="asc()"/> or <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDecisionRequirementsDefinitionName();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="asc()"/>
	  /// or <seealso cref="desc()"/>). 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByDeploymentId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of decision requirements definition without tenant id is database-specific. 
	  /// </summary>
	  DecisionRequirementsDefinitionQuery orderByTenantId();

	}

}