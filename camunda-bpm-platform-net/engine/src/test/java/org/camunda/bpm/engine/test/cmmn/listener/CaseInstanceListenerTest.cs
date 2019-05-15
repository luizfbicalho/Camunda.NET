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
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceListenerTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCreateListenerByClass.cmmn"})]
	  public virtual void testCreateListenerByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCreateListenerByDelegateExpression.cmmn"})]
	  public virtual void testCreateListenerByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCreateListenerByExpression.cmmn"})]
	  public virtual void testCreateListenerByExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCreateListenerByScript.cmmn"})]
	  public virtual void testCreateListenerByScript()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCompleteListenerByClass.cmmn"})]
	  public virtual void testCompleteListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		caseService.withCaseExecution(caseInstanceId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCompleteListenerByDelegateExpression.cmmn"})]
	  public virtual void testCompleteListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// when
		caseService.withCaseExecution(caseInstanceId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCompleteListenerByExpression.cmmn"})]
	  public virtual void testCompleteListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// when
		caseService.withCaseExecution(caseInstanceId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCompleteListenerByScript.cmmn"})]
	  public virtual void testCompleteListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		caseService.withCaseExecution(caseInstanceId).complete();

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testTerminateListenerByClass.cmmn"})]
	  public virtual void testTerminateListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		terminate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testTerminateListenerByDelegateExpression.cmmn"})]
	  public virtual void testTerminateListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// when
		terminate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testTerminateListenerByExpression.cmmn"})]
	  public virtual void testTerminateListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// when
		terminate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testTerminateListenerByScript.cmmn"})]
	  public virtual void testTerminateListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		terminate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testSuspendListenerByClass.cmmn"})]
	  public virtual void testSuspendListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testSuspendListenerByDelegateExpression.cmmn"})]
	  public virtual void testSuspendListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		// when
		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testSuspendListenerByExpression.cmmn"})]
	  public virtual void testSuspendListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		// when
		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testSuspendListenerByScript.cmmn"})]
	  public virtual void testSuspendListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		// when
		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testReActivateListenerByClass.cmmn"})]
	  public virtual void testReActivateListenerByClass()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		// when
		reactivate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(1, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testReActivateListenerByDelegateExpression.cmmn"})]
	  public virtual void testReActivateListenerByDelegateExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		terminate(caseInstanceId);

		// when
		reactivate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(1, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testReActivateListenerByExpression.cmmn"})]
	  public virtual void testReActivateListenerByExpression()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		suspend(caseInstanceId);

		// when
		reactivate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(5, query.count());

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(1, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testReActivateListenerByScript.cmmn"})]
	  public virtual void testReActivateListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		// when
		reactivate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(4, query.count());

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(1, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(1, query.variableName("eventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCloseListenerByClass.cmmn"})]
	  public virtual void testCloseListenerByClass()
	  {
		CloseCaseExecutionListener.clear();

		assertNull(CloseCaseExecutionListener.EVENT);
		assertEquals(0, CloseCaseExecutionListener.COUNTER);
		assertNull(CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		// when
		caseService.withCaseExecution(caseInstanceId).close();

		// then
		assertEquals("close", CloseCaseExecutionListener.EVENT);
		assertEquals(1, CloseCaseExecutionListener.COUNTER);
		assertEquals(caseInstanceId, CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCloseListenerByDelegateExpression.cmmn"})]
	  public virtual void testCloseListenerByDelegateExpression()
	  {
		CloseCaseExecutionListener.clear();

		assertNull(CloseCaseExecutionListener.EVENT);
		assertEquals(0, CloseCaseExecutionListener.COUNTER);
		assertNull(CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new CloseCaseExecutionListener()).create().Id;

		terminate(caseInstanceId);

		// when
		caseService.withCaseExecution(caseInstanceId).close();

		// then
		assertEquals("close", CloseCaseExecutionListener.EVENT);
		assertEquals(1, CloseCaseExecutionListener.COUNTER);
		assertEquals(caseInstanceId, CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCloseListenerByExpression.cmmn"})]
	  public virtual void testCloseListenerByExpression()
	  {
		CloseCaseExecutionListener.clear();

		assertNull(CloseCaseExecutionListener.EVENT);
		assertEquals(0, CloseCaseExecutionListener.COUNTER);
		assertNull(CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new CloseCaseExecutionListener()).create().Id;

		suspend(caseInstanceId);

		// when
		caseService.withCaseExecution(caseInstanceId).close();

		// then
		assertEquals("close", CloseCaseExecutionListener.EVENT);
		assertEquals(1, CloseCaseExecutionListener.COUNTER);
		assertEquals(caseInstanceId, CloseCaseExecutionListener.ON_CASE_EXECUTION_ID);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testCloseListenerByScript.cmmn"})]
	  public virtual void testCloseListenerByScript()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		// when
		caseService.withCaseExecution(caseInstanceId).close();

		// then
		// TODO: if history is provided, the historic variables have to be checked!

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testAllListenerByClass.cmmn"})]
	  public virtual void testAllListenerByClass()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		reactivate(caseInstanceId);

		terminate(caseInstanceId);

		reactivate(caseInstanceId);

		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(16, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(2, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

		assertEquals(6, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testAllListenerByDelegateExpression.cmmn"})]
	  public virtual void testAllListenerByDelegateExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MySpecialCaseExecutionListener()).create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		reactivate(caseInstanceId);

		terminate(caseInstanceId);

		reactivate(caseInstanceId);

		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(17, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(2, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

		assertEquals(6, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testAllListenerByExpression.cmmn"})]
	  public virtual void testAllListenerByExpression()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").setVariable("myListener", new MyCaseExecutionListener()).create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		reactivate(caseInstanceId);

		terminate(caseInstanceId);

		reactivate(caseInstanceId);

		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(17, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(2, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

		assertEquals(6, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testAllListenerByScript.cmmn"})]
	  public virtual void testAllListenerByScript()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		reactivate(caseInstanceId);

		terminate(caseInstanceId);

		reactivate(caseInstanceId);

		suspend(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(16, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("suspend").singleResult().Value);
		assertEquals(1, query.variableName("suspendEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("suspendOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("reactivate").singleResult().Value);
		assertEquals(2, query.variableName("reactivateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("reactivateOnCaseExecutionId").singleResult().Value);

		assertEquals(6, query.variableName("eventCounter").singleResult().Value);

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testFieldInjectionByClass.cmmn"})]
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

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testFieldInjectionByDelegateExpression.cmmn"})]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testListenerByScriptResource.cmmn", "org/camunda/bpm/engine/test/cmmn/listener/caseExecutionListener.groovy" })]
	  public virtual void testListenerByScriptResource()
	  {
		// given

		// when
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		caseService.withCaseExecution(caseInstanceId).complete();

		reactivate(caseInstanceId);

		terminate(caseInstanceId);

		// then
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().caseInstanceIdIn(caseInstanceId);

		assertEquals(10, query.count());

		assertTrue((bool?) query.variableName("create").singleResult().Value);
		assertEquals(1, query.variableName("createEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("createOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("complete").singleResult().Value);
		assertEquals(1, query.variableName("completeEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("completeOnCaseExecutionId").singleResult().Value);

		assertTrue((bool?) query.variableName("terminate").singleResult().Value);
		assertEquals(1, query.variableName("terminateEventCounter").singleResult().Value);
		assertEquals(caseInstanceId, query.variableName("terminateOnCaseExecutionId").singleResult().Value);

		assertEquals(3, query.variableName("eventCounter").singleResult().Value);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testDoesNotImplementCaseExecutionListenerInterfaceByClass.cmmn"})]
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

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testDoesNotImplementCaseExecutionListenerInterfaceByDelegateExpression.cmmn"})]
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

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/listener/CaseInstanceListenerTest.testListenerDoesNotExist.cmmn"})]
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
		  assertTextPresent("Exception while instantiating class", message);
		}

	  }

	}

}