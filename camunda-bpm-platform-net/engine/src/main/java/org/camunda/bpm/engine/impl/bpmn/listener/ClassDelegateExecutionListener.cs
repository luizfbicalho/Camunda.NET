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
namespace org.camunda.bpm.engine.impl.bpmn.listener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.instantiateDelegate;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using BpmnBehaviorLogger = org.camunda.bpm.engine.impl.bpmn.behavior.BpmnBehaviorLogger;
	using ServiceTaskJavaDelegateActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ServiceTaskJavaDelegateActivityBehavior;
	using ExecutionListenerInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.ExecutionListenerInvocation;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ClassDelegate = org.camunda.bpm.engine.impl.@delegate.ClassDelegate;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ClassDelegateExecutionListener : ClassDelegate, ExecutionListener
	{

	  protected internal static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  public ClassDelegateExecutionListener(string className, IList<FieldDeclaration> fieldDeclarations) : base(className, fieldDeclarations)
	  {
	  }

	  public ClassDelegateExecutionListener(Type clazz, IList<FieldDeclaration> fieldDeclarations) : base(clazz, fieldDeclarations)
	  {
	  }

	  // Execution listener
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		ExecutionListener executionListenerInstance = ExecutionListenerInstance;

		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ExecutionListenerInvocation(executionListenerInstance, execution));
	  }

	  protected internal virtual ExecutionListener ExecutionListenerInstance
	  {
		  get
		  {
			object delegateInstance = instantiateDelegate(className, fieldDeclarations);
			if (delegateInstance is ExecutionListener)
			{
			  return (ExecutionListener) delegateInstance;
    
			}
			else if (delegateInstance is JavaDelegate)
			{
			  return new ServiceTaskJavaDelegateActivityBehavior((JavaDelegate) delegateInstance);
    
			}
			else
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw LOG.missingDelegateParentClassException(delegateInstance.GetType().FullName, typeof(ExecutionListener).FullName, typeof(JavaDelegate).FullName);
			}
		  }
	  }

	}

}