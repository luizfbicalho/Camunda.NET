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
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using TestHelper = org.camunda.bpm.integrationtest.util.TestHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using Asset = org.jboss.shrinkwrap.api.asset.Asset;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestMultipleClasspathRoots extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestMultipleClasspathRoots : AbstractFoxPlatformIntegrationTest
	{

	  public static readonly string PROCESSES_XML = "<process-application xmlns=\"http://www.camunda.org/schema/1.0/ProcessApplication\">" +

			"<process-archive name=\"PA_NAME\">" +
			  "<properties>" +
				"<property name=\"isDeleteUponUndeploy\">true</property>" +
				"<property name=\"resourceRootPath\">classpath:directory</property>" +
			  "</properties>" +
			"</process-archive>" +

		  "</process-application>";

	  /// <summary>
	  /// <pre>
	  ///   |-- test.war
	  ///       |-- WEB-INF
	  ///           |-- classes
	  ///               |-- META-INF/processes.xml                   resourceRootPath: classpath:directory
	  ///               |-- directory/processes/process.bpmn         (1)
	  ///           |-- lib/
	  ///               |-- pa0.jar
	  ///                   |-- directory/processes/process.bpmn     (2)
	  /// </pre>
	  /// 
	  /// Processes (1) + (2) will have the same resource name (= "processes/process.bpmn"),
	  /// so that only one process should be deployed.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
	  public static WebArchive processArchive()
	  {

		Asset paProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML, new string[][]
		{
			new string[]{"PA_NAME", "PA0"}
		});


		Asset[] processAssets = TestHelper.generateProcessAssets(2);

		JavaArchive pa0 = ShrinkWrap.create(typeof(JavaArchive), "pa0.jar").addAsResource(processAssets[0], "directory/processes/process.bpmn");

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsLibraries(pa0).addAsResource(paProcessesXml, "META-INF/processes.xml").addAsResource(processAssets[1], "directory/processes/process.bpmn").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestResourceName));

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleClasspathRoots()
	  public virtual void testMultipleClasspathRoots()
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);

		RepositoryService repositoryService = processEngine.RepositoryService;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		long count = query.count();
		Assert.assertEquals(1, count);
	  }

	}

}