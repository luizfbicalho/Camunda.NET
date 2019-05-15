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
namespace org.camunda.bpm.engine.test.standalone.entity
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// @author Nikola Koevski <nikola.koevski@camunda.com>
	/// </summary>
	public class ExecutionEntityTest
	{
		private bool InstanceFieldsInitialized = false;

		public ExecutionEntityTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(processEngineRule);
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule processEngineRule = new ProcessEngineRule(true);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testRule = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(processEngineRule);
	  public ProcessEngineTestRule testRule;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestoreProcessInstance()
	  public virtual void testRestoreProcessInstance()
	  {
		//given parent execution
		IList<ExecutionEntity> entities = new List<ExecutionEntity>();
		ExecutionEntity parent = new ExecutionEntity();
		parent.Id = "parent";
		entities.Add(parent);
		//when restore process instance is called
		parent.restoreProcessInstance(entities, null, null, null, null, null, null);
		//then no problem should occure

		//when child is added and restore is called again
		ExecutionEntity entity = new ExecutionEntity();
		entity.Id = "child";
		entity.ParentId = parent.Id;
		entities.Add(entity);

		parent.restoreProcessInstance(entities, null, null, null, null, null, null);
		//then again no problem should occure

		//when parent is deleted from the list
		entities.Remove(parent);

		//then exception is thrown because child reference to parent which does not exist anymore
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot resolve parent with id 'parent' of execution 'child', perhaps it was deleted in the meantime");
		parent.restoreProcessInstance(entities, null, null, null, null, null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveExecutionSequence()
	  public virtual void testRemoveExecutionSequence()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("singleTaskProcess").startEvent().userTask("taskWithLocalVariables").camundaExecutionListenerClass("start", typeof(TestLocalVariableExecutionListener)).camundaTaskListenerClass("delete", typeof(TestLocalVariableTaskListener)).boundaryEvent().signal("interruptSignal").endEvent().moveToActivity("taskWithLocalVariables").endEvent().done();

		testRule.deploy(modelInstance);
		ProcessInstance pi = processEngineRule.RuntimeService.startProcessInstanceByKey("singleTaskProcess");
		Execution execution = processEngineRule.RuntimeService.createExecutionQuery().variableValueEquals("localVar", "localVarVal").singleResult();

		// when
		assertNotNull(execution);
		assertEquals(pi.Id, execution.ProcessInstanceId);
		processEngineRule.RuntimeService.signal(execution.Id);

		// then (see #TestLocalVariableTaskListener::notify)
	  }

	  public class TestLocalVariableExecutionListener : ExecutionListener
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  // given (see #testRemoveExecutionSequence)
		  execution.setVariableLocal("localVar", "localVarVal");
		}
	  }

	  public class TestLocalVariableTaskListener : TaskListener
	  {

		public virtual void notify(DelegateTask delegateTask)
		{
		  try
		  {
			// then (see #testRemoveExecutionSequence)
			StringValue var = delegateTask.Execution.getVariableLocalTyped("localVar");
			assertEquals("localVarVal", var.Value);
		  }
		  catch (System.NullReferenceException)
		  {
			fail("Local variable shouldn't be null.");
		  }
		}
	  }
	}

}