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
namespace org.camunda.bpm.engine.spring.test.application
{
	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using SpringProcessApplication = org.camunda.bpm.engine.spring.application.SpringProcessApplication;
	using BeansException = org.springframework.beans.BeansException;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using ApplicationEvent = org.springframework.context.ApplicationEvent;
	using ApplicationListener = org.springframework.context.ApplicationListener;
	using AnnotationConfigApplicationContext = org.springframework.context.annotation.AnnotationConfigApplicationContext;
	using AbstractApplicationContext = org.springframework.context.support.AbstractApplicationContext;

	public class PostDeployWithNestedContext : SpringProcessApplication
	{

	  public class MyEvent : ApplicationEvent
	  {
		  private readonly PostDeployWithNestedContext outerInstance;

		internal const long serialVersionUID = 1L;

		public MyEvent(PostDeployWithNestedContext outerInstance, object source) : base(source)
		{
			this.outerInstance = outerInstance;
		}
	  }

	  internal bool deployCalled = false;
	  internal bool triggered = false;
	  internal bool deployOnChildRefresh;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setApplicationContext(org.springframework.context.ApplicationContext mainContext) throws org.springframework.beans.BeansException
	  public override ApplicationContext ApplicationContext
	  {
		  set
		  {
			base.ApplicationContext = value;
    
			AnnotationConfigApplicationContext nestedContext = new AnnotationConfigApplicationContext();
			nestedContext.Parent = value;
    
			deployCalled = false;
			nestedContext.refresh();
			deployOnChildRefresh = deployCalled;
    
			((AbstractApplicationContext) value).addApplicationListener(new ApplicationListenerAnonymousInnerClass(this));
		  }
	  }

	  private class ApplicationListenerAnonymousInnerClass : ApplicationListener<MyEvent>
	  {
		  private readonly PostDeployWithNestedContext outerInstance;

		  public ApplicationListenerAnonymousInnerClass(PostDeployWithNestedContext outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public override void onApplicationEvent(MyEvent @event)
		  {
			outerInstance.triggered = true;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostDeploy public void registerProcessApplication(org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual void registerProcessApplication(ProcessEngine processEngine)
	  {
		deployCalled = true;
		applicationContext.publishEvent(new MyEvent(this, this));
	  }

	  public virtual bool DeployOnChildRefresh
	  {
		  get
		  {
			return deployOnChildRefresh;
		  }
	  }

	  public virtual bool LateEventTriggered
	  {
		  get
		  {
			return triggered;
		  }
	  }

	}

}