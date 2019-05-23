﻿/*
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
	/// Definition of a resource which was deployed
	/// </summary>
	public interface ResourceDefinition
	{

	  /// <summary>
	  /// unique identifier </summary>
	  string Id {get;}

	  /// <summary>
	  /// category name which is derived from the targetNamespace attribute in the definitions element </summary>
	  string Category {get;}

	  /// <summary>
	  /// label used for display purposes </summary>
	  string Name {get;}

	  /// <summary>
	  /// unique name for all versions this definition </summary>
	  string Key {get;}

	  /// <summary>
	  /// version of this definition </summary>
	  int Version {get;}

	  /// <summary>
	  /// name of <seealso cref="RepositoryService.getResourceAsStream(string, string) the resource"/> of this definition </summary>
	  string ResourceName {get;}

	  /// <summary>
	  /// The deployment in which this definition is contained. </summary>
	  string DeploymentId {get;}

	  /// <summary>
	  /// The diagram resource name for this definition if exist </summary>
	  string DiagramResourceName {get;}

	  /// <summary>
	  /// The id of the tenant this definition belongs to. Can be <code>null</code>
	  /// if the definition belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// History time to live. Is taken into account in history cleanup. </summary>
	  int? HistoryTimeToLive {get;}

	}

}