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
namespace org.camunda.bpm.engine.cdi.test.api.annotation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	using CompleteTaskInterceptor = org.camunda.bpm.engine.cdi.impl.annotation.CompleteTaskInterceptor;
	using DeclarativeProcessController = org.camunda.bpm.engine.cdi.test.impl.beans.DeclarativeProcessController;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Test = org.junit.Test;

	/// <summary>
	/// Testcase for assuring that the <seealso cref="CompleteTaskInterceptor"/> works as
	/// expected
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class CompleteTaskTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/annotation/CompleteTaskTest.bpmn20.xml") public void testCompleteTask()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/annotation/CompleteTaskTest.bpmn20.xml")]
	  public virtual void testCompleteTask()
	  {

		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		businessProcess.startProcessByKey("keyOfTheProcess");

		Task task = taskService.createTaskQuery().singleResult();

		// associate current unit of work with the task:
		businessProcess.startTask(task.Id);

		getBeanInstance(typeof(DeclarativeProcessController)).completeTask();

		// assert that now the task is completed
		assertNull(taskService.createTaskQuery().singleResult());
	  }


	}

}