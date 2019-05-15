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
namespace org.camunda.bpm.integrationtest.functional.connect
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using Task = org.camunda.bpm.engine.task.Task;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Connectors = org.camunda.connect.Connectors;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>Smoketest Make sure camunda connect can be used in a process application </para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaConnectSupportTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaConnectSupportTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		return initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/functional/connect/PaConnectSupportTest.connectorServiceTask.bpmn20.xml").addClass(typeof(TestConnector)).addClass(typeof(TestConnectorRequest)).addClass(typeof(TestConnectorResponse)).addClass(typeof(TestConnectors));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void httpConnectorShouldBeAvailable()
	  public virtual void httpConnectorShouldBeAvailable()
	  {
		assertNotNull(Connectors.http());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void soapConnectorShouldBeAvailable()
	  public virtual void soapConnectorShouldBeAvailable()
	  {
		assertNotNull(Connectors.soap());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void connectorServiceTask()
	  public virtual void connectorServiceTask()
	  {
		TestConnector connector = new TestConnector();
		TestConnectors.registerConnector(connector);

		runtimeService.startProcessInstanceByKey("testProcess");
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		string payload = (string) taskService.getVariable(task.Id, "payload");
		assertEquals("Hello world!", payload);

		TestConnectors.unregisterConnector(connector.Id);
	  }

	}

}