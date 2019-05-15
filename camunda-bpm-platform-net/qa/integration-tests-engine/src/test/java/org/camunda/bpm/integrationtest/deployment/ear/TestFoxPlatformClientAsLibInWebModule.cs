﻿/*
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
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EnterpriseArchive = org.jboss.shrinkwrap.api.spec.EnterpriseArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// This test verifies that a process archive packaging the camunda BPM platform client
	/// can be packaged inside an EAR application.
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestFoxPlatformClientAsLibInWebModule extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestFoxPlatformClientAsLibInWebModule : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.EnterpriseArchive deployment()
		public static EnterpriseArchive deployment()
		{

		// this creates the process archive as a WAR file
		WebArchive processArchive = initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchive.bpmn20.xml").addClass(typeof(TestFoxPlatformClientAsLibInWebModule));

		// this packages the WAR file inside an EAR file
		return ShrinkWrap.create(typeof(EnterpriseArchive), "test-application.ear").addAsModule(processArchive);

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;
		long count = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testDeployProcessArchive").count();

		Assert.assertEquals(1, count);
	  }

	}

}