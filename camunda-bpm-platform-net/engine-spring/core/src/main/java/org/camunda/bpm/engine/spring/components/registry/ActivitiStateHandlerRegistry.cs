using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
namespace org.camunda.bpm.engine.spring.components.registry
{
	using ReceiveTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ReceiveTaskActivityBehavior;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using BeansException = org.springframework.beans.BeansException;
	using BeanFactory = org.springframework.beans.factory.BeanFactory;
	using BeanFactoryAware = org.springframework.beans.factory.BeanFactoryAware;
	using BeanNameAware = org.springframework.beans.factory.BeanNameAware;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using Assert = org.springframework.util.Assert;



	/// <summary>
	/// this class records and manages all known <seealso cref="org.camunda.bpm.engine.annotations.State"/> - responding
	/// beans in the JVM. It <em>should</em> have metadata on all methods, and what
	/// those methods expect from a given invocation (ie: which process, which process variables).
	/// 
	/// @author Josh Long
	/// @since 1.0
	/// </summary>
	public class ActivitiStateHandlerRegistry : ReceiveTaskActivityBehavior, BeanFactoryAware, BeanNameAware, ActivityBehavior, InitializingBean
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger logger = Logger.getLogger(this.GetType().FullName);

		private string beanName;

		private BeanFactory beanFactory;

		private volatile ConcurrentDictionary<string, ActivitiStateHandlerRegistration> registrations = new ConcurrentDictionary<string, ActivitiStateHandlerRegistration>();

		private ProcessEngine processEngine;

		public virtual ProcessEngine ProcessEngine
		{
			set
			{
				this.processEngine = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object data) throws Exception
		public virtual void signal(ActivityExecution execution, string signalName, object data)
		{
			leave(execution);
		}

		protected internal virtual string registrationKey(string stateName, string processName)
		{
			return (org.camunda.commons.utils.StringUtil.defaultString(processName) + ":" + org.camunda.commons.utils.StringUtil.defaultString(stateName)).ToLower();
		}

		/// <summary>
		/// used at runtime to register state handlers as they are registered with the spring context
		/// </summary>
		/// <param name="registration"> the <seealso cref="org.camunda.bpm.engine.test.spring.components.registry.ActivitiStateHandlerRegistration"/> </param>
		public virtual void registerActivitiStateHandler(ActivitiStateHandlerRegistration registration)
		{
			string regKey = registrationKey(registration.ProcessName, registration.StateName);
			this.registrations[regKey] = registration;
		}

		/// <summary>
		/// this is responsible for looking up components in the registry and returning the appropriate handler based
		/// on specificity of the <seealso cref="org.camunda.bpm.engine.test.spring.components.registry.ActivitiStateHandlerRegistration"/>
		/// </summary>
		/// <param name="processName"> the process name to look for (optional) </param>
		/// <param name="stateName">	 the state name to look for (not optional) </param>
		/// <returns> all matching options </returns>
		public virtual ICollection<ActivitiStateHandlerRegistration> findRegistrationsForProcessAndState(string processName, string stateName)
		{
			ICollection<ActivitiStateHandlerRegistration> registrationCollection = new List<ActivitiStateHandlerRegistration>();
			string regKeyFull = registrationKey(processName, stateName);
			string regKeyWithJustState = registrationKey(null, stateName);

			foreach (string k in this.registrations.Keys)
			{
				if (k.Contains(regKeyFull))
				{
					registrationCollection.Add(this.registrations[k]);
				}
			}

			if (registrationCollection.Count == 0)
			{
				foreach (string k in this.registrations.Keys)
				{
					if (k.Contains(regKeyWithJustState))
					{
						registrationCollection.Add(this.registrations[k]);
					}
				}
			}

			return registrationCollection;
		}

		/// <summary>
		/// this scours the registry looking for candidate registrations that match a given process name and/ or state nanme
		/// </summary>
		/// <param name="processName"> the name of the process </param>
		/// <param name="stateName">	 the name of the state </param>
		/// <returns> an unambiguous <seealso cref="org.camunda.bpm.engine.test.spring.components.registry.ActivitiStateHandlerRegistry"/> or null </returns>
		public virtual ActivitiStateHandlerRegistration findRegistrationForProcessAndState(string processName, string stateName)
		{

			ActivitiStateHandlerRegistration r = null;

			string key = registrationKey(processName, stateName);

			ICollection<ActivitiStateHandlerRegistration> rs = this.findRegistrationsForProcessAndState(processName, stateName);

			foreach (ActivitiStateHandlerRegistration sr in rs)
			{
				string kName = registrationKey(sr.ProcessName, sr.StateName);
				if (key.Equals(kName, StringComparison.OrdinalIgnoreCase))
				{
					r = sr;
					break;
				}
			}

			foreach (ActivitiStateHandlerRegistration sr in rs)
			{
				string kName = registrationKey(null, sr.StateName);
				if (key.Equals(kName, StringComparison.OrdinalIgnoreCase))
				{
					r = sr;
					break;
				}
			}

			if ((r == null) && (rs.Count > 0))
			{
				r = rs.GetEnumerator().next();
			}

			return r;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBeanFactory(org.springframework.beans.factory.BeanFactory beanFactory) throws org.springframework.beans.BeansException
		public virtual BeanFactory BeanFactory
		{
			set
			{
				this.beanFactory = value;
			}
		}

		public virtual string BeanName
		{
			set
			{
				this.beanName = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
		public virtual void afterPropertiesSet()
		{
			Assert.notNull(this.processEngine, "the 'processEngine' can't be null");
			logger.info("this bean contains a processEngine reference. " + this.processEngine);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			logger.info("starting " + this.GetType().FullName);
		}
	}

}