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
namespace org.camunda.bpm.engine.test.api.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.reset;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using SingleConsumerCondition = org.camunda.bpm.engine.impl.util.SingleConsumerCondition;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using Mock = org.mockito.Mock;
	using MockitoAnnotations = org.mockito.MockitoAnnotations;

	/// <summary>
	/// Tests the signalling of external task conditions
	/// </summary>
	public class ExternalTaskConditionsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock public org.camunda.bpm.engine.impl.util.SingleConsumerCondition condition;
	  public SingleConsumerCondition condition;

	  private string deploymentId;

	  private readonly BpmnModelInstance testProcess = Bpmn.createExecutableProcess("theProcess").startEvent().serviceTask("theTask").camundaExternalTask("theTopic").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {

		MockitoAnnotations.initMocks(this);

		ProcessEngineImpl.EXT_TASK_CONDITIONS.addConsumer(condition);

		deploymentId = rule.RepositoryService.createDeployment().addModelInstance("process.bpmn", testProcess).deploy().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {

		ProcessEngineImpl.EXT_TASK_CONDITIONS.removeConsumer(condition);

		if (!string.ReferenceEquals(deploymentId, null))
		{
		  rule.RepositoryService.deleteDeployment(deploymentId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSignalConditionOnTaskCreate()
	  public virtual void shouldSignalConditionOnTaskCreate()
	  {

		// when
		rule.RuntimeService.startProcessInstanceByKey("theProcess");

		// then
		verify(condition, times(1)).signal();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSignalConditionOnTaskCreateMultipleTimes()
	  public virtual void shouldSignalConditionOnTaskCreateMultipleTimes()
	  {

		// when
		rule.RuntimeService.startProcessInstanceByKey("theProcess");
		rule.RuntimeService.startProcessInstanceByKey("theProcess");

		// then
		verify(condition, times(2)).signal();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSignalConditionOnUnlock()
	  public virtual void shouldSignalConditionOnUnlock()
	  {

		// given

		rule.RuntimeService.startProcessInstanceByKey("theProcess");

		reset(condition); // clear signal for create

		LockedExternalTask lockedTask = rule.ExternalTaskService.fetchAndLock(1, "theWorker").topic("theTopic", 10000).execute()[0];

		// when
		rule.ExternalTaskService.unlock(lockedTask.Id);

		// then
		verify(condition, times(1)).signal();
	  }

	}

}