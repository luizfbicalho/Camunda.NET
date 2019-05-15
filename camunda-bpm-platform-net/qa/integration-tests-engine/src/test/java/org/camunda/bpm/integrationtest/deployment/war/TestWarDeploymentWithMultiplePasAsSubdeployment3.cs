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
	using Assert = org.junit.Assert;

	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
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
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// 
	/// <summary>
	/// <pre>
	///   |-- My-Application.war
	///       |-- WEB-INF
	///           |-- classes
	///                   |-- process0.bpmn    
	///                   |-- directory/process1.bpmn
	///                   |-- alternateDirectory/process2.bpmn 
	///           |-- lib/
	///               |-- pa2.jar 
	///                   |-- META-INF/processes.xml uses classpath:directory/
	/// </pre> 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentWithMultiplePasAsSubdeployment3 extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentWithMultiplePasAsSubdeployment3 : AbstractFoxPlatformIntegrationTest
	{

	  public static readonly string PROCESSES_XML = "<process-application xmlns=\"http://www.camunda.org/schema/1.0/ProcessApplication\">" +

		  "<process-archive name=\"PA_NAME\">" +
			"<properties>" +
			  "<property name=\"isDeleteUponUndeploy\">true</property>" +
			  "<property name=\"resourceRootPath\">classpath:directory/</property>" +
			"</properties>" +
		  "</process-archive>" +

		"</process-application>";


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
	  public static WebArchive processArchive()
	  {

		Asset pa2ProcessesXml = TestHelper.getStringAsAssetWithReplacements(PROCESSES_XML, new string[][]
		{
			new string[]{"PA_NAME", "PA2"}
		});


		Asset[] processAssets = TestHelper.generateProcessAssets(9);

		JavaArchive pa2 = ShrinkWrap.create(typeof(JavaArchive), "pa2.jar").addAsResource(pa2ProcessesXml, "META-INF/processes.xml");

		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsLibraries(pa2).addAsResource(processAssets[0], "process0.bpmn").addAsResource(processAssets[1], "directory/process1.bpmn").addAsResource(processAssets[2], "alternateDirectory/process2.bpmn").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(deployment);

		return deployment;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {

		assertProcessNotDeployed("process-0");
		assertProcessDeployed("process-1", "PA2");
		assertProcessNotDeployed("process-2");

	  }

	 protected internal virtual void assertProcessNotDeployed(string processKey)
	 {

		long count = repositoryService.createProcessDefinitionQuery().latestVersion().processDefinitionKey(processKey).count();

		Assert.assertEquals("Process with key " + processKey + " should not be deployed", 0, count);
	 }

	  protected internal virtual void assertProcessDeployed(string processKey, string expectedDeploymentName)
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().latestVersion().processDefinitionKey(processKey).singleResult();

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery().deploymentId(processDefinition.DeploymentId);

		Assert.assertEquals(expectedDeploymentName, deploymentQuery.singleResult().Name);

	  }

	}

}