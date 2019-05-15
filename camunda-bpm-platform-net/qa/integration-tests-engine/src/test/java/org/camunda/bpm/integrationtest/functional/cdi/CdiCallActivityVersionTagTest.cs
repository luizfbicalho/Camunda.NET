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
namespace org.camunda.bpm.integrationtest.functional.cdi
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VersionTagBean = org.camunda.bpm.integrationtest.functional.cdi.beans.VersionTagBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiCallActivityVersionTagTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiCallActivityVersionTagTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeployment()
		public static WebArchive createProcessArchiveDeployment()
		{
		return initWebArchiveDeployment().addClass(typeof(VersionTagBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiCallActivityVersionTagTest.subProcess.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiCallActivityVersionTagTest.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testStartInstanceWithBeanCondition()
	  public virtual void testStartInstanceWithBeanCondition()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		// then
		ProcessInstance subInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("subProcess").superProcessInstanceId(processInstance.Id).singleResult();
		assertNotNull(subInstance);

	  }
	}

}