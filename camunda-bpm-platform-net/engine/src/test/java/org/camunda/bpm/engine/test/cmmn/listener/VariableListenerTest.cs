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
namespace org.camunda.bpm.engine.test.cmmn.listener
{

	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using DelegateCaseVariableInstance = org.camunda.bpm.engine.@delegate.DelegateCaseVariableInstance;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using CaseExecutionContext = org.camunda.bpm.engine.impl.context.CaseExecutionContext;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableListenerTest : PluggableProcessEngineTestCase
	{

	  protected internal IDictionary<object, object> beans = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();

		LogVariableListener.reset();
		beans = processEngineConfiguration.Beans;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAnyEventListenerByClass()
	  public virtual void testAnyEventListenerByClass()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable on a higher scope
		caseService.withCaseExecution(caseInstance.Id).setVariable("anInstanceVariable", "anInstanceValue").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);

		// when i set a variable on the human task (ie the source execution matters although the variable ends up in the same place)
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();

		// when i update the variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aNewTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);
		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE).name("aTaskVariable").value("aNewTaskValue").activityInstanceId(taskExecution.Id).matches(LogVariableListener.Invocations[0]);
		LogVariableListener.reset();

		// when i remove the variable from the human task
		caseService.withCaseExecution(taskExecution.Id).removeVariable("aTaskVariable").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);
		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE).name("aTaskVariable").value(null).activityInstanceId(taskExecution.Id).matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCreateEventListenerByClass()
	  public virtual void testCreateEventListenerByClass()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();

		// when i update the variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aNewTaskValue").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);

		// when i remove the variable from the human task
		caseService.withCaseExecution(taskExecution.Id).removeVariable("aTaskVariable").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUpdateEventListenerByClass()
	  public virtual void testUpdateEventListenerByClass()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);

		// when i update the variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aNewTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE).name("aTaskVariable").value("aNewTaskValue").activityInstanceId(taskExecution.Id).matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();


		// when i remove the variable from the human task
		caseService.withCaseExecution(taskExecution.Id).removeVariable("aTaskVariable").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerInvokedFromSourceScope()
	  public virtual void testVariableListenerInvokedFromSourceScope()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the case instance
		caseService.withCaseExecution(caseInstance.Id).setVariable("aTaskVariable", "aTaskValue").execute();

		// then the listener is not invoked
		assertEquals(0, LogVariableListener.Invocations.Count);

		// when i update the variable from the task execution
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(caseInstance).sourceExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE).name("aTaskVariable").value("aTaskValue").activityInstanceId(caseInstance.Id).matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeleteEventListenerByClass()
	  public virtual void testDeleteEventListenerByClass()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);

		// when i update the variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aNewTaskValue").execute();

		// then the listener is not invoked
		assertTrue(LogVariableListener.Invocations.Count == 0);

		// when i remove the variable from the human task
		caseService.withCaseExecution(taskExecution.Id).removeVariable("aTaskVariable").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE).name("aTaskVariable").value(null).activityInstanceId(taskExecution.Id).matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerByDelegateExpression()
	  public virtual void testVariableListenerByDelegateExpression()
	  {
		beans["listener"] = new LogVariableListener();

		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerByExpression()
	  public virtual void testVariableListenerByExpression()
	  {
		SimpleBean simpleBean = new SimpleBean();
		beans["bean"] = simpleBean;

		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertTrue(simpleBean.wasInvoked());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerByScript()
	  public virtual void testVariableListenerByScript()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i create a variable on the human task
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertTrue(SimpleBean.wasStaticallyInvoked());

		SimpleBean.reset();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/listener/VariableListenerTest.testListenerOnParentScope.cmmn")]
	  public virtual void testListenerSourceExecution()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable on a deeper scope execution but actually on the parent
		caseService.withCaseExecution(taskExecution.Id).setVariable("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		// and the source execution is the execution the variable was set on
		DelegateVariableInstanceSpec.fromCaseExecution(caseInstance).sourceExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testListenerOnParentScope()
	  public virtual void testListenerOnParentScope()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable on a deeper scope
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testChildListenersNotInvoked()
	  public virtual void testChildListenersNotInvoked()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		// when i set a variable on the parent scope
		caseService.withCaseExecution(caseInstance.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is not invoked
		assertEquals(0, LogVariableListener.Invocations.Count);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testListenerOnAncestorScope()
	  public virtual void testListenerOnAncestorScope()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution stageExecution = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult();
		assertNotNull(stageExecution);

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable on a deeper scope
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aTaskVariable").value("aTaskValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInvalidListenerClassName()
	  public virtual void testInvalidListenerClassName()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		try
		{
		  caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		  fail("expected exception during variable listener invocation");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testListenerDoesNotImplementInterface()
	  public virtual void testListenerDoesNotImplementInterface()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		try
		{
		  caseService.withCaseExecution(taskExecution.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		  fail("expected exception during variable listener invocation");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateInstanceIsProcessEngineAware()
	  public virtual void testDelegateInstanceIsProcessEngineAware()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();
		assertFalse(ProcessEngineAwareListener.hasFoundValidRuntimeService());

		// when i set a variable that causes the listener to be notified
		caseService.withCaseExecution(caseInstance.Id).setVariableLocal("aTaskVariable", "aTaskValue").execute();

		// then the listener is invoked and has found process engine services
		assertTrue(ProcessEngineAwareListener.hasFoundValidRuntimeService());

		ProcessEngineAwareListener.reset();
	  }

	  /// <summary>
	  /// TODO: add when history for case execution variables is implemented
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testListenerDoesNotInterfereWithHistory()
	  public virtual void FAILING_testListenerDoesNotInterfereWithHistory()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		// when i set a variable that causes the listener to be notified
		// and that listener sets the same variable to another value (here "value2")
		caseService.withCaseExecution(caseInstance.Id).setVariableLocal("variable", "value1").execute();

		// then there should be two historic variable updates for both values
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Id)
		{
		  IList<HistoricDetail> variableUpdates = historyService.createHistoricDetailQuery().variableUpdates().list();

		  assertEquals(2, variableUpdates.Count);

		  foreach (HistoricDetail detail in variableUpdates)
		  {
			HistoricVariableUpdate update = (HistoricVariableUpdate) detail;
			bool update1Processed = false;
			bool update2Processed = false;

			if (!update1Processed && update.Value.Equals("value1"))
			{
			  update1Processed = true;
			}
			else if (!update2Processed && update.Value.Equals("value2"))
			{
			  update2Processed = true;
			}
			else
			{
			  fail("unexpected variable update");
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testListenerInvocationFinishesBeforeSubsequentInvocations()
	  public virtual void testListenerInvocationFinishesBeforeSubsequentInvocations()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable and the listener itself sets another variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("variable", "value1").execute();

		// then all listeners for the first variable update are invoked first
		// and then the listeners for the second update are invoked
		IList<DelegateCaseVariableInstance> invocations = LogAndUpdateVariableListener.Invocations;
		assertEquals(6, invocations.Count);

		// the first invocations should regard the first value
		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("variable").value("value1").matches(LogAndUpdateVariableListener.Invocations[0]);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("variable").value("value1").matches(LogAndUpdateVariableListener.Invocations[1]);

		// the second invocations should regard the updated value
		// there are four invocations since both listeners have set "value2" and both were again executed, i.e. 2*2 = 4

		for (int i = 2; i < 6; i++)
		{
		  DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE).name("variable").value("value2").matches(LogAndUpdateVariableListener.Invocations[i]);
		}

		LogAndUpdateVariableListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoListenersOnSameScope()
	  public virtual void testTwoListenersOnSameScope()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("testVariable", "value1").execute();

		// then both listeners are invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("testVariable").value("value1").matches(LogVariableListener.Invocations[0]);

		assertEquals(1, LogAndUpdateVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(taskExecution).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("testVariable").value("value1").matches(LogAndUpdateVariableListener.Invocations[0]);

		LogVariableListener.reset();
		LogAndUpdateVariableListener.reset();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerByClassWithFieldExpressions()
	  public virtual void testVariableListenerByClassWithFieldExpressions()
	  {
		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("testVariable", "value1").execute();

		// then the field expressions are resolved
		assertEquals(1, LogInjectedValuesListener.ResolvedStringValueExpressions.Count);
		assertEquals("injectedValue", LogInjectedValuesListener.ResolvedStringValueExpressions[0]);

		assertEquals(1, LogInjectedValuesListener.ResolvedJuelExpressions.Count);
		assertEquals("cam", LogInjectedValuesListener.ResolvedJuelExpressions[0]);

		LogInjectedValuesListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerByDelegateExpressionWithFieldExpressions()
	  public virtual void testVariableListenerByDelegateExpressionWithFieldExpressions()
	  {
		beans["listener"] = new LogInjectedValuesListener();

		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("testVariable", "value1").execute();

		// then the field expressions are resolved
		assertEquals(1, LogInjectedValuesListener.ResolvedStringValueExpressions.Count);
		assertEquals("injectedValue", LogInjectedValuesListener.ResolvedStringValueExpressions[0]);

		assertEquals(1, LogInjectedValuesListener.ResolvedJuelExpressions.Count);
		assertEquals("cam", LogInjectedValuesListener.ResolvedJuelExpressions[0]);

		LogInjectedValuesListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableListenerExecutionContext()
	  public virtual void testVariableListenerExecutionContext()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("testVariable", "value1").execute();

		// then the listener is invoked
		assertEquals(1, LogExecutionContextListener.CaseExecutionContexts.Count);
		CaseExecutionContext executionContext = LogExecutionContextListener.CaseExecutionContexts[0];

		assertNotNull(executionContext);

		// although this is not inside a command, checking for IDs should be ok
		assertEquals(caseInstance.Id, executionContext.CaseInstance.Id);
		assertEquals(taskExecution.Id, executionContext.Execution.Id);

		LogExecutionContextListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInvokeBuiltinListenersOnly()
	  public virtual void testInvokeBuiltinListenersOnly()
	  {
		// disable custom variable listener invocation
		processEngineConfiguration.InvokeCustomVariableListeners = false;

		// add a builtin variable listener the hard way
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		processEngineConfiguration.DeploymentCache.getCaseDefinitionById(caseDefinition.Id).findActivity("PI_HumanTask_1").addBuiltInVariableListener(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE, new LogVariableListener());

		caseService.withCaseDefinitionByKey("case").create();

		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		assertNotNull(taskExecution);

		// when i set a variable
		caseService.withCaseExecution(taskExecution.Id).setVariableLocal("testVariable", "value1").execute();

		// then the builtin listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		// but the custom listener is not invoked
		assertEquals(0, LogExecutionContextListener.CaseExecutionContexts.Count);

		LogVariableListener.reset();
		LogExecutionContextListener.reset();

		// restore configuration
		processEngineConfiguration.InvokeCustomVariableListeners = true;
	  }

	  public virtual void testDefaultCustomListenerInvocationSetting()
	  {
		assertTrue(processEngineConfiguration.InvokeCustomVariableListeners);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/cmmn/listener/VariableListenerTest.testVariableListenerWithProcessTask.cmmn", "org/camunda/bpm/engine/test/cmmn/listener/VariableListenerTest.testVariableListenerWithProcessTask.bpmn20.xml" })]
	  public virtual void testVariableListenerWithProcessTask()
	  {
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("case");

		CaseExecution processTask = caseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult();

		string processTaskId = processTask.Id;

		caseService.withCaseExecution(processTaskId).manualStart();

		// then the listener is invoked
		assertEquals(1, LogVariableListener.Invocations.Count);

		DelegateVariableInstanceSpec.fromCaseExecution(caseInstance).sourceExecution(processTask).@event(org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE).name("aVariable").value("aValue").matches(LogVariableListener.Invocations[0]);

		LogVariableListener.reset();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		beans.Clear();

		base.tearDown();
	  }

	}

}