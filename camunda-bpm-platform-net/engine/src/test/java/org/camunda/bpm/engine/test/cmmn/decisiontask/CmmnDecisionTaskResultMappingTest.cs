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
namespace org.camunda.bpm.engine.test.cmmn.decisiontask
{

	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnDecisionTaskResultMappingTest : CmmnProcessEngineTestCase
	{

	  protected internal const string TEST_DECISION = "org/camunda/bpm/engine/test/dmn/result/DmnDecisionResultTest.dmn11.xml";
	  protected internal const string SINGLE_ENTRY_MAPPING_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testSingleEntryMapping.cmmn";
	  protected internal const string SINGLE_RESULT_MAPPING_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testSingleResultMapping.cmmn";
	  protected internal const string COLLECT_ENTRIES_MAPPING_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testCollectEntriesMapping.cmmn";
	  protected internal const string RESULT_LIST_MAPPING_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testResultListMapping.cmmn";
	  protected internal const string DEFAULT_MAPPING_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testDefaultResultMapping.cmmn";
	  protected internal const string OVERRIDE_DECISION_RESULT_CMMN = "org/camunda/bpm/engine/test/cmmn/decisiontask/DmnDecisionTableResultMappingTest.testFailedToOverrideDecisionResultVariable.cmmn";

	  [Deployment(resources : { SINGLE_ENTRY_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testSingleEntryMapping()
	  {
		CaseInstance caseInstance = createTestCase("single entry");

		assertEquals("foo", caseService.getVariable(caseInstance.Id, "result"));
		assertEquals(Variables.stringValue("foo"), caseService.getVariableTyped(caseInstance.Id, "result"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deployment(resources = { SINGLE_RESULT_MAPPING_CMMN, TEST_DECISION }) public void testSingleResultMapping()
	  [Deployment(resources : { SINGLE_RESULT_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testSingleResultMapping()
	  {
		CaseInstance caseInstance = createTestCase("multiple entries");

		IDictionary<string, object> output = (IDictionary<string, object>) caseService.getVariable(caseInstance.Id, "result");

		assertEquals(2, output.Count);
		assertEquals("foo", output["result1"]);
		assertEquals("bar", output["result2"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deployment(resources = { COLLECT_ENTRIES_MAPPING_CMMN, TEST_DECISION }) public void testCollectEntriesMapping()
	  [Deployment(resources : { COLLECT_ENTRIES_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testCollectEntriesMapping()
	  {
		CaseInstance caseInstance = createTestCase("single entry list");

		IList<string> output = (IList<string>) caseService.getVariable(caseInstance.Id, "result");

		assertEquals(2, output.Count);
		assertEquals("foo", output[0]);
		assertEquals("foo", output[1]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deployment(resources = { RESULT_LIST_MAPPING_CMMN, TEST_DECISION }) public void testResultListMapping()
	  [Deployment(resources : { RESULT_LIST_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testResultListMapping()
	  {
		CaseInstance caseInstance = createTestCase("multiple entries list");

		IList<IDictionary<string, object>> resultList = (IList<IDictionary<string, object>>) caseService.getVariable(caseInstance.Id, "result");
		assertEquals(2, resultList.Count);

		foreach (IDictionary<string, object> valueMap in resultList)
		{
		  assertEquals(2, valueMap.Count);
		  assertEquals("foo", valueMap["result1"]);
		  assertEquals("bar", valueMap["result2"]);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deployment(resources = { DEFAULT_MAPPING_CMMN, TEST_DECISION }) public void testDefaultResultMapping()
	  [Deployment(resources : { DEFAULT_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testDefaultResultMapping()
	  {
		CaseInstance caseInstance = createTestCase("multiple entries list");

		// default mapping is 'resultList'
		IList<IDictionary<string, object>> resultList = (IList<IDictionary<string, object>>) caseService.getVariable(caseInstance.Id, "result");
		assertEquals(2, resultList.Count);

		foreach (IDictionary<string, object> valueMap in resultList)
		{
		  assertEquals(2, valueMap.Count);
		  assertEquals("foo", valueMap["result1"]);
		  assertEquals("bar", valueMap["result2"]);
		}
	  }

	  [Deployment(resources : { SINGLE_ENTRY_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testSingleEntryMappingFailureMultipleOutputs()
	  {
		try
		{
		  createTestCase("single entry list");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { SINGLE_ENTRY_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testSingleEntryMappingFailureMultipleValues()
	  {
		try
		{
		  createTestCase("multiple entries");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { SINGLE_RESULT_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testSingleResultMappingFailure()
	  {
		try
		{
		  createTestCase("single entry list");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22001", e.Message);
		}
	  }

	  [Deployment(resources : { COLLECT_ENTRIES_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testCollectEntriesMappingFailure()
	  {
		try
		{
		  createTestCase("multiple entries");

		  fail("expect exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("ENGINE-22002", e.Message);
		}
	  }

	  [Deployment(resources : { DEFAULT_MAPPING_CMMN, TEST_DECISION })]
	  public virtual void testTransientDecisionResult()
	  {
		// when a decision is evaluated and the result is stored in a transient variable "decisionResult"
		CaseInstance caseInstance = createTestCase("single entry");

		// then the variable should not be available outside the decision task
		assertNull(caseService.getVariable(caseInstance.Id, "decisionResult"));
	  }

	  [Deployment(resources : { OVERRIDE_DECISION_RESULT_CMMN, TEST_DECISION })]
	  public virtual void testFailedToOverrideDecisionResultVariable()
	  {
		try
		{
		  // the transient variable "decisionResult" should not be overridden by the task result variable
		  createTestCase("single entry");
		  fail("expect exception");

		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("transient variable with name decisionResult to non-transient", e.Message);
		}
	  }

	  protected internal virtual CaseInstance createTestCase(string input)
	  {
		return createCaseInstanceByKey("case", Variables.createVariables().putValue("input", input));
	  }

	}

}