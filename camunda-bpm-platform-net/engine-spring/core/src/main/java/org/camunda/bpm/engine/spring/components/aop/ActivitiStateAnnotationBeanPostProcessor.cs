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

	using ProcessId = org.camunda.bpm.engine.spring.annotations.ProcessId;
	using ProcessVariable = org.camunda.bpm.engine.spring.annotations.ProcessVariable;
	using ProcessVariables = org.camunda.bpm.engine.spring.annotations.ProcessVariables;
	using State = org.camunda.bpm.engine.spring.annotations.State;
	using ActivitiStateHandlerRegistration = org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistration;
	using ActivitiStateHandlerRegistry = org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistry;
	using AopUtils = org.springframework.aop.support.AopUtils;
	using BeansException = org.springframework.beans.BeansException;
	using BeanClassLoaderAware = org.springframework.beans.factory.BeanClassLoaderAware;
	using BeanFactory = org.springframework.beans.factory.BeanFactory;
	using BeanFactoryAware = org.springframework.beans.factory.BeanFactoryAware;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using BeanPostProcessor = org.springframework.beans.factory.config.BeanPostProcessor;
	using Ordered = org.springframework.core.Ordered;
	using AnnotationUtils = org.springframework.core.annotation.AnnotationUtils;
	using Assert = org.springframework.util.Assert;
	using ClassUtils = org.springframework.util.ClassUtils;
	using ReflectionUtils = org.springframework.util.ReflectionUtils;
	using StringUtils = org.springframework.util.StringUtils;


	/// <summary>
	/// the idea is that this bean post processor is responsible for registering all beans
	/// that have the <seealso cref="org.camunda.bpm.engine.annotations.State"/> annotation.
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") public class ActivitiStateAnnotationBeanPostProcessor implements org.springframework.beans.factory.config.BeanPostProcessor, org.springframework.beans.factory.BeanClassLoaderAware, org.springframework.beans.factory.BeanFactoryAware, org.springframework.beans.factory.InitializingBean, org.springframework.core.Ordered
	public class ActivitiStateAnnotationBeanPostProcessor : BeanPostProcessor, BeanClassLoaderAware, BeanFactoryAware, InitializingBean, Ordered
	{

		private volatile ActivitiStateHandlerRegistry registry;

		private volatile int order = Ordered.LOWEST_PRECEDENCE;

		private volatile BeanFactory beanFactory;

		private volatile ClassLoader beanClassLoader = ClassUtils.DefaultClassLoader;

		public virtual BeanFactory BeanFactory
		{
			set
			{
				this.beanFactory = value;
			}
		}

		public virtual ClassLoader BeanClassLoader
		{
			set
			{
				this.beanClassLoader = value;
			}
		}

		public virtual int Order
		{
			get
			{
				return this.order;
			}
		}

		public virtual void afterPropertiesSet()
		{
			Assert.notNull(this.beanClassLoader, "beanClassLoader must not be null");
			Assert.notNull(this.beanFactory, "beanFactory must not be null");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object postProcessBeforeInitialization(Object bean, String beanName) throws org.springframework.beans.BeansException
		public virtual object postProcessBeforeInitialization(object bean, string beanName)
		{
			return bean;
		}

		public virtual ActivitiStateHandlerRegistry Registry
		{
			set
			{
				this.registry = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object postProcessAfterInitialization(final Object bean, final String beanName) throws org.springframework.beans.BeansException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual object postProcessAfterInitialization(object bean, string beanName)
		{
			// first sift through and get all the methods
			// then get all the annotations
			// then build the metadata and register the metadata
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class targetClass = org.springframework.aop.support.AopUtils.getTargetClass(bean);
			Type targetClass = AopUtils.getTargetClass(bean);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent component = targetClass.getAnnotation(org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent.class);
			org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent component = targetClass.getAnnotation(typeof(org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent));

			ReflectionUtils.doWithMethods(targetClass, new MethodCallbackAnonymousInnerClass(this, bean, beanName, component)
				   , new MethodFilterAnonymousInnerClass(this));

			return bean;
		}

		private class MethodCallbackAnonymousInnerClass : ReflectionUtils.MethodCallback
		{
			private readonly ActivitiStateAnnotationBeanPostProcessor outerInstance;

			private object bean;
			private string beanName;
			private org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent component;

			public MethodCallbackAnonymousInnerClass(ActivitiStateAnnotationBeanPostProcessor outerInstance, object bean, string beanName, org.camunda.bpm.engine.spring.annotations.ProcessEngineComponent component)
			{
				this.outerInstance = outerInstance;
				this.bean = bean;
				this.beanName = beanName;
				this.component = component;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void doWith(Method method) throws IllegalArgumentException, IllegalAccessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
			public void doWith(System.Reflection.MethodInfo method)
			{

				State state = AnnotationUtils.getAnnotation(method, typeof(State));

				string processName = component.processKey();

				if (StringUtils.hasText(state.process()))
				{
					processName = state.process();
				}

				string stateName = state.state();

				if (!StringUtils.hasText(stateName))
				{
					stateName = state.value();
				}

				Assert.notNull(stateName, "You must provide a stateName!");

				IDictionary<int, string> vars = new Dictionary<int, string>();
				Annotation[][] paramAnnotationsArray = method.ParameterAnnotations;

				int ctr = 0;
				int pvMapIndex = -1;
				int procIdIndex = -1;

				foreach (Annotation[] paramAnnotations in paramAnnotationsArray)
				{
					ctr += 1;

					foreach (Annotation pa in paramAnnotations)
					{
						if (pa is ProcessVariable)
						{
							ProcessVariable pv = (ProcessVariable) pa;
							string pvName = pv.value();
							vars[ctr] = pvName;
						}
						else if (pa is ProcessVariables)
						{
							pvMapIndex = ctr;
						}
						else if (pa is ProcessId)
						{
							procIdIndex = ctr;
						}
					}
				}

				ActivitiStateHandlerRegistration registration = new ActivitiStateHandlerRegistration(vars, method, bean, stateName, beanName, pvMapIndex, procIdIndex, processName);
				outerInstance.registry.registerActivitiStateHandler(registration);
			}
		}

		private class MethodFilterAnonymousInnerClass : ReflectionUtils.MethodFilter
		{
			private readonly ActivitiStateAnnotationBeanPostProcessor outerInstance;

			public MethodFilterAnonymousInnerClass(ActivitiStateAnnotationBeanPostProcessor outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public bool matches(System.Reflection.MethodInfo method)
			{
				return null != AnnotationUtils.getAnnotation(method, typeof(State));
			}
		}
	}

}