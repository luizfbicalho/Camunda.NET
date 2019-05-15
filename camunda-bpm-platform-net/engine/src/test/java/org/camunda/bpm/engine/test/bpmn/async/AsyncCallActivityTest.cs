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
namespace org.camunda.bpm.engine.test.bpmn.async
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AsyncCallActivityTest : PluggableProcessEngineTestCase
	{


	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/AsyncCallActivityTest.asyncStartEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/async/AsyncCallActivityTest.testCallSubProcess.bpmn20.xml" })]
	  public virtual void testCallProcessWithAsyncOnStartEvent()
	  {

		runtimeService.startProcessInstanceByKey("callAsyncSubProcess");

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		managementService.executeJob(job.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

	  }
	}

}