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
namespace org.camunda.bpm.engine.test.api.task
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Falko Menge
	/// </summary>
	public class DelegateTaskTest : PluggableProcessEngineTestCase
	{

	  /// <seealso cref= http://jira.codehaus.org/browse/ACT-380 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetCandidates()
	  public virtual void testGetCandidates()
	  {
		runtimeService.startProcessInstanceByKey("DelegateTaskTest.testGetCandidates");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Set<String> candidateUsers = (java.util.Set<String>) taskService.getVariable(task.getId(), DelegateTaskTestTaskListener.VARNAME_CANDIDATE_USERS);
		ISet<string> candidateUsers = (ISet<string>) taskService.getVariable(task.Id, DelegateTaskTestTaskListener.VARNAME_CANDIDATE_USERS);
		assertEquals(2, candidateUsers.Count);
		assertTrue(candidateUsers.Contains("kermit"));
		assertTrue(candidateUsers.Contains("gonzo"));

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Set<String> candidateGroups = (java.util.Set<String>) taskService.getVariable(task.getId(), DelegateTaskTestTaskListener.VARNAME_CANDIDATE_GROUPS);
		ISet<string> candidateGroups = (ISet<string>) taskService.getVariable(task.Id, DelegateTaskTestTaskListener.VARNAME_CANDIDATE_GROUPS);
		assertEquals(2, candidateGroups.Count);
		assertTrue(candidateGroups.Contains("management"));
		assertTrue(candidateGroups.Contains("accountancy"));
	  }

	}

}