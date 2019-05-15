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
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.applyFieldDeclaration;


	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ActivityBehaviorInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.ActivityBehaviorInvocation;
	using JavaDelegateInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.JavaDelegateInvocation;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;


	/// <summary>
	/// <seealso cref="ActivityBehavior"/> used when 'delegateExpression' is used
	/// for a serviceTask.
	/// 
	/// @author Joram Barrez
	/// @author Josh Long
	/// @author Slawomir Wojtasiak (Patch for ACT-1159)
	/// @author Falko Menge
	/// </summary>
	public class ServiceTaskDelegateExpressionActivityBehavior : TaskActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public ServiceTaskDelegateExpressionActivityBehavior(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final String signalName, final Object signalData) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		ProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
		if (ProcessApplicationContextUtil.requiresContextSwitch(targetProcessApplication))
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, execution, signalName, signalData)
		 , targetProcessApplication, new InvocationContext(execution));
		}
		else
		{
		  doSignal(execution, signalName, signalData);
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ServiceTaskDelegateExpressionActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private string signalName;
		  private object signalData;

		  public CallableAnonymousInnerClass(ServiceTaskDelegateExpressionActivityBehavior outerInstance, ActivityExecution execution, string signalName, object signalData)
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
//ORIGINAL LINE: public void doSignal(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final String signalName, final Object signalData) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void doSignal(ActivityExecution execution, string signalName, object signalData)
	  {
		object @delegate = expression.getValue(execution);
		applyFieldDeclaration(fieldDeclarations, @delegate);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.delegate.ActivityBehavior activityBehaviorInstance = getActivityBehaviorInstance(execution, delegate);
		ActivityBehavior activityBehaviorInstance = getActivityBehaviorInstance(execution, @delegate);

		if (activityBehaviorInstance is CustomActivityBehavior)
		{
		  CustomActivityBehavior behavior = (CustomActivityBehavior) activityBehaviorInstance;
		  ActivityBehavior delegateActivityBehavior = behavior.DelegateActivityBehavior;

		  if (!(delegateActivityBehavior is SignallableActivityBehavior))
		  {
			// legacy behavior: do nothing when it is not a signallable activity behavior
			return;
		  }
		}
		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass2(this, execution, signalName, signalData, activityBehaviorInstance));
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly ServiceTaskDelegateExpressionActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private string signalName;
		  private object signalData;
		  private ActivityBehavior activityBehaviorInstance;

		  public CallableAnonymousInnerClass2(ServiceTaskDelegateExpressionActivityBehavior outerInstance, ActivityExecution execution, string signalName, object signalData, ActivityBehavior activityBehaviorInstance)
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void performExecution(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public override void performExecution(ActivityExecution execution)
		{
		  Callable<Void> callable = new CallableAnonymousInnerClass3(this, execution);
		executeWithErrorPropagation(execution, callable);
		}

	  private class CallableAnonymousInnerClass3 : Callable<Void>
	  {
		  private readonly ServiceTaskDelegateExpressionActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass3(ServiceTaskDelegateExpressionActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			// Note: we can't cache the result of the expression, because the
			// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
			object @delegate = outerInstance.expression.getValue(execution);
			applyFieldDeclaration(outerInstance.fieldDeclarations, @delegate);

			if (@delegate is ActivityBehavior)
			{
			  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ActivityBehaviorInvocation((ActivityBehavior) @delegate, execution));

			}
			else if (@delegate is JavaDelegate)
			{
			  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new JavaDelegateInvocation((JavaDelegate) @delegate, execution));
			  outerInstance.leave(execution);

			}
			else
			{
			  throw LOG.resolveDelegateExpressionException(outerInstance.expression, typeof(ActivityBehavior), typeof(JavaDelegate));
			}
			return null;
		  }
	  }

	  protected internal virtual ActivityBehavior getActivityBehaviorInstance(ActivityExecution execution, object delegateInstance)
	  {

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