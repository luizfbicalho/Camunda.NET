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
	using ActivityBehaviorInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.ActivityBehaviorInvocation;
	using ActivityBehaviorSignalInvocation = org.camunda.bpm.engine.impl.bpmn.@delegate.ActivityBehaviorSignalInvocation;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CustomActivityBehavior : ActivityBehavior, SignallableActivityBehavior
	{

	  protected internal ActivityBehavior delegateActivityBehavior;

	  public CustomActivityBehavior(ActivityBehavior activityBehavior)
	  {
		this.delegateActivityBehavior = activityBehavior;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ActivityBehaviorInvocation(delegateActivityBehavior, execution));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalEvent, object signalData)
	  {
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ActivityBehaviorSignalInvocation((SignallableActivityBehavior) delegateActivityBehavior, execution, signalEvent, signalData));
	  }

	  public virtual ActivityBehavior DelegateActivityBehavior
	  {
		  get
		  {
			return delegateActivityBehavior;
		  }
	  }

	}

}