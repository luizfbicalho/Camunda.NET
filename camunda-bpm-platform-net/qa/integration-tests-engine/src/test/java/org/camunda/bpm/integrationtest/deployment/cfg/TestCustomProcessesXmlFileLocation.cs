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
namespace org.camunda.bpm.integrationtest.deployment.cfg
{
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>This test ensures that we can use a custom location for the <em>processes.xml</em> file.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestCustomProcessesXmlFileLocation extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestCustomProcessesXmlFileLocation : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/processes.xml", "my/alternate/location/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(CustomProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/invoice-it.bpmn20.xml");

		return archive;

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;
		long count = repositoryService.createProcessDefinitionQuery().processDefinitionKey("invoice-it").count();

		Assert.assertEquals(1, count);
	  }



	}

}