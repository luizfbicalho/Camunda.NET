using System;

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

	using CustomServletPA = org.camunda.bpm.integrationtest.deployment.war.apps.CustomServletPA;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
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
	/// <para>This test makes sure that if we deploy a process application which contains a persistence.xml 
	/// file which references a non existing datasource, the MSC does not run into a deadlock.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentWithNonExistingDS_JBOSS
	public class TestWarDeploymentWithNonExistingDS_JBOSS
	{

	  private const string DEPLOYMENT_WITH_EJB_PA = "deployment-with-EJB-PA";
	  private const string DEPLOYMENT_WITH_SERVLET_PA = "deployment-with-SERVLET-PA";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
	  private Deployer deployer;

	  [Deployment(managed:false, name:DEPLOYMENT_WITH_EJB_PA)]
	  public static WebArchive createDeployment1()
	  {
		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test1.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addAsResource("persistence-nonexisting-ds.xml", "META-INF/persistence.xml");

		TestContainer.addContainerSpecificResources(archive);

		return archive;
	  }

	  [Deployment(managed:false, name:DEPLOYMENT_WITH_SERVLET_PA)]
	  public static WebArchive createDeployment2()
	  {
		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test2.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addAsResource("persistence-nonexisting-ds.xml", "META-INF/persistence.xml").addClass(typeof(CustomServletPA));

		return archive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RunAsClient public void testDeploymentFails()
	  public virtual void testDeploymentFails()
	  {

		try
		{
		  deployer.deploy(DEPLOYMENT_WITH_EJB_PA);
		  Assert.fail("Deployment exception expected");
		}
		catch (Exception)
		{
		  // expected
		}

		try
		{
		  deployer.deploy(DEPLOYMENT_WITH_SERVLET_PA);
		  Assert.fail("Deployment exception expected");
		}
		catch (Exception)
		{
		  // expected
		}

	  }

	}

}