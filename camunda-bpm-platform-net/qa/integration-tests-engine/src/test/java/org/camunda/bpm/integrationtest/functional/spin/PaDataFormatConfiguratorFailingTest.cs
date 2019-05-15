using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.integrationtest.functional.spin
{
	using FailingJsonDataFormatConfigurator = org.camunda.bpm.integrationtest.functional.spin.dataformat.FailingJsonDataFormatConfigurator;
	using JsonSerializable = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonSerializable;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using DataFormatConfigurator = org.camunda.spin.spi.DataFormatConfigurator;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaDataFormatConfiguratorFailingTest
	public class PaDataFormatConfiguratorFailingTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
		private Deployer deployer;

	  [Deployment(managed : false, name : "deployment")]
	  public static WebArchive createDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "PaDataFormatConfiguratorFailingTest.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ReferenceStoringProcessApplication)).addAsResource("org/camunda/bpm/integrationtest/oneTaskProcess.bpmn").addClass(typeof(JsonSerializable)).addClass(typeof(FailingJsonDataFormatConfigurator)).addAsServiceProvider(typeof(DataFormatConfigurator), typeof(FailingJsonDataFormatConfigurator));

		TestContainer.addSpinJacksonJsonDataFormat(webArchive);

		return webArchive;

	  }

	  [Deployment(name : "checkDeployment")]
	  public static WebArchive createCheckDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive));
		TestContainer.addContainerSpecificResourcesForNonPa(webArchive);
		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		try
		{
		  deployer.deploy("deployment");
		  // The failing configurator provokes a RuntimeException in a servlet context listener.
		  // Apparently such an exception needs not cancel the deployment of a Java EE application.
		  // That means deployment fails for some servers and for others not.
		  // => we don't care if there is an exception here or not
		}
		catch (Exception)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("checkDeployment") public void testNoProcessApplicationIsDeployed()
	  public virtual void testNoProcessApplicationIsDeployed()
	  {
		ISet<string> registeredPAs = BpmPlatform.ProcessApplicationService.ProcessApplicationNames;
		Assert.assertTrue(registeredPAs.Count == 0);
	  }
	}

}