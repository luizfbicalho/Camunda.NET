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
namespace org.camunda.bpm.integrationtest.functional.scriptengine
{
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
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
//ORIGINAL LINE: @RunWith(Arquillian.class) public abstract class AbstractTemplateScriptEngineSupportTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public abstract class AbstractTemplateScriptEngineSupportTest : AbstractFoxPlatformIntegrationTest
	{

	  public const string PROCESS_ID = "testProcess";
	  public const string RESULT_VARIABLE = "foo";
	  public const string EXAMPLE_TEMPLATE = "Hello ${name}!";
	  public const string EXPECTED_RESULT = "Hello world!";

	  public string processInstanceId;

	  protected internal static StringAsset createScriptTaskProcess(string scriptFormat, string scriptText)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).camundaResultVariable(RESULT_VARIABLE).userTask().endEvent().done();
		return new StringAsset(Bpmn.convertToString(modelInstance));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldEvaluateTemplate()
	  public virtual void shouldEvaluateTemplate()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["name"] = "world";
		processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_ID, variables).Id;

		object result = runtimeService.getVariable(processInstanceId, RESULT_VARIABLE);
		assertNotNull(result);
		assertEquals(EXPECTED_RESULT, result);
	  }

	}

}