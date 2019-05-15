using System.Threading;

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
namespace org.camunda.bpm.engine.test.bpmn.@event.end
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class EndEventTest : PluggableProcessEngineTestCase
	{

	  // Test case for ACT-1259
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentEndOfSameProcess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testConcurrentEndOfSameProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskWithDelay");
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// We will now start two threads that both complete the task.
		// In the process, the task is followed by a delay of three seconds
		// This will cause both threads to call the taskService.complete method with enough time,
		// before ending the process. Both threads will now try to end the process
		// and only one should succeed (due to optimistic locking).
		TaskCompleter taskCompleter1 = new TaskCompleter(this, task.Id);
		TaskCompleter taskCompleter2 = new TaskCompleter(this, task.Id);

		assertFalse(taskCompleter1.Succeeded);
		assertFalse(taskCompleter2.Succeeded);

		taskCompleter1.Start();
		taskCompleter2.Start();
		taskCompleter1.Join();
		taskCompleter2.Join();

		int successCount = 0;
		if (taskCompleter1.Succeeded)
		{
		  successCount++;
		}
		if (taskCompleter2.Succeeded)
		{
		  successCount++;
		}

		assertEquals("(Only) one thread should have been able to successfully end the process", 1, successCount);
		assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// Helper class for concurrent testing </summary>
	  internal class TaskCompleter : Thread
	  {
		  private readonly EndEventTest outerInstance;


		protected internal string taskId;
		protected internal bool succeeded;

		public TaskCompleter(EndEventTest outerInstance, string taskId)
		{
			this.outerInstance = outerInstance;
		  this.taskId = taskId;
		}

		public virtual bool Succeeded
		{
			get
			{
			  return succeeded;
			}
		}

		public virtual void run()
		{
		  try
		  {
			outerInstance.taskService.complete(taskId);
			succeeded = true;
		  }
		  catch (OptimisticLockingException)
		  {
			// Exception is expected for one of the threads
		  }
		}
	  }

	}

}