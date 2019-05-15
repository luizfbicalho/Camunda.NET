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
namespace org.camunda.bpm.application
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// <para>Annotation that can be placed on a method of a <seealso cref="AbstractProcessApplication ProcessApplication"/> class.</para>
	/// 
	/// <para>The method will be invoked before the process application is undeployed.
	/// 
	/// </para>
	/// <para><strong>LIMITATION:</strong> the annotation must be placed on a method of the same class carrying the 
	/// <code>{@literal @}ProcessApplication</code> annotation. Methods of superclasses are not detected.</para>
	/// 
	/// <para><strong>NOTE:</strong> A process application class must only define a single <code>{@literal @}PostDeploy</code>
	/// Method.</para>
	/// 
	/// <para><strong>NOTE:</strong> if the {@literal @}PostDeploy method throws an exception, the exception is logged but 
	/// the container will still undeploy the application.</para>
	/// 
	/// <h2>Basic Usage example:</h2>
	/// <pre>
	/// {@literal @}ProcessApplication("My Process Application")
	/// public class MyProcessApplication extends ServletProcessApplication {
	/// 
	///  {@literal @}PreUndeploy
	///  public void cleanup(ProcessEngine processEngine) {
	///    ...
	///  }
	/// 
	/// }
	/// </pre>
	/// 
	/// <para>A method annotated with <code>{@literal @}PreUndeploy</code> may additionally take the following set of 
	/// parameters, in any oder: 
	/// <ul>
	///  <li><seealso cref="ProcessApplicationInfo"/>: the <seealso cref="ProcessApplicationInfo"/> object for this process application is injected</li>
	///  <li><seealso cref="ProcessEngine"/> the default process engine is injected</li>
	///  <li>{@code List<ProcessEngine>} all process engines to which this process application has performed deployments are 
	///  injected.</li>
	/// </ul>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= PostDeploy
	///  </seealso>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class PreUndeploy : System.Attribute
	{

	}

}