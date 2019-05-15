using System;
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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.executionByProcessDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.executionByProcessDefinitionKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.executionByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.hierarchical;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;


	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Assert = org.junit.Assert;


	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class ExecutionQueryTest : PluggableProcessEngineTestCase
	{

	  private static string CONCURRENT_PROCESS_KEY = "concurrent";
	  private static string SEQUENTIAL_PROCESS_KEY = "oneTaskProcess";

	  private IList<string> concurrentProcessInstanceIds;
	  private IList<string> sequentialProcessInstanceIds;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/runtime/concurrentExecution.bpmn20.xml").deploy();

		concurrentProcessInstanceIds = new List<string>();
		sequentialProcessInstanceIds = new List<string>();

		for (int i = 0; i < 4; i++)
		{
		  concurrentProcessInstanceIds.Add(runtimeService.startProcessInstanceByKey(CONCURRENT_PROCESS_KEY, "BUSINESS-KEY-" + i).Id);
		}
		sequentialProcessInstanceIds.Add(runtimeService.startProcessInstanceByKey(SEQUENTIAL_PROCESS_KEY).Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
		base.tearDown();
	  }

	  public virtual void testQueryByProcessDefinitionKey()
	  {
		// Concurrent process with 3 executions for each process instance
		assertEquals(12, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).list().size());
		assertEquals(1, runtimeService.createExecutionQuery().processDefinitionKey(SEQUENTIAL_PROCESS_KEY).list().size());
	  }

	  public virtual void testQueryByInvalidProcessDefinitionKey()
	  {
		ExecutionQuery query = runtimeService.createExecutionQuery().processDefinitionKey("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

	  public virtual void testQueryByProcessInstanceId()
	  {
		foreach (string processInstanceId in concurrentProcessInstanceIds)
		{
		  ExecutionQuery query = runtimeService.createExecutionQuery().processInstanceId(processInstanceId);
		  assertEquals(3, query.list().size());
		  assertEquals(3, query.count());
		}
		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(sequentialProcessInstanceIds[0]).list().size());
	  }

	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		ExecutionQuery query = runtimeService.createExecutionQuery().processInstanceId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

	  public virtual void testQueryExecutionId()
	  {
		Execution execution = runtimeService.createExecutionQuery().processDefinitionKey(SEQUENTIAL_PROCESS_KEY).singleResult();
		assertNotNull(runtimeService.createExecutionQuery().executionId(execution.Id));
	  }

	  public virtual void testQueryByInvalidExecutionId()
	  {
		ExecutionQuery query = runtimeService.createExecutionQuery().executionId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

	  public virtual void testQueryByActivityId()
	  {
		ExecutionQuery query = runtimeService.createExecutionQuery().activityId("receivePayment");
		assertEquals(4, query.list().size());
		assertEquals(4, query.count());

		try
		{
		  assertNull(query.singleResult());
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByInvalidActivityId()
	  {
		ExecutionQuery query = runtimeService.createExecutionQuery().activityId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());
	  }

	  public virtual void testQueryPaging()
	  {
		assertEquals(13, runtimeService.createExecutionQuery().count());
		assertEquals(4, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).listPage(0, 4).size());
		assertEquals(1, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).listPage(2, 1).size());
		assertEquals(10, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).listPage(1, 10).size());
		assertEquals(12, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).listPage(0, 20).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void testQuerySorting()
	  public virtual void testQuerySorting()
	  {

		// 13 executions: 3 for each concurrent, 1 for the sequential
		IList<Execution> executions = runtimeService.createExecutionQuery().orderByProcessInstanceId().asc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, executionByProcessInstanceId());

		executions = runtimeService.createExecutionQuery().orderByProcessDefinitionId().asc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, executionByProcessDefinitionId());

		executions = runtimeService.createExecutionQuery().orderByProcessDefinitionKey().asc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, executionByProcessDefinitionKey(processEngine));

		executions = runtimeService.createExecutionQuery().orderByProcessInstanceId().desc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, inverted(executionByProcessInstanceId()));

		executions = runtimeService.createExecutionQuery().orderByProcessDefinitionId().desc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, inverted(executionByProcessDefinitionId()));

		executions = runtimeService.createExecutionQuery().orderByProcessDefinitionKey().desc().list();
		assertEquals(13, executions.Count);
		verifySorting(executions, inverted(executionByProcessDefinitionKey(processEngine)));

		executions = runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).orderByProcessDefinitionId().asc().list();
		assertEquals(12, executions.Count);
		verifySorting(executions, executionByProcessDefinitionId());

		executions = runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).orderByProcessDefinitionId().desc().list();
		assertEquals(12, executions.Count);
		verifySorting(executions, executionByProcessDefinitionId());

		executions = runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).orderByProcessDefinitionKey().asc().orderByProcessInstanceId().desc().list();
		assertEquals(12, executions.Count);
		verifySorting(executions, hierarchical(executionByProcessDefinitionKey(processEngine), inverted(executionByProcessInstanceId())));
	  }

	  public virtual void testQueryInvalidSorting()
	  {
		try
		{
		  runtimeService.createExecutionQuery().orderByProcessDefinitionKey().list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

	  public virtual void testQueryByBusinessKey()
	  {
		assertEquals(3, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).processInstanceBusinessKey("BUSINESS-KEY-1").list().size());
		assertEquals(3, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).processInstanceBusinessKey("BUSINESS-KEY-2").list().size());
		assertEquals(0, runtimeService.createExecutionQuery().processDefinitionKey(CONCURRENT_PROCESS_KEY).processInstanceBusinessKey("NON-EXISTING").list().size());
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryStringVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["stringVar"] = "abcdef";
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["stringVar"] = "abcdef";
		vars["stringVar2"] = "ghijkl";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["stringVar"] = "azerty";
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Test EQUAL on single string variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("stringVar", "abcdef");
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Test EQUAL on two string variables, should result in single match
		query = runtimeService.createExecutionQuery().variableValueEquals("stringVar", "abcdef").variableValueEquals("stringVar2", "ghijkl");
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Test NOT_EQUAL, should return only 1 execution
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("stringVar", "abcdef").singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN, should return only matching 'azerty'
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("stringVar", "abcdef").singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("stringVar", "z").singleResult();
		Assert.assertNull(execution);

		// Test GREATER_THAN_OR_EQUAL, should return 3 results
		assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("stringVar", "abcdef").count());
		assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN, should return 2 results
		executions = runtimeService.createExecutionQuery().variableValueLessThan("stringVar", "abcdeg").list();
		Assert.assertEquals(2, executions.Count);
		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("stringVar", "abcdef").count());
		assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("stringVar", "abcdef").list();
		Assert.assertEquals(2, executions.Count);
		expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("stringVar", "z").count());
		assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("stringVar", "aa").count());

		// Test LIKE
		execution = runtimeService.createExecutionQuery().variableValueLike("stringVar", "azert%").singleResult();
		assertNotNull(execution);
		assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueLike("stringVar", "%y").singleResult();
		assertNotNull(execution);
		assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueLike("stringVar", "%zer%").singleResult();
		assertNotNull(execution);
		assertEquals(processInstance3.Id, execution.Id);

		assertEquals(3, runtimeService.createExecutionQuery().variableValueLike("stringVar", "a%").count());
		assertEquals(0, runtimeService.createExecutionQuery().variableValueLike("stringVar", "%x%").count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryLongVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["longVar"] = 12345L;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["longVar"] = 12345L;
		vars["longVar2"] = 67890L;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["longVar"] = 55555L;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Query on single long variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("longVar", 12345L);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Query on two long variables, should result in single match
		query = runtimeService.createExecutionQuery().variableValueEquals("longVar", 12345L).variableValueEquals("longVar2", 67890L);
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Query with unexisting variable value
		execution = runtimeService.createExecutionQuery().variableValueEquals("longVar", 999L).singleResult();
		Assert.assertNull(execution);

		// Test NOT_EQUALS
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("longVar", 12345L).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("longVar", 44444L).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThan("longVar", 55555L).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThan("longVar",1L).count());

		// Test GREATER_THAN_OR_EQUAL
		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("longVar", 44444L).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("longVar", 55555L).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("longVar",1L).count());

		// Test LESS_THAN
		executions = runtimeService.createExecutionQuery().variableValueLessThan("longVar", 55555L).list();
		Assert.assertEquals(2, executions.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("longVar", 12345L).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThan("longVar",66666L).count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("longVar", 55555L).list();
		Assert.assertEquals(3, executions.Count);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("longVar", 12344L).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryDoubleVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["doubleVar"] = 12345.6789;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["doubleVar"] = 12345.6789;
		vars["doubleVar2"] = 9876.54321;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["doubleVar"] = 55555.5555;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Query on single double variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("doubleVar", 12345.6789);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Query on two double variables, should result in single value
		query = runtimeService.createExecutionQuery().variableValueEquals("doubleVar", 12345.6789).variableValueEquals("doubleVar2", 9876.54321);
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Query with unexisting variable value
		execution = runtimeService.createExecutionQuery().variableValueEquals("doubleVar", 9999.99).singleResult();
		Assert.assertNull(execution);

		// Test NOT_EQUALS
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("doubleVar", 12345.6789).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("doubleVar", 44444.4444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThan("doubleVar", 55555.5555).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThan("doubleVar",1.234).count());

		// Test GREATER_THAN_OR_EQUAL
		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("doubleVar", 44444.4444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("doubleVar", 55555.5555).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("doubleVar",1.234).count());

		// Test LESS_THAN
		executions = runtimeService.createExecutionQuery().variableValueLessThan("doubleVar", 55555.5555).list();
		Assert.assertEquals(2, executions.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("doubleVar", 12345.6789).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThan("doubleVar",66666.6666).count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("doubleVar", 55555.5555).list();
		Assert.assertEquals(3, executions.Count);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("doubleVar", 12344.6789).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryIntegerVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["integerVar"] = 12345;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["integerVar"] = 12345;
		vars["integerVar2"] = 67890;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["integerVar"] = 55555;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Query on single integer variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("integerVar", 12345);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Query on two integer variables, should result in single value
		query = runtimeService.createExecutionQuery().variableValueEquals("integerVar", 12345).variableValueEquals("integerVar2", 67890);
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Query with unexisting variable value
		execution = runtimeService.createExecutionQuery().variableValueEquals("integerVar", 9999).singleResult();
		Assert.assertNull(execution);

		// Test NOT_EQUALS
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("integerVar", 12345).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("integerVar", 44444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThan("integerVar", 55555).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThan("integerVar",1).count());

		// Test GREATER_THAN_OR_EQUAL
		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("integerVar", 44444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("integerVar", 55555).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("integerVar",1).count());

		// Test LESS_THAN
		executions = runtimeService.createExecutionQuery().variableValueLessThan("integerVar", 55555).list();
		Assert.assertEquals(2, executions.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("integerVar", 12345).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThan("integerVar",66666).count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("integerVar", 55555).list();
		Assert.assertEquals(3, executions.Count);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("integerVar", 12344).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryShortVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		short shortVar = 1234;
		vars["shortVar"] = shortVar;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		short shortVar2 = 6789;
		vars = new Dictionary<string, object>();
		vars["shortVar"] = shortVar;
		vars["shortVar2"] = shortVar2;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["shortVar"] = (short)5555;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Query on single short variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("shortVar", shortVar);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Query on two short variables, should result in single value
		query = runtimeService.createExecutionQuery().variableValueEquals("shortVar", shortVar).variableValueEquals("shortVar2", shortVar2);
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Query with unexisting variable value
		short unexistingValue = (short)9999;
		execution = runtimeService.createExecutionQuery().variableValueEquals("shortVar", unexistingValue).singleResult();
		Assert.assertNull(execution);

		// Test NOT_EQUALS
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("shortVar", (short)1234).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("shortVar", (short)4444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThan("shortVar", (short)5555).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThan("shortVar",(short)1).count());

		// Test GREATER_THAN_OR_EQUAL
		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("shortVar", (short)4444).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("shortVar", (short)5555).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("shortVar",(short)1).count());

		// Test LESS_THAN
		executions = runtimeService.createExecutionQuery().variableValueLessThan("shortVar", (short)5555).list();
		Assert.assertEquals(2, executions.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("shortVar", (short)1234).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThan("shortVar",(short)6666).count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("shortVar", (short)5555).list();
		Assert.assertEquals(3, executions.Count);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("shortVar", (short)1233).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryDateVariable() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryDateVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		DateTime date1 = new DateTime();
		vars["dateVar"] = date1;

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		DateTime date2 = new DateTime();
		vars = new Dictionary<string, object>();
		vars["dateVar"] = date1;
		vars["dateVar2"] = date2;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		DateTime nextYear = new DateTime();
		nextYear.AddYears(1);
		vars = new Dictionary<string, object>();
		vars["dateVar"] = nextYear;
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		DateTime nextMonth = new DateTime();
		nextMonth.AddMonths(1);

		DateTime twoYearsLater = new DateTime();
		twoYearsLater.AddYears(2);

		DateTime oneYearAgo = new DateTime();
		oneYearAgo.AddYears(-1);

		// Query on single short variable, should result in 2 matches
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("dateVar", date1);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(2, executions.Count);

		// Query on two short variables, should result in single value
		query = runtimeService.createExecutionQuery().variableValueEquals("dateVar", date1).variableValueEquals("dateVar2", date2);
		Execution execution = query.singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance2.Id, execution.Id);

		// Query with unexisting variable value
		DateTime unexistingDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/01/1989 12:00:00");
		execution = runtimeService.createExecutionQuery().variableValueEquals("dateVar", unexistingDate).singleResult();
		Assert.assertNull(execution);

		// Test NOT_EQUALS
		execution = runtimeService.createExecutionQuery().variableValueNotEquals("dateVar", date1).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		// Test GREATER_THAN
		execution = runtimeService.createExecutionQuery().variableValueGreaterThan("dateVar", nextMonth).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueGreaterThan("dateVar", nextYear).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThan("dateVar", oneYearAgo).count());

		// Test GREATER_THAN_OR_EQUAL
		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("dateVar", nextMonth).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		execution = runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("dateVar", nextYear).singleResult();
		Assert.assertNotNull(execution);
		Assert.assertEquals(processInstance3.Id, execution.Id);

		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("dateVar",oneYearAgo).count());

		// Test LESS_THAN
		executions = runtimeService.createExecutionQuery().variableValueLessThan("dateVar", nextYear).list();
		Assert.assertEquals(2, executions.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(executions[0].Id, executions[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThan("dateVar", date1).count());
		Assert.assertEquals(3, runtimeService.createExecutionQuery().variableValueLessThan("dateVar", twoYearsLater).count());

		// Test LESS_THAN_OR_EQUAL
		executions = runtimeService.createExecutionQuery().variableValueLessThanOrEqual("dateVar", nextYear).list();
		Assert.assertEquals(3, executions.Count);

		Assert.assertEquals(0, runtimeService.createExecutionQuery().variableValueLessThanOrEqual("dateVar", oneYearAgo).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testBooleanVariable() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testBooleanVariable()
	  {

		// TEST EQUALS
		Dictionary<string, object> vars = new Dictionary<string, object>();
		vars["booleanVar"] = true;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["booleanVar"] = false;
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		IList<ProcessInstance> instances = runtimeService.createProcessInstanceQuery().variableValueEquals("booleanVar", true).list();

		assertNotNull(instances);
		assertEquals(1, instances.Count);
		assertEquals(processInstance1.Id, instances[0].Id);

		instances = runtimeService.createProcessInstanceQuery().variableValueEquals("booleanVar", false).list();

		assertNotNull(instances);
		assertEquals(1, instances.Count);
		assertEquals(processInstance2.Id, instances[0].Id);

		// TEST NOT_EQUALS
		instances = runtimeService.createProcessInstanceQuery().variableValueNotEquals("booleanVar", true).list();

		assertNotNull(instances);
		assertEquals(1, instances.Count);
		assertEquals(processInstance2.Id, instances[0].Id);

		instances = runtimeService.createProcessInstanceQuery().variableValueNotEquals("booleanVar", false).list();

		assertNotNull(instances);
		assertEquals(1, instances.Count);
		assertEquals(processInstance1.Id, instances[0].Id);

		// Test unsupported operations
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThan("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'greater than' condition", ae.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'greater than or equal' condition", ae.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThan("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'less than' condition", ae.Message);
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'less than or equal' condition", ae.Message);
		}

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryVariablesUpdatedToNullValue()
	  {
		// Start process instance with different types of variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 928374L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "coca-cola";
		variables["booleanVar"] = true;
		variables["dateVar"] = DateTime.Now;
		variables["nullVar"] = null;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("longVar", null).variableValueEquals("shortVar", null).variableValueEquals("integerVar", null).variableValueEquals("stringVar", null).variableValueEquals("booleanVar", null).variableValueEquals("dateVar", null);

		ExecutionQuery notQuery = runtimeService.createExecutionQuery().variableValueNotEquals("longVar", null).variableValueNotEquals("shortVar", null).variableValueNotEquals("integerVar", null).variableValueNotEquals("stringVar", null).variableValueNotEquals("booleanVar", null).variableValueNotEquals("dateVar", null);

		assertNull(query.singleResult());
		assertNotNull(notQuery.singleResult());

		// Set all existing variables values to null
		runtimeService.setVariable(processInstance.Id, "longVar", null);
		runtimeService.setVariable(processInstance.Id, "shortVar", null);
		runtimeService.setVariable(processInstance.Id, "integerVar", null);
		runtimeService.setVariable(processInstance.Id, "stringVar", null);
		runtimeService.setVariable(processInstance.Id, "booleanVar", null);
		runtimeService.setVariable(processInstance.Id, "dateVar", null);
		runtimeService.setVariable(processInstance.Id, "nullVar", null);

		Execution queryResult = query.singleResult();
		assertNotNull(queryResult);
		assertEquals(processInstance.Id, queryResult.Id);
		assertNull(notQuery.singleResult());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryNullVariable() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryNullVariable()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["nullVar"] = null;
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["nullVar"] = "notnull";
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["nullVarLong"] = "notnull";
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["nullVarDouble"] = "notnull";
		ProcessInstance processInstance4 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		vars = new Dictionary<string, object>();
		vars["nullVarByte"] = "testbytes".GetBytes();
		ProcessInstance processInstance5 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		// Query on null value, should return one value
		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("nullVar", null);
		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(1, executions.Count);
		Assert.assertEquals(processInstance1.Id, executions[0].Id);

		// Test NOT_EQUALS null
		Assert.assertEquals(1, runtimeService.createExecutionQuery().variableValueNotEquals("nullVar", null).count());
		Assert.assertEquals(1, runtimeService.createExecutionQuery().variableValueNotEquals("nullVarLong", null).count());
		Assert.assertEquals(1, runtimeService.createExecutionQuery().variableValueNotEquals("nullVarDouble", null).count());
		// When a byte-array refrence is present, the variable is not considered null
		Assert.assertEquals(1, runtimeService.createExecutionQuery().variableValueNotEquals("nullVarByte", null).count());

		// All other variable queries with null should throw exception
		try
		{
		  runtimeService.createExecutionQuery().variableValueGreaterThan("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'greater than' condition", ae.Message);
		}

		try
		{
		  runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'greater than or equal' condition", ae.Message);
		}

		try
		{
		  runtimeService.createExecutionQuery().variableValueLessThan("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'less than' condition", ae.Message);
		}

		try
		{
		  runtimeService.createExecutionQuery().variableValueLessThanOrEqual("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'less than or equal' condition", ae.Message);
		}

		try
		{
		  runtimeService.createExecutionQuery().variableValueLike("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Booleans and null cannot be used in 'like' condition", ae.Message);
		}

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
		runtimeService.deleteProcessInstance(processInstance4.Id, "test");
		runtimeService.deleteProcessInstance(processInstance5.Id, "test");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryInvalidTypes() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryInvalidTypes()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["bytesVar"] = "test".GetBytes();
		vars["serializableVar"] = new DummySerializable();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		try
		{
		  runtimeService.createExecutionQuery().variableValueEquals("bytesVar", "test".GetBytes()).list();
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Variables of type ByteArray cannot be used to query", ae.Message);
		}

		try
		{
		  runtimeService.createExecutionQuery().variableValueEquals("serializableVar", new DummySerializable()).list();
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Object values cannot be used to query", ae.Message);
		}

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
	  }

	  public virtual void testQueryVariablesNullNameArgument()
	  {
		try
		{
		  runtimeService.createExecutionQuery().variableValueEquals(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueNotEquals(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueGreaterThan(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueGreaterThanOrEqual(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueLessThan(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueLessThanOrEqual(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
		try
		{
		  runtimeService.createExecutionQuery().variableValueLike(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("name is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryAllVariableTypes() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryAllVariableTypes()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["nullVar"] = null;
		vars["stringVar"] = "string";
		vars["longVar"] = 10L;
		vars["doubleVar"] = 1.2;
		vars["integerVar"] = 1234;
		vars["booleanVar"] = true;
		vars["shortVar"] = (short) 123;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		ExecutionQuery query = runtimeService.createExecutionQuery().variableValueEquals("nullVar", null).variableValueEquals("stringVar", "string").variableValueEquals("longVar", 10L).variableValueEquals("doubleVar", 1.2).variableValueEquals("integerVar", 1234).variableValueEquals("booleanVar", true).variableValueEquals("shortVar", (short) 123);

		IList<Execution> executions = query.list();
		Assert.assertNotNull(executions);
		Assert.assertEquals(1, executions.Count);
		Assert.assertEquals(processInstance.Id, executions[0].Id);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testClashingValues() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testClashingValues()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["var"] = 1234L;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		IDictionary<string, object> vars2 = new Dictionary<string, object>();
		vars2["var"] = 1234;

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars2);

		IList<Execution> executions = runtimeService.createExecutionQuery().processDefinitionKey("oneTaskProcess").variableValueEquals("var", 1234L).list();

		assertEquals(1, executions.Count);
		assertEquals(processInstance.Id, executions[0].ProcessInstanceId);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryBySignalSubscriptionName()
	  public virtual void testQueryBySignalSubscriptionName()
	  {
		runtimeService.startProcessInstanceByKey("catchSignal");

		// it finds subscribed instances
		Execution execution = runtimeService.createExecutionQuery().signalEventSubscription("alert").singleResult();
		assertNotNull(execution);

		// test query for nonexisting subscription
		execution = runtimeService.createExecutionQuery().signalEventSubscription("nonExisitng").singleResult();
		assertNull(execution);

		// it finds more than one
		runtimeService.startProcessInstanceByKey("catchSignal");
		assertEquals(2, runtimeService.createExecutionQuery().signalEventSubscription("alert").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryBySignalSubscriptionNameBoundary()
	  public virtual void testQueryBySignalSubscriptionNameBoundary()
	  {
		runtimeService.startProcessInstanceByKey("signalProces");

		// it finds subscribed instances
		Execution execution = runtimeService.createExecutionQuery().signalEventSubscription("Test signal").singleResult();
		assertNotNull(execution);

		// test query for nonexisting subscription
		execution = runtimeService.createExecutionQuery().signalEventSubscription("nonExisitng").singleResult();
		assertNull(execution);

		// it finds more than one
		runtimeService.startProcessInstanceByKey("signalProces");
		assertEquals(2, runtimeService.createExecutionQuery().signalEventSubscription("Test signal").count());
	  }

	  public virtual void testNativeQuery()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		// just test that the query will be constructed and executed, details are tested in the TaskQueryTest
		assertEquals(tablePrefix + "ACT_RU_EXECUTION", managementService.getTableName(typeof(Execution)));

		long executionCount = runtimeService.createExecutionQuery().count();

		assertEquals(executionCount, runtimeService.createNativeExecutionQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Execution))).list().size());
		assertEquals(executionCount, runtimeService.createNativeExecutionQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Execution))).count());
	  }

	  public virtual void testNativeQueryPaging()
	  {
		assertEquals(5, runtimeService.createNativeExecutionQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Execution))).listPage(1, 5).size());
		assertEquals(1, runtimeService.createNativeExecutionQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Execution))).listPage(2, 1).size());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/concurrentExecution.bpmn20.xml"})]
	  public virtual void testExecutionQueryWithProcessVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["x"] = "parent";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("concurrent", variables);

		IList<Execution> concurrentExecutions = runtimeService.createExecutionQuery().processInstanceId(pi.Id).list();
		assertEquals(3, concurrentExecutions.Count);
		foreach (Execution execution in concurrentExecutions)
		{
		  if (!((ExecutionEntity)execution).ProcessInstanceExecution)
		  {
			// only the concurrent executions, not the root one, would be cooler to query that directly, see http://jira.codehaus.org/browse/ACT-1373
			runtimeService.setVariableLocal(execution.Id, "x", "child");
		  }
		}

		assertEquals(2, runtimeService.createExecutionQuery().processInstanceId(pi.Id).variableValueEquals("x", "child").count());
		assertEquals(1, runtimeService.createExecutionQuery().processInstanceId(pi.Id).variableValueEquals("x", "parent").count());

		assertEquals(3, runtimeService.createExecutionQuery().processInstanceId(pi.Id).processVariableValueEquals("x", "parent").count());
		assertEquals(3, runtimeService.createExecutionQuery().processInstanceId(pi.Id).processVariableValueNotEquals("x", "xxx").count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/concurrentExecution.bpmn20.xml"})]
	  public virtual void testExecutionQueryForSuspendedExecutions()
	  {
		IList<Execution> suspendedExecutions = runtimeService.createExecutionQuery().suspended().list();
		assertEquals(suspendedExecutions.Count, 0);

		foreach (string instanceId in concurrentProcessInstanceIds)
		{
		  runtimeService.suspendProcessInstanceById(instanceId);
		}

		suspendedExecutions = runtimeService.createExecutionQuery().suspended().list();
		assertEquals(12, suspendedExecutions.Count);

		IList<Execution> activeExecutions = runtimeService.createExecutionQuery().active().list();
		assertEquals(1, activeExecutions.Count);

		foreach (Execution activeExecution in activeExecutions)
		{
		  assertEquals(activeExecution.ProcessInstanceId, sequentialProcessInstanceIds[0]);
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentId(incident.Id).list();

		assertEquals(1, executionList.Count);
	  }

	  public virtual void testQueryByInvalidIncidentId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertEquals(0, query.incidentId("invalid").count());

		try
		{
		  query.incidentId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentType()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentType(incident.IncidentType).list();

		assertEquals(1, executionList.Count);
	  }

	  public virtual void testQueryByInvalidIncidentType()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertEquals(0, query.incidentType("invalid").count());

		try
		{
		  query.incidentType(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessage()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentMessage(incident.IncidentMessage).list();

		assertEquals(1, executionList.Count);
	  }

	  public virtual void testQueryByInvalidIncidentMessage()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertEquals(0, query.incidentMessage("invalid").count());

		try
		{
		  query.incidentMessage(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageLike()
	  {
		runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentMessageLike("%\\_exception%").list();

		assertEquals(1, executionList.Count);
	  }

	  public virtual void testQueryByInvalidIncidentMessageLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		assertEquals(0, query.incidentMessageLike("invalid").count());

		try
		{
		  query.incidentMessageLike(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentIdSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentId(incident.Id).list();

		assertEquals(1, executionList.Count);
		// execution id of subprocess != process instance id
		assertNotSame(processInstance.Id, executionList[0].Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentTypeInSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentType(incident.IncidentType).list();

		assertEquals(1, executionList.Count);
		// execution id of subprocess != process instance id
		assertNotSame(processInstance.Id, executionList[0].Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentMessage(incident.IncidentMessage).list();

		assertEquals(1, executionList.Count);
		// execution id of subprocess != process instance id
		assertNotSame(processInstance.Id, executionList[0].Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageLikeSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<Execution> executionList = runtimeService.createExecutionQuery().incidentMessageLike("%exception%").list();

		assertEquals(1, executionList.Count);
		// execution id of subprocess != process instance id
		assertNotSame(processInstance.Id, executionList[0].Id);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/oneMessageCatchProcess.bpmn20.xml"})]
	  public virtual void testQueryForExecutionsWithMessageEventSubscriptions()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneMessageCatchProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneMessageCatchProcess");

		IList<Execution> executions = runtimeService.createExecutionQuery().messageEventSubscription().orderByProcessInstanceId().asc().list();

		assertEquals(2, executions.Count);
		if (instance1.Id.CompareTo(instance2.Id) < 0)
		{
		  assertEquals(instance1.Id, executions[0].ProcessInstanceId);
		  assertEquals(instance2.Id, executions[1].ProcessInstanceId);
		}
		else
		{
		  assertEquals(instance2.Id, executions[0].ProcessInstanceId);
		  assertEquals(instance1.Id, executions[1].ProcessInstanceId);
		}

	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/api/runtime/oneMessageCatchProcess.bpmn20.xml")]
	  public virtual void testQueryForExecutionsWithMessageEventSubscriptionsOverlappingFilters()
	  {

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneMessageCatchProcess");

		Execution execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("newInvoiceMessage").messageEventSubscription().singleResult();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);

		runtimeService.createExecutionQuery().messageEventSubscription().messageEventSubscriptionName("newInvoiceMessage").list();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/twoBoundaryEventSubscriptions.bpmn20.xml")]
	  public virtual void testQueryForExecutionsWithMultipleSubscriptions()
	  {
		// given two message event subscriptions
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");

		IList<EventSubscription> subscriptions = runtimeService.createEventSubscriptionQuery().processInstanceId(instance.Id).list();
		assertEquals(2, subscriptions.Count);
		assertEquals(subscriptions[0].ExecutionId, subscriptions[1].ExecutionId);

		// should return the execution once (not twice)
		Execution execution = runtimeService.createExecutionQuery().messageEventSubscription().singleResult();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);

		// should return the execution once
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").singleResult();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);

		// should return the execution once
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_2").singleResult();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);

		// should return the execution once
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").messageEventSubscriptionName("messageName_2").singleResult();

		assertNotNull(execution);
		assertEquals(instance.Id, execution.ProcessInstanceId);

		// should not return the execution
		execution = runtimeService.createExecutionQuery().messageEventSubscriptionName("messageName_1").messageEventSubscriptionName("messageName_2").messageEventSubscriptionName("another").singleResult();

		assertNull(execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueEqualsNumber()
	  {
		// long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123));

		// untyped null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", null));

		// typed null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, runtimeService.createExecutionQuery().processVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, runtimeService.createExecutionQuery().processVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, runtimeService.createExecutionQuery().processVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, runtimeService.createExecutionQuery().processVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, runtimeService.createExecutionQuery().processVariableValueEquals("var", Variables.numberValue(null)).count());

		assertEquals(4, runtimeService.createExecutionQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, runtimeService.createExecutionQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, runtimeService.createExecutionQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, runtimeService.createExecutionQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, runtimeService.createExecutionQuery().variableValueEquals("var", Variables.numberValue(null)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueNumberComparison() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueNumberComparison()
	  {
		// long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123));

		// untyped null
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", null));

		// typed null
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, runtimeService.createExecutionQuery().processVariableValueNotEquals("var", Variables.numberValue(123)).count());
	  }

	  public virtual void testNullBusinessKeyForChildExecutions()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONCURRENT_PROCESS_KEY, "76545");
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).list();
		foreach (Execution e in executions)
		{
		  if (((ExecutionEntity) e).ProcessInstanceExecution)
		  {
			assertEquals("76545", ((ExecutionEntity) e).BusinessKeyWithoutCascade);
		  }
		  else
		  {
			assertNull(((ExecutionEntity) e).BusinessKeyWithoutCascade);
		  }
		}
	  }

	}

}