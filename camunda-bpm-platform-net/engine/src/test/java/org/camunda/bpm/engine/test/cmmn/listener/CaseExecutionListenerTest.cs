using System;

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
namespace org.camunda.bpm.engine.test.cmmn.listener
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionListenerTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCreateListenerByClass.cmmn"})]
	  public virtual void testCreateListenerByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCreateListenerByDelegateExpression.cmmn"})]
	  public virtual void testCreateListenerByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCreateListenerByExpression.cmmn"})]
	  public virtual void testCreateListenerByExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCreateListenerByScript.cmmn"})]
	  public virtual void testCreateListenerByScript()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testEnableListenerByClass.cmmn"})]
	  public virtual void testEnableListenerByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testEnableListenerByDelegateExpression.cmmn"})]
	  public virtual void testEnableListenerByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testEnableListenerByExpression.cmmn"})]
	  public virtual void testEnableListenerByExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testEnableListenerByScript.cmmn"})]
	  public virtual void testEnableListenerByScript()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDisableListenerByClass.cmmn"})]
	  public virtual void testDisableListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).disable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDisableListenerByDelegateExpression.cmmn"})]
	  public virtual void testDisableListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).disable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDisableListenerByExpression.cmmn"})]
	  public virtual void testDisableListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).disable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDisableListenerByScript.cmmn"})]
	  public virtual void testDisableListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).disable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testReEnableListenerByClass.cmmn"})]
	  public virtual void testReEnableListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(humanTaskId).disable();

		// when
		caseService.withCaseExecution(humanTaskId).reenable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testReEnableListenerByDelegateExpression.cmmn"})]
	  public virtual void testReEnableListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(humanTaskId).disable();

		// when
		caseService.withCaseExecution(humanTaskId).reenable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testReEnableListenerByExpression.cmmn"})]
	  public virtual void testReEnableListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(humanTaskId).disable();

		// when
		caseService.withCaseExecution(humanTaskId).reenable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testReEnableListenerByScript.cmmn"})]
	  public virtual void testReEnableListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(humanTaskId).disable();

		// when
		caseService.withCaseExecution(humanTaskId).reenable();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testStartListenerByClass.cmmn"})]
	  public virtual void testStartListenerByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("start").singleResult().Value);
		assertEquals(1, query.variableName("startEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("startOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testStartListenerByDelegateExpression.cmmn"})]
	  public virtual void testStartListenerByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("start").singleResult().Value);
		assertEquals(1, query.variableName("startEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("startOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testStartListenerByExpression.cmmn"})]
	  public virtual void testStartListenerByExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("start").singleResult().Value);
		assertEquals(1, query.variableName("startEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("startOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testStartListenerByScript.cmmn"})]
	  public virtual void testStartListenerByScript()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("start").singleResult().Value);
		assertEquals(1, query.variableName("startEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("startOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testManualStartListenerByClass.cmmn"})]
	  public virtual void testManualStartListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).manualStart();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testManualStartListenerByDelegateExpression.cmmn"})]
	  public virtual void testManualStartListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).manualStart();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testManualStartListenerByExpression.cmmn"})]
	  public virtual void testManualStartListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).manualStart();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testManualStartListenerByScript.cmmn"})]
	  public virtual void testManualStartListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).manualStart();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCompleteListenerByClass.cmmn"})]
	  public virtual void testCompleteListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCompleteListenerByDelegateExpression.cmmn"})]
	  public virtual void testCompleteListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCompleteListenerByExpression.cmmn"})]
	  public virtual void testCompleteListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testCompleteListenerByScript.cmmn"})]
	  public virtual void testCompleteListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testTerminateListenerByClass.cmmn"})]
	  public virtual void testTerminateListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testTerminateListenerByDelegateExpression.cmmn"})]
	  public virtual void testTerminateListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testTerminateListenerByExpression.cmmn"})]
	  public virtual void testTerminateListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testTerminateListenerByScript.cmmn"})]
	  public virtual void testTerminateListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		terminate(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testExitListenerByClass.cmmn"})]
	  public virtual void testExitListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		exit(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("exit").singleResult().Value);
		assertEquals(1, query.variableName("exitEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("exitOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testExitListenerByDelegateExpression.cmmn"})]
	  public virtual void testExitListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		exit(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("exit").singleResult().Value);
		assertEquals(1, query.variableName("exitEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("exitOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testExitListenerByExpression.cmmn"})]
	  public virtual void testExitListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		exit(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("exit").singleResult().Value);
		assertEquals(1, query.variableName("exitEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("exitOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testExitListenerByScript.cmmn"})]
	  public virtual void testExitListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		exit(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("exit").singleResult().Value);
		assertEquals(1, query.variableName("exitEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("exitOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentTerminateListenerByClass.cmmn"})]
	  public virtual void testParentTerminateListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		parentTerminate(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentTerminate").singleResult().Value);
		assertEquals(1, query.variableName("parentTerminateEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("parentTerminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentTerminateListenerByDelegateExpression.cmmn"})]
	  public virtual void testParentTerminateListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		parentTerminate(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentTerminate").singleResult().Value);
		assertEquals(1, query.variableName("parentTerminateEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("parentTerminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentTerminateListenerByExpression.cmmn"})]
	  public virtual void testParentTerminateListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		parentTerminate(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentTerminate").singleResult().Value);
		assertEquals(1, query.variableName("parentTerminateEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("parentTerminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentTerminateListenerByScript.cmmn"})]
	  public virtual void testParentTerminateListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		parentTerminate(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentTerminate").singleResult().Value);
		assertEquals(1, query.variableName("parentTerminateEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("parentTerminateOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testSuspendListenerByClass.cmmn"})]
	  public virtual void testSuspendListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		suspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testSuspendListenerByDelegateExpression.cmmn"})]
	  public virtual void testSuspendListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		suspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testSuspendListenerByExpression.cmmn"})]
	  public virtual void testSuspendListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		suspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testSuspendListenerByScript.cmmn"})]
	  public virtual void testSuspendListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		suspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentSuspendListenerByClass.cmmn"})]
	  public virtual void testParentSuspendListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		parentSuspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentSuspend").singleResult().Value);
		assertEquals(1, query.variableName("parentSuspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentSuspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentSuspendListenerByDelegateExpression.cmmn"})]
	  public virtual void testParentSuspendListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		parentSuspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentSuspend").singleResult().Value);
		assertEquals(1, query.variableName("parentSuspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentSuspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentSuspendListenerByExpression.cmmn"})]
	  public virtual void testParentSuspendListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		parentSuspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentSuspend").singleResult().Value);
		assertEquals(1, query.variableName("parentSuspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentSuspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentSuspendListenerByScript.cmmn"})]
	  public virtual void testParentSuspendListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		parentSuspend(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentSuspend").singleResult().Value);
		assertEquals(1, query.variableName("parentSuspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentSuspendOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testResumeListenerByClass.cmmn"})]
	  public virtual void testResumeListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		resume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testResumeListenerByDelegateExpression.cmmn"})]
	  public virtual void testResumeListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		resume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testResumeListenerByExpression.cmmn"})]
	  public virtual void testResumeListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		resume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testResumeListenerByScript.cmmn"})]
	  public virtual void testResumeListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		resume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentResumeListenerByClass.cmmn"})]
	  public virtual void testParentResumeListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		parentResume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentResume").singleResult().Value);
		assertEquals(1, query.variableName("parentResumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentResumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentResumeListenerByDelegateExpression.cmmn"})]
	  public virtual void testParentResumeListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		parentResume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentResume").singleResult().Value);
		assertEquals(1, query.variableName("parentResumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentResumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentResumeListenerByExpression.cmmn"})]
	  public virtual void testParentResumeListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		parentResume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("parentResume").singleResult().Value);
		assertEquals(1, query.variableName("parentResumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentResumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testParentResumeListenerByScript.cmmn"})]
	  public virtual void testParentResumeListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		suspend(humanTaskId);

		// when
		parentResume(humanTaskId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("parentResume").singleResult().Value);
		assertEquals(1, query.variableName("parentResumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("parentResumeOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testOccurListenerByClass.cmmn"})]
	  public virtual void testOccurListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		occur(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("occur").singleResult().Value);
		assertEquals(1, query.variableName("occurEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("occurOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testOccurListenerByDelegateExpression.cmmn"})]
	  public virtual void testOccurListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		occur(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("occur").singleResult().Value);
		assertEquals(1, query.variableName("occurEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("occurOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testOccurListenerByExpression.cmmn"})]
	  public virtual void testOccurListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		occur(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("occur").singleResult().Value);
		assertEquals(1, query.variableName("occurEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("occurOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testOccurListenerByScript.cmmn"})]
	  public virtual void testOccurListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string milestoneId = caseService.createCaseExecutionQuery().activityId("PI_Milestone_1").singleResult().Id;

		// when
		occur(milestoneId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("occur").singleResult().Value);
		assertEquals(1, query.variableName("occurEventCounter").singleResult().Value);
		assertEquals(milestoneId, query.variableName("occurOnCaseExecutionId").singleResult().Value);

		assertEquals(1, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testAllListenerByClass.cmmn"})]
	  public virtual void testAllListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		caseService.withCaseExecution(humanTaskId).disable();

		caseService.withCaseExecution(humanTaskId).reenable();

		caseService.withCaseExecution(humanTaskId).manualStart();

		suspend(humanTaskId);

		resume(humanTaskId);

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(25, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(8, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testAllListenerByDelegateExpression.cmmn"})]
	  public virtual void testAllListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		caseService.withCaseExecution(humanTaskId).disable();

		caseService.withCaseExecution(humanTaskId).reenable();

		caseService.withCaseExecution(humanTaskId).manualStart();

		suspend(humanTaskId);

		resume(humanTaskId);

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(26, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(8, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testAllListenerByExpression.cmmn"})]
	  public virtual void testAllListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		caseService.withCaseExecution(humanTaskId).disable();

		caseService.withCaseExecution(humanTaskId).reenable();

		caseService.withCaseExecution(humanTaskId).manualStart();

		suspend(humanTaskId);

		resume(humanTaskId);

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(26, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(8, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testAllListenerByScript.cmmn"})]
	  public virtual void testAllListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when

		caseService.withCaseExecution(humanTaskId).disable();

		caseService.withCaseExecution(humanTaskId).reenable();

		caseService.withCaseExecution(humanTaskId).manualStart();

		suspend(humanTaskId);

		resume(humanTaskId);

		caseService.withCaseExecution(humanTaskId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(25, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("enable").singleResult().Value);
		assertEquals(1, query.variableName("enableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("enableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("resume").singleResult().Value);
		assertEquals(1, query.variableName("resumeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("resumeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertEquals(8, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testFieldInjectionByClass.cmmn"})]
	  public virtual void testFieldInjectionByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertEquals("Hello from The Case", query.variableName("greeting").singleResult().Value);
		assertEquals("Hello World", query.variableName("helloWorld").singleResult().Value);
		assertEquals("cam", query.variableName("prefix").singleResult().Value);
		assertEquals("unda", query.variableName("suffix").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testFieldInjectionByDelegateExpression.cmmn"})]
	  public virtual void testFieldInjectionByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new FieldInjectionCaseExecutionListener()).create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertEquals("Hello from The Case", query.variableName("greeting").singleResult().Value);
		assertEquals("Hello World", query.variableName("helloWorld").singleResult().Value);
		assertEquals("cam", query.variableName("prefix").singleResult().Value);
		assertEquals("unda", query.variableName("suffix").singleResult().Value);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testListenerByScriptResource.cmmn", "org/camunda/bpm/engine/test/cmmn/listener/caseExecutionListener.groovy" })]
	  public virtual void testListenerByScriptResource()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		// when
		caseService.withCaseExecution(humanTaskId).disable();

		caseService.withCaseExecution(humanTaskId).reenable();

		caseService.withCaseExecution(humanTaskId).manualStart();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(10, query.count());

		assertTrue((bool?) query.variableName("disable").singleResult().Value);
		assertEquals(1, query.variableName("disableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("disableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reenable").singleResult().Value);
		assertEquals(1, query.variableName("reenableEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("reenableOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("manualStart").singleResult().Value);
		assertEquals(1, query.variableName("manualStartEventCounter").singleResult().Value);
		assertEquals(humanTaskId, query.variableName("manualStartOnCaseExecutionId").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDoesNotImplementCaseExecutionListenerInterfaceByClass.cmmn"})]
	  public virtual void testDoesNotImplementCaseExecutionListenerInterfaceByClass()
	  {
		// given


		try
		{
		  // when
		  caseService.withCaseDefinitionByKey("case").create();
		}
		catch (Exception e)
		{
		  // then
		  string message = e.Message;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertTextPresent("ENGINE-05016 Class 'org.camunda.bpm.engine.test.cmmn.listener.NotCaseExecutionListener' doesn't implement '" + typeof(CaseExecutionListener).FullName + "'", message);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testDoesNotImplementCaseExecutionListenerInterfaceByDelegateExpression.cmmn"})]
	  public virtual void testDoesNotImplementCaseExecutionListenerInterfaceByDelegateExpression()
	  {
		// given

		try
		{
		  // when
		  caseService.withCaseDefinitionByKey("case").setVariable("myListener", new NotCaseExecutionListener()).create();
		}
		catch (Exception e)
		{
		  // then
		  string message = e.Message;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertTextPresent("Delegate expression ${myListener} did not resolve to an implementation of interface " + typeof(CaseExecutionListener).FullName, message);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testListenerDoesNotExist.cmmn"})]
	  public virtual void testListenerDoesNotExist()
	  {
		// given

		try
		{
		  // when
		  caseService.withCaseDefinitionByKey("case").create().Id;
		}
		catch (Exception e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent("Exception while instantiating class 'org.camunda.bpm.engine.test.cmmn.listener.NotExistingCaseExecutionListener'", message);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseExecutionListenerTest.testBusinessKeyAsCaseBusinessKey.cmmn"})]
	  public virtual void testBusinessKeyAsCaseBusinessKey()
	  {
		// given

		// when
		caseService.withCaseDefinitionByKey("case").businessKey("myBusinessKey").create().Id;

		// then
		VariableInstance v1 = runtimeService.createVariableInstanceQuery().variableName("businessKey").singleResult();
		VariableInstance v2 = runtimeService.createVariableInstanceQuery().variableName("caseBusinessKey").singleResult();
		assertNotNull(v1);
		assertNotNull(v2);
		assertEquals("myBusinessKey", v1.Value);
		assertEquals(v1.Value, v2.Value);
	  }

	}

}