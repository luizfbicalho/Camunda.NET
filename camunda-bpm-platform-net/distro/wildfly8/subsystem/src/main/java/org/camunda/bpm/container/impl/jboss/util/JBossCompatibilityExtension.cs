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
namespace org.camunda.bpm.container.impl.jboss.util
{
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using InjectedValue = org.jboss.msc.value.InjectedValue;

	/// <summary>
	/// Provides method abstractions to make our subsystem compatible with different JBoss versions.
	/// This affects mainly EAP versions.
	/// </summary>
	public class JBossCompatibilityExtension
	{

	  /// <summary>
	  /// The service name of the root application server service.
	  /// Copied from org.jboss.as.server.Services - JBoss 7.2.0.Final
	  /// </summary>
	  public static readonly ServiceName JBOSS_AS = ServiceName.JBOSS.append("as");

	  /// <summary>
	  /// The service corresponding to the <seealso cref="java.util.concurrent.ExecutorService"/> for this instance.
	  /// Copied from org.jboss.as.server.Services - JBoss 7.2.0.Final
	  /// </summary>
	  internal static readonly ServiceName JBOSS_SERVER_EXECUTOR = JBOSS_AS.append("server-executor");

	  /// <summary>
	  /// Adds the JBoss server executor as a dependency to the given service.
	  /// Copied from org.jboss.as.server.Services - JBoss 7.2.0.Final
	  /// </summary>
	  public static void addServerExecutorDependency<T1>(ServiceBuilder<T1> serviceBuilder, InjectedValue<ExecutorService> injector, bool optional)
	  {
		ServiceBuilder.DependencyType type = optional ? ServiceBuilder.DependencyType.OPTIONAL : ServiceBuilder.DependencyType.REQUIRED;
		serviceBuilder.addDependency(type, JBOSS_SERVER_EXECUTOR, typeof(ExecutorService), injector);
	  }
	}

}