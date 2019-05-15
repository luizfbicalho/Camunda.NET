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

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnDecisionResultEntries = org.camunda.bpm.dmn.engine.DmnDecisionResultEntries;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// Tests the decision result that is retrieved by an execution listener.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class DmnDecisionResultListenerTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TEST_PROCESS = "org/camunda/bpm/engine/test/dmn/result/DmnDecisionResultTest.bpmn20.xml";
	  protected internal const string TEST_DECISION = "org/camunda/bpm/engine/test/dmn/result/DmnDecisionResultTest.dmn11.xml";
	  protected internal const string TEST_DECISION_COLLECT_SUM = "org/camunda/bpm/engine/test/dmn/result/DmnDecisionResultCollectSumHitPolicyTest.dmn11.xml";
	  protected internal const string TEST_DECISION_COLLECT_COUNT = "org/camunda/bpm/engine/test/dmn/result/DmnDecisionResultCollectCountHitPolicyTest.dmn11.xml";

	  protected internal DmnDecisionResult results;

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testNoOutput()
	  {
		startTestProcess("no output");

		assertTrue("The decision result 'ruleResult' should be empty", results.Empty);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testEmptyOutput()
	  {
		startTestProcess("empty output");

		assertFalse("The decision result 'ruleResult' should not be empty", results.Empty);

		DmnDecisionResultEntries decisionOutput = results.get(0);
		assertNull(decisionOutput.FirstEntry);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testEmptyMap()
	  {
		startTestProcess("empty map");

		assertEquals(2, results.size());

		foreach (DmnDecisionResultEntries output in results)
		{
		  assertTrue("The decision output should be empty", output.Empty);
		}
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testSingleEntry()
	  {
		startTestProcess("single entry");

		DmnDecisionResultEntries firstOutput = results.get(0);
		assertEquals("foo", firstOutput.FirstEntry);
		assertEquals(Variables.stringValue("foo"), firstOutput.FirstEntryTyped);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testMultipleEntries()
	  {
		startTestProcess("multiple entries");

		DmnDecisionResultEntries firstOutput = results.get(0);
		assertEquals("foo", firstOutput.get("result1"));
		assertEquals("bar", firstOutput.get("result2"));

		assertEquals(Variables.stringValue("foo"), firstOutput.getEntryTyped("result1"));
		assertEquals(Variables.stringValue("bar"), firstOutput.getEntryTyped("result2"));
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testSingleEntryList()
	  {
		startTestProcess("single entry list");

		assertEquals(2, results.size());

		foreach (DmnDecisionResultEntries output in results)
		{
		  assertEquals("foo", output.FirstEntry);
		  assertEquals(Variables.stringValue("foo"), output.FirstEntryTyped);
		}
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION})]
	  public virtual void testMultipleEntriesList()
	  {
		startTestProcess("multiple entries list");

		assertEquals(2, results.size());

		foreach (DmnDecisionResultEntries output in results)
		{
		  assertEquals(2, output.size());
		  assertEquals("foo", output.get("result1"));
		  assertEquals("bar", output.get("result2"));

		  assertEquals(Variables.stringValue("foo"), output.getEntryTyped("result1"));
		  assertEquals(Variables.stringValue("bar"), output.getEntryTyped("result2"));
		}
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION_COLLECT_COUNT })]
	  public virtual void testCollectCountHitPolicyNoOutput()
	  {
		startTestProcess("no output");

		assertEquals(1, results.size());
		DmnDecisionResultEntries firstOutput = results.get(0);

		assertEquals(0, firstOutput.FirstEntry);
		assertEquals(Variables.integerValue(0), firstOutput.FirstEntryTyped);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
	  public virtual void testCollectSumHitPolicyNoOutput()
	  {
		startTestProcess("no output");

		assertTrue("The decision result 'ruleResult' should be empty", results.Empty);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
	  public virtual void testCollectSumHitPolicySingleEntry()
	  {
		startTestProcess("single entry");

		assertEquals(1, results.size());
		DmnDecisionResultEntries firstOutput = results.get(0);

		assertEquals(12, firstOutput.FirstEntry);
		assertEquals(Variables.integerValue(12), firstOutput.FirstEntryTyped);
	  }

	  [Deployment(resources : { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
	  public virtual void testCollectSumHitPolicySingleEntryList()
	  {
		startTestProcess("single entry list");

		assertEquals(1, results.size());
		DmnDecisionResultEntries firstOutput = results.get(0);

		assertEquals(33, firstOutput.FirstEntry);
		assertEquals(Variables.integerValue(33), firstOutput.FirstEntryTyped);
	  }

	  protected internal virtual ProcessInstance startTestProcess(string input)
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("input", input));

		// get the result from an execution listener that is invoked at the end of the business rule activity
		results = DecisionResultTestListener.DecisionResult;
		assertNotNull(results);

		return processInstance;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		// reset the invoked execution listener
		DecisionResultTestListener.reset();
	  }

	}

}