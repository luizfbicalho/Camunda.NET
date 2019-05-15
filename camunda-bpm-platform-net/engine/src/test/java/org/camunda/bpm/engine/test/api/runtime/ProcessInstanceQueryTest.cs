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
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.processInstanceByProcessDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.processInstanceByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.processInstanceByBusinessKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// </summary>
	public class ProcessInstanceQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessInstanceQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  public static readonly BpmnModelInstance FORK_JOIN_SUB_PROCESS_MODEL = ProcessModels.newModel().startEvent().subProcess("subProcess").embeddedSubProcess().startEvent().parallelGateway("fork").userTask("userTask1").name("completeMe").parallelGateway("join").endEvent().moveToNode("fork").userTask("userTask2").connectTo("join").subProcessDone().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  private static string PROCESS_DEFINITION_KEY = "oneTaskProcess";
	  private static string PROCESS_DEFINITION_KEY_2 = "otherOneTaskProcess";

	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal ManagementService managementService;
	  protected internal CaseService caseService;

	  protected internal IList<string> processInstanceIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
		managementService = engineRule.ManagementService;
		caseService = engineRule.CaseService;
	  }


	  /// <summary>
	  /// Setup starts 4 process instances of oneTaskProcess
	  /// and 1 instance of otherOneTaskProcess
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployTestProcesses() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void deployTestProcesses()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = engineRule.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/runtime/otherOneTaskProcess.bpmn20.xml").deploy();

		engineRule.manageDeployment(deployment);

		RuntimeService runtimeService = engineRule.RuntimeService;
		processInstanceIds = new List<string>();
		for (int i = 0; i < 4; i++)
		{
		  processInstanceIds.Add(runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "").Id);
		}
		processInstanceIds.Add(runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_2, "businessKey_123").Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoSpecificsList()
	  public virtual void testQueryNoSpecificsList()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertEquals(5, query.count());
		assertEquals(5, query.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoSpecificsSingleResult()
	  public virtual void testQueryNoSpecificsSingleResult()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKeySingleResult()
	  public virtual void testQueryByProcessDefinitionKeySingleResult()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY_2);
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessDefinitionKey()
	  public virtual void testQueryByInvalidProcessDefinitionKey()
	  {
		assertNull(runtimeService.createProcessInstanceQuery().processDefinitionKey("invalid").singleResult());
		assertEquals(0, runtimeService.createProcessInstanceQuery().processDefinitionKey("invalid").list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKeyMultipleResults()
	  public virtual void testQueryByProcessDefinitionKeyMultipleResults()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY);
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());

		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		foreach (string processInstanceId in processInstanceIds)
		{
		  assertNotNull(runtimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult());
		  assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).list().size());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByBusinessKeyAndProcessDefinitionKey()
	  public virtual void testQueryByBusinessKeyAndProcessDefinitionKey()
	  {
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("0", PROCESS_DEFINITION_KEY).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("1", PROCESS_DEFINITION_KEY).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("2", PROCESS_DEFINITION_KEY).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("3", PROCESS_DEFINITION_KEY).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("businessKey_123", PROCESS_DEFINITION_KEY_2).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByBusinessKey()
	  public virtual void testQueryByBusinessKey()
	  {
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("0").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("1").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("businessKey_123").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByBusinessKeyLike()
	  public virtual void testQueryByBusinessKeyLike()
	  {
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKeyLike("business%").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKeyLike("%sinessKey\\_123").count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().processInstanceBusinessKeyLike("%siness%").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidBusinessKey()
	  public virtual void testQueryByInvalidBusinessKey()
	  {
		assertEquals(0, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("invalid").count());

		try
		{
		  runtimeService.createProcessInstanceQuery().processInstanceBusinessKey(null).count();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessInstanceId()
	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		assertNull(runtimeService.createProcessInstanceQuery().processInstanceId("I do not exist").singleResult());
		assertEquals(0, runtimeService.createProcessInstanceQuery().processInstanceId("I do not exist").list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/superProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"}) public void testQueryBySuperProcessInstanceId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testQueryBySuperProcessInstanceId()
	  {
		ProcessInstance superProcessInstance = runtimeService.startProcessInstanceByKey("subProcessQueryTest");

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().superProcessInstanceId(superProcessInstance.Id);
		ProcessInstance subProcessInstance = query.singleResult();
		assertNotNull(subProcessInstance);
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidSuperProcessInstanceId()
	  public virtual void testQueryByInvalidSuperProcessInstanceId()
	  {
		assertNull(runtimeService.createProcessInstanceQuery().superProcessInstanceId("invalid").singleResult());
		assertEquals(0, runtimeService.createProcessInstanceQuery().superProcessInstanceId("invalid").list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/superProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"}) @Test public void testQueryBySubProcessInstanceId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testQueryBySubProcessInstanceId()
	  {
		ProcessInstance superProcessInstance = runtimeService.startProcessInstanceByKey("subProcessQueryTest");

		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(superProcessInstance.Id).singleResult();
		assertNotNull(subProcessInstance);
		assertEquals(superProcessInstance.Id, runtimeService.createProcessInstanceQuery().subProcessInstanceId(subProcessInstance.Id).singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidSubProcessInstanceId()
	  public virtual void testQueryByInvalidSubProcessInstanceId()
	  {
		assertNull(runtimeService.createProcessInstanceQuery().subProcessInstanceId("invalid").singleResult());
		assertEquals(0, runtimeService.createProcessInstanceQuery().subProcessInstanceId("invalid").list().size());
	  }

	  // Nested subprocess make the query complexer, hence this test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"}) public void testQueryBySuperProcessInstanceIdNested()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testQueryBySuperProcessInstanceIdNested()
	  {
		ProcessInstance superProcessInstance = runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");

		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(superProcessInstance.Id).singleResult();
		assertNotNull(subProcessInstance);

		ProcessInstance nestedSubProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(subProcessInstance.Id).singleResult();
		assertNotNull(nestedSubProcessInstance);
	  }

	  //Nested subprocess make the query complexer, hence this test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"}) public void testQueryBySubProcessInstanceIdNested()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml"})]
	  public virtual void testQueryBySubProcessInstanceIdNested()
	  {
		ProcessInstance superProcessInstance = runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");

		ProcessInstance subProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(superProcessInstance.Id).singleResult();
		assertEquals(superProcessInstance.Id, runtimeService.createProcessInstanceQuery().subProcessInstanceId(subProcessInstance.Id).singleResult().Id);

		ProcessInstance nestedSubProcessInstance = runtimeService.createProcessInstanceQuery().superProcessInstanceId(subProcessInstance.Id).singleResult();
		assertEquals(subProcessInstance.Id, runtimeService.createProcessInstanceQuery().subProcessInstanceId(nestedSubProcessInstance.Id).singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryPaging()
	  public virtual void testQueryPaging()
	  {
		assertEquals(4, runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).count());
		assertEquals(2, runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).listPage(0, 2).size());
		assertEquals(3, runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).listPage(1, 3).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySorting()
	  public virtual void testQuerySorting()
	  {
		IList<ProcessInstance> processInstances = runtimeService.createProcessInstanceQuery().orderByProcessInstanceId().asc().list();
		assertEquals(5, processInstances.Count);
		verifySorting(processInstances, processInstanceByProcessInstanceId());

		processInstances = runtimeService.createProcessInstanceQuery().orderByProcessDefinitionId().asc().list();
		assertEquals(5, processInstances.Count);
		verifySorting(processInstances, processInstanceByProcessDefinitionId());

		processInstances = runtimeService.createProcessInstanceQuery().orderByBusinessKey().asc().list();
		assertEquals(5, processInstances.Count);
		verifySorting(processInstances, processInstanceByBusinessKey());

		assertEquals(5, runtimeService.createProcessInstanceQuery().orderByProcessDefinitionKey().asc().list().size());

		assertEquals(5, runtimeService.createProcessInstanceQuery().orderByProcessInstanceId().desc().list().size());
		assertEquals(5, runtimeService.createProcessInstanceQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(5, runtimeService.createProcessInstanceQuery().orderByProcessDefinitionKey().desc().list().size());
		assertEquals(5, runtimeService.createProcessInstanceQuery().orderByBusinessKey().desc().list().size());

		assertEquals(4, runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).orderByProcessInstanceId().asc().list().size());
		assertEquals(4, runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).orderByProcessInstanceId().desc().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryInvalidSorting()
	  public virtual void testQueryInvalidSorting()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().orderByProcessDefinitionId().list(); // asc - desc not called -> exception
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryStringVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("stringVar", "abcdef");
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Test EQUAL on two string variables, should result in single match
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("stringVar", "abcdef").variableValueEquals("stringVar2", "ghijkl");
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Test NOT_EQUAL, should return only 1 resultInstance
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("stringVar", "abcdef").singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN, should return only matching 'azerty'
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("stringVar", "abcdef").singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("stringVar", "z").singleResult();
		assertNull(resultInstance);

		// Test GREATER_THAN_OR_EQUAL, should return 3 results
		assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("stringVar", "abcdef").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN, should return 2 results
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("stringVar", "abcdeg").list();
		Assert.assertEquals(2, processInstances.Count);
		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("stringVar", "abcdef").count());
		assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "z").count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "abcdef").list();
		Assert.assertEquals(2, processInstances.Count);
		expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "z").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("stringVar", "aa").count());

		// Test LIKE
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueLike("stringVar", "azert%").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueLike("stringVar", "%y").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueLike("stringVar", "%zer%").singleResult();
		assertNotNull(resultInstance);
		assertEquals(processInstance3.Id, resultInstance.Id);

		assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLike("stringVar", "a%").count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLike("stringVar", "%x%").count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryLongVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("longVar", 12345L);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two long variables, should result in single match
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("longVar", 12345L).variableValueEquals("longVar2", 67890L);
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueEquals("longVar", 999L).singleResult();
		assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("longVar", 12345L).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("longVar", 44444L).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("longVar", 55555L).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("longVar",1L).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("longVar", 44444L).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("longVar", 55555L).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("longVar",1L).count());

		// Test LESS_THAN
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("longVar", 55555L).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("longVar", 12345L).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThan("longVar",66666L).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("longVar", 55555L).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("longVar", 12344L).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryDoubleVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("doubleVar", 12345.6789);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two double variables, should result in single value
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("doubleVar", 12345.6789).variableValueEquals("doubleVar2", 9876.54321);
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueEquals("doubleVar", 9999.99).singleResult();
		assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("doubleVar", 12345.6789).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("doubleVar", 44444.4444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("doubleVar", 55555.5555).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("doubleVar",1.234).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("doubleVar", 44444.4444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("doubleVar", 55555.5555).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("doubleVar",1.234).count());

		// Test LESS_THAN
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("doubleVar", 55555.5555).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("doubleVar", 12345.6789).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThan("doubleVar",66666.6666).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("doubleVar", 55555.5555).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("doubleVar", 12344.6789).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryIntegerVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("integerVar", 12345);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two integer variables, should result in single value
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("integerVar", 12345).variableValueEquals("integerVar2", 67890);
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueEquals("integerVar", 9999).singleResult();
		assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("integerVar", 12345).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("integerVar", 44444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("integerVar", 55555).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("integerVar",1).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("integerVar", 44444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("integerVar", 55555).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("integerVar",1).count());

		// Test LESS_THAN
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("integerVar", 55555).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("integerVar", 12345).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThan("integerVar",66666).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("integerVar", 55555).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("integerVar", 12344).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryShortVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("shortVar", shortVar);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two short variables, should result in single value
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("shortVar", shortVar).variableValueEquals("shortVar2", shortVar2);
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		short unexistingValue = (short)9999;
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueEquals("shortVar", unexistingValue).singleResult();
		assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("shortVar", (short)1234).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("shortVar", (short)4444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("shortVar", (short)5555).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("shortVar",(short)1).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("shortVar", (short)4444).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("shortVar", (short)5555).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("shortVar",(short)1).count());

		// Test LESS_THAN
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("shortVar", (short)5555).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("shortVar", (short)1234).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThan("shortVar",(short)6666).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("shortVar", (short)5555).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("shortVar", (short)1233).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryDateVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("dateVar", date1);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(2, processInstances.Count);

		// Query on two short variables, should result in single value
		query = runtimeService.createProcessInstanceQuery().variableValueEquals("dateVar", date1).variableValueEquals("dateVar2", date2);
		ProcessInstance resultInstance = query.singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance2.Id, resultInstance.Id);

		// Query with unexisting variable value
		DateTime unexistingDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/01/1989 12:00:00");
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueEquals("dateVar", unexistingDate).singleResult();
		assertNull(resultInstance);

		// Test NOT_EQUALS
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueNotEquals("dateVar", date1).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		// Test GREATER_THAN
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThan("dateVar", nextMonth).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("dateVar", nextYear).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("dateVar", oneYearAgo).count());

		// Test GREATER_THAN_OR_EQUAL
		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar", nextMonth).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		resultInstance = runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar", nextYear).singleResult();
		assertNotNull(resultInstance);
		Assert.assertEquals(processInstance3.Id, resultInstance.Id);

		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("dateVar",oneYearAgo).count());

		// Test LESS_THAN
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThan("dateVar", nextYear).list();
		Assert.assertEquals(2, processInstances.Count);

		IList<string> expecedIds = Arrays.asList(processInstance1.Id, processInstance2.Id);
		IList<string> ids = new List<string>(Arrays.asList(processInstances[0].Id, processInstances[1].Id));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		ids.removeAll(expecedIds);
		assertTrue(ids.Count == 0);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("dateVar", date1).count());
		Assert.assertEquals(3, runtimeService.createProcessInstanceQuery().variableValueLessThan("dateVar", twoYearsLater).count());

		// Test LESS_THAN_OR_EQUAL
		processInstances = runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("dateVar", nextYear).list();
		Assert.assertEquals(3, processInstances.Count);

		Assert.assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("dateVar", oneYearAgo).count());

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testBooleanVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'greater than' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'greater than or equal' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThan("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'less than' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("booleanVar", true);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'less than or equal' condition"));
		}

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryVariablesUpdatedToNullValue()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryVariablesUpdatedToNullValue()
	  {
		// Start process instance with different types of variables
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 928374L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "coca-cola";
		variables["dateVar"] = DateTime.Now;
		variables["booleanVar"] = true;
		variables["nullVar"] = null;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("longVar", null).variableValueEquals("shortVar", null).variableValueEquals("integerVar", null).variableValueEquals("stringVar", null).variableValueEquals("booleanVar", null).variableValueEquals("dateVar", null);

		ProcessInstanceQuery notQuery = runtimeService.createProcessInstanceQuery().variableValueNotEquals("longVar", null).variableValueNotEquals("shortVar", null).variableValueNotEquals("integerVar", null).variableValueNotEquals("stringVar", null).variableValueNotEquals("booleanVar", null).variableValueNotEquals("dateVar", null);

		assertNull(query.singleResult());
		assertNotNull(notQuery.singleResult());

		// Set all existing variables values to null
		runtimeService.setVariable(processInstance.Id, "longVar", null);
		runtimeService.setVariable(processInstance.Id, "shortVar", null);
		runtimeService.setVariable(processInstance.Id, "integerVar", null);
		runtimeService.setVariable(processInstance.Id, "stringVar", null);
		runtimeService.setVariable(processInstance.Id, "dateVar", null);
		runtimeService.setVariable(processInstance.Id, "nullVar", null);
		runtimeService.setVariable(processInstance.Id, "booleanVar", null);

		Execution queryResult = query.singleResult();
		assertNotNull(queryResult);
		assertEquals(processInstance.Id, queryResult.Id);
		assertNull(notQuery.singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryNullVariable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("nullVar", null);
		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(1, processInstances.Count);
		Assert.assertEquals(processInstance1.Id, processInstances[0].Id);

		// Test NOT_EQUALS null
		Assert.assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueNotEquals("nullVar", null).count());
		Assert.assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueNotEquals("nullVarLong", null).count());
		Assert.assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueNotEquals("nullVarDouble", null).count());
		// When a byte-array refrence is present, the variable is not considered null
		Assert.assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueNotEquals("nullVarByte", null).count());

		// All other variable queries with null should throw exception
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThan("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'greater than' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'greater than or equal' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThan("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'less than' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'less than or equal' condition"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLike("nullVar", null);
		  fail("Excetion expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Booleans and null cannot be used in 'like' condition"));
		}

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");
		runtimeService.deleteProcessInstance(processInstance3.Id, "test");
		runtimeService.deleteProcessInstance(processInstance4.Id, "test");
		runtimeService.deleteProcessInstance(processInstance5.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryInvalidTypes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryInvalidTypes()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["bytesVar"] = "test".GetBytes();
		vars["serializableVar"] = new DummySerializable();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueEquals("bytesVar", "test".GetBytes()).list();
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Variables of type ByteArray cannot be used to query"));
		}

		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueEquals("serializableVar", new DummySerializable()).list();
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("Object values cannot be used to query"));
		}

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryVariablesNullNameArgument()
	  public virtual void testQueryVariablesNullNameArgument()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueEquals(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueNotEquals(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThan(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThan(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLike(null, "value");
		  fail("Expected exception");
		}
		catch (ProcessEngineException ae)
		{
		  assertThat(ae.Message, containsString("name is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryAllVariableTypes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
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

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().variableValueEquals("nullVar", null).variableValueEquals("stringVar", "string").variableValueEquals("longVar", 10L).variableValueEquals("doubleVar", 1.2).variableValueEquals("integerVar", 1234).variableValueEquals("booleanVar", true).variableValueEquals("shortVar", (short) 123);

		IList<ProcessInstance> processInstances = query.list();
		assertNotNull(processInstances);
		Assert.assertEquals(1, processInstances.Count);
		Assert.assertEquals(processInstance.Id, processInstances[0].Id);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testClashingValues() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testClashingValues()
	  {
		  IDictionary<string, object> vars = new Dictionary<string, object>();
		  vars["var"] = 1234L;

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);

		  IDictionary<string, object> vars2 = new Dictionary<string, object>();
		  vars2["var"] = 1234;

		  ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", vars2);

		  IList<ProcessInstance> foundInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").variableValueEquals("var", 1234L).list();

		  assertEquals(1, foundInstances.Count);
		  assertEquals(processInstance.Id, foundInstances[0].Id);

		  runtimeService.deleteProcessInstance(processInstance.Id, "test");
		  runtimeService.deleteProcessInstance(processInstance2.Id, "test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIds()
	  public virtual void testQueryByProcessInstanceIds()
	  {
		ISet<string> processInstanceIds = new HashSet<string>(this.processInstanceIds);

		// start an instance that will not be part of the query
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_2, "2");

		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceIds(processInstanceIds);
		assertEquals(5, processInstanceQuery.count());

		IList<ProcessInstance> processInstances = processInstanceQuery.list();
		assertNotNull(processInstances);
		assertEquals(5, processInstances.Count);

		foreach (ProcessInstance processInstance in processInstances)
		{
		  assertTrue(processInstanceIds.Contains(processInstance.Id));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIdsEmpty()
	  public virtual void testQueryByProcessInstanceIdsEmpty()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().processInstanceIds(new HashSet<string>());
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertThat(re.Message, containsString("Set of process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIdsNull()
	  public virtual void testQueryByProcessInstanceIdsNull()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().processInstanceIds(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertThat(re.Message, containsString("Set of process instance ids is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActive() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryByActive()
	  {
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery();

		assertEquals(5, processInstanceQuery.active().count());

		repositoryService.suspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY);

		assertEquals(5, processInstanceQuery.active().count());

		repositoryService.suspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY, true, null);

		assertEquals(1, processInstanceQuery.active().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBySuspended() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryBySuspended()
	  {
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery();

		assertEquals(0, processInstanceQuery.suspended().count());

		repositoryService.suspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY);

		assertEquals(0, processInstanceQuery.suspended().count());

		repositoryService.suspendProcessDefinitionByKey(PROCESS_DEFINITION_KEY, true, null);

		assertEquals(4, processInstanceQuery.suspended().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNativeQuery()
	  public virtual void testNativeQuery()
	  {
		string tablePrefix = engineRule.ProcessEngineConfiguration.DatabaseTablePrefix;
		// just test that the query will be constructed and executed, details are tested in the TaskQueryTest
		assertEquals(tablePrefix + "ACT_RU_EXECUTION", managementService.getTableName(typeof(ProcessInstance)));

		long piCount = runtimeService.createProcessInstanceQuery().count();

		assertEquals(piCount, runtimeService.createNativeProcessInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(ProcessInstance))).list().size());
		assertEquals(piCount, runtimeService.createNativeProcessInstanceQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(ProcessInstance))).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNativeQueryPaging()
	  public virtual void testNativeQueryPaging()
	  {
		assertEquals(5, runtimeService.createNativeProcessInstanceQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(ProcessInstance))).listPage(0, 5).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testQueryWithIncident()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryWithIncident()
	  {
		ProcessInstance instanceWithIncident = runtimeService.startProcessInstanceByKey("failingProcess");
		ProcessInstance instanceWithoutIncident = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		ProcessInstance instance = runtimeService.createProcessInstanceQuery().withIncident().singleResult();
		assertThat(instance.Id, @is(instanceWithIncident.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentId()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentId(incident.Id).list();

		assertEquals(1, processInstanceList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentId()
	  public virtual void testQueryByInvalidIncidentId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentType()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentType()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentType(incident.IncidentType).list();

		assertEquals(1, processInstanceList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentType()
	  public virtual void testQueryByInvalidIncidentType()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessage()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessage()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentMessage(incident.IncidentMessage).list();

		assertEquals(1, processInstanceList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentMessage()
	  public virtual void testQueryByInvalidIncidentMessage()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessageLike()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageLike()
	  {
		runtimeService.startProcessInstanceByKey("failingProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentMessageLike("%\\_exception%").list();

		assertEquals(1, processInstanceList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidIncidentMessageLike()
	  public virtual void testQueryByInvalidIncidentMessageLike()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentIdInSubProcess()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentIdInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentId(incident.Id).list();

		assertEquals(1, processInstanceList.Count);
		assertEquals(processInstance.Id, processInstanceList[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentTypeInSubProcess()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentTypeInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentType(incident.IncidentType).list();

		assertEquals(1, processInstanceList.Count);
		assertEquals(processInstance.Id, processInstanceList[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessageInSubProcess()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentMessage(incident.IncidentMessage).list();

		assertEquals(1, processInstanceList.Count);
		assertEquals(processInstance.Id, processInstanceList[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessageLikeInSubProcess()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/runtime/failingSubProcessCreateOneIncident.bpmn20.xml"})]
	  public virtual void testQueryByIncidentMessageLikeInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingSubProcess");

		testHelper.executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		IList<ProcessInstance> processInstanceList = runtimeService.createProcessInstanceQuery().incidentMessageLike("%exception%").list();

		assertEquals(1, processInstanceList.Count);
		assertEquals(processInstance.Id, processInstanceList[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testQueryByCaseInstanceId()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceId()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneProcessTaskCase").create().Id;

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(1, query.count());

		IList<ProcessInstance> result = query.list();
		assertEquals(1, result.Count);

		ProcessInstance processInstance = result[0];
		assertEquals(caseInstanceId, processInstance.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidCaseInstanceId()
	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		query.caseInstanceId("invalid");

		assertEquals(0, query.count());

		try
		{
		  query.caseInstanceId(null);
		  fail("The passed case instance should not be null.");
		}
		catch (Exception)
		{
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/superCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/superProcessWithCallActivityInsideSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" }) public void testQueryByCaseInstanceIdHierarchy()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/superProcessWithCallActivityInsideSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceIdHierarchy()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneProcessTaskCase").businessKey("aBusinessKey").create().Id;

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		query.caseInstanceId(caseInstanceId);

		assertEquals(2, query.count());

		IList<ProcessInstance> result = query.list();
		assertEquals(2, result.Count);

		ProcessInstance firstProcessInstance = result[0];
		assertEquals(caseInstanceId, firstProcessInstance.CaseInstanceId);

		ProcessInstance secondProcessInstance = result[1];
		assertEquals(caseInstanceId, secondProcessInstance.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueEqualsNumber() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
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
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap("var", null));

		// typed null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.numberValue(null)).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueNumberComparison() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
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
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap("var", null));

		// typed null
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueNotEquals("var", Variables.numberValue(123)).count());
		assertEquals(1, runtimeService.createProcessInstanceQuery().variableValueGreaterThan("var", Variables.numberValue(123)).count());
		assertEquals(5, runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("var", Variables.numberValue(123)).count());
		assertEquals(0, runtimeService.createProcessInstanceQuery().variableValueLessThan("var", Variables.numberValue(123)).count());
		assertEquals(4, runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("var", Variables.numberValue(123)).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn"}) public void testQueryBySuperCaseInstanceId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn"})]
	  public virtual void testQueryBySuperCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneProcessTaskCase").Id;

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().superCaseInstanceId(superCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		ProcessInstance subProcessInstance = query.singleResult();
		assertNotNull(subProcessInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidSuperCaseInstanceId()
	  public virtual void testQueryByInvalidSuperCaseInstanceId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		assertNull(query.superProcessInstanceId("invalid").singleResult());
		assertEquals(0, query.superProcessInstanceId("invalid").list().size());

		try
		{
		  query.superCaseInstanceId(null);
		  fail();
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testQueryBySubCaseInstanceId()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySubCaseInstanceId()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		string subCaseInstanceId = caseService.createCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId).singleResult().Id;

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().subCaseInstanceId(subCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		ProcessInstance superProcessInstance = query.singleResult();
		assertNotNull(superProcessInstance);
		assertEquals(superProcessInstanceId, superProcessInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivityInsideSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testQueryBySubCaseInstanceIdNested()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivityInsideSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySubCaseInstanceIdNested()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		string subCaseInstanceId = caseService.createCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId).singleResult().Id;

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().subCaseInstanceId(subCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		ProcessInstance superProcessInstance = query.singleResult();
		assertNotNull(superProcessInstance);
		assertEquals(superProcessInstanceId, superProcessInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidSubCaseInstanceId()
	  public virtual void testQueryByInvalidSubCaseInstanceId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		assertNull(query.subProcessInstanceId("invalid").singleResult());
		assertEquals(0, query.subProcessInstanceId("invalid").list().size());

		try
		{
		  query.subCaseInstanceId(null);
		  fail();
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testQueryNullValue()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryNullValue()
	  {
		// typed null
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("var", Variables.stringValue(null)));

		// untyped null
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("var", null));

		// non-null String value
		ProcessInstance processInstance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "a String Value"));

		ProcessInstance processInstance4 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "another String Value"));

		// (1) query for untyped null: should return typed and untyped null (notEquals: the opposite)
		IList<ProcessInstance> instances = runtimeService.createProcessInstanceQuery().variableValueEquals("var", null).list();
		verifyResultContainsExactly(instances, asSet(processInstance1.Id, processInstance2.Id));
		instances = runtimeService.createProcessInstanceQuery().variableValueNotEquals("var", null).list();
		verifyResultContainsExactly(instances, asSet(processInstance3.Id, processInstance4.Id));

		// (2) query for typed null: should return typed null only (notEquals: the opposite)
		instances = runtimeService.createProcessInstanceQuery().variableValueEquals("var", Variables.stringValue(null)).list();
		verifyResultContainsExactly(instances, asSet(processInstance1.Id));
		instances = runtimeService.createProcessInstanceQuery().variableValueNotEquals("var", Variables.stringValue(null)).list();
		verifyResultContainsExactly(instances, asSet(processInstance2.Id, processInstance3.Id, processInstance4.Id));

		// (3) query for typed value: should return typed value only (notEquals: the opposite)
		instances = runtimeService.createProcessInstanceQuery().variableValueEquals("var", "a String Value").list();
		verifyResultContainsExactly(instances, asSet(processInstance3.Id));
		instances = runtimeService.createProcessInstanceQuery().variableValueNotEquals("var", "a String Value").list();
		verifyResultContainsExactly(instances, asSet(processInstance1.Id, processInstance2.Id, processInstance4.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDeploymentId()
	  public virtual void testQueryByDeploymentId()
	  {
		// given
		string firstDeploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		// make a second deployment and start an instance
		org.camunda.bpm.engine.repository.Deployment secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml").deploy();

		ProcessInstance secondProcessInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().deploymentId(firstDeploymentId);

		// then the instance belonging to the second deployment is not returned
		assertEquals(5, query.count());

		IList<ProcessInstance> instances = query.list();
		assertEquals(5, instances.Count);

		foreach (ProcessInstance returnedInstance in instances)
		{
		  assertTrue(!returnedInstance.Id.Equals(secondProcessInstance.Id));
		}

		// cleanup
		repositoryService.deleteDeployment(secondDeployment.Id, true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidDeploymentId()
	  public virtual void testQueryByInvalidDeploymentId()
	  {
		assertEquals(0, runtimeService.createProcessInstanceQuery().deploymentId("invalid").count());

		try
		{
		  runtimeService.createProcessInstanceQuery().deploymentId(null).count();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullActivityId()
	  public virtual void testQueryByNullActivityId()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().activityIdIn((string) null);
		  fail("exception expected");
		}
		catch (NullValueException e)
		{
			assertThat(e.Message, containsString("activity ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNullActivityIds()
	  public virtual void testQueryByNullActivityIds()
	  {
		try
		{
		  runtimeService.createProcessInstanceQuery().activityIdIn((string[]) null);
		  fail("exception expected");
		}
		catch (NullValueException e)
		{
		  assertThat(e.Message, containsString("activity ids is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnknownActivityId()
	  public virtual void testQueryByUnknownActivityId()
	  {
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("unknown");

		assertNoProcessInstancesReturned(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByLeafActivityId()
	  public virtual void testQueryByLeafActivityId()
	  {
		// given
		ProcessDefinition oneTaskDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition gatewaySubProcessDefinition = testHelper.deployAndGetDefinition(FORK_JOIN_SUB_PROCESS_MODEL);

		// when
		ProcessInstance oneTaskInstance1 = runtimeService.startProcessInstanceById(oneTaskDefinition.Id);
		ProcessInstance oneTaskInstance2 = runtimeService.startProcessInstanceById(oneTaskDefinition.Id);
		ProcessInstance gatewaySubProcessInstance1 = runtimeService.startProcessInstanceById(gatewaySubProcessDefinition.Id);
		ProcessInstance gatewaySubProcessInstance2 = runtimeService.startProcessInstanceById(gatewaySubProcessDefinition.Id);

		Task task = engineRule.TaskService.createTaskQuery().processInstanceId(gatewaySubProcessInstance2.Id).taskName("completeMe").singleResult();
		engineRule.TaskService.complete(task.Id);

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("userTask");
		assertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("userTask1", "userTask2");
		assertReturnedProcessInstances(query, gatewaySubProcessInstance1, gatewaySubProcessInstance2);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("userTask", "userTask1");
		assertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2, gatewaySubProcessInstance1);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("userTask", "userTask1", "userTask2");
		assertReturnedProcessInstances(query, oneTaskInstance1, oneTaskInstance2, gatewaySubProcessInstance1, gatewaySubProcessInstance2);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("join");
		assertReturnedProcessInstances(query, gatewaySubProcessInstance2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonLeafActivityId()
	  public virtual void testQueryByNonLeafActivityId()
	  {
		// given
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(FORK_JOIN_SUB_PROCESS_MODEL);

		// when
		runtimeService.startProcessInstanceById(processDefinition.Id);

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("subProcess", "fork");
		assertNoProcessInstancesReturned(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByAsyncBeforeActivityId()
	  public virtual void testQueryByAsyncBeforeActivityId()
	  {
		// given
		ProcessDefinition testProcess = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent("start").camundaAsyncBefore().subProcess("subProcess").camundaAsyncBefore().embeddedSubProcess().startEvent().serviceTask("task").camundaAsyncBefore().camundaExpression("${true}").endEvent().subProcessDone().endEvent("end").camundaAsyncBefore().done());

		// when
		ProcessInstance instanceBeforeStart = runtimeService.startProcessInstanceById(testProcess.Id);
		ProcessInstance instanceBeforeSubProcess = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceBeforeSubProcess);
		ProcessInstance instanceBeforeTask = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceBeforeTask);
		executeJobForProcessInstance(instanceBeforeTask);
		ProcessInstance instanceBeforeEnd = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceBeforeEnd);
		executeJobForProcessInstance(instanceBeforeEnd);
		executeJobForProcessInstance(instanceBeforeEnd);

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("start");
		assertReturnedProcessInstances(query, instanceBeforeStart);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("subProcess");
		assertReturnedProcessInstances(query, instanceBeforeSubProcess);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("task");
		assertReturnedProcessInstances(query, instanceBeforeTask);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("end");
		assertReturnedProcessInstances(query, instanceBeforeEnd);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByAsyncAfterActivityId()
	  public virtual void testQueryByAsyncAfterActivityId()
	  {
		// given
		ProcessDefinition testProcess = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent("start").camundaAsyncAfter().subProcess("subProcess").camundaAsyncAfter().embeddedSubProcess().startEvent().serviceTask("task").camundaAsyncAfter().camundaExpression("${true}").endEvent().subProcessDone().endEvent("end").camundaAsyncAfter().done());

		// when
		ProcessInstance instanceAfterStart = runtimeService.startProcessInstanceById(testProcess.Id);
		ProcessInstance instanceAfterTask = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceAfterTask);
		ProcessInstance instanceAfterSubProcess = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceAfterSubProcess);
		executeJobForProcessInstance(instanceAfterSubProcess);
		ProcessInstance instanceAfterEnd = runtimeService.startProcessInstanceById(testProcess.Id);
		executeJobForProcessInstance(instanceAfterEnd);
		executeJobForProcessInstance(instanceAfterEnd);
		executeJobForProcessInstance(instanceAfterEnd);

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("start");
		assertReturnedProcessInstances(query, instanceAfterStart);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("task");
		assertReturnedProcessInstances(query, instanceAfterTask);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("subProcess");
		assertReturnedProcessInstances(query, instanceAfterSubProcess);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("end");
		assertReturnedProcessInstances(query, instanceAfterEnd);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityIdBeforeCompensation()
	  public virtual void testQueryByActivityIdBeforeCompensation()
	  {
		// given
		ProcessDefinition testProcess = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		// when
		runtimeService.startProcessInstanceById(testProcess.Id);
		testHelper.completeTask("userTask1");

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("subProcess");
		assertNoProcessInstancesReturned(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityIdDuringCompensation()
	  public virtual void testQueryByActivityIdDuringCompensation()
	  {
		// given
		ProcessDefinition testProcess = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(testProcess.Id);
		testHelper.completeTask("userTask1");
		testHelper.completeTask("userTask2");

		// then
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().activityIdIn("subProcess");
		assertReturnedProcessInstances(query, processInstance);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("compensationEvent");
		assertReturnedProcessInstances(query, processInstance);

		query = runtimeService.createProcessInstanceQuery().activityIdIn("compensationHandler");
		assertReturnedProcessInstances(query, processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootProcessInstances()
	  public virtual void testQueryByRootProcessInstances()
	  {
		// given
		string superProcess = "calling";
		string subProcess = "called";
		BpmnModelInstance callingInstance = ProcessModels.newModel(superProcess).startEvent().callActivity().calledElement(subProcess).endEvent().done();

		BpmnModelInstance calledInstance = ProcessModels.newModel(subProcess).startEvent().userTask().endEvent().done();

		testHelper.deploy(callingInstance, calledInstance);
		string businessKey = "theOne";
		string processInstanceId = runtimeService.startProcessInstanceByKey(superProcess, businessKey).ProcessInstanceId;

		// when
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processInstanceBusinessKey(businessKey).rootProcessInstances();

		// then
		assertEquals(1, query.count());
		IList<ProcessInstance> list = query.list();
		assertEquals(1, list.Count);
		assertEquals(processInstanceId, list[0].Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootProcessInstancesAndSuperProcess()
	  public virtual void testQueryByRootProcessInstancesAndSuperProcess()
	  {
		// when
		try
		{
		  runtimeService.createProcessInstanceQuery().rootProcessInstances().superProcessInstanceId("processInstanceId");

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId"));
		}

		// when
		try
		{
		  runtimeService.createProcessInstanceQuery().superProcessInstanceId("processInstanceId").rootProcessInstances();

		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  assertTrue(e.Message.contains("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId"));
		}
	  }

	  protected internal virtual void executeJobForProcessInstance(ProcessInstance processInstance)
	  {
		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		managementService.executeJob(job.Id);
	  }

	  protected internal virtual ISet<T> asSet<T>(params T[] elements)
	  {
		return new HashSet<T>(Arrays.asList(elements));
	  }

	  protected internal virtual void assertNoProcessInstancesReturned(ProcessInstanceQuery query)
	  {
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  protected internal virtual void assertReturnedProcessInstances(ProcessInstanceQuery query, params ProcessInstance[] processInstances)
	  {
		int expectedSize = processInstances.Length;
		assertEquals(expectedSize, query.count());
		assertEquals(expectedSize, query.list().size());

		verifyResultContainsExactly(query.list(), collectProcessInstanceIds(Arrays.asList(processInstances)));
	  }

	  protected internal virtual void verifyResultContainsExactly(IList<ProcessInstance> instances, ISet<string> processInstanceIds)
	  {
		ISet<string> retrievedInstanceIds = collectProcessInstanceIds(instances);
		assertEquals(processInstanceIds, retrievedInstanceIds);
	  }

	  protected internal virtual ISet<string> collectProcessInstanceIds(IList<ProcessInstance> instances)
	  {
		ISet<string> retrievedInstanceIds = new HashSet<string>();
		foreach (ProcessInstance instance in instances)
		{
		  retrievedInstanceIds.Add(instance.Id);
		}
		return retrievedInstanceIds;
	  }

	}

}