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
namespace org.camunda.spin.plugin.variables
{
	using DmnDecisionResultImpl = org.camunda.bpm.dmn.engine.impl.DmnDecisionResultImpl;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// The test is copied from the engine to check how JSON serialization will behave with DMN result object.
	/// 
	/// @author Svetlana Dorokhova
	/// </summary>
	public class DmnBusinessRuleTaskResultMappingTest : ResourceProcessEngineTestCase
	{

	  protected internal const string TEST_DECISION = "org/camunda/spin/plugin/DmnBusinessRuleTaskResultMappingTest.dmn11.xml";
	  protected internal const string CUSTOM_MAPPING_BPMN = "org/camunda/spin/plugin/DmnBusinessRuleTaskResultMappingTest.testCustomOutputMapping.bpmn20.xml";
	  protected internal const string SINGLE_ENTRY_BPMN = "org/camunda/spin/plugin/DmnBusinessRuleTaskResultMappingTest.testSingleEntry.bpmn20.xml";
	  protected internal const string DEFAULT_MAPPING_BPMN = "org/camunda/spin/plugin/DmnBusinessRuleTaskResultMappingTest.testDefaultMapping.bpmn20.xml";
	  protected internal const string STORE_DECISION_RESULT_BPMN = "org/camunda/spin/plugin/DmnBusinessRuleTaskResultMappingTest.testStoreDecisionResult.bpmn20.xml";

	  public DmnBusinessRuleTaskResultMappingTest() : base("org/camunda/spin/plugin/json.camunda.cfg.xml")
	  {
	  }

	  [Deployment(resources : {STORE_DECISION_RESULT_BPMN, TEST_DECISION })]
	  public virtual void testStoreDecisionResult()
	  {
		ProcessInstance processInstance = startTestProcess("multiple entries");

		//deserialization is not working for this type of object -> deserializeValue parameter is false
		assertNotNull(runtimeService.getVariableTyped(processInstance.Id, "result", false));
	  }

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

	  protected internal virtual ProcessInstance startTestProcess(string input)
	  {
		return runtimeService.startProcessInstanceByKey("testProcess", Collections.singletonMap<string, object>("input", input));
	  }

	}

}