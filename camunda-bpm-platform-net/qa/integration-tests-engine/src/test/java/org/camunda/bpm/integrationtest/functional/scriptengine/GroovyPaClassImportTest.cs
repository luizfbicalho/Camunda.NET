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

	using CustomClass = org.camunda.bpm.integrationtest.functional.scriptengine.classes.CustomClass;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class GroovyPaClassImportTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class GroovyPaClassImportTest : AbstractFoxPlatformIntegrationTest
	{

	  public const string SCRIPT_WITH_IMPORT = "import org.camunda.bpm.integrationtest.functional.scriptengine.classes.CustomClass\n"
		+ "execution.setVariable('greeting', new CustomClass().greet())";

	  public const string GROOVY_MODULE_DEPENDENCY = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
		+ "<jboss-deployment-structure>"
		+ "  <deployment>"
		+ "    <dependencies>"
		+ "      <module name=\"org.codehaus.groovy.groovy-all\" services=\"import\" />"
		+ "    </dependencies>"
		+ "  </deployment>"
		+ "</jboss-deployment-structure>";

	  protected internal static StringAsset createScriptTaskProcess(string scriptFormat, string scriptText, string pdk)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(pdk).startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).userTask().endEvent().done();
		return new StringAsset(Bpmn.convertToString(modelInstance));
	  }

	  [Deployment(name:"pa1")]
	  public static WebArchive createProcessApplication1()
	  {
		return initWebArchiveDeployment("pa1.war").addAsWebInfResource(new StringAsset(GROOVY_MODULE_DEPENDENCY),"jboss-deployment-structure.xml").addAsResource(createScriptTaskProcess("groovy", "", "process1"), "process1.bpmn20.xml");
	  }

	  [Deployment(name:"pa2")]
	  public static WebArchive createProcessApplication2()
	  {
		return initWebArchiveDeployment("pa2.war").addClass(typeof(CustomClass)).addAsWebInfResource(new StringAsset(GROOVY_MODULE_DEPENDENCY),"jboss-deployment-structure.xml").addAsResource(createScriptTaskProcess("groovy", SCRIPT_WITH_IMPORT, "process2"), "process2.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa1") public void shouldSetVariable()
	  public virtual void shouldSetVariable()
	  {
		// first start process 1 (this creates and caches the groovy engine)
		runtimeService.startProcessInstanceByKey("process1").Id;

		// then start process 2
		string processInstanceId = runtimeService.startProcessInstanceByKey("process2").Id;
		object foo = runtimeService.getVariable(processInstanceId, "greeting");
		assertNotNull(foo);
		assertEquals("Hi Ho", foo);
	  }

	}

}