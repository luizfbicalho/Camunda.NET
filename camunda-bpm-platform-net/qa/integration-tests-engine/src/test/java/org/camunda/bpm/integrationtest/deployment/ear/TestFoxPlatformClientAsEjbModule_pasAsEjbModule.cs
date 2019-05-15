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
namespace org.camunda.bpm.integrationtest.deployment.ear
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using EeComponent = org.camunda.bpm.integrationtest.deployment.ear.beans.EeComponent;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using EnterpriseArchive = org.jboss.shrinkwrap.api.spec.EnterpriseArchive;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// This test verifies that a process archive packaging the camunda BPM platform client
	/// can be packaged inside an EAR application.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestFoxPlatformClientAsEjbModule_pasAsEjbModule extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestFoxPlatformClientAsEjbModule_pasAsEjbModule : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.EnterpriseArchive paAsEjbModule() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static EnterpriseArchive paAsEjbModule()
		{

		JavaArchive processArchive1Jar = ShrinkWrap.create(typeof(JavaArchive), "pa.jar").addClass(typeof(EeComponent)).addAsResource("org/camunda/bpm/integrationtest/deployment/ear/paAsEjbModule-process.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/deployment/ear/paAsEjbModule-pa.xml", "META-INF/processes.xml");

		JavaArchive foxPlatformClientJar = DeploymentHelper.EjbClient;

		WebArchive testJar = ShrinkWrap.create(typeof(WebArchive), "paAsEjbModule-test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestFoxPlatformClientAsEjbModule_pasAsEjbModule));

		return ShrinkWrap.create(typeof(EnterpriseArchive), "paAsEjbModule.ear").addAsModule(processArchive1Jar).addAsModule(foxPlatformClientJar).addAsModule(testJar).addAsLibrary(DeploymentHelper.EngineCdi);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaAsEjbModule()
	  public virtual void testPaAsEjbModule()
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;
		long count = repositoryService.createProcessDefinitionQuery().processDefinitionKey("paAsEjbModule-process").count();
		Assert.assertEquals(1, count);
	  }

	}

}