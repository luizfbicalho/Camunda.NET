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
namespace org.camunda.bpm.engine.test.dmn.businessruletask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Tests the mapping of the decision result.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class DmnBusinessRuleTaskResultMappingTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TEST_DECISION = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.dmn11.xml";
	  protected internal const string CUSTOM_MAPPING_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testCustomOutputMapping.bpmn20.xml";
	  protected internal const string SINGLE_ENTRY_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testSingleEntry.bpmn20.xml";
	  protected internal const string SINGLE_RESULT_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testSingleResult.bpmn20.xml";
	  protected internal const string COLLECT_ENTRIES_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testCollectEntries.bpmn20.xml";
	  protected internal const string RESULT_LIST_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testResultList.bpmn20.xml";
	  protected internal const string DEFAULT_MAPPING_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testDefaultMapping.bpmn20.xml";
	  protected internal const string INVALID_MAPPING_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testInvalidMapping.bpmn20.xml";
	  protected internal const string OVERRIDE_DECISION_RESULT_BPMN = "org/camunda/bpm/engine/test/dmn/result/DmnBusinessRuleTaskResultMappingTest.testOverrideVariable.bpmn20.xml";

	  [Deployment(resources : {CUSTOM_MAPPING_BPMN, TEST_DECISION })]
	  public virtual void testCustomOutputMapping()
	  {
		ProcessInstance processInstance = startTestProcess("multiple entries");

		assertEquals("foo", runtimeService.getVariable(processInstance.Id, "result1"));
		assertEquals(Variables.stringValue("foo"), runtimeService.getVariableTyped(processInstance.Id, "result1"));

		assertEquals("bar", runtimeService.getVariable(processInstance.Id, "result2"));
		assertEquals(Variables.stringValue("bar"), runtimeService.getVariableTyped(processInstance.Id, "result2"));
	  }

	  [Deployment(resources : { SINGLE_ENTRY_BPMN, TEST_DECISION})]
	  public virtual void testSingleEntryMapping()
	  {
		ProcessInstance processInstance = startTestProcess("single entry");

		assertEquals("foo", runtimeService.getVariable(processInstance.Id, "result"));
		assertEquals(Variables.stringValue("foo"), runtimeService.getVariableTyped(processInstance.Id, "result"));
	  }

	  [Deployment(resources : { SINGLE_RESULT_BPMN, TEST_DECISION })]
	  public virtual void testSingleResultMapping()
	  {
		ProcessInstance processInstance = startTestProcess("multiple entries");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, Object> output = (java.util.Map<String, Object>) runtimeService.getVariable(processInstance.getId(), "result");
		IDictionary<string, object> output = (IDictionary<string, object>) runtimeService.getVariable(processInstance.Id, "result");

		assertEquals(2, output.Count);
		assertEquals("foo", output["result1"]);
		assertEquals("bar", output["result2"]);
	  }

	  [Deployment(resources : { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
	  public virtual void testCollectEntriesMapping()
	  {
		ProcessInstance processInstance = startTestProcess("single entry list");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<String> output = (java.util.List<String>) runtimeService.getVariable(processInstance.getId(), "result");
		IList<string> output = (IList<string>) runtimeService.getVariable(processInstance.Id, "result");

		assertEquals(2, output.Count);
		assertEquals("foo", output[0]);
		assertEquals("foo", output[1]);
	  }

	  [Deployment(resources : { RESULT_LIST_BPMN, TEST_DECISION })]
	  public virtual void testResultListMapping()
	  {
		ProcessInstance processInstance = startTestProcess("multiple entries list");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<java.util.Map<String, Object>> resultList = (java.util.List<java.util.Map<String, Object>>) runtimeService.getVariable(processInstance.getId(), "result");
		IList<IDictionary<string, object>> resultList = (IList<IDictionary<string, object>>) runtimeService.getVariable(processInstance.Id, "result");
		assertEquals(2, resultList.Count);

		foreach (IDictionary<string, object> valueMap in resultList)
		{
		  assertEquals(2, valueMap.Count);
		  assertEquals("foo", valueMap["result1"]);
		  assertEquals("bar", valueMap["result2"]);
		}
	  }

	  [Deployment(resources : { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
	  public virtual void testDefaultResultMapping()
	  {
		ProcessInstance processInstance = startTestProcess("multiple entries list");

		// default mapping is 'resultList'
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<java.util.Map<String, Object>> resultList = (java.util.List<java.util.Map<String, Object>>) runtimeService.getVariable(processInstance.getId(), "result");
		IList<IDictionary<string, object>> resultList = (IList<IDictionary<string, object>>) runtimeService.getVariable(processInstance.Id, "result");
		assertEquals(2, resultList.Count);

		foreach (IDictionary<string, object> valueMap in resultList)
		{
		  assertEquals(2, valueMap.Count);
		  assertEquals("foo", valueMap["result1"]);
		  assertEquals("bar", valueMap["result2"]);
		}
	  }

	  [Deployment(resources : { SINGLE_ENTRY_BPMN, TEST_DECISION })]
	  public virtual void testSingleEntryMappingFailureMultipleOutputs()
	  {
		try
		{
		  startTestProcess("single entry list");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { SINGLE_ENTRY_BPMN, TEST_DECISION })]
	  public virtual void testSingleEntryMappingFailureMultipleValues()
	  {
		try
		{
		  startTestProcess("multiple entries");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { SINGLE_RESULT_BPMN, TEST_DECISION })]
	  public virtual void testSingleResultMappingFailure()
	  {
		try
		{
		  startTestProcess("single entry list");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
	  public virtual void testCollectEntriesMappingFailure()
	  {
		try
		{
		  startTestProcess("multiple entries");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22002", e.Message);
		}
	  }

	  public virtual void testInvalidMapping()
	  {
		try
		{
		  deploymentId = repositoryService.createDeployment().addClasspathResource(INVALID_MAPPING_BPMN).deploy().Id;

		  fail("expect parse exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("No decision result mapper found for name 'invalid'", e.Message);
		}
	  }

	  [Deployment(resources : { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
	  public virtual void testTransientDecisionResult()
	  {
		// when a decision is evaluated and the result is stored in a transient variable "decisionResult"
		ProcessInstance processInstance = startTestProcess("single entry");

		// then the variable should not be available outside the business rule task
		assertNull(runtimeService.getVariable(processInstance.Id, "decisionResult"));
		// and should not create an entry in history since it is not persistent
		assertNull(historyService.createHistoricVariableInstanceQuery().variableName("decisionResult").singleResult());
	  }

	  [Deployment(resources : { OVERRIDE_DECISION_RESULT_BPMN, TEST_DECISION })]
	  public virtual void testFailedToOverrideDecisionResultVariable()
	  {
		try
		{
		  // the transient variable "decisionResult" should not be overridden by the task result variable
		  startTestProcess("single entry");
		  fail("expect exception");

		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("transient variable with name decisionResult to non-transient", e.Message);
		}
	  }

	  [Deployment(resources : { SINGLE_ENTRY_BPMN, TEST_DECISION })]
	  public virtual void testSingleEntryEmptyResult()
	  {
		ProcessInstance processInstance = startTestProcess("empty result");

		object result = runtimeService.getVariable(processInstance.Id, "result");
		assertNull(result);
		TypedValue resultTyped = runtimeService.getVariableTyped(processInstance.Id, "result");
		assertEquals(Variables.untypedNullValue(), resultTyped);
	  }

	  [Deployment(resources : { SINGLE_RESULT_BPMN, TEST_DECISION })]
	  public virtual void testSingleResultEmptyResult()
	  {
		ProcessInstance processInstance = startTestProcess("empty result");

		object result = runtimeService.getVariable(processInstance.Id, "result");
		assertNull(result);
		TypedValue resultTyped = runtimeService.getVariableTyped(processInstance.Id, "result");
		assertEquals(Variables.untypedNullValue(), resultTyped);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { COLLECT_ENTRIES_BPMN, TEST_DECISION }) @SuppressWarnings("unchecked") public void testCollectEntriesEmptyResult()
	  [Deployment(resources : { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
	  public virtual void testCollectEntriesEmptyResult()
	  {
		ProcessInstance processInstance = startTestProcess("empty result");

		IList<object> result = (IList<object>) runtimeService.getVariable(processInstance.Id, "result");
		assertTrue(result.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { RESULT_LIST_BPMN, TEST_DECISION }) @SuppressWarnings("unchecked") public void testResultListEmptyResult()
	  [Deployment(resources : { RESULT_LIST_BPMN, TEST_DECISION })]
	  public virtual void testResultListEmptyResult()
	  {
		ProcessInstance processInstance = startTestProcess("empty result");

		IList<object> result = (IList<object>) runtimeService.getVariable(processInstance.Id, "result");
		assertTrue(result.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { DEFAULT_MAPPING_BPMN, TEST_DECISION }) @SuppressWarnings("unchecked") public void testDefaultMappingEmptyResult()
	  [Deployment(resources : { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
	  public virtual void testDefaultMappingEmptyResult()
	  {
		ProcessInstance processInstance = startTestProcess("empty result");

		IList<object> result = (IList<object>) runtimeService.getVariable(processInstance.Id, "result");
		assertTrue(result.Count == 0);
	  }

	  protected internal virtual ProcessInstance startTestProcess(string input)
	  {
		return runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("input", input));
	  }

	}

}