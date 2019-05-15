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
namespace org.camunda.bpm.integrationtest.deployment.war
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentDeployChangedOnlyWithJarAsLib extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentDeployChangedOnlyWithJarAsLib : AbstractFoxPlatformIntegrationTest
	{

	  private const string PA1 = "PA1";
	  private const string PA2 = "PA2";

	  /// <summary>
	  /// <pre>
	  ///   |-- pa1.war
	  ///       |-- WEB-INF
	  ///           |-- lib/
	  ///               |-- test-v1.jar
	  ///                   |-- META-INF/processes.xml
	  ///                   |-- process.bpmn
	  /// </pre>
	  /// </summary>
	  [Deployment(order:1, name:PA1)]
	  public static WebArchive archive1()
	  {

		JavaArchive processArchiveJar = ShrinkWrap.create(typeof(JavaArchive), "test-v1.jar").addAsResource("org/camunda/bpm/integrationtest/deployment/war/testDeployProcessArchiveUnchanged.bpmn20.xml", "process.bpmn").addAsResource("META-INF/processes.xml");

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "pa1.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsLibraries(processArchiveJar).addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestWarDeploymentDeployChangedOnlyWithJarAsLib));

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

	  /// <summary>
	  /// <pre>
	  ///   |-- pa2.war
	  ///       |-- WEB-INF
	  ///           |-- lib/
	  ///               |-- test-v2.jar
	  ///                   |-- META-INF/processes.xml
	  ///                   |-- process.bpmn
	  /// </pre>
	  /// </summary>
	  [Deployment(order:2, name:PA2)]
	  public static WebArchive archive2()
	  {

		JavaArchive processArchiveJar = ShrinkWrap.create(typeof(JavaArchive), "test-v2.jar").addAsResource("org/camunda/bpm/integrationtest/deployment/war/testDeployProcessArchiveUnchanged.bpmn20.xml", "process.bpmn").addAsResource("META-INF/processes.xml");

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "pa2.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsLibraries(processArchiveJar).addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestWarDeploymentDeployChangedOnlyWithJarAsLib));

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment(value=PA2) public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);

		RepositoryService repositoryService = processEngine.RepositoryService;

		long count = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testDeployProcessArchiveUnchanged").count();

		Assert.assertEquals(1, count);
	  }

	}

}