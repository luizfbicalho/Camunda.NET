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

	/// <summary>
	/// Used to annotate a user-provided <seealso cref="AbstractProcessApplication"/> class and specify
	/// the unique name of the process application.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ProcessApplication : System.Attribute
	{

	  private string DEFAULT_META_INF_PROCESSES_XML = "META-INF/processes.xml";

	  /// <summary>
	  /// Allows specifying the name of the process application.
	  /// Overrides the {@code name} property.
	  /// </summary>
	  internal string value;

	  /// <summary>
	  /// Allows specifying the name of the process application.
	  /// Only applies if the {@code value} property is not set.
	  /// </summary>
	  internal string name;

	  /// <summary>
	  /// Returns the location(s) of the <code>processes.xml</code> deployment descriptors.
	  /// The default value is<code>{META-INF/processes.xml}</code>. The provided path(s)
	  /// must be resolvable through the <seealso cref="ClassLoader#getResourceAsStream(String)"/>-Method
	  /// of the classloader returned  by the <seealso cref="AbstractProcessApplication#getProcessApplicationClassloader()"/>
	  /// method provided by the process application.
	  /// </summary>
	  /// <returns> the location of the <code>processes.xml</code> file. </returns>
	  internal string[] deploymentDescriptors;


		public ProcessApplication(String value = "", String name = "", String[] deploymentDescriptors = {DEFAULT_META_INF_PROCESSES_XML})
		{
			this.value = value;
			this.name = name;
			this.deploymentDescriptors = deploymentDescriptors;
		}
	}

}