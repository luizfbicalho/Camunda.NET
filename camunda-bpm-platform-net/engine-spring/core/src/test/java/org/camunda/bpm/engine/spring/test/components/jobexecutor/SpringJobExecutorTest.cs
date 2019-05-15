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
namespace org.camunda.bpm.engine.spring.test.components.jobexecutor
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// @author Pablo Ganga
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/components/SpringjobExecutorTest-context.xml") public class SpringJobExecutorTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class SpringJobExecutorTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/spring/test/components/SpringTimersProcess.bpmn20.xml", "org/camunda/bpm/engine/spring/test/components/SpringJobExecutorRollBack.bpmn20.xml"}) public void testHappyJobExecutorPath()throws Exception
		[Deployment(resources:{"org/camunda/bpm/engine/spring/test/components/SpringTimersProcess.bpmn20.xml", "org/camunda/bpm/engine/spring/test/components/SpringJobExecutorRollBack.bpmn20.xml"})]
		public virtual void testHappyJobExecutorPath()
		{

			ProcessInstance instance = runtimeService.startProcessInstanceByKey("process1");

			assertNotNull(instance);

			waitForJobExecutorToProcessAllJobs(10000);

			IList<Task> activeTasks = taskService.createTaskQuery().processInstanceId(instance.Id).list();
			assertTrue(activeTasks.Count == 0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/spring/test/components/SpringTimersProcess.bpmn20.xml", "org/camunda/bpm/engine/spring/test/components/SpringJobExecutorRollBack.bpmn20.xml"}) public void testRollbackJobExecutorPath()throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/spring/test/components/SpringTimersProcess.bpmn20.xml", "org/camunda/bpm/engine/spring/test/components/SpringJobExecutorRollBack.bpmn20.xml"})]
	  public virtual void testRollbackJobExecutorPath()
	  {

		// shutdown job executor first, otherwise waitForJobExecutorToProcessAllJobs will not actually start it....
		processEngineConfiguration.JobExecutor.shutdown();

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("errorProcess1");

		assertNotNull(instance);

		waitForJobExecutorToProcessAllJobs(10000);

		IList<Task> activeTasks = taskService.createTaskQuery().processInstanceId(instance.Id).list();
		assertTrue(activeTasks.Count == 1);
	  }


	}

}