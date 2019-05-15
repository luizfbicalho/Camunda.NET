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
namespace org.camunda.bpm.rest
{
	using CustomProcessEngineProvider = org.camunda.bpm.rest.beans.CustomProcessEngineProvider;
	using CustomRestApplication = org.camunda.bpm.rest.beans.CustomRestApplication;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using RunAsClient = org.jboss.arquillian.container.test.api.RunAsClient;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Maven = org.jboss.shrinkwrap.resolver.api.maven.Maven;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class EmbeddedEngineRest_WILDFLY
	public class EmbeddedEngineRest_WILDFLY
	{

	  private const string EMBEDDED_ENGINE_REST = "embedded-engine-rest";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
	  private Deployer deployer;

	  [Deployment(managed:false, name : EMBEDDED_ENGINE_REST)]
	  public static WebArchive createDeployment()
	  {
		JavaArchive[] engineRestClasses = EngineRestClasses;
		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "embedded-engine-rest.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsWebInfResource("jboss-deployment-structure.xml").addAsManifestResource("org.camunda.bpm.engine.rest.spi.ProcessEngineProvider", "services/org.camunda.bpm.engine.rest.spi.ProcessEngineProvider").addAsLibraries(engineRestClasses).addClasses(typeof(CustomRestApplication), typeof(CustomProcessEngineProvider));

		return archive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RunAsClient public void testDeploymentWorks() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeploymentWorks()
	  {
		try
		{
		  deployer.deploy(EMBEDDED_ENGINE_REST);
		  deployer.undeploy(EMBEDDED_ENGINE_REST);
		}
		catch (Exception e)
		{
		  Assert.fail("Embedded engine-rest deployment failed because of " + e);
		}
	  }

	  private static JavaArchive[] EngineRestClasses
	  {
		  get
		  {
			string coordinates = "org.camunda.bpm:camunda-engine-rest:jar:classes:" + System.getProperty("projectversion");
    
			JavaArchive[] resolvedArchives = Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve(coordinates).withTransitivity().@as(typeof(JavaArchive));
    
			  if (resolvedArchives.Length < 1)
			  {
				throw new Exception("Could not resolve " + coordinates);
			  }
			  else
			  {
				return resolvedArchives;
			  }
		  }
	  }
	}

}