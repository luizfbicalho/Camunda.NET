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
namespace org.camunda.bpm.engine.cdi.test.impl.@event
{
	using org.camunda.bpm.engine.cdi.annotation.@event;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ApplicationScoped public class TestEventListener
	public class TestEventListener
	{

	  public virtual void reset()
	  {
		startActivityService1 = 0;
		endActivityService1 = 0;
		takeTransition1_Conflict = 0;
		createTaskUser1 = 0;
		assignTaskUser1 = 0;
		completeTaskUser1 = 0;
		deleteTaskUser1 = 0;

		eventsReceivedByKey.Clear();
		eventsReceived.Clear();
	  }

	  private readonly ISet<BusinessProcessEvent> eventsReceivedByKey = new HashSet<BusinessProcessEvent>();

	  // receives all events related to "process1"
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onProcessEventByKey(@Observes @BusinessProcessDefinition("process1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onProcessEventByKey(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		assertEquals("process1", businessProcessEvent.ProcessDefinition.Key);
		eventsReceivedByKey.Add(businessProcessEvent);
	  }

	  public virtual ISet<BusinessProcessEvent> EventsReceivedByKey
	  {
		  get
		  {
			return eventsReceivedByKey;
		  }
	  }


	  // ---------------------------------------------------------

	  private readonly ISet<BusinessProcessEvent> eventsReceived = new HashSet<BusinessProcessEvent>();

	  // receives all events
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onProcessEvent(@Observes BusinessProcessEvent businessProcessEvent)
	  public virtual void onProcessEvent(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		eventsReceived.Add(businessProcessEvent);
	  }

	  public virtual ISet<BusinessProcessEvent> EventsReceived
	  {
		  get
		  {
			return eventsReceived;
		  }
	  }

	  // ---------------------------------------------------------

	  private int startActivityService1 = 0;
	  private int endActivityService1 = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private int takeTransition1_Conflict = 0;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onStartActivityService1(@Observes @StartActivity("service1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onStartActivityService1(BusinessProcessEvent businessProcessEvent)
	  {
		assertEquals("service1", businessProcessEvent.ActivityId);
		assertNotNull(businessProcessEvent);
		assertNull(businessProcessEvent.Task);
		assertNull(businessProcessEvent.TaskId);
		assertNull(businessProcessEvent.TaskDefinitionKey);
		startActivityService1 += 1;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onEndActivityService1(@Observes @EndActivity("service1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onEndActivityService1(BusinessProcessEvent businessProcessEvent)
	  {
		assertEquals("service1", businessProcessEvent.ActivityId);
		assertNotNull(businessProcessEvent);
		assertNull(businessProcessEvent.Task);
		assertNull(businessProcessEvent.TaskId);
		assertNull(businessProcessEvent.TaskDefinitionKey);
		endActivityService1 += 1;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void takeTransition1(@Observes @TakeTransition("t1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void takeTransition1(BusinessProcessEvent businessProcessEvent)
	  {
		assertEquals("t1", businessProcessEvent.TransitionName);
		assertNotNull(businessProcessEvent);
		assertNull(businessProcessEvent.Task);
		assertNull(businessProcessEvent.TaskId);
		assertNull(businessProcessEvent.TaskDefinitionKey);
		takeTransition1_Conflict += 1;
	  }

	  public virtual int EndActivityService1
	  {
		  get
		  {
			return endActivityService1;
		  }
	  }

	  public virtual int StartActivityService1
	  {
		  get
		  {
			return startActivityService1;
		  }
	  }

	  public virtual int TakeTransition1
	  {
		  get
		  {
			return takeTransition1_Conflict;
		  }
	  }


	  // ---------------------------------------------------------

	  private int createTaskUser1 = 0;
	  private int assignTaskUser1 = 0;
	  private int completeTaskUser1 = 0;
	  private int deleteTaskUser1 = 0;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onCreateTask(@Observes @CreateTask("user1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onCreateTask(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		assertNotNull(businessProcessEvent.Task);
		assertNotNull(businessProcessEvent.TaskId);
		assertEquals("user1", businessProcessEvent.TaskDefinitionKey);
		createTaskUser1++;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onAssignTask(@Observes @AssignTask("user1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onAssignTask(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		assertNotNull(businessProcessEvent.Task);
		assertNotNull(businessProcessEvent.TaskId);
		assertEquals("user1", businessProcessEvent.TaskDefinitionKey);
		assignTaskUser1++;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onCompleteTask(@Observes @CompleteTask("user1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onCompleteTask(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		assertNotNull(businessProcessEvent.Task);
		assertNotNull(businessProcessEvent.TaskId);
		assertEquals("user1", businessProcessEvent.TaskDefinitionKey);
		completeTaskUser1++;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public void onDeleteTask(@Observes @DeleteTask("user1") org.camunda.bpm.engine.cdi.BusinessProcessEvent businessProcessEvent)
	  public virtual void onDeleteTask(BusinessProcessEvent businessProcessEvent)
	  {
		assertNotNull(businessProcessEvent);
		assertNotNull(businessProcessEvent.Task);
		assertNotNull(businessProcessEvent.TaskId);
		assertEquals("user1", businessProcessEvent.TaskDefinitionKey);
		deleteTaskUser1++;
	  }

	  public virtual int CreateTaskUser1
	  {
		  get
		  {
			return createTaskUser1;
		  }
	  }

	  public virtual int AssignTaskUser1
	  {
		  get
		  {
			return assignTaskUser1;
		  }
	  }

	  public virtual int CompleteTaskUser1
	  {
		  get
		  {
			return completeTaskUser1;
		  }
	  }

	  public virtual int DeleteTaskUser1
	  {
		  get
		  {
			return deleteTaskUser1;
		  }
	  }

	}

}