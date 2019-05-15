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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.ASSIGNEE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.CASE_INSTANCE_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DELEGATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.DUE_DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.FOLLOW_UP_DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.OWNER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.PARENT_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.PRIORITY;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogTaskServiceAndBeanTest : AbstractUserOperationLogTest
	{

	  protected internal Task task;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();

		if (task != null)
		{
		  taskService.deleteTask(task.Id, true);
		}
	  }

	  public virtual void testBeanPropertyChanges()
	  {
		TaskEntity entity = new TaskEntity();

		// assign and validate changes
		entity.Assignee = "icke";
		IDictionary<string, PropertyChange> changes = entity.PropertyChanges;
		assertEquals(1, changes.Count);
		assertNull(changes[ASSIGNEE].OrgValue);
		assertEquals("icke", changes[ASSIGNEE].NewValue);

		// assign it again
		entity.Assignee = "er";
		changes = entity.PropertyChanges;
		assertEquals(1, changes.Count);

		// original value is still null because the task was not saved
		assertNull(changes[ASSIGNEE].OrgValue);
		assertEquals("er", changes[ASSIGNEE].NewValue);

		// set a due date
		entity.DueDate = DateTime.Now;
		changes = entity.PropertyChanges;
		assertEquals(2, changes.Count);
	  }

	  public virtual void testNotTrackChangeToTheSameValue()
	  {
		TaskEntity entity = new TaskEntity();

		// get and set a properties
		entity.Priority = entity.Priority;
		entity.Owner = entity.Owner;
		entity.FollowUpDate = entity.FollowUpDate;

		// should not track this change
		assertTrue(entity.PropertyChanges.Count == 0);
	  }

	  public virtual void testRemoveChangeWhenSetBackToTheOrgValue()
	  {
		TaskEntity entity = new TaskEntity();

		// set an owner (default is null)
		entity.Owner = "icke";

		// should track this change
		assertFalse(entity.PropertyChanges.Count == 0);

		// reset the owner
		entity.Owner = null;

		// the change is removed
		assertTrue(entity.PropertyChanges.Count == 0);
	  }

	  public virtual void testAllTrackedProperties()
	  {
		DateTime yesterday = new DateTime((DateTime.Now).Ticks - 86400000);
		DateTime tomorrow = new DateTime((DateTime.Now).Ticks + 86400000);

		TaskEntity entity = new TaskEntity();

		// call all tracked setter methods
		entity.Assignee = "er";
		entity.DelegationState = DelegationState.PENDING;
		entity.Deleted = true;
		entity.Description = "a description";
		entity.DueDate = tomorrow;
		entity.FollowUpDate = yesterday;
		entity.Name = "to do";
		entity.Owner = "icke";
		entity.ParentTaskId = "parent";
		entity.Priority = 73;

		// and validate the change list
		IDictionary<string, PropertyChange> changes = entity.PropertyChanges;
		assertEquals("er", changes[ASSIGNEE].NewValue);
		assertSame(DelegationState.PENDING, changes[DELEGATION].NewValue);
		assertTrue((bool?) changes[DELETE].NewValue);
		assertEquals("a description", changes[DESCRIPTION].NewValue);
		assertEquals(tomorrow, changes[DUE_DATE].NewValue);
		assertEquals(yesterday, changes[FOLLOW_UP_DATE].NewValue);
		assertEquals("to do", changes[NAME].NewValue);
		assertEquals("icke", changes[OWNER].NewValue);
		assertEquals("parent", changes[PARENT_TASK].NewValue);
		assertEquals(73, changes[PRIORITY].NewValue);
	  }

	  public virtual void testDeleteTask()
	  {
		// given: a single task
		task = taskService.newTask();
		taskService.saveTask(task);

		// then: delete the task
		taskService.deleteTask(task.Id, "duplicated");

		// expect: one entry for the deletion
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_DELETE);
		assertEquals(1, query.count());

		// assert: details
		UserOperationLogEntry delete = query.singleResult();
		assertEquals(DELETE, delete.Property);
		assertFalse(bool.Parse(delete.OrgValue));
		assertTrue(bool.Parse(delete.NewValue));
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, delete.Category);
	  }

	  public virtual void testCompositeBeanInteraction()
	  {
		// given: a manually created task
		task = taskService.newTask();

		// then: save the task without any property change
		taskService.saveTask(task);

		// expect: no entry
		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_CREATE);
		UserOperationLogEntry create = query.singleResult();
		assertNotNull(create);
		assertEquals(EntityTypes.TASK, create.EntityType);
		assertNull(create.OrgValue);
		assertNull(create.NewValue);
		assertNull(create.Property);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, create.Category);

		task.Assignee = "icke";
		task.Name = "to do";

		// then: save the task again
		taskService.saveTask(task);

		// expect: two update entries with the same operation id
		IList<UserOperationLogEntry> entries = queryOperationDetails(OPERATION_TYPE_UPDATE).list();
		assertEquals(2, entries.Count);
		assertEquals(entries[0].OperationId, entries[1].OperationId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, entries[0].Category);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, entries[1].Category);
	  }

	  public virtual void testMultipleValueChange()
	  {
		// given: a single task
		task = taskService.newTask();
		taskService.saveTask(task);

		// then: change a property twice
		task.Name = "a task";
		task.Name = "to do";
		taskService.saveTask(task);
		UserOperationLogEntry update = queryOperationDetails(OPERATION_TYPE_UPDATE).singleResult();
		assertNull(update.OrgValue);
		assertEquals("to do", update.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, update.Category);
	  }

	  public virtual void testSetDateProperty()
	  {
		// given: a single task
		task = taskService.newTask();
		DateTime now = ClockUtil.CurrentTime;
		task.DueDate = now;
		taskService.saveTask(task);

		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertEquals(now.Ticks.ToString(), logEntry.NewValue);
	  }

	  public virtual void testResetChange()
	  {
		// given: a single task
		task = taskService.newTask();
		taskService.saveTask(task);

		// then: change the name
		string name = "a task";
		task.Name = name;
		taskService.saveTask(task);
		UserOperationLogEntry update = queryOperationDetails(OPERATION_TYPE_UPDATE).singleResult();
		assertNull(update.OrgValue);
		assertEquals(name, update.NewValue);

		// then: change the name some times and set it back to the original value
		task.Name = "to do 1";
		task.Name = "to do 2";
		task.Name = name;
		taskService.saveTask(task);

		// expect: there is no additional change tracked
		update = queryOperationDetails(OPERATION_TYPE_UPDATE).singleResult();
		assertNull(update.OrgValue);
		assertEquals(name, update.NewValue);
	  }

	  public virtual void testConcurrentTaskChange()
	  {
		// create a task
		task = taskService.newTask();
		taskService.saveTask(task);

		// change the bean property
		task.Assignee = "icke";

		// use the service method to do an other assignment
		taskService.setAssignee(task.Id, "er");

		try
		{ // now try to save the task and overwrite the change
		  taskService.saveTask(task);
		}
		catch (Exception e)
		{
		  assertNotNull(e); // concurrent modification
		}
	  }

	  public virtual void testCaseInstanceId()
	  {
		// create new task
		task = taskService.newTask();
		taskService.saveTask(task);

		UserOperationLogQuery query = queryOperationDetails(OPERATION_TYPE_UPDATE);
		assertEquals(0, query.count());

		// set case instance id and save task
		task.CaseInstanceId = "aCaseInstanceId";
		taskService.saveTask(task);

		assertEquals(1, query.count());

		UserOperationLogEntry entry = query.singleResult();
		assertNotNull(entry);

		assertNull(entry.OrgValue);
		assertEquals("aCaseInstanceId", entry.NewValue);
		assertEquals(CASE_INSTANCE_ID, entry.Property);

		// change case instance id and save task
		task.CaseInstanceId = "anotherCaseInstanceId";
		taskService.saveTask(task);

		assertEquals(2, query.count());

		IList<UserOperationLogEntry> entries = query.list();
		assertEquals(2, entries.Count);

		foreach (UserOperationLogEntry currentEntry in entries)
		{
		  if (!currentEntry.Id.Equals(entry.Id))
		  {
			assertEquals("aCaseInstanceId", currentEntry.OrgValue);
			assertEquals("anotherCaseInstanceId", currentEntry.NewValue);
			assertEquals(CASE_INSTANCE_ID, currentEntry.Property);
		  }
		}
	  }

	  private UserOperationLogQuery queryOperationDetails(string type)
	  {
		return historyService.createUserOperationLogQuery().operationType(type);
	  }

	}

}