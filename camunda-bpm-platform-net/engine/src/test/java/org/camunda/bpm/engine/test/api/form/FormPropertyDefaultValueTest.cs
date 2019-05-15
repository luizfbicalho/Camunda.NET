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
namespace org.camunda.bpm.engine.test.api.form
{

	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	public class FormPropertyDefaultValueTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDefaultValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDefaultValue()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("FormPropertyDefaultValueTest.testDefaultValue");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		TaskFormData formData = formService.getTaskFormData(task.Id);
		IList<FormProperty> formProperties = formData.FormProperties;
		assertEquals(4, formProperties.Count);

		foreach (FormProperty prop in formProperties)
		{
		  if ("booleanProperty".Equals(prop.Id))
		  {
			assertEquals("true", prop.Value);
		  }
		  else if ("stringProperty".Equals(prop.Id))
		  {
			assertEquals("someString", prop.Value);
		  }
		  else if ("longProperty".Equals(prop.Id))
		  {
			assertEquals("42", prop.Value);
		  }
		  else if ("longExpressionProperty".Equals(prop.Id))
		  {
			assertEquals("23", prop.Value);
		  }
		  else
		  {
			assertTrue("Invalid form property: " + prop.Id, false);
		  }
		}

		IDictionary<string, string> formDataUpdate = new Dictionary<string, string>();
		formDataUpdate["longExpressionProperty"] = "1";
		formDataUpdate["booleanProperty"] = "false";
		formService.submitTaskFormData(task.Id, formDataUpdate);

		assertEquals(false, runtimeService.getVariable(processInstance.Id, "booleanProperty"));
		assertEquals("someString", runtimeService.getVariable(processInstance.Id, "stringProperty"));
		assertEquals(42L, runtimeService.getVariable(processInstance.Id, "longProperty"));
		assertEquals(1L, runtimeService.getVariable(processInstance.Id, "longExpressionProperty"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStartFormDefaultValue() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testStartFormDefaultValue()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("FormPropertyDefaultValueTest.testDefaultValue").latestVersion().singleResult().Id;

		StartFormData startForm = formService.getStartFormData(processDefinitionId);


		IList<FormProperty> formProperties = startForm.FormProperties;
		assertEquals(4, formProperties.Count);

		foreach (FormProperty prop in formProperties)
		{
		  if ("booleanProperty".Equals(prop.Id))
		  {
			assertEquals("true", prop.Value);
		  }
		  else if ("stringProperty".Equals(prop.Id))
		  {
			assertEquals("someString", prop.Value);
		  }
		  else if ("longProperty".Equals(prop.Id))
		  {
			assertEquals("42", prop.Value);
		  }
		  else if ("longExpressionProperty".Equals(prop.Id))
		  {
			assertEquals("23", prop.Value);
		  }
		  else
		  {
			assertTrue("Invalid form property: " + prop.Id, false);
		  }
		}

		// Override 2 properties. The others should pe posted as the default-value
		IDictionary<string, string> formDataUpdate = new Dictionary<string, string>();
		formDataUpdate["longExpressionProperty"] = "1";
		formDataUpdate["booleanProperty"] = "false";
		ProcessInstance processInstance = formService.submitStartFormData(processDefinitionId, formDataUpdate);

		assertEquals(false, runtimeService.getVariable(processInstance.Id, "booleanProperty"));
		assertEquals("someString", runtimeService.getVariable(processInstance.Id, "stringProperty"));
		assertEquals(42L, runtimeService.getVariable(processInstance.Id, "longProperty"));
		assertEquals(1L, runtimeService.getVariable(processInstance.Id, "longExpressionProperty"));
	  }
	}

}