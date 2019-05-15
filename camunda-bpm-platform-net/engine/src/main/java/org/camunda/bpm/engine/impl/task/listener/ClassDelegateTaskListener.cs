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
namespace org.camunda.bpm.engine.impl.task.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.instantiateDelegate;

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ClassDelegate = org.camunda.bpm.engine.impl.@delegate.ClassDelegate;
	using TaskListenerInvocation = org.camunda.bpm.engine.impl.task.@delegate.TaskListenerInvocation;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ClassDelegateTaskListener : ClassDelegate, TaskListener
	{

	  public ClassDelegateTaskListener(string className, IList<FieldDeclaration> fieldDeclarations) : base(className, fieldDeclarations)
	  {
	  }

	  public ClassDelegateTaskListener(Type clazz, IList<FieldDeclaration> fieldDeclarations) : base(clazz, fieldDeclarations)
	  {
	  }

	  public virtual void notify(DelegateTask delegateTask)
	  {
		TaskListener taskListenerInstance = TaskListenerInstance;
		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new TaskListenerInvocation(taskListenerInstance, delegateTask));

		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Exception while invoking TaskListener: " + e.Message, e);
		}
	  }

	  protected internal virtual TaskListener TaskListenerInstance
	  {
		  get
		  {
			object delegateInstance = instantiateDelegate(className, fieldDeclarations);
    
			if (delegateInstance is TaskListener)
			{
			  return (TaskListener) delegateInstance;
    
			}
			else
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ProcessEngineException(delegateInstance.GetType().FullName + " doesn't implement " + typeof(TaskListener));
			}
		  }
	  }

	}

}