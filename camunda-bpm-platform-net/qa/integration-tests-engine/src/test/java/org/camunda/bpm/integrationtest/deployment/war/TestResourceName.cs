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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
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
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestResourceName extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestResourceName : AbstractFoxPlatformIntegrationTest
	{

	  public static readonly string PROCESSES_XML = "<process-application xmlns=\"http://www.camunda.org/schema/1.0/ProcessApplication\">" +

			"<process-archive name=\"PA_NAME\">" +
			  "<properties>" +
				"<property name=\"isDeleteUponUndeploy\">true</property>" +
			  "</properties>" +
			"</process-archive>" +

		  "</process-application>";

	  public static readonly string PROCESSES_XML_WITH_RESOURCE_ROOT_PATH = "<process-application xmlns=\"http://www.camunda.org/schema/1.0/ProcessApplication\">" +

			"<process-archive name=\"PA_NAME\">" +
			  "<properties>" +
				"<property name=\"isDeleteUponUndeploy\">true</property>" +
				"<property name=\"resourceRootPath\">RESOURCE_ROOT_PATH</property>" +
			  "</properties>" +
			"</process-archive>" +

		  "</process-application>";


	  /// <summary>
	  /// <pre>
	  ///   |-- test.war
	  ///       |-- WEB-INF
	  ///           |-- classes
	  ///               |-- alternateDirectory/process4.bpmn
	  ///               |-- alternateDirectory/subDirectory/process5.bpmn
	  ///           |-- lib/
	  ///               |-- pa0.jar
	  ///                   |-- META-INF/processes.xml
	  ///                   |-- process0.bpmn
	  ///               |-- pa1.jar
	  ///                   |-- META-INF/processes.xml
	  ///                   |-- processes/process1.bpmn
	  ///               |-- pa2.jar
	  ///                   |-- META-INF/processes.xml                resourceRootPath: pa:directory
	  ///                   |-- directory/process2.bpmn
	  ///                   |-- directory/subDirectory/process3.bpmn
	  ///               |-- pa3.jar
	  ///                   |-- META-INF/processes.xml                resourceRootPath: classpath:alternateDirectory
	  /// </pre>
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
	  public static WebArchive processArchive()
	  {

		Asset pa1ProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML, new string[][]
		{
			new string[]{"PA_NAME", "PA0"}
		});

		Asset pa2ProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML, new string[][]
		{
			new string[]{"PA_NAME", "PA1"}
		});

		Asset pa3ProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML_WITH_RESOURCE_ROOT_PATH, new string[][]
		{
			new string[]{"PA_NAME", "PA2"},
			new string[]{"RESOURCE_ROOT_PATH", "pa:directory"}
		});

		Asset pa4ProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML_WITH_RESOURCE_ROOT_PATH, new string[][]
		{
			new string[]{"PA_NAME", "PA3"},
			new string[]{"RESOURCE_ROOT_PATH", "classpath:alternateDirectory"}
		});

		Asset[] processAssets = TestHelper.generateProcessAssets(6);

		JavaArchive pa1 = ShrinkWrap.create(typeof(JavaArchive), "pa0.jar").addAsResource(pa1ProcessesXml, "META-INF/processes.xml").addAsResource(processAssets[0], "process0.bpmn");

		JavaArchive pa2 = ShrinkWrap.create(typeof(JavaArchive), "pa1.jar").addAsResource(pa2ProcessesXml, "META-INF/processes.xml").addAsResource(processAssets[1], "processes/process1.bpmn");

		JavaArchive pa3 = ShrinkWrap.create(typeof(JavaArchive), "pa2.jar").addAsResource(pa3ProcessesXml, "META-INF/processes.xml").addAsResource(processAssets[2], "directory/process2.bpmn").addAsResource(processAssets[3], "directory/subDirectory/process3.bpmn");

		JavaArchive pa4 = ShrinkWrap.create(typeof(JavaArchive), "pa3.jar").addAsResource(pa4ProcessesXml, "META-INF/processes.xml");

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsLibraries(pa1).addAsLibraries(pa2).addAsLibraries(pa3).addAsLibraries(pa4).addAsResource(processAssets[4], "alternateDirectory/process4.bpmn").addAsResource(processAssets[5], "alternateDirectory/subDirectory/process5.bpmn").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestContainer)).addClass(typeof(TestResourceName));

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResourceName()
	  public virtual void testResourceName()
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);

		RepositoryService repositoryService = processEngine.RepositoryService;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		ProcessDefinition definition = query.processDefinitionKey("process-0").singleResult();
		Assert.assertEquals("process0.bpmn", definition.ResourceName);

		definition = query.processDefinitionKey("process-1").singleResult();
		Assert.assertEquals("processes/process1.bpmn", definition.ResourceName);

		definition = query.processDefinitionKey("process-2").singleResult();
		Assert.assertEquals("process2.bpmn", definition.ResourceName);

		definition = query.processDefinitionKey("process-3").singleResult();
		Assert.assertEquals("subDirectory/process3.bpmn", definition.ResourceName);

		definition = query.processDefinitionKey("process-4").singleResult();
		Assert.assertEquals("process4.bpmn", definition.ResourceName);

		definition = query.processDefinitionKey("process-5").singleResult();
		Assert.assertEquals("subDirectory/process5.bpmn", definition.ResourceName);
	  }

	}

}