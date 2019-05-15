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
namespace org.camunda.bpm.engine.spring.test.taskassignment
{
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/taskassignment/taskassignment-context.xml") public class CustomTaskAssignmentTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class CustomTaskAssignmentTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetAssigneeThroughSpringService()
		public virtual void testSetAssigneeThroughSpringService()
		{
		runtimeService.startProcessInstanceByKey("assigneeThroughSpringService", CollectionUtil.singletonMap("emp", "fozzie"));
		assertEquals(1, taskService.createTaskQuery().taskAssignee("Kermit The Frog").count());
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetCandidateUsersThroughSpringService()
	  public virtual void testSetCandidateUsersThroughSpringService()
	  {
		runtimeService.startProcessInstanceByKey("candidateUsersThroughSpringService", CollectionUtil.singletonMap("emp", "fozzie"));
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("kermit").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("fozzie").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateUser("gonzo").count());
		assertEquals(0, taskService.createTaskQuery().taskCandidateUser("mispiggy").count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetCandidateGroupsThroughSpringService()
	  public virtual void testSetCandidateGroupsThroughSpringService()
	  {
		runtimeService.startProcessInstanceByKey("candidateUsersThroughSpringService", CollectionUtil.singletonMap("emp", "fozzie"));
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("management").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("directors").count());
		assertEquals(1, taskService.createTaskQuery().taskCandidateGroup("accountancy").count());
		assertEquals(0, taskService.createTaskQuery().taskCandidateGroup("sales").count());
	  }

	}

}