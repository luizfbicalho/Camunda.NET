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
namespace org.camunda.bpm.container
{
	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using RuntimeContainerDelegateImpl = org.camunda.bpm.container.impl.RuntimeContainerDelegateImpl;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// <para>The <seealso cref="RuntimeContainerDelegate"/> in an SPI that allows the process engine to integrate with the
	/// runtime container in which it is deployed. Examples of "runtime containers" are
	/// <ul>
	///  <li>JBoss AS 7 (Module Service Container),</li>
	///  <li>The JMX Container,</li>
	///  <li>An OSGi Runtime,</li>
	///  <li>...</li>
	/// </ul>
	/// 
	/// </para>
	/// <para>The current <seealso cref="RuntimeContainerDelegate"/> can be obtained through the static <seealso cref="#INSTANCE"/> field.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface RuntimeContainerDelegate
	{

	  /// <summary>
	  /// Holds the current <seealso cref="RuntimeContainerDelegate"/> instance </summary>

	  /// <summary>
	  /// <para>Adds a managed <seealso cref="ProcessEngine"/> to the runtime container.</para>
	  /// <para>Process Engines registered through this method are returned by the <seealso cref="ProcessEngineService"/>.</para>
	  /// </summary>
	  void registerProcessEngine(ProcessEngine processEngine);

	  /// <summary>
	  /// <para>Unregisters a managed <seealso cref="ProcessEngine"/> instance from the Runtime Container.</para>
	  /// </summary>
	  void unregisterProcessEngine(ProcessEngine processEngine);

	  /// <summary>
	  /// Deploy a <seealso cref="AbstractProcessApplication"/> into the runtime container.
	  /// 
	  /// </summary>
	  void deployProcessApplication(AbstractProcessApplication processApplication);

	  /// <summary>
	  /// Undeploy a <seealso cref="AbstractProcessApplication"/> from the runtime container.
	  /// 
	  /// </summary>
	  void undeployProcessApplication(AbstractProcessApplication processApplication);

	  /// <returns> the Container's <seealso cref="ProcessEngineService"/> implementation. </returns>
	  ProcessEngineService ProcessEngineService {get;}

	  /// <returns> the Container's <seealso cref="ProcessApplicationService"/> implementation </returns>
	  ProcessApplicationService ProcessApplicationService {get;}

	  /// <returns> the Runtime Container's <seealso cref="ExecutorService"/> implementation </returns>
	  ExecutorService ExecutorService {get;}

	  /// <returns> a reference to the process application with the given name if deployed; null otherwise </returns>
	  ProcessApplicationReference getDeployedProcessApplication(string name);

	  /// <summary>
	  /// Holder of the current <seealso cref="RuntimeContainerDelegate"/> instance.
	  /// </summary>

	}

	public static class RuntimeContainerDelegate_Fields
	{
	  public static readonly RuntimeContainerDelegate_RuntimeContainerDelegateInstance INSTANCE = new RuntimeContainerDelegate_RuntimeContainerDelegateInstance();
	}

	  public class RuntimeContainerDelegate_RuntimeContainerDelegateInstance
	  {

	// hide
	internal RuntimeContainerDelegate_RuntimeContainerDelegateInstance()
	{
	}

	internal RuntimeContainerDelegate @delegate = new RuntimeContainerDelegateImpl();

	public virtual RuntimeContainerDelegate get()
	{
	  return @delegate;
	}

	public virtual void set(RuntimeContainerDelegate @delegate)
	{
	  this.@delegate = @delegate;
	}

	  }

}