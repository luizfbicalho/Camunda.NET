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
namespace org.camunda.bpm.engine.cdi.test.api
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class BusinessProcessBeanTest : CdiProcessEngineTestCase
	{

	  /* General test asserting that the business process bean is functional */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void test() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void test()
	  {

		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// start the process
		businessProcess.startProcessByKey("businessProcessBeanTest").Id;

		// ensure that the process is started:
		assertNotNull(processEngine.RuntimeService.createProcessInstanceQuery().singleResult());

		// ensure that there is a single task waiting
		Task task = processEngine.TaskService.createTaskQuery().singleResult();
		assertNotNull(task);

		string value = "value";
		businessProcess.setVariable("key", Variables.stringValue(value));
		assertEquals(value, businessProcess.getVariable("key"));

		// Typed variable API
		TypedValue typedValue = businessProcess.getVariableTyped("key");
		assertEquals(ValueType.STRING, typedValue.Type);
		assertEquals(value, typedValue.Value);

		// Local variables
		string localValue = "localValue";
		businessProcess.setVariableLocal("localKey", Variables.stringValue(localValue));
		assertEquals(localValue, businessProcess.getVariableLocal("localKey"));

		// Local typed variable API
		TypedValue typedLocalValue = businessProcess.getVariableLocalTyped("localKey");
		assertEquals(ValueType.STRING, typedLocalValue.Type);
		assertEquals(localValue, typedLocalValue.Value);

		// complete the task
		assertEquals(task.Id, businessProcess.startTask(task.Id).Id);
		businessProcess.completeTask();

		// assert the task is completed
		assertNull(processEngine.TaskService.createTaskQuery().singleResult());

		// assert that the process is ended:
		assertNull(processEngine.RuntimeService.createProcessInstanceQuery().singleResult());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testProcessWithoutWatestate()
	  public virtual void testProcessWithoutWatestate()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// start the process
		businessProcess.startProcessByKey("businessProcessBeanTest").Id;

		// assert that the process is ended:
		assertNull(processEngine.RuntimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testResolveProcessInstanceBean()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testResolveProcessInstanceBean()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		assertNull(getBeanInstance(typeof(ProcessInstance)));
		assertNull(getBeanInstance("processInstanceId"));
		assertNull(getBeanInstance(typeof(Execution)));
		assertNull(getBeanInstance("executionId"));

		string pid = businessProcess.startProcessByKey("businessProcessBeanTest").Id;

		// assert that now we can resolve the ProcessInstance-bean
		assertEquals(pid, getBeanInstance(typeof(ProcessInstance)).Id);
		assertEquals(pid, getBeanInstance("processInstanceId"));
		assertEquals(pid, getBeanInstance(typeof(Execution)).Id);
		assertEquals(pid, getBeanInstance("executionId"));

		taskService.complete(taskService.createTaskQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testResolveTaskBean()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testResolveTaskBean()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		assertNull(getBeanInstance(typeof(Task)));
		assertNull(getBeanInstance("taskId"));


		businessProcess.startProcessByKey("businessProcessBeanTest");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		businessProcess.startTask(taskId);

		// assert that now we can resolve the Task-bean
		assertEquals(taskId, getBeanInstance(typeof(Task)).Id);
		assertEquals(taskId, getBeanInstance("taskId"));

		taskService.complete(taskService.createTaskQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") @SuppressWarnings("deprecation") public void testGetVariableCache()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetVariableCache()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableCache);

		// set a variable
		businessProcess.setVariable("aVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.VariableCache);

		// getting the variable cache does not empty it:
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.VariableCache);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableCache);

		// set a variable
		businessProcess.setVariable("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.VariableCache);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testGetCachedVariableMap()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetCachedVariableMap()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedVariableMap);

		// set a variable
		businessProcess.setVariable("aVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.CachedVariableMap);

		// getting the variable cache does not empty it:
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.CachedVariableMap);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedVariableMap);

		// set a variable
		businessProcess.setVariable("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.CachedVariableMap);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") @SuppressWarnings("deprecation") public void testGetAndClearVariableCache()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetAndClearVariableCache()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearVariableCache);

		// set a variable
		businessProcess.setVariable("aVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.AndClearVariableCache);

		// now the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearVariableCache);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableCache);

		// set a variable
		businessProcess.setVariable("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.VariableCache);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testGetAndClearCachedVariableMap()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetAndClearCachedVariableMap()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedVariableMap);

		// set a variable
		businessProcess.setVariable("aVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("aVariableName", "aVariableValue"), businessProcess.AndClearCachedVariableMap);

		// now the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedVariableMap);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedVariableMap);

		// set a variable
		businessProcess.setVariable("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.AndClearCachedVariableMap);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") @SuppressWarnings("deprecation") public void testGetVariableLocalCache()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetVariableLocalCache()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableLocalCache);

		// set a variable - this should fail before the process is started
		try
		{
		  businessProcess.setVariableLocal("aVariableName", "aVariableValue");
		  fail("exception expected!");
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("Cannot set a local cached variable: neither a Task nor an Execution is associated.", e.Message);
		}

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableLocalCache);

		// set a variable
		businessProcess.setVariableLocal("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.VariableLocalCache);

		// getting the variable cache does not empty it:
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.VariableLocalCache);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testGetCachedLocalVariableMap()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetCachedLocalVariableMap()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedLocalVariableMap);

		// set a variable - this should fail before the process is started
		try
		{
		  businessProcess.setVariableLocal("aVariableName", "aVariableValue");
		  fail("exception expected!");
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("Cannot set a local cached variable: neither a Task nor an Execution is associated.", e.Message);
		}

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedLocalVariableMap);

		// set a variable
		businessProcess.setVariableLocal("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.CachedLocalVariableMap);

		// getting the variable cache does not empty it:
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.CachedLocalVariableMap);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testGetVariableLocal()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetVariableLocal()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));
		ProcessInstance processInstance = businessProcess.startProcessByKey("businessProcessBeanTest");

		TaskService taskService = getBeanInstance(typeof(TaskService));
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(task);

		businessProcess.startTask(task.Id);

		businessProcess.setVariableLocal("aVariableName", "aVariableValue");

		// Flushing and re-getting should retain the value (CAM-1806):
		businessProcess.flushVariableCache();
		assertTrue(businessProcess.CachedLocalVariableMap.Empty);
		assertEquals("aVariableValue", businessProcess.getVariableLocal("aVariableName"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") @SuppressWarnings("deprecation") public void testGetAndClearVariableLocalCache()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetAndClearVariableLocalCache()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearVariableLocalCache);

		// set a variable - this should fail before the process is started
		try
		{
		  businessProcess.setVariableLocal("aVariableName", "aVariableValue");
		  fail("exception expected!");
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("Cannot set a local cached variable: neither a Task nor an Execution is associated.", e.Message);
		}

		// the variable cache is still empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearVariableLocalCache);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.VariableLocalCache);

		// set a variable
		businessProcess.setVariableLocal("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.VariableLocalCache);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testGetAndClearCachedLocalVariableMap()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testGetAndClearCachedLocalVariableMap()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// initially the variable cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedLocalVariableMap);

		// set a variable - this should fail before the process is started
		try
		{
		  businessProcess.setVariableLocal("aVariableName", "aVariableValue");
		  fail("exception expected!");
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("Cannot set a local cached variable: neither a Task nor an Execution is associated.", e.Message);
		}

		// the variable cache is still empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedLocalVariableMap);

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// now the variable cache is empty again:
		assertEquals(Collections.EMPTY_MAP, businessProcess.AndClearCachedLocalVariableMap);

		// set a variable
		businessProcess.setVariableLocal("anotherVariableName", "aVariableValue");

		// now the variable is set
		assertEquals(Collections.singletonMap("anotherVariableName", "aVariableValue"), businessProcess.AndClearCachedLocalVariableMap);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testFlushVariableCache()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testFlushVariableCache()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// cannot flush variable cache in absence of an association:
		try
		{
		  businessProcess.flushVariableCache();
		  fail("exception expected!");

		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("Cannot flush variable cache: neither a Task nor an Execution is associated.", e.Message);

		}

		businessProcess.startProcessByKey("businessProcessBeanTest");

		// set a variable
		businessProcess.setVariable("aVariableName", "aVariable");

		// the variable is not yet present in the execution:
		assertNull(runtimeService.getVariable(businessProcess.ExecutionId, "aVariableName"));

		// set a local variable
		businessProcess.setVariableLocal("aVariableLocalName", "aVariableLocal");

		// the local variable is not yet present in the execution:
		assertNull(runtimeService.getVariable(businessProcess.ExecutionId, "aVariableLocalName"));

		// flush the cache
		businessProcess.flushVariableCache();

		// the variable is flushed to the execution
		assertNotNull(runtimeService.getVariable(businessProcess.ExecutionId, "aVariableName"));

		// the local variable is flushed to the execution
		assertNotNull(runtimeService.getVariable(businessProcess.ExecutionId, "aVariableLocalName"));

		// the cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedVariableMap);

		// the cache is empty
		assertEquals(Collections.EMPTY_MAP, businessProcess.CachedLocalVariableMap);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testSaveTask()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testSaveTask()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// cannot save task in absence of an association:
		try
		{
		  businessProcess.saveTask();
		  fail();
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("No task associated. Call businessProcess.startTask() first.", e.Message);
		}

		// start the process
		string processInstanceId = businessProcess.startProcessByKey("businessProcessBeanTest", Collections.singletonMap("key", (object) "value")).Id;
		assertEquals("value", runtimeService.getVariable(processInstanceId, "key"));

		businessProcess.startTask(taskService.createTaskQuery().singleResult().Id);

		// assignee is not set to jonny
		assertNull(taskService.createTaskQuery().taskAssignee("jonny").singleResult());
		Task task = businessProcess.Task;
		task.Assignee = "jonny";

		assertNull(taskService.createTaskQuery().taskAssignee("jonny").singleResult());

		// if we save the task
		businessProcess.saveTask();

		// THEN

		// assignee is now set to jonny
		assertNotNull(taskService.createTaskQuery().taskAssignee("jonny").singleResult());
		// business process is still associated with task:
		assertTrue(businessProcess.TaskAssociated);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testStopTask()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testStopTask()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		// cannot stop task in absence of an association:
		try
		{
		  businessProcess.stopTask();
		  fail();
		}
		catch (ProcessEngineCdiException e)
		{
		  assertEquals("No task associated. Call businessProcess.startTask() first.", e.Message);
		}

		// start the process
		string processInstanceId = businessProcess.startProcessByKey("businessProcessBeanTest", Collections.singletonMap("key", (object) "value")).Id;
		assertEquals("value", runtimeService.getVariable(processInstanceId, "key"));

		businessProcess.startTask(taskService.createTaskQuery().singleResult().Id);

		// assignee is not set to jonny
		assertNull(taskService.createTaskQuery().taskAssignee("jonny").singleResult());
		Task task = businessProcess.Task;
		task.Assignee = "jonny";

		// if we stop the task
		businessProcess.stopTask();

		// THEN

		// assignee is not set to jonny
		assertNull(taskService.createTaskQuery().taskAssignee("jonny").singleResult());
		// business process is not associated with task:
		assertFalse(businessProcess.TaskAssociated);
		assertFalse(businessProcess.Associated);
	  }

	}

}