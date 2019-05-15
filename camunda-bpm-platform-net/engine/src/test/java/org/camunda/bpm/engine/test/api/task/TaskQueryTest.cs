using System;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByAssignee;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByCaseExecutionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByCaseInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByCreateTime;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByDescription;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByDueDate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByExecutionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByFollowUpDate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByName;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByPriority;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.taskByProcessInstanceId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySortingAndCount;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// </summary>
	public class TaskQueryTest : PluggableProcessEngineTestCase
	{

	  private IList<string> taskIds;

	  // The range of Oracle's NUMBER field is limited to ~10e+125
	  // which is below Double.MAX_VALUE, so we only test with the following
	  // max value
	  protected internal const double MAX_DOUBLE_VALUE = 10E+124;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {

		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("gonzo"));
		identityService.saveUser(identityService.newUser("fozzie"));

		identityService.saveGroup(identityService.newGroup("management"));
		identityService.saveGroup(identityService.newGroup("accountancy"));

		identityService.createMembership("kermit", "management");
		identityService.createMembership("kermit", "accountancy");
		identityService.createMembership("fozzie", "management");

		taskIds = generateTestTasks();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		identityService.deleteGroup("accountancy");
		identityService.deleteGroup("management");
		identityService.deleteUser("fozzie");
		identityService.deleteUser("gonzo");
		identityService.deleteUser("kermit");
		taskService.deleteTasks(taskIds, true);
	  }

	  public virtual void tesBasicTaskPropertiesNotNull()
	  {
		Task task = taskService.createTaskQuery().taskId(taskIds[0]).singleResult();
		assertNotNull(task.Description);
		assertNotNull(task.Id);
		assertNotNull(task.Name);
		assertNotNull(task.CreateTime);
	  }

	  public virtual void testQueryNoCriteria()
	  {
		TaskQuery query = taskService.createTaskQuery();
		assertEquals(12, query.count());
		assertEquals(12, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByTaskId()
	  {
		TaskQuery query = taskService.createTaskQuery().taskId(taskIds[0]);
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidTaskId()
	  {
		TaskQuery query = taskService.createTaskQuery().taskId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  taskService.createTaskQuery().taskId(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByName()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("testTask");
		assertEquals(6, query.list().size());
		assertEquals(6, query.count());

		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByInvalidName()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  taskService.createTaskQuery().taskName(null).singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByNameLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskNameLike("gonzo\\_%");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidNameLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("1");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  taskService.createTaskQuery().taskName(null).singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByDescription()
	  {
		TaskQuery query = taskService.createTaskQuery().taskDescription("testTask description");
		assertEquals(6, query.list().size());
		assertEquals(6, query.count());

		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByInvalidDescription()
	  {
		TaskQuery query = taskService.createTaskQuery().taskDescription("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  taskService.createTaskQuery().taskDescription(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }


	  /// <summary>
	  /// CAM-6363
	  /// 
	  /// Verify that search by name returns case insensitive results
	  /// </summary>
	  public virtual void testTaskQueryLookupByNameCaseInsensitive()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskName("testTask");


		IList<Task> tasks = query.list();
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(6));

		query = taskService.createTaskQuery();
		query.taskName("TeStTaSk");

		tasks = query.list();
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(6));
	  }

	  /// <summary>
	  /// CAM-6165
	  /// 
	  /// Verify that search by name like returns case insensitive results
	  /// </summary>
	  public virtual void testTaskQueryLookupByNameLikeCaseInsensitive()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskNameLike("%task%");


		IList<Task> tasks = query.list();
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(10));

		query = taskService.createTaskQuery();
		query.taskNameLike("%Task%");

		tasks = query.list();
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(10));
	  }

	  public virtual void testQueryByDescriptionLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskDescriptionLike("%gonzo\\_%");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidDescriptionLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskDescriptionLike("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  taskService.createTaskQuery().taskDescriptionLike(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{

		}
	  }

	  public virtual void testQueryByPriority()
	  {
		TaskQuery query = taskService.createTaskQuery().taskPriority(10);
		assertEquals(2, query.list().size());
		assertEquals(2, query.count());

		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

		query = taskService.createTaskQuery().taskPriority(100);
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		query = taskService.createTaskQuery().taskMinPriority(50);
		assertEquals(3, query.list().size());

		query = taskService.createTaskQuery().taskMinPriority(10);
		assertEquals(5, query.list().size());

		query = taskService.createTaskQuery().taskMaxPriority(10);
		assertEquals(9, query.list().size());

		query = taskService.createTaskQuery().taskMaxPriority(3);
		assertEquals(6, query.list().size());

		query = taskService.createTaskQuery().taskMinPriority(50).taskMaxPriority(10);
		assertEquals(0, query.list().size());

		query = taskService.createTaskQuery().taskPriority(30).taskMaxPriority(10);
		assertEquals(0, query.list().size());

		query = taskService.createTaskQuery().taskMinPriority(30).taskPriority(10);
		assertEquals(0, query.list().size());

		query = taskService.createTaskQuery().taskMinPriority(30).taskPriority(20).taskMaxPriority(10);
		assertEquals(0, query.list().size());
	  }

	  public virtual void testQueryByInvalidPriority()
	  {
		try
		{
		  taskService.createTaskQuery().taskPriority(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByAssignee()
	  {
		TaskQuery query = taskService.createTaskQuery().taskAssignee("gonzo_");
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		query = taskService.createTaskQuery().taskAssignee("kermit");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
	  }

	  public virtual void testQueryByAssigneeLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskAssigneeLike("gonz%\\_");
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		assertNotNull(query.singleResult());

		query = taskService.createTaskQuery().taskAssignee("gonz");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		assertNull(query.singleResult());
	  }

	  public virtual void testQueryByNullAssignee()
	  {
		try
		{
		  taskService.createTaskQuery().taskAssignee(null).list();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByUnassigned()
	  {
		TaskQuery query = taskService.createTaskQuery().taskUnassigned();
		assertEquals(10, query.count());
		assertEquals(10, query.list().size());
	  }

	  public virtual void testQueryByAssigned()
	  {
		TaskQuery query = taskService.createTaskQuery().taskAssigned();
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
	  }

	  public virtual void testQueryByCandidateUser()
	  {
		// kermit is candidate for 12 tasks, two of them are already assigned
		TaskQuery query = taskService.createTaskQuery().taskCandidateUser("kermit");
		assertEquals(10, query.count());
		assertEquals(10, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateUser("kermit").includeAssignedTasks();
		assertEquals(12, query.count());
		assertEquals(12, query.list().size());

		// fozzie is candidate for one task and her groups are candidate for 2 tasks, one of them is already assigned
		query = taskService.createTaskQuery().taskCandidateUser("fozzie");
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateUser("fozzie").includeAssignedTasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		// gonzo is candidate for one task, which is already assinged
		query = taskService.createTaskQuery().taskCandidateUser("gonzo");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateUser("gonzo").includeAssignedTasks();
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
	  }

	  public virtual void testQueryByNullCandidateUser()
	  {
		try
		{
		  taskService.createTaskQuery().taskCandidateUser(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByIncludeAssignedTasksWithMissingCandidateUserOrGroup()
	  {
		try
		{
		  taskService.createTaskQuery().includeAssignedTasks();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByCandidateGroup()
	  {
		// management group is candidate for 3 tasks, one of them is already assigned
		TaskQuery query = taskService.createTaskQuery().taskCandidateGroup("management");
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroup("management").includeAssignedTasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());


		// accountancy group is candidate for 3 tasks, one of them is already assigned
		query = taskService.createTaskQuery().taskCandidateGroup("accountancy");
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroup("accountancy").includeAssignedTasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		// sales group is candidate for no tasks
		query = taskService.createTaskQuery().taskCandidateGroup("sales");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroup("sales").includeAssignedTasks();
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  public virtual void testQueryWithCandidateGroups()
	  {
		// test withCandidateGroups
		TaskQuery query = taskService.createTaskQuery().withCandidateGroups();
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());

		assertEquals(5, query.includeAssignedTasks().count());
		assertEquals(5, query.includeAssignedTasks().list().size());
	  }

	  public virtual void testQueryWithoutCandidateGroups()
	  {
		// test withoutCandidateGroups
		TaskQuery query = taskService.createTaskQuery().withoutCandidateGroups();
		assertEquals(6, query.count());
		assertEquals(6, query.list().size());

		assertEquals(7, query.includeAssignedTasks().count());
		assertEquals(7, query.includeAssignedTasks().list().size());
	  }

	  public virtual void testQueryByNullCandidateGroup()
	  {
		try
		{
		  taskService.createTaskQuery().taskCandidateGroup(null).list();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByCandidateGroupIn()
	  {
		IList<string> groups = Arrays.asList("management", "accountancy");
		TaskQuery query = taskService.createTaskQuery().taskCandidateGroupIn(groups);
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).includeAssignedTasks();
		assertEquals(5, query.count());
		assertEquals(5, query.list().size());

		// Unexisting groups or groups that don't have candidate tasks shouldn't influence other results
		groups = Arrays.asList("management", "accountancy", "sales", "unexising");
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups);
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).includeAssignedTasks();
		assertEquals(5, query.count());
		assertEquals(5, query.list().size());
	  }

	  public virtual void testQueryByCandidateGroupInAndCandidateGroup()
	  {
		IList<string> groups = Arrays.asList("management", "accountancy");
		string candidateGroup = "management";
		TaskQuery query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup(candidateGroup);
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup(candidateGroup).includeAssignedTasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		// Unexisting groups or groups that don't have candidate tasks shouldn't influence other results
		groups = Arrays.asList("management", "accountancy", "sales", "unexising");
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup(candidateGroup);
		assertEquals(2, query.count());
		assertEquals(2, query.list().size());
		try
		{
		  query.singleResult();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup(candidateGroup).includeAssignedTasks();
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		// sales group is candidate for no tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup("sales");
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		// test including assigned tasks
		query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup("sales").includeAssignedTasks();
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  public virtual void testQueryByCandidateGroupInAndCandidateGroupNotIntersected()
	  {
		IList<string> groups = Arrays.asList("accountancy");
		string candidateGroup = "management";
		TaskQuery query = taskService.createTaskQuery().taskCandidateGroupIn(groups).taskCandidateGroup(candidateGroup);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  public virtual void testQueryByNullCandidateGroupIn()
	  {
		try
		{
		  taskService.createTaskQuery().taskCandidateGroupIn(null).list();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
		try
		{
		  taskService.createTaskQuery().taskCandidateGroupIn(new List<string>()).list();
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  public virtual void testQueryByDelegationState()
	  {
		TaskQuery query = taskService.createTaskQuery().taskDelegationState(null);
		assertEquals(12, query.count());
		assertEquals(12, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.PENDING);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		string taskId = taskService.createTaskQuery().taskAssignee("gonzo_").singleResult().Id;
		taskService.delegateTask(taskId, "kermit");

		query = taskService.createTaskQuery().taskDelegationState(null);
		assertEquals(11, query.count());
		assertEquals(11, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.PENDING);
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		taskService.resolveTask(taskId);

		query = taskService.createTaskQuery().taskDelegationState(null);
		assertEquals(11, query.count());
		assertEquals(11, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.PENDING);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
		query = taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED);
		assertEquals(1, query.count());
		assertEquals(1, query.list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryCreatedOn() throws Exception
	  public virtual void testQueryCreatedOn()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// Exact matching of createTime, should result in 6 tasks
		DateTime createTime = sdf.parse("01/01/2001 01:01:01.000");

		TaskQuery query = taskService.createTaskQuery().taskCreatedOn(createTime);
		assertEquals(6, query.count());
		assertEquals(6, query.list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryCreatedBefore() throws Exception
	  public virtual void testQueryCreatedBefore()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// Should result in 7 tasks
		DateTime before = sdf.parse("03/02/2002 02:02:02.000");

		TaskQuery query = taskService.createTaskQuery().taskCreatedBefore(before);
		assertEquals(7, query.count());
		assertEquals(7, query.list().size());

		before = sdf.parse("01/01/2001 01:01:01.000");
		query = taskService.createTaskQuery().taskCreatedBefore(before);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testQueryCreatedAfter() throws Exception
	  public virtual void testQueryCreatedAfter()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// Should result in 3 tasks
		DateTime after = sdf.parse("03/03/2003 03:03:03.000");

		TaskQuery query = taskService.createTaskQuery().taskCreatedAfter(after);
		assertEquals(3, query.count());
		assertEquals(3, query.list().size());

		after = sdf.parse("05/05/2005 05:05:05.000");
		query = taskService.createTaskQuery().taskCreatedAfter(after);
		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testCreateTimeCombinations() throws java.text.ParseException
	  public virtual void testCreateTimeCombinations()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

		// Exact matching of createTime, should result in 6 tasks
		DateTime createTime = sdf.parse("01/01/2001 01:01:01.000");

		DateTime oneHourAgo = new DateTime(createTime.Ticks - 60 * 60 * 1000);
		DateTime oneHourLater = new DateTime(createTime.Ticks + 60 * 60 * 1000);

		assertEquals(6, taskService.createTaskQuery().taskCreatedAfter(oneHourAgo).taskCreatedOn(createTime).taskCreatedBefore(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().taskCreatedAfter(oneHourLater).taskCreatedOn(createTime).taskCreatedBefore(oneHourAgo).count());
		assertEquals(0, taskService.createTaskQuery().taskCreatedAfter(oneHourLater).taskCreatedOn(createTime).count());
		assertEquals(0, taskService.createTaskQuery().taskCreatedOn(createTime).taskCreatedBefore(oneHourAgo).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml") public void testTaskDefinitionKey() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml")]
	  public virtual void testTaskDefinitionKey()
	  {

		// Start process instance, 2 tasks will be available
		runtimeService.startProcessInstanceByKey("taskDefinitionKeyProcess");

		// 1 task should exist with key "taskKey_1"
		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKey("taskKey_1").list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("taskKey_1", tasks[0].TaskDefinitionKey);

		// No task should be found with unexisting key
		long? count = taskService.createTaskQuery().taskDefinitionKey("unexistingKey").count();
		assertEquals(0L, count.Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml") public void testTaskDefinitionKeyLike() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml")]
	  public virtual void testTaskDefinitionKeyLike()
	  {

		// Start process instance, 2 tasks will be available
		runtimeService.startProcessInstanceByKey("taskDefinitionKeyProcess");

		// Ends with matching, TaskKey_1 and TaskKey_123 match
		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKeyLike("taskKey\\_1%").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);

		assertEquals("taskKey_1", tasks[0].TaskDefinitionKey);
		assertEquals("taskKey_123", tasks[1].TaskDefinitionKey);

		// Starts with matching, TaskKey_123 matches
		tasks = taskService.createTaskQuery().taskDefinitionKeyLike("%\\_123").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("taskKey_123", tasks[0].TaskDefinitionKey);

		// Contains matching, TaskKey_123 matches
		tasks = taskService.createTaskQuery().taskDefinitionKeyLike("%Key\\_12%").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("taskKey_123", tasks[0].TaskDefinitionKey);


		// No task should be found with unexisting key
		long? count = taskService.createTaskQuery().taskDefinitionKeyLike("%unexistingKey%").count();
		assertEquals(0L, count.Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml") public void testTaskDefinitionKeyIn() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml")]
	  public virtual void testTaskDefinitionKeyIn()
	  {

		// Start process instance, 2 tasks will be available
		runtimeService.startProcessInstanceByKey("taskDefinitionKeyProcess");

		// 1 Task should be found with TaskKey1
		IList<Task> tasks = taskService.createTaskQuery().taskDefinitionKeyIn("taskKey_1").list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("taskKey_1", tasks[0].TaskDefinitionKey);

		// 2 Tasks should be found with TaskKey_1 and TaskKey_123
		tasks = taskService.createTaskQuery().taskDefinitionKeyIn("taskKey_1", "taskKey_123").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);

		assertEquals("taskKey_1", tasks[0].TaskDefinitionKey);
		assertEquals("taskKey_123", tasks[1].TaskDefinitionKey);

		// 2 Tasks should be found with TaskKey1, TaskKey123 and UnexistingKey
		tasks = taskService.createTaskQuery().taskDefinitionKeyIn("taskKey_1", "taskKey_123", "unexistingKey").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);

		assertEquals("taskKey_1", tasks[0].TaskDefinitionKey);
		assertEquals("taskKey_123", tasks[1].TaskDefinitionKey);

		// No task should be found with UnexistingKey
		long? count = taskService.createTaskQuery().taskDefinitionKeyIn("unexistingKey").count();
		assertEquals(0L, count.Value);

		count = taskService.createTaskQuery().taskDefinitionKey("unexistingKey").taskDefinitionKeyIn("taskKey1").count();
		assertEquals(0l, count.Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testTaskVariableNameEqualsIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testTaskVariableNameEqualsIgnoreCase()
	  {
		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, variableName, variableValue);

		// query for case-insensitive variable name should only return a result if case-insensitive search is used
		assertEquals(1, taskService.createTaskQuery().matchVariableNamesIgnoreCase().taskVariableValueEquals(variableName.ToLower(), variableValue).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals(variableName.ToLower(), variableValue).count());

		// query should treat all variables case-insensitively, even when flag is set after variable
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals(variableName.ToLower(), variableValue).matchVariableNamesIgnoreCase().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskVariableValueEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTaskVariableValueEquals()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// No task should be found for an unexisting var
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("unexistingVar", "value").count());

		// Create a map with a variable for all default types
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 928374L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		taskService.setVariablesLocal(task.Id, variables);

		// Test query matches
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("longVar", 928374L).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("integerVar", 1234).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("stringVar", "stringValue").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("booleanVar", true).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("dateVar", date).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("nullVar", null).count());

		// Test query for other values on existing variables
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("longVar", 999L).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("shortVar", (short) 999).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("integerVar", 999).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("stringVar", "999").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("booleanVar", false).count());
		DateTime otherDate = new DateTime();
		otherDate.AddYears(1);
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("dateVar", otherDate).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("nullVar", "999").count());

		// Test query for not equals
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals("longVar", 999L).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals("shortVar", (short) 999).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals("integerVar", 999).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals("stringVar", "999").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals("booleanVar", false).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml") public void testTaskVariableValueEqualsIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml")]
	  public virtual void testTaskVariableValueEqualsIgnoreCase()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";

		taskService.setVariableLocal(task.Id, variableName, variableValue);

		// query for existing variable should return one result
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals(variableName, variableValue).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName, variableValue.ToLower()).count());

		// query for non existing variable should return zero results
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("nonExistentVariable", variableValue.ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueEquals("nonExistentVariable", variableValue.ToLower()).count());

		// query for existing variable with different value should return zero results
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals(variableName, "nonExistentValue").count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName, "nonExistentValue".ToLower()).count());

		// query for case-insensitive variable value should only return a result when case-insensitive search is used
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals(variableName, variableValue.ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName, variableValue.ToLower()).count());

		// query for case-insensitive variable with not equals operator should only return a result when case-sensitive search is used
		assertEquals(1, taskService.createTaskQuery().taskVariableValueNotEquals(variableName, variableValue.ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueNotEquals(variableName, variableValue.ToLower()).count());

		// query should treat all variables case-insensitively, even when flag is set after variable
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals(variableName, variableValue.ToLower()).matchVariableValuesIgnoreCase().count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testTaskVariableValueNameEqualsIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testTaskVariableValueNameEqualsIgnoreCase()
	  {
		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, variableName, variableValue);

		// query for case-insensitive variable name should only return a result if case-insensitive search is used
		assertEquals(1, taskService.createTaskQuery().matchVariableNamesIgnoreCase().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName.ToLower(), variableValue.ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals(variableName.ToLower(), variableValue).count());

		// query should treat all variables case-insensitively, even when flag is set after variable
		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals(variableName.ToLower(), variableValue).matchVariableNamesIgnoreCase().count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml") public void testTaskVariableValueLike() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml")]
	  public virtual void testTaskVariableValueLike()
	  {

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  variables["stringVar"] = "stringValue";

		  taskService.setVariablesLocal(task.Id, variables);

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLike("stringVar", "stringVal%").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueLike("stringVar", "%ngValue").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueLike("stringVar", "%ngVal%").count());

		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("stringVar", "stringVar%").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("stringVar", "%ngVar").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("stringVar", "%ngVar%").count());

		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("stringVar", "stringVal").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("nonExistingVar", "string%").count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().taskVariableValueLike("stringVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml") public void testTaskVariableValueLikeIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml")]
	  public virtual void testTaskVariableValueLikeIgnoreCase()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "stringValue";

		taskService.setVariablesLocal(task.Id, variables);

		assertEquals(0, taskService.createTaskQuery().taskVariableValueLike("stringVar", "stringVal%".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "stringVal%".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "%ngValue".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "%ngVal%".ToLower()).count());

		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "stringVar%".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "%ngVar".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "%ngVar%".ToLower()).count());

		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("stringVar", "stringVal".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike("nonExistingVar", "stringVal%".ToLower()).count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().taskVariableValueLike("stringVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml") public void testTaskVariableValueCompare() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml")]
	  public virtual void testTaskVariableValueCompare()
	  {

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		  Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  variables["numericVar"] = 928374;
		  DateTime date = (new GregorianCalendar(2014, 2, 2, 2, 2, 2)).Time;
		  variables["dateVar"] = date;
		  variables["stringVar"] = "ab";
		  variables["nullVar"] = null;

		  taskService.setVariablesLocal(task.Id, variables);

		// test compare methods with numeric values
		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThan("numericVar", 928373).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("numericVar", 928375).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("numericVar", 928373).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("numericVar", 928375).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThan("numericVar", 928375).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("numericVar", 928373).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("numericVar", 928375).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("numericVar", 928373).count());

		// test compare methods with date values
		DateTime before = (new GregorianCalendar(2014, 2, 2, 2, 2, 1)).Time;
		DateTime after = (new GregorianCalendar(2014, 2, 2, 2, 2, 3)).Time;

		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThan("dateVar", before).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("dateVar", after).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("dateVar", before).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("dateVar", after).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThan("dateVar", after).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("dateVar", before).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("dateVar", after).count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("dateVar", before).count());

		//test with string values
		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThan("stringVar", "aa").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThan("stringVar", "ba").count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("stringVar", "aa").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("stringVar", "ba").count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThan("stringVar", "ba").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThan("stringVar", "aa").count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("stringVar", "ba").count());
		assertEquals(1, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("stringVar", "aa").count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().taskVariableValueGreaterThan("nullVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueLessThan("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueLessThanOrEquals("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}

		// test with boolean value
		try
		{
		  taskService.createTaskQuery().taskVariableValueGreaterThan("nullVar", true).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueGreaterThanOrEquals("nullVar", false).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueLessThan("nullVar", true).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().taskVariableValueLessThanOrEquals("nullVar", false).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}

	 // test non existing variable
		assertEquals(0, taskService.createTaskQuery().taskVariableValueLessThanOrEquals("nonExisting", 123).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessVariableValueEquals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessVariableValueEquals()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["longVar"] = 928374L;
		variables["shortVar"] = (short) 123;
		variables["integerVar"] = 1234;
		variables["stringVar"] = "stringValue";
		variables["booleanVar"] = true;
		DateTime date = new DateTime();
		variables["dateVar"] = date;
		variables["nullVar"] = null;

		// Start process-instance with all types of variables
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// Test query matches
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("longVar", 928374L).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("integerVar", 1234).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("stringVar", "stringValue").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("booleanVar", true).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("dateVar", date).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("nullVar", null).count());

		// Test query for other values on existing variables
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("longVar", 999L).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("shortVar", (short) 999).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("integerVar", 999).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("stringVar", "999").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("booleanVar", false).count());
		DateTime otherDate = new DateTime();
		otherDate.AddYears(1);
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("dateVar", otherDate).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("nullVar", "999").count());

		// Test querying for task variables don't match the process-variables
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("longVar", 928374L).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("shortVar", (short) 123).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("integerVar", 1234).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("stringVar", "stringValue").count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("booleanVar", true).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().taskVariableValueEquals("nullVar", null).count());

		// Test querying for task variables not equals
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals("longVar", 999L).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals("shortVar", (short) 999).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals("integerVar", 999).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals("stringVar", "999").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals("booleanVar", false).count());

		// and query for the existing variable with NOT should result in nothing found:
		assertEquals(0, taskService.createTaskQuery().processVariableValueNotEquals("longVar", 928374L).count());

		// Test combination of task-variable and process-variable
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		taskService.setVariableLocal(task.Id, "taskVar", "theValue");
		taskService.setVariableLocal(task.Id, "longVar", 928374L);

		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("longVar", 928374L).taskVariableValueEquals("taskVar", "theValue").count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("longVar", 928374L).taskVariableValueEquals("longVar", 928374L).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableNameEqualsIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableNameEqualsIgnoreCase()
	  {
		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = variableValue;

		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// query for case-insensitive variable name should only return a result if case-insensitive search is used
		assertEquals(1, taskService.createTaskQuery().matchVariableNamesIgnoreCase().processVariableValueEquals(variableName.ToLower(), variableValue).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals(variableName.ToLower(), variableValue).count());

		// query should treat all variables case-insensitively, even when flag is set after variable
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals(variableName.ToLower(), variableValue).matchVariableNamesIgnoreCase().count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml") public void testProcessVariableValueEqualsIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testTaskVariableValueEquals.bpmn20.xml")]
	  public virtual void testProcessVariableValueEqualsIgnoreCase()
	  {
		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = variableValue;

		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// query for existing variable should return one result
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals(variableName, variableValue).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueEquals(variableName, variableValue.ToLower()).count());

		// query for non existing variable should return zero results
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("nonExistentVariable", variableValue.ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueEquals("nonExistentVariable", variableValue.ToLower()).count());

		// query for existing variable with different value should return zero results
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals(variableName, "nonExistentValue").count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueEquals(variableName, "nonExistentValue".ToLower()).count());

		// query for case-insensitive variable value should only return a result when case-insensitive search is used
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals(variableName, variableValue.ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueEquals(variableName, variableValue.ToLower()).count());

		// query for case-insensitive variable with not equals operator should only return a result when case-sensitive search is used
		assertEquals(1, taskService.createTaskQuery().processVariableValueNotEquals(variableName, variableValue.ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueNotEquals(variableName, variableValue.ToLower()).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml") public void testProcessVariableValueLike() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml")]
	  public virtual void testProcessVariableValueLike()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "stringValue";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		assertEquals(1, taskService.createTaskQuery().processVariableValueLike("stringVar", "stringVal%").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueLike("stringVar", "%ngValue").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueLike("stringVar", "%ngVal%").count());

		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("stringVar", "stringVar%").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("stringVar", "%ngVar").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("stringVar", "%ngVar%").count());

		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("stringVar", "stringVal").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("nonExistingVar", "string%").count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().processVariableValueLike("stringVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml") public void testProcessVariableValueLikeIgnoreCase() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml")]
	  public virtual void testProcessVariableValueLikeIgnoreCase()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["stringVar"] = "stringValue";
		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		assertEquals(0, taskService.createTaskQuery().processVariableValueLike("stringVar", "stringVal%".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "stringVal%".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "%ngValue".ToLower()).count());
		assertEquals(1, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "%ngVal%".ToLower()).count());

		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "stringVar%".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "%ngVar".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "%ngVar%".ToLower()).count());

		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", "stringVal".ToLower()).count());
		assertEquals(0, taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("nonExistingVar", "stringVal%".ToLower()).count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike("stringVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources="org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml") public void testProcessVariableValueCompare() throws Exception
	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessVariableValueEquals.bpmn20.xml")]
	  public virtual void testProcessVariableValueCompare()
	  {

		  IDictionary<string, object> variables = new Dictionary<string, object>();
		  variables["numericVar"] = 928374;
		  DateTime date = (new GregorianCalendar(2014, 2, 2, 2, 2, 2)).Time;
		  variables["dateVar"] = date;
		  variables["stringVar"] = "ab";
		  variables["nullVar"] = null;

		runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// test compare methods with numeric values
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThan("numericVar", 928373).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("numericVar", 928375).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("numericVar", 928373).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("numericVar", 928375).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThan("numericVar", 928375).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("numericVar", 928373).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("numericVar", 928375).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("numericVar", 928374).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThanOrEquals("numericVar", 928373).count());

		// test compare methods with date values
		DateTime before = (new GregorianCalendar(2014, 2, 2, 2, 2, 1)).Time;
		DateTime after = (new GregorianCalendar(2014, 2, 2, 2, 2, 3)).Time;

		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThan("dateVar", before).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("dateVar", after).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("dateVar", before).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("dateVar", after).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThan("dateVar", after).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("dateVar", before).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("dateVar", after).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("dateVar", date).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThanOrEquals("dateVar", before).count());

		//test with string values
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThan("stringVar", "aa").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThan("stringVar", "ba").count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("stringVar", "aa").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("stringVar", "ba").count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThan("stringVar", "ba").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("stringVar", "aa").count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("stringVar", "ba").count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueLessThanOrEquals("stringVar", "ab").count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThanOrEquals("stringVar", "aa").count());

		// test with null value
		try
		{
		  taskService.createTaskQuery().processVariableValueGreaterThan("nullVar", null).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueLessThan("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueLessThanOrEquals("nullVar", null).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}

		// test with boolean value
		try
		{
		  taskService.createTaskQuery().processVariableValueGreaterThan("nullVar", true).count();
		  fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("nullVar", false).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueLessThan("nullVar", true).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}
		try
		{
			taskService.createTaskQuery().processVariableValueLessThanOrEquals("nullVar", false).count();
			fail("expected exception");
		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final org.camunda.bpm.engine.ProcessEngineException e)
		catch (g.camunda.bpm.engine.ProcessEngineException)
		{
		}

		// test non existing variable
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThanOrEquals("nonExisting", 123).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueEqualsNumber()
	  {
		// long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123));

		// untyped null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", null));

		// typed null (should not match)
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(null)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testProcessVariableValueNumberComparison() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testProcessVariableValueNumberComparison()
	  {
		// long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123L));

		// non-matching long
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 12345L));

		// short
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 123));

		// double
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123.0d));

		// integer
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 123));

		// untyped null
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", null));

		// typed null
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", Variables.longValue(null)));

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "123"));

		assertEquals(4, taskService.createTaskQuery().processVariableValueNotEquals("var", Variables.numberValue(123)).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueGreaterThan("var", Variables.numberValue(123)).count());
		assertEquals(5, taskService.createTaskQuery().processVariableValueGreaterThanOrEquals("var", Variables.numberValue(123)).count());
		assertEquals(0, taskService.createTaskQuery().processVariableValueLessThan("var", Variables.numberValue(123)).count());
		assertEquals(4, taskService.createTaskQuery().processVariableValueLessThanOrEquals("var", Variables.numberValue(123)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testTaskVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testTaskVariableValueEqualsNumber()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").list();
		assertEquals(8, tasks.Count);
		taskService.setVariableLocal(tasks[0].Id, "var", 123L);
		taskService.setVariableLocal(tasks[1].Id, "var", 12345L);
		taskService.setVariableLocal(tasks[2].Id, "var", (short) 123);
		taskService.setVariableLocal(tasks[3].Id, "var", 123.0d);
		taskService.setVariableLocal(tasks[4].Id, "var", 123);
		taskService.setVariableLocal(tasks[5].Id, "var", null);
		taskService.setVariableLocal(tasks[6].Id, "var", Variables.longValue(null));
		taskService.setVariableLocal(tasks[7].Id, "var", "123");

		assertEquals(4, taskService.createTaskQuery().taskVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, taskService.createTaskQuery().taskVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, taskService.createTaskQuery().taskVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, taskService.createTaskQuery().taskVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, taskService.createTaskQuery().taskVariableValueEquals("var", Variables.numberValue(null)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testVariableEqualsNumberMax() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testVariableEqualsNumberMax()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", MAX_DOUBLE_VALUE));
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", long.MaxValue));

		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(MAX_DOUBLE_VALUE)).count());
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(long.MaxValue)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testVariableEqualsNumberLongValueOverflow() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testVariableEqualsNumberLongValueOverflow()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", MAX_DOUBLE_VALUE));

		// this results in an overflow
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (long) MAX_DOUBLE_VALUE));

		// the query should not find the long variable
		assertEquals(1, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(MAX_DOUBLE_VALUE)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml") public void testVariableEqualsNumberNonIntegerDoubleShouldNotMatchInteger() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml")]
	  public virtual void testVariableEqualsNumberNonIntegerDoubleShouldNotMatchInteger()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 42).putValue("var2", 52.4d));

		// querying by 42.4 should not match the integer variable 42
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(42.4d)).count());

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 42.4d));

		// querying by 52 should not find the double variable 52.4
		assertEquals(0, taskService.createTaskQuery().processVariableValueEquals("var", Variables.numberValue(52)).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessDefinitionId() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessDefinitionId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionId(processInstance.ProcessDefinitionId).list();
		assertEquals(1, tasks.Count);
		assertEquals(processInstance.Id, tasks[0].ProcessInstanceId);

		assertEquals(0, taskService.createTaskQuery().processDefinitionId("unexisting").count());
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessDefinitionKey() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessDefinitionKey()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").list();
		assertEquals(1, tasks.Count);
		assertEquals(processInstance.Id, tasks[0].ProcessInstanceId);

		assertEquals(0, taskService.createTaskQuery().processDefinitionKey("unexisting").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testProcessDefinitionKeyIn() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/task/taskDefinitionProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testProcessDefinitionKeyIn()
	  {

		// Start for each deployed process definition a process instance
		runtimeService.startProcessInstanceByKey("taskDefinitionKeyProcess");
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// 1 task should be found with oneTaskProcess
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKeyIn("oneTaskProcess").list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("theTask", tasks[0].TaskDefinitionKey);

		// 2 Tasks should be found with both process definition keys
		tasks = taskService.createTaskQuery().processDefinitionKeyIn("oneTaskProcess", "taskDefinitionKeyProcess").list();
		assertNotNull(tasks);
		assertEquals(3, tasks.Count);

		ISet<string> keysFound = new HashSet<string>();
		foreach (Task task in tasks)
		{
		  keysFound.Add(task.TaskDefinitionKey);
		}
		assertTrue(keysFound.Contains("taskKey_123"));
		assertTrue(keysFound.Contains("theTask"));
		assertTrue(keysFound.Contains("taskKey_1"));

		// 1 Tasks should be found with oneTaskProcess,and NonExistingKey
		tasks = taskService.createTaskQuery().processDefinitionKeyIn("oneTaskProcess", "NonExistingKey").orderByTaskName().asc().list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("theTask", tasks[0].TaskDefinitionKey);

		// No task should be found with NonExistingKey
		long? count = taskService.createTaskQuery().processDefinitionKeyIn("NonExistingKey").count();
		assertEquals(0L, count.Value);

		count = taskService.createTaskQuery().processDefinitionKeyIn("oneTaskProcess").processDefinitionKey("NonExistingKey").count();
		assertEquals(0L, count.Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessDefinitionName() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessDefinitionName()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionName("The%One%Task%Process").list();
		assertEquals(1, tasks.Count);
		assertEquals(processInstance.Id, tasks[0].ProcessInstanceId);

		assertEquals(0, taskService.createTaskQuery().processDefinitionName("unexisting").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessDefinitionNameLike() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessDefinitionNameLike()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		IList<Task> tasks = taskService.createTaskQuery().processDefinitionNameLike("The\\%One\\%Task%").list();
		assertEquals(1, tasks.Count);
		assertEquals(processInstance.Id, tasks[0].ProcessInstanceId);

		assertEquals(0, taskService.createTaskQuery().processDefinitionNameLike("The One Task").count());
		assertEquals(0, taskService.createTaskQuery().processDefinitionNameLike("The Other Task%").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessInstanceBusinessKey() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessInstanceBusinessKey()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

		assertEquals(1, taskService.createTaskQuery().processDefinitionName("The%One%Task%Process").processInstanceBusinessKey("BUSINESS-KEY-1").list().size());
		assertEquals(1, taskService.createTaskQuery().processInstanceBusinessKey("BUSINESS-KEY-1").list().size());
		assertEquals(0, taskService.createTaskQuery().processInstanceBusinessKey("NON-EXISTING").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessInstanceBusinessKeyIn() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessInstanceBusinessKeyIn()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");
		runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-2");

		// 1 task should be found with BUSINESS-KEY-1
		IList<Task> tasks = taskService.createTaskQuery().processInstanceBusinessKeyIn("BUSINESS-KEY-1").list();
		assertNotNull(tasks);
		assertEquals(1, tasks.Count);

		assertEquals("theTask", tasks[0].TaskDefinitionKey);

		// 2 tasks should be found with BUSINESS-KEY-1 and BUSINESS-KEY-2
		tasks = taskService.createTaskQuery().processInstanceBusinessKeyIn("BUSINESS-KEY-1", "BUSINESS-KEY-2").list();
		assertNotNull(tasks);
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  assertEquals("theTask", task.TaskDefinitionKey);
		}

		// 1 tasks should be found with BUSINESS-KEY-1 and NON-EXISTING-KEY
		Task task = taskService.createTaskQuery().processInstanceBusinessKeyIn("BUSINESS-KEY-1", "NON-EXISTING-KEY").singleResult();

		assertNotNull(tasks);
		assertEquals("theTask", task.TaskDefinitionKey);

		long count = taskService.createTaskQuery().processInstanceBusinessKeyIn("BUSINESS-KEY-1").processInstanceBusinessKey("NON-EXISTING-KEY").count();
		assertEquals(0l, count);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testProcessInstanceBusinessKeyLike() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testProcessInstanceBusinessKeyLike()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

		assertEquals(1, taskService.createTaskQuery().processDefinitionName("The%One%Task%Process").processInstanceBusinessKey("BUSINESS-KEY-1").list().size());
		assertEquals(1, taskService.createTaskQuery().processInstanceBusinessKeyLike("BUSINESS-KEY%").list().size());
		assertEquals(0, taskService.createTaskQuery().processInstanceBusinessKeyLike("BUSINESS-KEY").count());
		assertEquals(0, taskService.createTaskQuery().processInstanceBusinessKeyLike("BUZINESS-KEY%").count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testTaskDueDate() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testTaskDueDate()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set due-date on task
		DateTime dueDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 01:12:13");
		task.DueDate = dueDate;
		taskService.saveTask(task);

		assertEquals(1, taskService.createTaskQuery().dueDate(dueDate).count());

		DateTime otherDate = new DateTime();
		otherDate.AddYears(1);
		assertEquals(0, taskService.createTaskQuery().dueDate(otherDate).count());

		DateTime priorDate = new DateTime();
		priorDate = new DateTime(dueDate);
		priorDate.roll(DateTime.YEAR, -1);
		assertEquals(1, taskService.createTaskQuery().dueAfter(priorDate).count());

		assertEquals(1, taskService.createTaskQuery().dueBefore(otherDate).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testTaskDueBefore() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testTaskDueBefore()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set due-date on task
		DateTime dueDateCal = new DateTime();
		task.DueDate = dueDateCal;
		taskService.saveTask(task);

		DateTime oneHourAgo = new DateTime();
		oneHourAgo = new DateTime(dueDateCal);
		oneHourAgo.add(DateTime.HOUR, -1);

		DateTime oneHourLater = new DateTime();
		oneHourLater = new DateTime(dueDateCal);
		oneHourLater.add(DateTime.HOUR, 1);

		assertEquals(1, taskService.createTaskQuery().dueBefore(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().dueBefore(oneHourAgo).count());

		// Update due-date to null, shouldn't show up anymore in query that matched before
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		task.DueDate = null;
		taskService.saveTask(task);

		assertEquals(0, taskService.createTaskQuery().dueBefore(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().dueBefore(oneHourAgo).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testTaskDueAfter() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testTaskDueAfter()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set due-date on task
		DateTime dueDateCal = new DateTime();
		task.DueDate = dueDateCal;
		taskService.saveTask(task);

		DateTime oneHourAgo = new DateTime();
		oneHourAgo = new DateTime(dueDateCal);
		oneHourAgo.add(DateTime.HOUR, -1);

		DateTime oneHourLater = new DateTime();
		oneHourLater = new DateTime(dueDateCal);
		oneHourLater.add(DateTime.HOUR, 1);

		assertEquals(1, taskService.createTaskQuery().dueAfter(oneHourAgo).count());
		assertEquals(0, taskService.createTaskQuery().dueAfter(oneHourLater).count());

		// Update due-date to null, shouldn't show up anymore in query that matched before
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		task.DueDate = null;
		taskService.saveTask(task);

		assertEquals(0, taskService.createTaskQuery().dueAfter(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().dueAfter(oneHourAgo).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testTaskDueDateCombinations() throws java.text.ParseException
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testTaskDueDateCombinations()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set due-date on task
		DateTime dueDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 01:12:13");
		task.DueDate = dueDate;
		taskService.saveTask(task);

		DateTime oneHourAgo = new DateTime(dueDate.Ticks - 60 * 60 * 1000);
		DateTime oneHourLater = new DateTime(dueDate.Ticks + 60 * 60 * 1000);

		assertEquals(1, taskService.createTaskQuery().dueAfter(oneHourAgo).dueDate(dueDate).dueBefore(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().dueAfter(oneHourLater).dueDate(dueDate).dueBefore(oneHourAgo).count());
		assertEquals(0, taskService.createTaskQuery().dueAfter(oneHourLater).dueDate(dueDate).count());
		assertEquals(0, taskService.createTaskQuery().dueDate(dueDate).dueBefore(oneHourAgo).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testFollowUpDate() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testFollowUpDate()
	  {
		DateTime otherDate = new DateTime();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// do not find any task instances with follow up date
		assertEquals(0, taskService.createTaskQuery().followUpDate(otherDate).count());
		assertEquals(1, taskService.createTaskQuery().processInstanceId(processInstance.Id).followUpBeforeOrNotExistent(otherDate).count());

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// set follow-up date on task
		DateTime followUpDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 01:12:13");
		task.FollowUpDate = followUpDate;
		taskService.saveTask(task);

		assertEquals(followUpDate, taskService.createTaskQuery().taskId(task.Id).singleResult().FollowUpDate);
		assertEquals(1, taskService.createTaskQuery().followUpDate(followUpDate).count());

		otherDate = new DateTime(followUpDate);

		otherDate.AddYears(1);
		assertEquals(0, taskService.createTaskQuery().followUpDate(otherDate).count());
		assertEquals(1, taskService.createTaskQuery().followUpBefore(otherDate).count());
		assertEquals(1, taskService.createTaskQuery().processInstanceId(processInstance.Id).followUpBeforeOrNotExistent(otherDate).count());
		assertEquals(0, taskService.createTaskQuery().followUpAfter(otherDate).count());

		otherDate.AddYears(-2);
		assertEquals(1, taskService.createTaskQuery().followUpAfter(otherDate).count());
		assertEquals(0, taskService.createTaskQuery().followUpBefore(otherDate).count());
		assertEquals(0, taskService.createTaskQuery().processInstanceId(processInstance.Id).followUpBeforeOrNotExistent(otherDate).count());

		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testFollowUpDateCombinations() throws java.text.ParseException
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testFollowUpDateCombinations()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// Set follow-up date on task
		DateTime dueDate = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("01/02/2003 01:12:13");
		task.FollowUpDate = dueDate;
		taskService.saveTask(task);

		DateTime oneHourAgo = new DateTime(dueDate.Ticks - 60 * 60 * 1000);
		DateTime oneHourLater = new DateTime(dueDate.Ticks + 60 * 60 * 1000);

		assertEquals(1, taskService.createTaskQuery().followUpAfter(oneHourAgo).followUpDate(dueDate).followUpBefore(oneHourLater).count());
		assertEquals(0, taskService.createTaskQuery().followUpAfter(oneHourLater).followUpDate(dueDate).followUpBefore(oneHourAgo).count());
		assertEquals(0, taskService.createTaskQuery().followUpAfter(oneHourLater).followUpDate(dueDate).count());
		assertEquals(0, taskService.createTaskQuery().followUpDate(dueDate).followUpBefore(oneHourAgo).count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testQueryByActivityInstanceId() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testQueryByActivityInstanceId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string activityInstanceId = runtimeService.getActivityInstance(processInstance.Id).ChildActivityInstances[0].Id;

		assertEquals(1, taskService.createTaskQuery().activityInstanceIdIn(activityInstanceId).list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testQueryByMultipleActivityInstanceIds() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testQueryByMultipleActivityInstanceIds()
	  {
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string activityInstanceId1 = runtimeService.getActivityInstance(processInstance1.Id).ChildActivityInstances[0].Id;

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		string activityInstanceId2 = runtimeService.getActivityInstance(processInstance2.Id).ChildActivityInstances[0].Id;

		IList<Task> result1 = taskService.createTaskQuery().activityInstanceIdIn(activityInstanceId1).list();
		assertEquals(1, result1.Count);
		assertEquals(processInstance1.Id, result1[0].ProcessInstanceId);

		IList<Task> result2 = taskService.createTaskQuery().activityInstanceIdIn(activityInstanceId2).list();
		assertEquals(1, result2.Count);
		assertEquals(processInstance2.Id, result2[0].ProcessInstanceId);

		assertEquals(2, taskService.createTaskQuery().activityInstanceIdIn(activityInstanceId1, activityInstanceId2).list().size());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"}) public void testQueryByInvalidActivityInstanceId() throws Exception
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml"})]
	  public virtual void testQueryByInvalidActivityInstanceId()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		assertEquals(0, taskService.createTaskQuery().activityInstanceIdIn("anInvalidActivityInstanceId").list().size());
	  }

	  public virtual void testQueryPaging()
	  {
		TaskQuery query = taskService.createTaskQuery().taskCandidateUser("kermit");

		assertEquals(10, query.listPage(0, int.MaxValue).size());

		// Verifying the un-paged results
		assertEquals(10, query.count());
		assertEquals(10, query.list().size());

		// Verifying paged results
		assertEquals(2, query.listPage(0, 2).size());
		assertEquals(2, query.listPage(2, 2).size());
		assertEquals(3, query.listPage(4, 3).size());
		assertEquals(1, query.listPage(9, 3).size());
		assertEquals(1, query.listPage(9, 1).size());

		// Verifying odd usages
		assertEquals(0, query.listPage(-1, -1).size());
		assertEquals(0, query.listPage(10, 2).size()); // 9 is the last index with a result
		assertEquals(10, query.listPage(0, 15).size()); // there are only 10 tasks
	  }

	  public virtual void testQuerySorting()
	  {
		// default ordering is by id
		int expectedCount = 12;
		verifySortingAndCount(taskService.createTaskQuery(), expectedCount, taskById());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskId().asc(), expectedCount, taskById());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskName().asc(), expectedCount, taskByName());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskPriority().asc(), expectedCount, taskByPriority());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskAssignee().asc(), expectedCount, taskByAssignee());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskDescription().asc(), expectedCount, taskByDescription());
		verifySortingAndCount(taskService.createTaskQuery().orderByProcessInstanceId().asc(), expectedCount, taskByProcessInstanceId());
		verifySortingAndCount(taskService.createTaskQuery().orderByExecutionId().asc(), expectedCount, taskByExecutionId());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskCreateTime().asc(), expectedCount, taskByCreateTime());
		verifySortingAndCount(taskService.createTaskQuery().orderByDueDate().asc(), expectedCount, taskByDueDate());
		verifySortingAndCount(taskService.createTaskQuery().orderByFollowUpDate().asc(), expectedCount, taskByFollowUpDate());
		verifySortingAndCount(taskService.createTaskQuery().orderByCaseInstanceId().asc(), expectedCount, taskByCaseInstanceId());
		verifySortingAndCount(taskService.createTaskQuery().orderByCaseExecutionId().asc(), expectedCount, taskByCaseExecutionId());

		verifySortingAndCount(taskService.createTaskQuery().orderByTaskId().desc(), expectedCount, inverted(taskById()));
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskName().desc(), expectedCount, inverted(taskByName()));
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskPriority().desc(), expectedCount, inverted(taskByPriority()));
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskAssignee().desc(), expectedCount, inverted(taskByAssignee()));
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskDescription().desc(), expectedCount, inverted(taskByDescription()));
		verifySortingAndCount(taskService.createTaskQuery().orderByProcessInstanceId().desc(), expectedCount, inverted(taskByProcessInstanceId()));
		verifySortingAndCount(taskService.createTaskQuery().orderByExecutionId().desc(), expectedCount, inverted(taskByExecutionId()));
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskCreateTime().desc(), expectedCount, inverted(taskByCreateTime()));
		verifySortingAndCount(taskService.createTaskQuery().orderByDueDate().desc(), expectedCount, inverted(taskByDueDate()));
		verifySortingAndCount(taskService.createTaskQuery().orderByFollowUpDate().desc(), expectedCount, inverted(taskByFollowUpDate()));
		verifySortingAndCount(taskService.createTaskQuery().orderByCaseInstanceId().desc(), expectedCount, inverted(taskByCaseInstanceId()));
		verifySortingAndCount(taskService.createTaskQuery().orderByCaseExecutionId().desc(), expectedCount, inverted(taskByCaseExecutionId()));

		verifySortingAndCount(taskService.createTaskQuery().orderByTaskId().taskName("testTask").asc(), 6, taskById());
		verifySortingAndCount(taskService.createTaskQuery().orderByTaskId().taskName("testTask").desc(), 6, inverted(taskById()));
	  }

	  public virtual void testQuerySortingByNameShouldBeCaseInsensitive()
	  {
		// create task with capitalized name
		Task task = taskService.newTask("caseSensitiveTestTask");
		task.Name = "CaseSensitiveTestTask";
		taskService.saveTask(task);

		// create task filter
		Filter filter = filterService.newTaskFilter("taskNameOrdering");
		filterService.saveFilter(filter);

		IList<string> sortedNames = getTaskNamesFromTasks(taskService.createTaskQuery().list());
		sortedNames.Sort(string.CASE_INSENSITIVE_ORDER);

		// ascending ordering
		TaskQuery taskQuery = taskService.createTaskQuery().orderByTaskNameCaseInsensitive().asc();
		IList<string> ascNames = getTaskNamesFromTasks(taskQuery.list());
		assertEquals(sortedNames, ascNames);

		// test filter merging
		ascNames = getTaskNamesFromTasks(filterService.list(filter.Id, taskQuery));
		assertEquals(sortedNames, ascNames);

		// descending ordering

		// reverse sorted names to test descending ordering
		sortedNames.Reverse();

		taskQuery = taskService.createTaskQuery().orderByTaskNameCaseInsensitive().desc();
		IList<string> descNames = getTaskNamesFromTasks(taskQuery.list());
		assertEquals(sortedNames, descNames);

		// test filter merging
		descNames = getTaskNamesFromTasks(filterService.list(filter.Id, taskQuery));
		assertEquals(sortedNames, descNames);

		// delete test task
		taskService.deleteTask(task.Id, true);

		// delete filter
		filterService.deleteFilter(filter.Id);
	  }

	  public virtual void testQueryOrderByTaskName()
	  {

		// asc
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(12, tasks.Count);


		IList<string> taskNames = getTaskNamesFromTasks(tasks);
		assertEquals("accountancy description", taskNames[0]);
		assertEquals("accountancy description", taskNames[1]);
		assertEquals("gonzo_Task", taskNames[2]);
		assertEquals("managementAndAccountancyTask", taskNames[3]);
		assertEquals("managementTask", taskNames[4]);
		assertEquals("managementTask", taskNames[5]);
		assertEquals("testTask", taskNames[6]);
		assertEquals("testTask", taskNames[7]);
		assertEquals("testTask", taskNames[8]);
		assertEquals("testTask", taskNames[9]);
		assertEquals("testTask", taskNames[10]);
		assertEquals("testTask", taskNames[11]);

		// desc
		tasks = taskService.createTaskQuery().orderByTaskName().desc().list();
		assertEquals(12, tasks.Count);

		taskNames = getTaskNamesFromTasks(tasks);
		assertEquals("testTask", taskNames[0]);
		assertEquals("testTask", taskNames[1]);
		assertEquals("testTask", taskNames[2]);
		assertEquals("testTask", taskNames[3]);
		assertEquals("testTask", taskNames[4]);
		assertEquals("testTask", taskNames[5]);
		assertEquals("managementTask", taskNames[6]);
		assertEquals("managementTask", taskNames[7]);
		assertEquals("managementAndAccountancyTask", taskNames[8]);
		assertEquals("gonzo_Task", taskNames[9]);
		assertEquals("accountancy description", taskNames[10]);
		assertEquals("accountancy description", taskNames[11]);
	  }

	  public virtual IList<string> getTaskNamesFromTasks(IList<Task> tasks)
	  {
		IList<string> names = new List<string>();
		foreach (Task task in tasks)
		{
		  names.Add(task.Name);
		}
		return names;
	  }

	  public virtual void testNativeQuery()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		assertEquals(tablePrefix + "ACT_RU_TASK", managementService.getTableName(typeof(Task)));
		assertEquals(tablePrefix + "ACT_RU_TASK", managementService.getTableName(typeof(TaskEntity)));
		assertEquals(12, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Task))).list().size());
		assertEquals(12, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Task))).count());

		assertEquals(144, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + tablePrefix + "ACT_RU_TASK T1, " + tablePrefix + "ACT_RU_TASK T2").count());

		// join task and variable instances
		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Task)) + " T1, " + managementService.getTableName(typeof(VariableInstanceEntity)) + " V1 WHERE V1.TASK_ID_ = T1.ID_").count());
		IList<Task> tasks = taskService.createNativeTaskQuery().sql("SELECT T1.* FROM " + managementService.getTableName(typeof(Task)) + " T1, " + managementService.getTableName(typeof(VariableInstanceEntity)) + " V1 WHERE V1.TASK_ID_ = T1.ID_").list();
		assertEquals(1, tasks.Count);
		assertEquals("gonzo_Task", tasks[0].Name);

		// select with distinct
		assertEquals(12, taskService.createNativeTaskQuery().sql("SELECT DISTINCT T1.* FROM " + tablePrefix + "ACT_RU_TASK T1").list().size());

		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Task)) + " T WHERE T.NAME_ = 'gonzo_Task'").count());
		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Task)) + " T WHERE T.NAME_ = 'gonzo_Task'").list().size());

		// use parameters
		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Task)) + " T WHERE T.NAME_ = #{taskName}").parameter("taskName", "gonzo_Task").count());
	  }

	  public virtual void testNativeQueryPaging()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		assertEquals(tablePrefix + "ACT_RU_TASK", managementService.getTableName(typeof(Task)));
		assertEquals(tablePrefix + "ACT_RU_TASK", managementService.getTableName(typeof(TaskEntity)));
		assertEquals(5, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Task))).listPage(0, 5).size());
		assertEquals(2, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Task))).listPage(10, 12).size());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionId()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionId(caseDefinitionId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionId()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionId(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionKey()
	  {
		string caseDefinitionKey = repositoryService.createCaseDefinitionQuery().singleResult().Key;

		caseService.withCaseDefinitionByKey(caseDefinitionKey).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionKey(caseDefinitionKey);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionKey()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionKey(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseDefinitionName()
	  {
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();

		string caseDefinitionId = caseDefinition.Id;
		string caseDefinitionName = caseDefinition.Name;

		caseService.withCaseDefinition(caseDefinitionId).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionName(caseDefinitionName);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionName()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionName("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionName(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/repository/three_.cmmn"})]
	  public virtual void testQueryByCaseDefinitionNameLike()
	  {
		IList<string> caseDefinitionIds = CaseDefinitionIds;

		foreach (string caseDefinitionId in caseDefinitionIds)
		{
		  caseService.withCaseDefinition(caseDefinitionId).create();
		}
		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionNameLike("One T%");
		verifyQueryResults(query, 1);

		query.caseDefinitionNameLike("%Task Case");
		verifyQueryResults(query, 1);

		query.caseDefinitionNameLike("%Task%");
		verifyQueryResults(query, 1);

		query.caseDefinitionNameLike("%z\\_");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionNameLike()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseDefinitionNameLike("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionNameLike(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceId()
	  {
		string caseDefinitionId = CaseDefinitionId;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceId(caseInstanceId);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources: { "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testQueryByCaseInstanceIdHierarchy.cmmn", "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testQueryByCaseInstanceIdHierarchy.bpmn20.xml" })]
	  public virtual void testQueryByCaseInstanceIdHierarchy()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string processTaskId = caseService.createCaseExecutionQuery().activityId("PI_ProcessTask_1").singleResult().Id;

		// then

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceId(caseInstanceId);

		verifyQueryResults(query, 2);

		foreach (Task task in query.list())
		{
		  assertEquals(caseInstanceId, task.CaseInstanceId);
		  taskService.complete(task.Id);
		}

		verifyQueryResults(query, 1);
		assertEquals(caseInstanceId, query.singleResult().CaseInstanceId);

		taskService.complete(query.singleResult().Id);

		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseInstanceId(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceBusinessKey()
	  {
		string caseDefinitionId = CaseDefinitionId;

		string businessKey = "aBusinessKey";

		caseService.withCaseDefinition(caseDefinitionId).businessKey(businessKey).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceBusinessKey(businessKey);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseInstanceBusinessKey()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceBusinessKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseInstanceBusinessKey(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByCaseInstanceBusinessKeyLike()
	  {
		string caseDefinitionId = CaseDefinitionId;

		string businessKey = "aBusiness_Key";

		caseService.withCaseDefinition(caseDefinitionId).businessKey(businessKey).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceBusinessKeyLike("aBus%");
		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKeyLike("%siness\\_Key");
		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKeyLike("%sines%");
		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKeyLike("%sines%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseInstanceBusinessKeyLike()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceBusinessKeyLike("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseInstanceBusinessKeyLike(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testQueryByCaseExecutionId()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).create();

		string humanTaskExecutionId = startDefaultCaseExecutionManually();

		TaskQuery query = taskService.createTaskQuery();

		query.caseExecutionId(humanTaskExecutionId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseExecutionId()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.caseExecutionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseExecutionId(null);
		  fail("expected exception");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aNullValue", null);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aStringValue", "abc");

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCaseInstanceVariableNameEqualsIgnoreCase() throws Exception
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCaseInstanceVariableNameEqualsIgnoreCase()
	  {
		string caseDefinitionId = CaseDefinitionId;

		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";

		caseService.withCaseDefinition(caseDefinitionId).setVariable(variableName, variableValue).create();

		// query for case-insensitive variable name should only return a result if case-insensitive search is used
		assertEquals(1, taskService.createTaskQuery().matchVariableNamesIgnoreCase().caseInstanceVariableValueEquals(variableName.ToLower(), variableValue).count());
		assertEquals(0, taskService.createTaskQuery().caseInstanceVariableValueEquals(variableName.ToLower(), variableValue).count());

		// query should treat all variables case-insensitively, even when flag is set after variable
		assertEquals(1, taskService.createTaskQuery().caseInstanceVariableValueEquals(variableName.ToLower(), variableValue).matchVariableNamesIgnoreCase().count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueEqualsIgnoreCase()
	  {
		string caseDefinitionId = CaseDefinitionId;

		string variableName = "someVariable";
		string variableValue = "someCamelCaseValue";

		caseService.withCaseDefinition(caseDefinitionId).setVariable(variableName, variableValue).create();

		TaskQuery query;

		// query for case-insensitive variable value should only return a result when case-insensitive search is used
		query = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueEquals(variableName, variableValue.ToLower());
		verifyQueryResults(query, 1);
		query = taskService.createTaskQuery().caseInstanceVariableValueEquals(variableName, variableValue.ToLower());
		verifyQueryResults(query, 0);

		// query for non existing variable should return zero results
		query = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueEquals("nonExistingVariable", variableValue.ToLower());
		verifyQueryResults(query, 0);

		// query for existing variable with different value should return zero results
		query = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueEquals(variableName, "nonExistentValue".ToLower());
		verifyQueryResults(query, 0);

		// query for case-insensitive variable with not equals operator should only return a result when case-sensitive search is used
		query = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueNotEquals(variableName, variableValue.ToLower());
		verifyQueryResults(query, 0);
		query = taskService.createTaskQuery().caseInstanceVariableValueNotEquals(variableName, variableValue.ToLower());
		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aBooleanValue", true);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("anIntegerValue", 456);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aDateValue", now);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableValueEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueEquals()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueEquals(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  /// <summary>
	  /// Starts the one deployed case at the point of the manual activity PI_HumanTask_1
	  /// with the given variable.
	  /// </summary>
	  protected internal virtual void startDefaultCaseWithVariable(object variableValue, string variableName)
	  {
		string caseDefinitionId = CaseDefinitionId;
		createCaseWithVariable(caseDefinitionId, variableValue, variableName);
	  }

	  /// <returns> the case definition id if only one case is deployed. </returns>
	  protected internal virtual string CaseDefinitionId
	  {
		  get
		  {
			string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;
			return caseDefinitionId;
		  }
	  }

	  /// <returns> the case definition ids </returns>
	  protected internal virtual IList<string> CaseDefinitionIds
	  {
		  get
		  {
			IList<string> caseDefinitionIds = new List<string>();
			IList<CaseDefinition> caseDefinitions = repositoryService.createCaseDefinitionQuery().list();
			foreach (CaseDefinition caseDefinition in caseDefinitions)
			{
			  caseDefinitionIds.Add(caseDefinition.Id);
			}
			return caseDefinitionIds;
		  }
	  }

	  protected internal virtual void createCaseWithVariable(string caseDefinitionId, object variableValue, string variableName)
	  {
		caseService.withCaseDefinition(caseDefinitionId).setVariable(variableName, variableValue).create();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aStringValue", "abd");

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aBooleanValue", false);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aShortValue", (short) 124);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("anIntegerValue", 457);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aLongValue", (long) 790);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		DateTime before = new DateTime(now.Ticks - 100000);

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aDateValue", before);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueNotEquals("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueNotEquals()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		TaskQuery query = taskService.createTaskQuery();
		try
		{
		  query.caseInstanceVariableValueNotEquals(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  /// <summary>
	  /// @return
	  /// </summary>
	  protected internal virtual FileValue createDefaultFileValue()
	  {
		FileValue fileValue = Variables.fileValue("tst.txt").file("somebytes".GetBytes()).create();
		return fileValue;
	  }

	  /// <summary>
	  /// Starts the case execution for oneTaskCase.cmmn<para>
	  /// Only works for testcases, which deploy that process.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the execution id for the activity PI_HumanTask_1 </returns>
	  protected internal virtual string startDefaultCaseExecutionManually()
	  {
		string humanTaskExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		caseService.withCaseExecution(humanTaskExecutionId).manualStart();
		return humanTaskExecutionId;
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueNotEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueNotEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueNotEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThan("aStringValue", "ab");

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThan("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThan("anIntegerValue", 455);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThan("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		TaskQuery query = taskService.createTaskQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.caseInstanceVariableValueGreaterThan("aDateValue", before);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThan("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableGreaterThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueGreaterThan()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		startDefaultCaseExecutionManually();
		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEquals("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aStringValue", "ab");

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEquals("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThanOrEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("anIntegerValue", 455);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		TaskQuery query = taskService.createTaskQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.caseInstanceVariableValueGreaterThanOrEquals("aDateValue", before);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueGreaterThanOrEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableGreaterThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEquals(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThan("aStringValue", "abd");

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThan("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThan("anIntegerValue", 457);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThan("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		TaskQuery query = taskService.createTaskQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.caseInstanceVariableValueLessThan("aDateValue", after);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThan("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableLessThan()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueLessThan()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		TaskQuery query = taskService.createTaskQuery();
		try
		{
		  query.caseInstanceVariableValueLessThan(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEquals("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aStringValue", "abd");

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByBooleanCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aBooleanValue", true).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEquals("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByShortCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aShortValue", (short) 123).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByIntegerCaseInstanceVariableValueLessThanOrEquals()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("anIntegerValue", 456).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("anIntegerValue", 457);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByLongCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aLongValue", (long) 789).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDateCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		DateTime now = DateTime.Now;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDateValue", now).create();

		TaskQuery query = taskService.createTaskQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.caseInstanceVariableValueLessThanOrEquals("aDateValue", after);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByDoubleCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aDoubleValue", 1.5).create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLessThanOrEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aByteArrayValue", bytes).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryBySerializableCaseInstanceVariableLessThanOrEqual()
	  {
		string caseDefinitionId = CaseDefinitionId;

		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aSerializableValue", serializable).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByFileCaseInstanceVariableValueLessThanOrEqual()
	  {
		FileValue fileValue = createDefaultFileValue();
		string variableName = "aFileValue";

		startDefaultCaseWithVariable(fileValue, variableName);
		TaskQuery query = taskService.createTaskQuery();
		try
		{
		  query.caseInstanceVariableValueLessThanOrEquals(variableName, fileValue).list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Variables of type File cannot be used to query"));
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByNullCaseInstanceVariableValueLike()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aNullValue", null).create();

		TaskQuery query = taskService.createTaskQuery();

		try
		{
		  query.caseInstanceVariableValueLike("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueLike()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringValue", "abc").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLike("aStringValue", "ab%");

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLike("aStringValue", "%bc");

		verifyQueryResults(query, 1);

		query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLike("aStringValue", "%b%");

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByStringCaseInstanceVariableValueLikeIgnoreCase()
	  {
		string caseDefinitionId = CaseDefinitionId;

		caseService.withCaseDefinition(caseDefinitionId).setVariable("aStringVariable", "aStringValue").create();

		TaskQuery query = taskService.createTaskQuery();

		query.caseInstanceVariableValueLike("aStringVariable", "aString%".ToLower());

		verifyQueryResults(query, 0);

		query = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueLike("aStringVariable", "aString%".ToLower());

		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryByVariableInParallelBranch() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testQueryByVariableInParallelBranch()
	  {
		runtimeService.startProcessInstanceByKey("parallelGateway");

		// when there are two process variables of the same name but different types
		Execution task1Execution = runtimeService.createExecutionQuery().activityId("task1").singleResult();
		runtimeService.setVariableLocal(task1Execution.Id, "var", 12345L);
		Execution task2Execution = runtimeService.createExecutionQuery().activityId("task2").singleResult();
		runtimeService.setVariableLocal(task2Execution.Id, "var", 12345);

		// then the task query should be able to filter by both variables and return both tasks
		assertEquals(2, taskService.createTaskQuery().processVariableValueEquals("var", 12345).count());
		assertEquals(2, taskService.createTaskQuery().processVariableValueEquals("var", 12345L).count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByProcessVariables()
	  {
		// given three tasks with String process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "bValue"));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "cValue"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "aValue"));

		// when I make a task query with ascending variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly
		assertEquals(3, tasks.Count);
		// then in alphabetical order
		assertEquals(instance3.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance1.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance2.Id, tasks[2].ProcessInstanceId);

		// when I make a task query with descending variable ordering by String values
		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).desc().list();

		// then the tasks are ordered correctly
		assertEquals(3, tasks.Count);
		// then in alphabetical order
		assertEquals(instance2.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance1.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance3.Id, tasks[2].ProcessInstanceId);


		// when I make a task query with variable ordering by Integer values
		IList<Task> unorderedTasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).asc().list();

		// then the tasks are in no particular ordering
		assertEquals(3, unorderedTasks.Count);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testLocalExecutionVariable.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByExecutionVariables()
	  {
		// given three tasks with String process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("parallelGateway", Collections.singletonMap<string, object>("var", "aValue"));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("parallelGateway", Collections.singletonMap<string, object>("var", "bValue"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("parallelGateway", Collections.singletonMap<string, object>("var", "cValue"));

		// and some local variables on the tasks
		Task task1 = taskService.createTaskQuery().processInstanceId(instance1.Id).singleResult();
		runtimeService.setVariableLocal(task1.ExecutionId, "var", "cValue");

		Task task2 = taskService.createTaskQuery().processInstanceId(instance2.Id).singleResult();
		runtimeService.setVariableLocal(task2.ExecutionId, "var", "bValue");

		Task task3 = taskService.createTaskQuery().processInstanceId(instance3.Id).singleResult();
		runtimeService.setVariableLocal(task3.ExecutionId, "var", "aValue");

		// when I make a task query with ascending variable ordering by tasks variables
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("parallelGateway").orderByExecutionVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly by their local variables
		assertEquals(3, tasks.Count);
		assertEquals(instance3.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance2.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance1.Id, tasks[2].ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByTaskVariables()
	  {
		// given three tasks with String process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "aValue"));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "bValue"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "cValue"));

		// and some local variables on the tasks
		Task task1 = taskService.createTaskQuery().processInstanceId(instance1.Id).singleResult();
		taskService.setVariableLocal(task1.Id, "var", "cValue");

		Task task2 = taskService.createTaskQuery().processInstanceId(instance2.Id).singleResult();
		taskService.setVariableLocal(task2.Id, "var", "bValue");

		Task task3 = taskService.createTaskQuery().processInstanceId(instance3.Id).singleResult();
		taskService.setVariableLocal(task3.Id, "var", "aValue");

		// when I make a task query with ascending variable ordering by tasks variables
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByTaskVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly by their local variables
		assertEquals(3, tasks.Count);
		assertEquals(instance3.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance2.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance1.Id, tasks[2].ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testQueryResultOrderingByCaseInstanceVariables()
	  {
		// given three tasks with String case instance variables
		CaseInstance instance1 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "cValue"));
		CaseInstance instance2 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "aValue"));
		CaseInstance instance3 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "bValue"));

		// when I make a task query with ascending variable ordering by tasks variables
		IList<Task> tasks = taskService.createTaskQuery().caseDefinitionKey("oneTaskCase").orderByCaseInstanceVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly by their local variables
		assertEquals(3, tasks.Count);
		assertEquals(instance2.Id, tasks[0].CaseInstanceId);
		assertEquals(instance3.Id, tasks[1].CaseInstanceId);
		assertEquals(instance1.Id, tasks[2].CaseInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn")]
	  public virtual void testQueryResultOrderingByCaseExecutionVariables()
	  {
		// given three tasks with String case instance variables
		CaseInstance instance1 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "cValue"));
		CaseInstance instance2 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "aValue"));
		CaseInstance instance3 = caseService.createCaseInstanceByKey("oneTaskCase", Collections.singletonMap<string, object>("var", "bValue"));

		// and local case execution variables
		CaseExecution caseExecution1 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").caseInstanceId(instance1.Id).singleResult();

		caseService.withCaseExecution(caseExecution1.Id).setVariableLocal("var", "aValue").manualStart();

		CaseExecution caseExecution2 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").caseInstanceId(instance2.Id).singleResult();

		caseService.withCaseExecution(caseExecution2.Id).setVariableLocal("var", "bValue").manualStart();

		CaseExecution caseExecution3 = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").caseInstanceId(instance3.Id).singleResult();

		caseService.withCaseExecution(caseExecution3.Id).setVariableLocal("var", "cValue").manualStart();

		// when I make a task query with ascending variable ordering by tasks variables
		IList<Task> tasks = taskService.createTaskQuery().caseDefinitionKey("oneTaskCase").orderByCaseExecutionVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly by their local variables
		assertEquals(3, tasks.Count);
		assertEquals(instance1.Id, tasks[0].CaseInstanceId);
		assertEquals(instance2.Id, tasks[1].CaseInstanceId);
		assertEquals(instance3.Id, tasks[2].CaseInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByVariablesWithNullValues()
	  {
		// given three tasks with String process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "bValue"));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "cValue"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "aValue"));
		ProcessInstance instance4 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().list();

		Task firstTask = tasks[0];

		// the null-valued task should be either first or last
		if (firstTask.ProcessInstanceId.Equals(instance4.Id))
		{
		  // then the others in ascending order
		  assertEquals(instance3.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance1.Id, tasks[2].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[3].ProcessInstanceId);
		}
		else
		{
		  assertEquals(instance3.Id, tasks[0].ProcessInstanceId);
		  assertEquals(instance1.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[2].ProcessInstanceId);
		  assertEquals(instance4.Id, tasks[3].ProcessInstanceId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByVariablesWithMixedTypes()
	  {
		// given three tasks with String and Integer process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 42));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "cValue"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "aValue"));

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().list();

		Task firstTask = tasks[0];

		// the numeric-valued task should be either first or last
		if (firstTask.ProcessInstanceId.Equals(instance1.Id))
		{
		  // then the others in ascending order
		  assertEquals(instance3.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[2].ProcessInstanceId);
		}
		else
		{
		  assertEquals(instance3.Id, tasks[0].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance1.Id, tasks[2].ProcessInstanceId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByStringVariableWithMixedCase()
	  {
		// given three tasks with String and Integer process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "a"));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "B"));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "c"));

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly
		assertEquals(3, tasks.Count);
		// first the numeric valued task (since it is treated like null-valued)
		assertEquals(instance1.Id, tasks[0].ProcessInstanceId);
		// then the others in alphabetical order
		assertEquals(instance2.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance3.Id, tasks[2].ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByVariablesOfAllPrimitiveTypes()
	  {
		// given three tasks with String and Integer process instance variables
		ProcessInstance booleanInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", true));
		ProcessInstance shortInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", (short) 16));
		ProcessInstance longInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 500L));
		ProcessInstance intInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 400));
		ProcessInstance stringInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", "300"));
		ProcessInstance dateInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", new DateTime(1000L)));
		ProcessInstance doubleInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 42.5d));

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.BOOLEAN).asc().list();

		verifyFirstOrLastTask(tasks, booleanInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.SHORT).asc().list();

		verifyFirstOrLastTask(tasks, shortInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.LONG).asc().list();

		verifyFirstOrLastTask(tasks, longInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).asc().list();

		verifyFirstOrLastTask(tasks, intInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().list();

		verifyFirstOrLastTask(tasks, stringInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.DATE).asc().list();

		verifyFirstOrLastTask(tasks, dateInstance);

		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.DOUBLE).asc().list();

		verifyFirstOrLastTask(tasks, doubleInstance);
	  }

	  public virtual void testQueryByUnsupportedValueTypes()
	  {
		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", ValueType.BYTES);
		  fail("this type is not supported");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot order by variables of type byte", e.Message);
		}

		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", ValueType.NULL);
		  fail("this type is not supported");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot order by variables of type null", e.Message);
		}

		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", ValueType.NUMBER);
		  fail("this type is not supported");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot order by variables of type number", e.Message);
		}

		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", ValueType.OBJECT);
		  fail("this type is not supported");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot order by variables of type object", e.Message);
		}

		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", ValueType.FILE);
		  fail("this type is not supported");
		}
		catch (ProcessEngineException e)
		{
		  // happy path
		  assertTextPresent("Cannot order by variables of type file", e.Message);
		}
	  }

	  /// <summary>
	  /// verify that either the first or the last task of the list belong to the given process instance
	  /// </summary>
	  protected internal virtual void verifyFirstOrLastTask(IList<Task> tasks, ProcessInstance belongingProcessInstance)
	  {
		if (tasks.Count == 0)
		{
		  fail("no tasks given");
		}

		int numTasks = tasks.Count;
		bool matches = tasks[0].ProcessInstanceId.Equals(belongingProcessInstance.Id);
		matches = matches || tasks[numTasks - 1].ProcessInstanceId.Equals(belongingProcessInstance.Id);

		assertTrue("neither first nor last task belong to process instance " + belongingProcessInstance.Id, matches);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByVariablesWithMixedTypesAndSameColumn()
	  {
		// given three tasks with Integer and Long process instance variables
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 42));
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 800));
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Collections.singletonMap<string, object>("var", 500L));

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).asc().list();

		// then the tasks are ordered correctly
		assertEquals(3, tasks.Count);

		Task firstTask = tasks[0];

		// the Long-valued task should be either first or last
		if (firstTask.ProcessInstanceId.Equals(instance3.Id))
		{
		  // then the others in ascending order
		  assertEquals(instance1.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[2].ProcessInstanceId);
		}
		else
		{
		  assertEquals(instance1.Id, tasks[0].ProcessInstanceId);
		  assertEquals(instance2.Id, tasks[1].ProcessInstanceId);
		  assertEquals(instance3.Id, tasks[2].ProcessInstanceId);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByTwoVariables()
	  {
		// given three tasks with String process instance variables
		ProcessInstance bInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "b").putValue("var2", 14));
		ProcessInstance bInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "b").putValue("var2", 30));
		ProcessInstance cInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "c").putValue("var2", 50));
		ProcessInstance cInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "c").putValue("var2", 30));
		ProcessInstance aInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "a").putValue("var2", 14));
		ProcessInstance aInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "a").putValue("var2", 50));

		// when I make a task query with variable primary ordering by var values
		// and secondary ordering by var2 values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).desc().orderByProcessVariable("var2", ValueType.INTEGER).asc().list();

		// then the tasks are ordered correctly
		assertEquals(6, tasks.Count);
		// var = c; var2 = 30
		assertEquals(cInstance2.Id, tasks[0].ProcessInstanceId);
		// var = c; var2 = 50
		assertEquals(cInstance1.Id, tasks[1].ProcessInstanceId);
		// var = b; var2 = 14
		assertEquals(bInstance1.Id, tasks[2].ProcessInstanceId);
		// var = b; var2 = 30
		assertEquals(bInstance2.Id, tasks[3].ProcessInstanceId);
		// var = a; var2 = 14
		assertEquals(aInstance1.Id, tasks[4].ProcessInstanceId);
		// var = a; var2 = 50
		assertEquals(aInstance2.Id, tasks[5].ProcessInstanceId);

		// when I make a task query with variable primary ordering by var2 values
		// and secondary ordering by var values
		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var2", ValueType.INTEGER).desc().orderByProcessVariable("var", ValueType.STRING).asc().list();

		// then the tasks are ordered correctly
		assertEquals(6, tasks.Count);
		// var = a; var2 = 50
		assertEquals(aInstance2.Id, tasks[0].ProcessInstanceId);
		// var = c; var2 = 50
		assertEquals(cInstance1.Id, tasks[1].ProcessInstanceId);
		// var = b; var2 = 30
		assertEquals(bInstance2.Id, tasks[2].ProcessInstanceId);
		// var = c; var2 = 30
		assertEquals(cInstance2.Id, tasks[3].ProcessInstanceId);
		// var = a; var2 = 14
		assertEquals(aInstance1.Id, tasks[4].ProcessInstanceId);
		// var = b; var2 = 14
		assertEquals(bInstance1.Id, tasks[5].ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryResultOrderingByVariablesWithSecondaryOrderingByProcessInstanceId()
	  {
		// given three tasks with String process instance variables
		ProcessInstance bInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "b"));
		ProcessInstance bInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "b"));
		ProcessInstance cInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "c"));
		ProcessInstance cInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "c"));
		ProcessInstance aInstance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "a"));
		ProcessInstance aInstance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", "a"));

		// when I make a task query with variable ordering by String values
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.STRING).asc().orderByProcessInstanceId().asc().list();

		// then the tasks are ordered correctly
		assertEquals(6, tasks.Count);

		// var = a
		verifyTasksSortedByProcessInstanceId(Arrays.asList(aInstance1, aInstance2), tasks.subList(0, 2));

		// var = b
		verifyTasksSortedByProcessInstanceId(Arrays.asList(bInstance1, bInstance2), tasks.subList(2, 4));

		// var = c
		verifyTasksSortedByProcessInstanceId(Arrays.asList(cInstance1, cInstance2), tasks.subList(4, 6));
	  }

	  public virtual void testQueryResultOrderingWithInvalidParameters()
	  {
		try
		{
		  taskService.createTaskQuery().orderByProcessVariable(null, ValueType.STRING).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByProcessVariable("var", null).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByExecutionVariable(null, ValueType.STRING).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByExecutionVariable("var", null).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByTaskVariable(null, ValueType.STRING).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByTaskVariable("var", null).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByCaseInstanceVariable(null, ValueType.STRING).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByCaseInstanceVariable("var", null).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByCaseExecutionVariable(null, ValueType.STRING).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}

		try
		{
		  taskService.createTaskQuery().orderByCaseExecutionVariable("var", null).asc().list();
		  fail("should not succeed");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

	  protected internal virtual void verifyTasksSortedByProcessInstanceId(IList<ProcessInstance> expectedProcessInstances, IList<Task> actualTasks)
	  {

		assertEquals(expectedProcessInstances.Count, actualTasks.Count);
		IList<ProcessInstance> instances = new List<ProcessInstance>(expectedProcessInstances);

		instances.Sort(new ComparatorAnonymousInnerClass(this));

		for (int i = 0; i < instances.Count; i++)
		{
		  assertEquals(instances[i].Id, actualTasks[i].ProcessInstanceId);
		}
	  }

	  private class ComparatorAnonymousInnerClass : IComparer<ProcessInstance>
	  {
		  private readonly TaskQueryTest outerInstance;

		  public ComparatorAnonymousInnerClass(TaskQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public int compare(ProcessInstance p1, ProcessInstance p2)
		  {
			return p1.Id.CompareTo(p2.Id);
		  }
	  }

	  private void verifyQueryResults(TaskQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  private void verifySingleResultFails(TaskQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/oneTaskWithFormKeyProcess.bpmn20.xml"})]
	  public virtual void testInitializeFormKeys()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		// if initializeFormKeys
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).initializeFormKeys().singleResult();

		// then the form key is present
		assertEquals("exampleFormKey", task.FormKey);

		// if NOT initializeFormKeys
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		try
		{
		  // then the form key is not retrievable
		  task.FormKey;
		  fail("exception expected.");
		}
		catch (BadUserRequestException e)
		{
		  assertEquals("ENGINE-03052 The form key is not initialized. You must call initializeFormKeys() on the task query before you can retrieve the form key.", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryOrderByProcessVariableInteger()
	  {
		ProcessInstance instance500 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 500));
		ProcessInstance instance1000 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 1000));
		ProcessInstance instance250 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 250));

		// asc
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).asc().list();

		assertEquals(3, tasks.Count);
		assertEquals(instance250.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance500.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance1000.Id, tasks[2].ProcessInstanceId);

		// desc
		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).desc().list();

		assertEquals(3, tasks.Count);
		assertEquals(instance1000.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance500.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance250.Id, tasks[2].ProcessInstanceId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/task/TaskQueryTest.testProcessDefinition.bpmn20.xml")]
	  public virtual void testQueryOrderByTaskVariableInteger()
	  {
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task500 = taskService.createTaskQuery().processInstanceId(instance1.Id).singleResult();
		taskService.setVariableLocal(task500.Id, "var", 500);

		Task task250 = taskService.createTaskQuery().processInstanceId(instance2.Id).singleResult();
		taskService.setVariableLocal(task250.Id, "var", 250);

		Task task1000 = taskService.createTaskQuery().processInstanceId(instance3.Id).singleResult();
		taskService.setVariableLocal(task1000.Id, "var", 1000);

		// asc
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByTaskVariable("var", ValueType.INTEGER).asc().list();

		assertEquals(3, tasks.Count);
		assertEquals(task250.Id, tasks[0].Id);
		assertEquals(task500.Id, tasks[1].Id);
		assertEquals(task1000.Id, tasks[2].Id);

		// desc
		tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").orderByProcessVariable("var", ValueType.INTEGER).desc().list();

		assertEquals(3, tasks.Count);
		assertEquals(task1000.Id, tasks[0].Id);
		assertEquals(task500.Id, tasks[1].Id);
		assertEquals(task250.Id, tasks[2].Id);
	  }

	  public virtual void testQueryByParentTaskId()
	  {
		string parentTaskId = "parentTask";
		Task parent = taskService.newTask(parentTaskId);
		taskService.saveTask(parent);

		Task sub1 = taskService.newTask("subTask1");
		sub1.ParentTaskId = parentTaskId;
		taskService.saveTask(sub1);

		Task sub2 = taskService.newTask("subTask2");
		sub2.ParentTaskId = parentTaskId;
		taskService.saveTask(sub2);

		TaskQuery query = taskService.createTaskQuery().taskParentTaskId(parentTaskId);

		verifyQueryResults(query, 2);

		taskService.deleteTask(parentTaskId, true);
	  }

	  public virtual void testExtendTaskQueryList_ProcessDefinitionKeyIn()
	  {
		// given
		string processDefinitionKey = "invoice";
		TaskQuery query = taskService.createTaskQuery().processDefinitionKeyIn(processDefinitionKey);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		// when
		TaskQuery result = ((TaskQueryImpl)query).extend(extendingQuery);

		// then
		string[] processDefinitionKeys = ((TaskQueryImpl) result).ProcessDefinitionKeys;
		assertEquals(1, processDefinitionKeys.Length);
		assertEquals(processDefinitionKey, processDefinitionKeys[0]);
	  }

	  public virtual void testExtendingTaskQueryList_ProcessDefinitionKeyIn()
	  {
		// given
		string processDefinitionKey = "invoice";
		TaskQuery query = taskService.createTaskQuery();

		TaskQuery extendingQuery = taskService.createTaskQuery().processDefinitionKeyIn(processDefinitionKey);

		// when
		TaskQuery result = ((TaskQueryImpl)query).extend(extendingQuery);

		// then
		string[] processDefinitionKeys = ((TaskQueryImpl) result).ProcessDefinitionKeys;
		assertEquals(1, processDefinitionKeys.Length);
		assertEquals(processDefinitionKey, processDefinitionKeys[0]);
	  }

	  public virtual void testExtendTaskQueryList_TaskDefinitionKeyIn()
	  {
		// given
		string taskDefinitionKey = "assigneApprover";
		TaskQuery query = taskService.createTaskQuery().taskDefinitionKeyIn(taskDefinitionKey);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		// when
		TaskQuery result = ((TaskQueryImpl)query).extend(extendingQuery);

		// then
		string[] key = ((TaskQueryImpl) result).Keys;
		assertEquals(1, key.Length);
		assertEquals(taskDefinitionKey, key[0]);
	  }

	  public virtual void testExtendingTaskQueryList_TaskDefinitionKeyIn()
	  {
		// given
		string taskDefinitionKey = "assigneApprover";
		TaskQuery query = taskService.createTaskQuery();

		TaskQuery extendingQuery = taskService.createTaskQuery().taskDefinitionKeyIn(taskDefinitionKey);

		// when
		TaskQuery result = ((TaskQueryImpl)query).extend(extendingQuery);

		// then
		string[] key = ((TaskQueryImpl) result).Keys;
		assertEquals(1, key.Length);
		assertEquals(taskDefinitionKey, key[0]);
	  }

	  public virtual void testQueryWithCandidateUsers()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().userTask().camundaCandidateUsers("anna").endEvent().done();

		deployment(process);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).withCandidateUsers().list();
		assertEquals(1, tasks.Count);
	  }

	  public virtual void testQueryWithoutCandidateUsers()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().userTask().camundaCandidateGroups("sales").endEvent().done();

		deployment(process);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).withoutCandidateUsers().list();
		assertEquals(1, tasks.Count);
	  }

	  public virtual void testQueryAssignedTasksWithCandidateUsers()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().userTask().camundaCandidateGroups("sales").endEvent().done();

		deployment(process);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		try
		{
		  taskService.createTaskQuery().processInstanceId(processInstance.Id).includeAssignedTasks().withCandidateUsers().list();
		   fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }


	  public virtual void testQueryAssignedTasksWithoutCandidateUsers()
	  {
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().userTask().camundaCandidateGroups("sales").endEvent().done();

		deployment(process);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		try
		{
		   taskService.createTaskQuery().processInstanceId(processInstance.Id).includeAssignedTasks().withoutCandidateUsers().list();
		   fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNameNotEqual()
	  {
		TaskQuery query = taskService.createTaskQuery().taskNameNotEqual("gonzo_Task");
		assertEquals(11, query.list().size());
	  }

	  public virtual void testQueryByNameNotLike()
	  {
		TaskQuery query = taskService.createTaskQuery().taskNameNotLike("management%");
		assertEquals(9, query.list().size());
		assertEquals(9, query.count());

		query = taskService.createTaskQuery().taskNameNotLike("gonzo\\_%");
		assertEquals(11, query.list().size());
		assertEquals(11, query.count());
	  }

	  /// <summary>
	  /// Generates some test tasks.
	  /// - 6 tasks where kermit is a candidate
	  /// - 1 tasks where gonzo is assignee and kermit and gonzo are candidates
	  /// - 2 tasks assigned to management group
	  /// - 2 tasks assigned to accountancy group
	  /// - 1 task assigned to fozzie and to both the management and accountancy group
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.List<String> generateTestTasks() throws Exception
	  private IList<string> generateTestTasks()
	  {
		IList<string> ids = new List<string>();

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");
		// 6 tasks for kermit
		ClockUtil.CurrentTime = sdf.parse("01/01/2001 01:01:01.000");
		for (int i = 0; i < 6; i++)
		{
		  Task task = taskService.newTask();
		  task.Name = "testTask";
		  task.Description = "testTask description";
		  task.Priority = 3;
		  taskService.saveTask(task);
		  ids.Add(task.Id);
		  taskService.addCandidateUser(task.Id, "kermit");
		}

		ClockUtil.CurrentTime = sdf.parse("02/02/2002 02:02:02.000");
		// 1 task for gonzo
		Task task = taskService.newTask();
		task.Name = "gonzo_Task";
		task.Description = "gonzo_description";
		task.Priority = 4;
		taskService.saveTask(task);
		taskService.setAssignee(task.Id, "gonzo_");
		taskService.setVariable(task.Id, "testVar", "someVariable");
		taskService.addCandidateUser(task.Id, "kermit");
		taskService.addCandidateUser(task.Id, "gonzo");
		ids.Add(task.Id);

		ClockUtil.CurrentTime = sdf.parse("03/03/2003 03:03:03.000");
		// 2 tasks for management group
		for (int i = 0; i < 2; i++)
		{
		  task = taskService.newTask();
		  task.Name = "managementTask";
		  task.Priority = 10;
		  taskService.saveTask(task);
		  taskService.addCandidateGroup(task.Id, "management");
		  ids.Add(task.Id);
		}

		ClockUtil.CurrentTime = sdf.parse("04/04/2004 04:04:04.000");
		// 2 tasks for accountancy group
		for (int i = 0; i < 2; i++)
		{
		  task = taskService.newTask();
		  task.Name = "accountancyTask";
		  task.Name = "accountancy description";
		  taskService.saveTask(task);
		  taskService.addCandidateGroup(task.Id, "accountancy");
		  ids.Add(task.Id);
		}

		ClockUtil.CurrentTime = sdf.parse("05/05/2005 05:05:05.000");
		// 1 task assigned to management and accountancy group
		task = taskService.newTask();
		task.Name = "managementAndAccountancyTask";
		taskService.saveTask(task);
		taskService.setAssignee(task.Id, "fozzie");
		taskService.addCandidateGroup(task.Id, "management");
		taskService.addCandidateGroup(task.Id, "accountancy");
		ids.Add(task.Id);

		return ids;
	  }

	}

}