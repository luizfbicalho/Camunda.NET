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
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using GroovyProcessEnginePlugin = org.camunda.bpm.integrationtest.deployment.war.beans.GroovyProcessEnginePlugin;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Maven = org.jboss.shrinkwrap.resolver.api.maven.Maven;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	/// <summary>
	/// Assert that we can deploy a WAR with a process engine plugin
	/// which ships and requires groovy as a dependency for scripting purposes.
	/// 
	/// Does not work on JBoss, see https://app.camunda.com/jira/browse/CAM-1778
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentWithProcessEnginePlugin extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentWithProcessEnginePlugin : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment("test.war", "singleEngineWithProcessEnginePlugin.xml").addClass(typeof(GroovyProcessEnginePlugin)).addAsResource("org/camunda/bpm/integrationtest/deployment/war/groovy.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/deployment/war/groovyAsync.bpmn20.xml").addAsLibraries(Maven.resolver().offline().loadPomFromFile("pom.xml").resolve("org.codehaus.groovy:groovy-all").withoutTransitivity().@as(typeof(JavaArchive)));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPAGroovyProcessEnginePlugin()
	  public virtual void testPAGroovyProcessEnginePlugin()
	  {
		ProcessEngine groovyEngine = processEngineService.getProcessEngine("groovy");
		Assert.assertNotNull(groovyEngine);

		ProcessInstance pi = groovyEngine.RuntimeService.startProcessInstanceByKey("groovy");
		HistoricProcessInstance hpi = groovyEngine.HistoryService.createHistoricProcessInstanceQuery().processDefinitionKey("groovy").finished().singleResult();
		assertEquals(pi.Id, hpi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPAGroovyAsyncProcessEnginePlugin()
	  public virtual void testPAGroovyAsyncProcessEnginePlugin()
	  {
		ProcessEngine groovyEngine = processEngineService.getProcessEngine("groovy");
		Assert.assertNotNull(groovyEngine);

		ProcessInstance pi = groovyEngine.RuntimeService.startProcessInstanceByKey("groovyAsync");

		waitForJobExecutorToProcessAllJobs();

		HistoricProcessInstance hpi = groovyEngine.HistoryService.createHistoricProcessInstanceQuery().processDefinitionKey("groovyAsync").finished().singleResult();
		assertEquals(pi.Id, hpi.Id);
	  }

	}

}