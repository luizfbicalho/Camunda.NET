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
namespace org.camunda.bpm.container.impl.spi
{

	using DeploymentOperationBuilder = org.camunda.bpm.container.impl.spi.DeploymentOperation.DeploymentOperationBuilder;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public interface PlatformServiceContainer
	{

	  void startService<S>(PlatformServiceContainer_ServiceType serviceType, string localName, PlatformService<S> service);

	  void startService<S>(string serviceName, PlatformService<S> service);

	  void stopService(PlatformServiceContainer_ServiceType serviceType, string localName);

	  void stopService(string serviceName);

	  DeploymentOperationBuilder createDeploymentOperation(string name);

	  DeploymentOperationBuilder createUndeploymentOperation(string name);

	  /// <summary>
	  /// get a specific service by name or null if no such Service exists.
	  /// 
	  /// </summary>
	  S getService<S>(PlatformServiceContainer_ServiceType type, string localName);

	  /// <summary>
	  /// get the service value for a specific service by name or null if no such
	  /// Service exists.
	  /// 
	  /// </summary>
	  S getServiceValue<S>(PlatformServiceContainer_ServiceType type, string localName);

	  /// <returns> all services for a specific <seealso cref="ServiceType"/> </returns>
	  IList<PlatformService<S>> getServicesByType<S>(PlatformServiceContainer_ServiceType type);

	  /// <returns> the service names ( <seealso cref="ObjectName"/> ) for all services for a given type </returns>
	  ISet<string> getServiceNames(PlatformServiceContainer_ServiceType type);

	  /// <returns> the values of all services for a specific <seealso cref="ServiceType"/> </returns>
	  IList<S> getServiceValuesByType<S>(PlatformServiceContainer_ServiceType type);

	  void executeDeploymentOperation(DeploymentOperation operation);

	  /// <summary>
	  /// A ServiceType is a collection of services that share a common name prefix.
	  /// </summary>


	}

	  public interface PlatformServiceContainer_ServiceType
	  {

	/// <summary>
	/// Returns a wildcard name that allows to query the service container
	/// for all services of the type represented by this ServiceType.
	/// </summary>
	string TypeName {get;}

	  }
}