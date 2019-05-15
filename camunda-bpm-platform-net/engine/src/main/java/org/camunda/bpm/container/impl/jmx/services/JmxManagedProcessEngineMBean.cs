using System.Collections.Generic;

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
namespace org.camunda.bpm.container.impl.jmx.services
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// An MBean interface for the <seealso cref="ProcessEngine"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface JmxManagedProcessEngineMBean
	{

	  /// <returns> the name of the <seealso cref="ProcessEngine"/> </returns>
	  string Name {get;}

	  /// <summary>
	  /// If the engine's job executor is deloyment aware, these are the deployments it
	  /// acquires jobs for.
	  /// </summary>
	  /// <returns> all deployments that are registered with this <seealso cref="ProcessEngine"/> </returns>
	  ISet<string> RegisteredDeployments {get;}

	  void registerDeployment(string deploymentId);

	  void unregisterDeployment(string deploymentId);

	  void reportDbMetrics();
	}

}