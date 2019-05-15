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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.instantiateDelegate;


	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;


	/// <summary>
	/// Helper class for bpmn constructs that allow class delegation.
	/// 
	/// This class will lazily instantiate the referenced classes when needed at runtime.
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Roman Smirnov
	/// </summary>
	public class ClassDelegateActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal string className;
	  protected internal IList<FieldDeclaration> fieldDeclarations;

	  public ClassDelegateActivityBehavior(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.className = className;
		this.fieldDeclarations = fieldDeclarations;
	  }

	  public ClassDelegateActivityBehavior(Type clazz, IList<FieldDeclaration> fieldDeclarations) : this(clazz.FullName, fieldDeclarations)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  }

	  // Activity Behavior
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void execute(ActivityExecution execution)
	  {
		this.executeWithErrorPropagation(execution, new CallableAnonymousInnerClass(this, execution));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ClassDelegateActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(ClassDelegateActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			outerInstance.getActivityBehaviorInstance(execution).execute(execution);
			return null;
		  }
	  }

	  // Signallable activity behavior
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final String signalName, final Object signalData) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		ProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
		if (ProcessApplicationContextUtil.requiresContextSwitch(targetProcessApplication))
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass2(this, execution, signalName, signalData)
		 , targetProcessApplication, new InvocationContext(execution));
		}
		else
		{
		  doSignal(execution, signalName, signalData);
		}
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly ClassDelegateActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private string signalName;
		  private object signalData;

		  public CallableAnonymousInnerClass2(ClassDelegateActivityBehavior outerInstance, ActivityExecution execution, string signalName, object signalData)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.signalName = signalName;
			  this.signalData = signalData;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.signal(execution, signalName, signalData);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doSignal(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final String signalName, final Object signalData) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void doSignal(ActivityExecution execution, string signalName, object signalData)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.delegate.ActivityBehavior activityBehaviorInstance = getActivityBehaviorInstance(execution);
		ActivityBehavior activityBehaviorInstance = getActivityBehaviorInstance(execution);

		if (activityBehaviorInstance is CustomActivityBehavior)
		{
		  CustomActivityBehavior behavior = (CustomActivityBehavior) activityBehaviorInstance;
		  ActivityBehavior @delegate = behavior.DelegateActivityBehavior;

		  if (!(@delegate is SignallableActivityBehavior))
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.incorrectlyUsedSignalException(typeof(SignallableActivityBehavior).FullName);
		  }
		}
		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass3(this, execution, signalName, signalData, activityBehaviorInstance));
	  }

	  private class CallableAnonymousInnerClass3 : Callable<Void>
	  {
		  private readonly ClassDelegateActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private string signalName;
		  private object signalData;
		  private ActivityBehavior activityBehaviorInstance;

		  public CallableAnonymousInnerClass3(ClassDelegateActivityBehavior outerInstance, ActivityExecution execution, string signalName, object signalData, ActivityBehavior activityBehaviorInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.signalName = signalName;
			  this.signalData = signalData;
			  this.activityBehaviorInstance = activityBehaviorInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			((SignallableActivityBehavior) activityBehaviorInstance).signal(execution, signalName, signalData);
			return null;
		  }
	  }

	  protected internal virtual ActivityBehavior getActivityBehaviorInstance(ActivityExecution execution)
	  {
		object delegateInstance = instantiateDelegate(className, fieldDeclarations);

		if (delegateInstance is ActivityBehavior)
		{
		  return new CustomActivityBehavior((ActivityBehavior) delegateInstance);
		}
		else if (delegateInstance is JavaDelegate)
		{
		  return new ServiceTaskJavaDelegateActivityBehavior((JavaDelegate) delegateInstance);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.missingDelegateParentClassException(delegateInstance.GetType().FullName, typeof(JavaDelegate).FullName, typeof(ActivityBehavior).FullName);
		}
	  }

	}

}