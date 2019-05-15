﻿using System;

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
namespace org.camunda.bpm.integrationtest.functional.drools
{
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using RunAsClient = org.jboss.arquillian.container.test.api.RunAsClient;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// In this test we make sure that if a user deploys a WAR file with a broken
	/// .bpmn-XML file, the deployment fails.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestDeploymentWithDroolsTaskFails
	public class TestDeploymentWithDroolsTaskFails
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
		private Deployer deployer;

	  [Deployment(managed:false, name:"deployment")]
	  public static WebArchive processArchive()
	  {

		return ShrinkWrap.create(typeof(WebArchive), "test.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EjbClient).addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addAsResource("org/camunda/bpm/integrationtest/functional/drools/TestDeploymentWithDroolsTaskFails.testDeployDroolsFails.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RunAsClient public void testDeployDroolsFails()
	  public virtual void testDeployDroolsFails()
	  {
		try
		{
		  deployer.deploy("deployment");
		  Assert.fail("exception expected");
		}
		catch (Exception)
		{
		  // expected
		}
	  }

	}

}