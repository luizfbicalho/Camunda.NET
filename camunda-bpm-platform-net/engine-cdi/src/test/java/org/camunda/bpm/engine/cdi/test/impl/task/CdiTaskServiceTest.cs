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
namespace org.camunda.bpm.engine.cdi.test.impl.task
{
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Test = org.junit.Test;


	public class CdiTaskServiceTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimTask()
	  public virtual void testClaimTask()
	  {
		Task newTask = taskService.newTask();
		taskService.saveTask(newTask);
		taskService.claim(newTask.Id, "kermit");
		taskService.deleteTask(newTask.Id,true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testTaskAssigneeExpression()
	  public virtual void testTaskAssigneeExpression()
	  {
		// given
		runtimeService.startProcessInstanceByKey("taskTest");
		identityService.AuthenticatedUserId = "user";

		// when
		taskService.createTaskQuery().taskAssigneeExpression("${currentUser()}").list();

		// then no exception is thrown
	  }

	}

}