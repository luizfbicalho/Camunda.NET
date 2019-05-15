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

	/// <summary>
	/// Represents a deployment that is already present in the process repository.
	/// 
	/// A deployment is a container for resources such as process definitions, images, forms, etc.
	/// 
	/// When a deployment is 'deployed' through the <seealso cref="org.camunda.bpm.engine.RepositoryService"/>,
	/// the engine will recognize certain of such resource types and act upon
	/// them (e.g. process definitions will be parsed to an executable Java artifact).
	/// 
	/// To create a Deployment, use the <seealso cref="org.camunda.bpm.engine.repository.DeploymentBuilder"/>.
	/// A Deployment on itself is a <b>read-only</b> object and its content cannot be
	/// changed after deployment (hence the builder that needs to be used).
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface Deployment
	{

	  string Id {get;}

	  string Name {get;}

	  DateTime DeploymentTime {get;}

	  string Source {get;}

	  /// <summary>
	  /// Returns the id of the tenant this deployment belongs to. Can be <code>null</code>
	  /// if the deployment belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	}

}