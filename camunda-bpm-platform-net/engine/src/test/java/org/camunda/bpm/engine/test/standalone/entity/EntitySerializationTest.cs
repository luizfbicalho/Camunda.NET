using System;
using System.IO;

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
namespace org.camunda.bpm.engine.test.standalone.entity
{
	using TestCase = junit.framework.TestCase;

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;

	public class EntitySerializationTest : TestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testTaskEntitySerialization() throws Exception
	  public virtual void testTaskEntitySerialization()
	  {
		TaskEntity task = new TaskEntity();
		task.DelegationState = DelegationState.RESOLVED;
		task.setExecution(new ExecutionEntity());
		task.ProcessInstance = new ExecutionEntity();
		task.TaskDefinition = new TaskDefinition(null);

		task.Assignee = "kermit";
		task.CreateTime = DateTime.Now;
		task.Description = "Test description";
		task.DueDate = DateTime.Now;
		task.Name = "myTask";
		task.EventName = "end";
		task.Deleted = false;
		task.DelegationStateString = DelegationState.RESOLVED.name();

		sbyte[] data = writeObject(task);
		task = (TaskEntity) readObject(data);

		assertEquals("kermit", task.Assignee);
		assertEquals("myTask", task.Name);
		assertEquals("end", task.EventName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testExecutionEntitySerialization() throws Exception
	  public virtual void testExecutionEntitySerialization()
	  {
	   ExecutionEntity execution = new ExecutionEntity();

	   ActivityImpl activityImpl = new ActivityImpl("test", null);
	   activityImpl.ExecutionListeners["start"] = Collections.singletonList<ExecutionListener>(new TestExecutionListener());
	   execution.setActivity(activityImpl);

	   ProcessDefinitionImpl processDefinitionImpl = new ProcessDefinitionImpl("test");
	   processDefinitionImpl.ExecutionListeners["start"] = Collections.singletonList<ExecutionListener>(new TestExecutionListener());
	   execution.setProcessDefinition(processDefinitionImpl);

	   TransitionImpl transitionImpl = new TransitionImpl("test", new ProcessDefinitionImpl("test"));
	   transitionImpl.addExecutionListener(new TestExecutionListener());
	   execution.Transition = transitionImpl;

	   execution.ProcessInstanceStartContext.Initial = activityImpl;
	   execution.setSuperExecution(new ExecutionEntity());

	   execution.Active = true;
	   execution.Canceled = false;
	   execution.BusinessKey = "myBusinessKey";
	   execution.DeleteReason = "no reason";
	   execution.ActivityInstanceId = "123";
	   execution.Scope = false;

	   sbyte[] data = writeObject(execution);
	   execution = (ExecutionEntity) readObject(data);

	   assertEquals("myBusinessKey", execution.BusinessKey);
	   assertEquals("no reason", execution.DeleteReason);
	   assertEquals("123", execution.ActivityInstanceId);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private byte[] writeObject(Object object) throws java.io.IOException
	  private sbyte[] writeObject(object @object)
	  {
		MemoryStream buffer = new MemoryStream();

		ObjectOutputStream outputStream = new ObjectOutputStream(buffer);
		outputStream.writeObject(@object);
		outputStream.flush();
		outputStream.close();

		return buffer.toByteArray();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readObject(byte[] data) throws java.io.IOException, ClassNotFoundException
	  private object readObject(sbyte[] data)
	  {
		Stream buffer = new MemoryStream(data);
		ObjectInputStream inputStream = new ObjectInputStream(buffer);
		object @object = inputStream.readObject();

		return @object;
	  }

	}

}