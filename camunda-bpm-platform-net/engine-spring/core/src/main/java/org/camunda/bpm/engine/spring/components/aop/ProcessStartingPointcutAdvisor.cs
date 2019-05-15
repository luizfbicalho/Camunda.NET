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
namespace org.camunda.bpm.engine.spring.components.aop
{

	using Advice = org.aopalliance.aop.Advice;
	using MethodInterceptor = org.aopalliance.intercept.MethodInterceptor;
	using StartProcess = org.camunda.bpm.engine.spring.annotations.StartProcess;
	using MetaAnnotationMatchingPointcut = org.camunda.bpm.engine.spring.components.aop.util.MetaAnnotationMatchingPointcut;
	using Pointcut = org.springframework.aop.Pointcut;
	using PointcutAdvisor = org.springframework.aop.PointcutAdvisor;
	using ComposablePointcut = org.springframework.aop.support.ComposablePointcut;

	/// <summary>
	/// AOP advice for methods annotated with (by default) <seealso cref="StartProcess"/>.
	/// 
	/// Advised methods start a process after the method executes.
	/// 
	/// Advised methods can declare a return
	/// type of <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance"/> and then subsequently
	/// return null. The real return ProcessInstance value will be given by the aspect.
	/// 
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
	[Serializable]
	public class ProcessStartingPointcutAdvisor : PointcutAdvisor
	{


		/// <summary>
		/// annotations that shall be scanned
		/// </summary>
		private ISet<Type> annotations = new HashSet<Type>(Arrays.asList(typeof(StartProcess)));

		/// <summary>
		/// the <seealso cref="org.aopalliance.intercept.MethodInterceptor"/> that handles launching the business process.
		/// </summary>
		protected internal MethodInterceptor advice;

		/// <summary>
		/// matches any method containing the <seealso cref="StartProcess"/> annotation.
		/// </summary>
		protected internal Pointcut pointcut;

		/// <summary>
		/// the injected reference to the <seealso cref="org.camunda.bpm.engine.ProcessEngine"/>
		/// </summary>
		protected internal ProcessEngine processEngine;

		public ProcessStartingPointcutAdvisor(ProcessEngine pe)
		{
			this.processEngine = pe;
			this.pointcut = buildPointcut();
			this.advice = buildAdvise();

		}

		protected internal virtual MethodInterceptor buildAdvise()
		{
			return new ProcessStartingMethodInterceptor(this.processEngine);
		}

		public virtual Pointcut Pointcut
		{
			get
			{
				return pointcut;
			}
		}

		public virtual Advice Advice
		{
			get
			{
				return advice;
			}
		}

		public virtual bool PerInstance
		{
			get
			{
				return true;
			}
		}

		private Pointcut buildPointcut()
		{
			ComposablePointcut result = null;
			foreach (Type publisherAnnotationType in this.annotations)
			{
				Pointcut mpc = new MetaAnnotationMatchingPointcut(null, publisherAnnotationType);
				if (result == null)
				{
					result = new ComposablePointcut(mpc);
				}
				else
				{
					result.union(mpc);
				}
			}
			return result;
		}


	}


}