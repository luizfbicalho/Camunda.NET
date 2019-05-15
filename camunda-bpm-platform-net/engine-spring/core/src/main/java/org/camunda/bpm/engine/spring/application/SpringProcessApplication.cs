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
namespace org.camunda.bpm.engine.spring.application
{

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationReferenceImpl = org.camunda.bpm.application.impl.ProcessApplicationReferenceImpl;
	using BeansException = org.springframework.beans.BeansException;
	using BeanNameAware = org.springframework.beans.factory.BeanNameAware;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using ApplicationContextAware = org.springframework.context.ApplicationContextAware;
	using ApplicationListener = org.springframework.context.ApplicationListener;
	using ApplicationContextEvent = org.springframework.context.@event.ApplicationContextEvent;
	using ContextClosedEvent = org.springframework.context.@event.ContextClosedEvent;
	using ContextRefreshedEvent = org.springframework.context.@event.ContextRefreshedEvent;

	/// <summary>
	/// <para>Process Application implementation to be used in a Spring Application.</para>
	/// 
	/// <para>This implementation is meant to be bootstrapped by a Spring Application Context.
	/// You can either reference the bean in a Spring application-context XML file or use
	/// spring annotation-based bootstrapping from a subclass.</para>
	/// 
	/// <para><strong>HINT:</strong> If your application is a Web Application, consider using the
	/// <seealso cref="SpringServletProcessApplication"/></para>
	/// 
	/// <para>The SpringProcessApplication will use the Bean Name assigned to the bean in the spring
	/// application context (see <seealso cref="BeanNameAware"/>). You should always assign a unique bean name
	/// to a process application bean. That is, the bean name must be unique accross all applications
	/// deployed to the camunda BPM platform.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpringProcessApplication : AbstractProcessApplication, ApplicationContextAware, BeanNameAware, ApplicationListener<ApplicationContextEvent>
	{

	  protected internal IDictionary<string, string> properties = new Dictionary<string, string>();
	  protected internal ApplicationContext applicationContext;
	  protected internal string beanName;

	  protected internal override string autodetectProcessApplicationName()
	  {
		return beanName;
	  }

	  public override ProcessApplicationReference Reference
	  {
		  get
		  {
			return new ProcessApplicationReferenceImpl(this);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setApplicationContext(org.springframework.context.ApplicationContext applicationContext) throws org.springframework.beans.BeansException
	  public virtual ApplicationContext ApplicationContext
	  {
		  set
		  {
			this.applicationContext = value;
		  }
		  get
		  {
			return applicationContext;
		  }
	  }

	  public virtual string BeanName
	  {
		  set
		  {
			this.beanName = value;
		  }
	  }

	  public override IDictionary<string, string> Properties
	  {
		  get
		  {
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }



	  public override void onApplicationEvent(ApplicationContextEvent @event)
	  {
		try
		{
		  // we only want to listen for context events of the main application
		  // context, not its children
		  if (@event.Source.Equals(applicationContext))
		  {
			if (@event is ContextRefreshedEvent && !isDeployed)
			{
			  // deploy the process application
			  afterPropertiesSet();
			}
			else if (@event is ContextClosedEvent)
			{
			  // undeploy the process application
			  destroy();
			}
			else
			{
			  // ignore
			}
		  }
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	  public virtual void start()
	  {
		deploy();
	  }

	  public virtual void stop()
	  {
		undeploy();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
	  public virtual void afterPropertiesSet()
	  {
		// for backwards compatibility
		start();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws Exception
	  public virtual void destroy()
	  {
		// for backwards compatibility
		stop();
	  }

	}

}