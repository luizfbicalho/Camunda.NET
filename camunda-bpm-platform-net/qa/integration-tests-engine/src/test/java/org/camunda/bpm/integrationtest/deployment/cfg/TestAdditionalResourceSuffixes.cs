﻿using System.Collections.Generic;

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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Resource = org.camunda.bpm.engine.repository.Resource;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestAdditionalResourceSuffixes extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestAdditionalResourceSuffixes : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/processes-additional-resource-suffixes.xml", "META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(DummyProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/invoice-it.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/hello.groovy").addAsResource("org/camunda/bpm/integrationtest/deployment/cfg/hello.py");

		return archive;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;

		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery().processDefinitionKey("invoice-it");

		assertEquals(1, processDefinitionQuery.count());
		ProcessDefinition processDefinition = processDefinitionQuery.singleResult();

		string deploymentId = repositoryService.createDeploymentQuery().deploymentId(processDefinition.DeploymentId).singleResult().Id;
		IList<Resource> deploymentResources = repositoryService.getDeploymentResources(deploymentId);
		assertEquals(3, deploymentResources.Count);
	  }

	}

}