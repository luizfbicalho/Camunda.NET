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
namespace org.camunda.bpm.engine.test.bpmn.parse
{
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.parser.DefaultFailedJobParseListener.FAILED_JOB_CONFIGURATION;

	public class FoxFailedJobParseListenerTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testUserTask.bpmn20.xml" })]
	  public virtual void testUserTaskParseFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("asyncUserTaskFailedJobRetryTimeCycle");

		ActivityImpl userTask = findActivity(pi, "task");
		checkFoxFailedJobConfig(userTask);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/CamundaFailedJobParseListenerTest.testUserTask.bpmn20.xml" })]
	  public virtual void testUserTaskParseFailedJobRetryTimeCycleInActivitiNamespace()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("asyncUserTaskFailedJobRetryTimeCycle");

		ActivityImpl userTask = findActivity(pi, "task");
		checkFoxFailedJobConfig(userTask);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testUserTask.bpmn20.xml" })]
	  public virtual void testNotAsyncUserTaskParseFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("notAsyncUserTaskFailedJobRetryTimeCycle");

		ActivityImpl userTask = findActivity(pi, "notAsyncTask");
		checkNotContainingFoxFailedJobConfig(userTask);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testUserTask.bpmn20.xml" })]
	  public virtual void testAsyncUserTaskButWithoutParseFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("asyncUserTaskButWithoutFailedJobRetryTimeCycle");

		ActivityImpl userTask = findActivity(pi, "asyncTaskWithoutFailedJobRetryTimeCycle");
		checkNotContainingFoxFailedJobConfig(userTask);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testTimer.bpmn20.xml" })]
	  public virtual void testTimerBoundaryEventWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("boundaryEventWithFailedJobRetryTimeCycle");

		ActivityImpl boundaryActivity = findActivity(pi, "boundaryTimerWithFailedJobRetryTimeCycle");
		checkFoxFailedJobConfig(boundaryActivity);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testTimer.bpmn20.xml" })]
	  public virtual void testTimerBoundaryEventWithoutFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("boundaryEventWithoutFailedJobRetryTimeCycle");

		ActivityImpl boundaryActivity = findActivity(pi, "boundaryTimerWithoutFailedJobRetryTimeCycle");
		checkNotContainingFoxFailedJobConfig(boundaryActivity);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testTimer.bpmn20.xml" })]
	  public virtual void testTimerStartEventWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("startEventWithFailedJobRetryTimeCycle");

		ActivityImpl startEvent = findActivity(pi, "startEventFailedJobRetryTimeCycle");
		checkFoxFailedJobConfig(startEvent);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testTimer.bpmn20.xml" })]
	  public virtual void testIntermediateCatchTimerEventWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("intermediateTimerEventWithFailedJobRetryTimeCycle");

		ActivityImpl timer = findActivity(pi, "timerEventWithFailedJobRetryTimeCycle");
		checkFoxFailedJobConfig(timer);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/parse/FoxFailedJobParseListenerTest.testSignal.bpmn20.xml" })]
	  public virtual void testSignalEventWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("signalEventWithFailedJobRetryTimeCycle");

		ActivityImpl signal = findActivity(pi, "signalWithFailedJobRetryTimeCycle");
		checkFoxFailedJobConfig(signal);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultiInstanceBodyWithFailedJobRetryTimeCycle()
	  public virtual void testMultiInstanceBodyWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		ActivityImpl miBody = findMultiInstanceBody(pi, "task");
		checkFoxFailedJobConfig(miBody);

		ActivityImpl innerActivity = findActivity(pi, "task");
		checkNotContainingFoxFailedJobConfig(innerActivity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInnerMultiInstanceActivityWithFailedJobRetryTimeCycle()
	  public virtual void testInnerMultiInstanceActivityWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		ActivityImpl miBody = findMultiInstanceBody(pi, "task");
		checkNotContainingFoxFailedJobConfig(miBody);

		ActivityImpl innerActivity = findActivity(pi, "task");
		checkFoxFailedJobConfig(innerActivity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultiInstanceBodyAndInnerActivityWithFailedJobRetryTimeCycle()
	  public virtual void testMultiInstanceBodyAndInnerActivityWithFailedJobRetryTimeCycle()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process");

		ActivityImpl miBody = findMultiInstanceBody(pi, "task");
		checkFoxFailedJobConfig(miBody);

		ActivityImpl innerActivity = findActivity(pi, "task");
		checkFoxFailedJobConfig(innerActivity);
	  }

	  protected internal virtual ActivityImpl findActivity(ProcessInstance pi, string activityId)
	  {

		ProcessInstanceWithVariablesImpl entity = (ProcessInstanceWithVariablesImpl) pi;
		ProcessDefinitionEntity processDefEntity = entity.ExecutionEntity.ProcessDefinition;

		assertNotNull(processDefEntity);
		ActivityImpl activity = processDefEntity.findActivity(activityId);
		assertNotNull(activity);
		return activity;
	  }

	  protected internal virtual ActivityImpl findMultiInstanceBody(ProcessInstance pi, string activityId)
	  {
		return findActivity(pi, activityId + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX);
	  }

	  protected internal virtual void checkFoxFailedJobConfig(ActivityImpl activity)
	  {
		assertNotNull(activity);

		assertTrue(activity.Properties.contains(FAILED_JOB_CONFIGURATION));

		object value = activity.Properties.get(FAILED_JOB_CONFIGURATION).RetryIntervals.get(0);
		assertEquals("R5/PT5M", value);
	  }

	  protected internal virtual void checkNotContainingFoxFailedJobConfig(ActivityImpl activity)
	  {
		assertFalse(activity.Properties.contains(FAILED_JOB_CONFIGURATION));
	  }

	}

}