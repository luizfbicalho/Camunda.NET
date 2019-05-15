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
namespace org.camunda.bpm.engine.test
{

	/// <summary>
	/// Annotation for a test method or class to create and delete a deployment around a test method.
	/// 
	/// <para>Usage - Example 1 (method-level annotation):</para>
	/// <pre>
	/// package org.example;
	/// 
	/// ...
	/// 
	/// public class ExampleTest {
	/// 
	///   &#64;Deployment
	///   public void testForADeploymentWithASingleResource() {
	///     // a deployment will be available in the engine repository
	///     // containing the single resource <b>org/example/ExampleTest.testForADeploymentWithASingleResource.bpmn20.xml</b>
	///   }
	/// 
	///   &#64;Deployment(resources = {
	///     "org/example/processOne.bpmn20.xml",
	///     "org/example/processTwo.bpmn20.xml",
	///     "org/example/some.other.resource" })
	///   public void testForADeploymentWithASingleResource() {
	///     // a deployment will be available in the engine repository
	///     // containing the three resources
	///   }
	/// </pre>
	/// 
	/// <para>Usage - Example 2 (class-level annotation):</para>
	/// <pre>
	/// package org.example;
	/// 
	/// ...
	/// 
	/// &#64;Deployment
	/// public class ExampleTest2 {
	/// 
	///   public void testForADeploymentWithASingleResource() {
	///     // a deployment will be available in the engine repository
	///     // containing the single resource <b>org/example/ExampleTest2.bpmn20.xml</b>
	///   }
	/// 
	///   &#64;Deployment(resources = "org/example/process.bpmn20.xml")
	///   public void testForADeploymentWithASingleResource() {
	///     // the method-level annotation overrides the class-level annotation
	///   }
	/// </pre>
	/// 
	/// @author Dave Syer
	/// @author Tom Baeyens
	/// </summary>
	public class Deployment : System.Attribute
	{

	  /// <summary>
	  /// Specify resources that make up the process definition. </summary>
	  public string[] resources;


		public Deployment(public String[] resources = {})
		{
			this.resources = resources;
		}
	}

}