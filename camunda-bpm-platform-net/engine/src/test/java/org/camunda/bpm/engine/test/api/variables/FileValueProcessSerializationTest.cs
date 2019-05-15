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
namespace org.camunda.bpm.engine.test.api.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class FileValueProcessSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSerializeFileVariable()
	  public virtual void testSerializeFileVariable()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy();
		VariableMap variables = Variables.createVariables();
		string filename = "test.txt";
		string type = "text/plain";
		FileValue fileValue = Variables.fileValue(filename).file("ABC".GetBytes()).encoding("UTF-8").mimeType(type).create();
		variables.put("file", fileValue);
		runtimeService.startProcessInstanceByKey("process", variables);
		Task task = taskService.createTaskQuery().singleResult();
		VariableInstance result = runtimeService.createVariableInstanceQuery().processInstanceIdIn(task.ProcessInstanceId).singleResult();
		FileValue value = (FileValue) result.TypedValue;

		assertThat(value.Filename, @is(filename));
		assertThat(value.MimeType, @is(type));
		assertThat(value.Encoding, @is("UTF-8"));
		assertThat(value.EncodingAsCharset, @is(Charset.forName("UTF-8")));
		using (Scanner scanner = new Scanner(value.Value))
		{
		  assertThat(scanner.nextLine(), @is("ABC"));
		}

		// clean up
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializeNullMimeType()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializeNullMimeType()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("fileVar", Variables.fileValue("test.txt").file("ABC".GetBytes()).encoding("UTF-8").create()));

		FileValue fileVar = runtimeService.getVariableTyped(pi.Id, "fileVar");
		assertNull(fileVar.MimeType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializeNullMimeTypeAndNullEncoding()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializeNullMimeTypeAndNullEncoding()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("fileVar", Variables.fileValue("test.txt").file("ABC".GetBytes()).create()));

		FileValue fileVar = runtimeService.getVariableTyped(pi.Id, "fileVar");
		assertNull(fileVar.MimeType);
		assertNull(fileVar.Encoding);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializeNullEncoding()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializeNullEncoding()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("fileVar", Variables.fileValue("test.txt").mimeType("some mimetype").file("ABC".GetBytes()).create()));

		FileValue fileVar = runtimeService.getVariableTyped(pi.Id, "fileVar");
		assertNull(fileVar.Encoding);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializeNullValue()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializeNullValue()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("fileVar", Variables.fileValue("test.txt").create()));

		FileValue fileVar = runtimeService.getVariableTyped(pi.Id, "fileVar");
		assertNull(fileVar.MimeType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = ONE_TASK_PROCESS) public void testSerializeEmptyFileName()
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializeEmptyFileName()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("fileVar", Variables.fileValue("").create()));

		FileValue fileVar = runtimeService.getVariableTyped(pi.Id, "fileVar");
		assertEquals("", fileVar.Filename);
	  }

	}

}