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
namespace org.camunda.bpm.integrationtest.functional.scriptengine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using Job = org.camunda.bpm.engine.runtime.Job;
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
//ORIGINAL LINE: @RunWith(Arquillian.class) public class GroovyAsyncScriptExecutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class GroovyAsyncScriptExecutionTest : AbstractFoxPlatformIntegrationTest
	{

	  protected internal static string process = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
		  "<definitions id=\"definitions\" \r\n" +
		  "  xmlns=\"http://www.omg.org/spec/BPMN/20100524/MODEL\"\r\n" +
		  "  xmlns:camunda=\"http://camunda.org/schema/1.0/bpmn\"\r\n" +
		  "  targetNamespace=\"Examples\">\r\n" +
		  "  <process id=\"process\" isExecutable=\"true\">\r\n" +
		  "    <startEvent id=\"theStart\" />\r\n" +
		  "    <sequenceFlow id=\"flow1\" sourceRef=\"theStart\" targetRef=\"theScriptTask\" />\r\n" +
		  "    <scriptTask id=\"theScriptTask\" name=\"Execute script\" scriptFormat=\"groovy\" camunda:asyncBefore=\"true\">\r\n" +
		  "      <script>execution.setVariable(\"foo\", S(\"&lt;bar /&gt;\").name())</script>\r\n" +
		  "    </scriptTask>\r\n" +
		  "    <sequenceFlow id=\"flow2\" sourceRef=\"theScriptTask\" targetRef=\"theTask\" />\r\n" +
		  "    <userTask id=\"theTask\" name=\"my task\" />\r\n" +
		  "    <sequenceFlow id=\"flow3\" sourceRef=\"theTask\" targetRef=\"theEnd\" />\r\n" +
		  "    <endEvent id=\"theEnd\" />\r\n" +
		  "  </process>\r\n" +
		  "</definitions>";

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);
		TestContainer.addContainerSpecificResourcesForNonPa(deployment);
		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void shouldSetVariable()
	  public virtual void shouldSetVariable()
	  {
		string deploymentId = repositoryService.createDeployment().addString("process.bpmn", process).deploy().Id;

		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		waitForJobExecutorToProcessAllJobs(30000);

		object foo = runtimeService.getVariable(processInstanceId, "foo");
		assertNotNull(foo);
		assertEquals("bar", foo);

		repositoryService.deleteDeployment(deploymentId, true);
	  }
	}

}