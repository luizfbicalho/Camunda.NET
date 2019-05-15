using System;
using System.Collections.Concurrent;
using System.Threading;

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
namespace org.camunda.bpm.engine.spring.components.scope
{

	using MethodInterceptor = org.aopalliance.intercept.MethodInterceptor;
	using MethodInvocation = org.aopalliance.intercept.MethodInvocation;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Scopifier = org.camunda.bpm.engine.spring.components.aop.util.Scopifier;
	using StringUtil = org.camunda.commons.utils.StringUtil;
	using ProxyFactory = org.springframework.aop.framework.ProxyFactory;
	using ScopedObject = org.springframework.aop.scope.ScopedObject;
	using BeansException = org.springframework.beans.BeansException;
	using DisposableBean = org.springframework.beans.factory.DisposableBean;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using ObjectFactory = org.springframework.beans.factory.ObjectFactory;
	using BeanDefinition = org.springframework.beans.factory.config.BeanDefinition;
	using BeanFactoryPostProcessor = org.springframework.beans.factory.config.BeanFactoryPostProcessor;
	using ConfigurableListableBeanFactory = org.springframework.beans.factory.config.ConfigurableListableBeanFactory;
	using Scope = org.springframework.beans.factory.config.Scope;
	using BeanDefinitionRegistry = org.springframework.beans.factory.support.BeanDefinitionRegistry;
	using Assert = org.springframework.util.Assert;
	using ClassUtils = org.springframework.util.ClassUtils;

	/// <summary>
	/// binds variables to a currently executing Activiti business process (a <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance"/>).
	/// <p/>
	/// Parts of this code are lifted wholesale from Dave Syer's work on the Spring 3.1 RefreshScope.
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
	public class ProcessScope : Scope, InitializingBean, BeanFactoryPostProcessor, DisposableBean
	{

		/// <summary>
		/// Map of the processVariables. Supports correct, scoped access to process variables so that
		/// <code>
		/// 
		/// @Value("#{ processVariables['customerId'] }") long customerId;
		/// </code>
		/// <p/>
		/// works in any bean - scoped or not
		/// </summary>
		public const string PROCESS_SCOPE_PROCESS_VARIABLES_SINGLETON = "processVariables";
		public const string PROCESS_SCOPE_NAME = "process";

		private ClassLoader classLoader = ClassUtils.DefaultClassLoader;

		private bool proxyTargetClass = true;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger logger = Logger.getLogger(this.GetType().FullName);

		private ProcessEngine processEngine;

		private RuntimeService runtimeService;

		// set through Namespace reflection if nothing else
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") public void setProcessEngine(org.camunda.bpm.engine.ProcessEngine processEngine)
		public virtual ProcessEngine ProcessEngine
		{
			set
			{
				this.processEngine = value;
			}
		}

		public virtual object get<T1>(string name, ObjectFactory<T1> objectFactory)
		{

			ExecutionEntity executionEntity = null;
			try
			{
				logger.fine("returning scoped object having beanName '" + name + "' for conversation ID '" + this.ConversationId + "'. ");

				ProcessInstance processInstance = Context.ExecutionContext.ProcessInstance;
				executionEntity = (ExecutionEntity) processInstance;

				object scopedObject = executionEntity.getVariable(name);
				if (scopedObject == null)
				{
					scopedObject = objectFactory.Object;
					if (scopedObject is ScopedObject)
					{
						ScopedObject sc = (ScopedObject) scopedObject;
						scopedObject = sc.TargetObject;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						logger.fine("de-referencing " + typeof(ScopedObject).FullName + "#targetObject before persisting variable");
					}
					persistVariable(name, scopedObject);
				}
				return createDirtyCheckingProxy(name, scopedObject);
			}
			catch (Exception th)
			{
				logger.warning("couldn't return value from process scope! " + StringUtil.getStackTrace(th));
			}
			finally
			{
				if (executionEntity != null)
				{
					logger.fine("set variable '" + name + "' on executionEntity# " + executionEntity.Id);
				}
			}
			return null;
		}

		public virtual void registerDestructionCallback(string name, ThreadStart callback)
		{
			logger.fine("no support for registering descruction callbacks implemented currently. registerDestructionCallback('" + name + "',callback) will do nothing.");
		}

		private string ExecutionId
		{
			get
			{
				return Context.ExecutionContext.Execution.Id;
			}
		}

		public virtual object remove(string name)
		{

			logger.fine("remove '" + name + "'");
			return runtimeService.getVariable(ExecutionId, name);
		}

		public virtual object resolveContextualObject(string key)
		{

			if ("executionId".Equals(key, StringComparison.OrdinalIgnoreCase))
			{
				return Context.ExecutionContext.Execution.Id;
			}

			if ("processInstance".Equals(key, StringComparison.OrdinalIgnoreCase))
			{
				return Context.ExecutionContext.ProcessInstance;
			}

			if ("processInstanceId".Equals(key, StringComparison.OrdinalIgnoreCase))
			{
				return Context.ExecutionContext.ProcessInstance.Id;
			}

			return null;
		}

		/// <summary>
		/// creates a proxy that dispatches invocations to the currently bound <seealso cref="ProcessInstance"/>
		/// </summary>
		/// <returns> shareable <seealso cref="ProcessInstance"/> </returns>
		private object createSharedProcessInstance()
		{
			ProxyFactory proxyFactoryBean = new ProxyFactory(typeof(ProcessInstance), new MethodInterceptorAnonymousInnerClass(this));
			return proxyFactoryBean.getProxy(this.classLoader);
		}

		private class MethodInterceptorAnonymousInnerClass : MethodInterceptor
		{
			private readonly ProcessScope outerInstance;

			public MethodInterceptorAnonymousInnerClass(ProcessScope outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(org.aopalliance.intercept.MethodInvocation methodInvocation) throws Throwable
			public object invoke(MethodInvocation methodInvocation)
			{
				string methodName = methodInvocation.Method.Name;

				outerInstance.logger.info("method invocation for " + methodName + ".");
				if (methodName.Equals("toString"))
				{
					return "SharedProcessInstance";
				}


				ProcessInstance processInstance = Context.ExecutionContext.ProcessInstance;
				System.Reflection.MethodInfo method = methodInvocation.Method;
				object[] args = methodInvocation.Arguments;
				object result = method.invoke(processInstance, args);
				return result;
			}
		}

		public virtual string ConversationId
		{
			get
			{
				return ExecutionId;
			}
		}

		private readonly ConcurrentDictionary<string, object> processVariablesMap = new ConcurrentHashMapAnonymousInnerClass();

		private class ConcurrentHashMapAnonymousInnerClass : ConcurrentDictionary<string, object>
		{
			public override object get(object o)
			{

				Assert.isInstanceOf(typeof(string), o, "the 'key' must be a String");

				string varName = (string) o;

				ProcessInstance processInstance = Context.ExecutionContext.ProcessInstance;
				ExecutionEntity executionEntity = (ExecutionEntity) processInstance;
				if (executionEntity.VariableNames.Contains(varName))
				{
					return executionEntity.getVariable(varName);
				}
				throw new Exception("no processVariable by the name of '" + varName + "' is available!");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void postProcessBeanFactory(org.springframework.beans.factory.config.ConfigurableListableBeanFactory beanFactory) throws org.springframework.beans.BeansException
		public virtual void postProcessBeanFactory(ConfigurableListableBeanFactory beanFactory)
		{

			beanFactory.registerScope(ProcessScope.PROCESS_SCOPE_NAME, this);

			Assert.isInstanceOf(typeof(BeanDefinitionRegistry), beanFactory, "BeanFactory was not a BeanDefinitionRegistry, so ProcessScope cannot be used.");

			BeanDefinitionRegistry registry = (BeanDefinitionRegistry) beanFactory;

			foreach (string beanName in beanFactory.BeanDefinitionNames)
			{
				BeanDefinition definition = beanFactory.getBeanDefinition(beanName);
				// Replace this or any of its inner beans with scoped proxy if it has this scope
				bool scoped = PROCESS_SCOPE_NAME.Equals(definition.Scope);
				Scopifier scopifier = new Scopifier(registry, PROCESS_SCOPE_NAME, proxyTargetClass, scoped);
				scopifier.visitBeanDefinition(definition);
				if (scoped)
				{
					Scopifier.createScopedProxy(beanName, definition, registry, proxyTargetClass);
				}
			}

			beanFactory.registerSingleton(ProcessScope.PROCESS_SCOPE_PROCESS_VARIABLES_SINGLETON, this.processVariablesMap);
			beanFactory.registerResolvableDependency(typeof(ProcessInstance), createSharedProcessInstance());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws Exception
		public virtual void destroy()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			logger.info(typeof(ProcessScope).FullName + "#destroy() called ...");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
		public virtual void afterPropertiesSet()
		{
			Assert.notNull(this.processEngine, "the 'processEngine' must not be null!");
			this.runtimeService = this.processEngine.RuntimeService;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object createDirtyCheckingProxy(final String name, final Object scopedObject) throws Throwable
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		private object createDirtyCheckingProxy(string name, object scopedObject)
		{
			ProxyFactory proxyFactoryBean = new ProxyFactory(scopedObject);
			proxyFactoryBean.ProxyTargetClass = this.proxyTargetClass;
			proxyFactoryBean.addAdvice(new MethodInterceptorAnonymousInnerClass2(this, name, scopedObject));
			return proxyFactoryBean.getProxy(this.classLoader);
		}

		private class MethodInterceptorAnonymousInnerClass2 : MethodInterceptor
		{
			private readonly ProcessScope outerInstance;

			private string name;
			private object scopedObject;

			public MethodInterceptorAnonymousInnerClass2(ProcessScope outerInstance, string name, object scopedObject)
			{
				this.outerInstance = outerInstance;
				this.name = name;
				this.scopedObject = scopedObject;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(org.aopalliance.intercept.MethodInvocation methodInvocation) throws Throwable
			public object invoke(MethodInvocation methodInvocation)
			{
				object result = methodInvocation.proceed();
				outerInstance.persistVariable(name, scopedObject);
				return result;
			}
		}

		private void persistVariable(string variableName, object scopedObject)
		{
			ProcessInstance processInstance = Context.ExecutionContext.ProcessInstance;
			ExecutionEntity executionEntity = (ExecutionEntity) processInstance;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Assert.isTrue(scopedObject is Serializable, "the scopedObject is not " + typeof(Serializable).FullName + "!");
			executionEntity.setVariable(variableName, scopedObject);
		}
	}


}