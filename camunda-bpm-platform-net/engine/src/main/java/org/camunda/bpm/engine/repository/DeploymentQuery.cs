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

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="Deployment"/>s.
	/// 
	/// Note that it is impossible to retrieve the deployment resources through the
	/// results of this operation, since that would cause a huge transfer of
	/// (possibly) unneeded bytes over the wire.
	/// 
	/// To retrieve the actual bytes of a deployment resource use the operations on the
	/// <seealso cref="RepositoryService#getDeploymentResourceNames(String)"/>
	/// and <seealso cref="RepositoryService#getResourceAsStream(String, String)"/>
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Ingo Richtsmeier
	/// </summary>
	public interface DeploymentQuery : Query<DeploymentQuery, Deployment>
	{

	  /// <summary>
	  /// Only select deployments with the given deployment id. </summary>
	  DeploymentQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select deployments with the given name. </summary>
	  DeploymentQuery deploymentName(string name);

	  /// <summary>
	  /// Only select deployments with a name like the given string. </summary>
	  DeploymentQuery deploymentNameLike(string nameLike);

	  /// <summary>
	  /// If the given <code>source</code> is <code>null</code>,
	  /// then deployments are returned where source is equal to null.
	  /// Otherwise only deployments with the given source are
	  /// selected.
	  /// </summary>
	  DeploymentQuery deploymentSource(string source);

	  /// <summary>
	  /// Only select deployments deployed before the given date </summary>
	  DeploymentQuery deploymentBefore(DateTime before);

	  /// <summary>
	  /// Only select deployments deployed after the given date </summary>
	  DeploymentQuery deploymentAfter(DateTime after);

	  /// <summary>
	  /// Only select deployments with one of the given tenant ids. </summary>
	  DeploymentQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select deployments which have no tenant id. </summary>
	  DeploymentQuery withoutTenantId();

	  /// <summary>
	  /// Select deployments which have no tenant id. Can be used in
	  /// combination with <seealso cref="#tenantIdIn(String...)"/>.
	  /// </summary>
	  DeploymentQuery includeDeploymentsWithoutTenantId();

	  //sorting ////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  DeploymentQuery orderByDeploymentId();

	  /// <summary>
	  /// Order by deployment name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  DeploymentQuery orderByDeploymentName();

	  /// <summary>
	  /// Order by deployment time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  /// @deprecated Use <seealso cref="#orderByDeploymentTime()"/> instead</p> 
	  [Obsolete("Use <seealso cref=\"#orderByDeploymentTime()\"/> instead</p>")]
	  DeploymentQuery orderByDeploymenTime();

	  /// <summary>
	  /// Order by deployment time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  DeploymentQuery orderByDeploymentTime();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of process instances without tenant id is database-specific. 
	  /// </summary>
	  DeploymentQuery orderByTenantId();

	}

}