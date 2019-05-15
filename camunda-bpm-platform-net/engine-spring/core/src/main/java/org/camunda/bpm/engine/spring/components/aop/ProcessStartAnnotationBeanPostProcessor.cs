using System;

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
	using Log = org.apache.commons.logging.Log;
	using LogFactory = org.apache.commons.logging.LogFactory;
	using ProcessVariable = org.camunda.bpm.engine.spring.annotations.ProcessVariable;
	using StartProcess = org.camunda.bpm.engine.spring.annotations.StartProcess;
	using Advised = org.springframework.aop.framework.Advised;
	using AopInfrastructureBean = org.springframework.aop.framework.AopInfrastructureBean;
	using ProxyConfig = org.springframework.aop.framework.ProxyConfig;
	using ProxyFactory = org.springframework.aop.framework.ProxyFactory;
	using AopUtils = org.springframework.aop.support.AopUtils;
	using BeansException = org.springframework.beans.BeansException;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using BeanPostProcessor = org.springframework.beans.factory.config.BeanPostProcessor;
	using ClassUtils = org.springframework.util.ClassUtils;

	/// <summary>
	/// Proxies beans with methods annotated with <seealso cref="StartProcess"/>.
	/// If the method is invoked successfully, the process described by the annotaton is created.
	/// Parameters passed to the method annotated with <seealso cref="ProcessVariable"/>
	/// are passed to the business process.
	/// 
	/// @author Josh Long
	/// @since 5,3
	/// </summary>
	public class ProcessStartAnnotationBeanPostProcessor : ProxyConfig, BeanPostProcessor, InitializingBean
	{

		private Log log = LogFactory.getLog(this.GetType());

		/// <summary>
		/// the process engine as created by a <seealso cref="org.camunda.bpm.engine.spring.ProcessEngineFactoryBean"/>
		/// </summary>
		private ProcessEngine processEngine;

		private ProcessStartingPointcutAdvisor advisor;

		private volatile ClassLoader beanClassLoader = ClassUtils.DefaultClassLoader;

		public virtual ProcessEngine ProcessEngine
		{
			set
			{
				this.processEngine = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
		public virtual void afterPropertiesSet()
		{
			this.advisor = new ProcessStartingPointcutAdvisor(this.processEngine);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object postProcessAfterInitialization(Object bean, String beanName) throws org.springframework.beans.BeansException
		public virtual object postProcessAfterInitialization(object bean, string beanName)
		{
			 if (bean is AopInfrastructureBean)
			 {
				// Ignore AOP infrastructure such as scoped proxies.
				return bean;
			 }
			Type targetClass = AopUtils.getTargetClass(bean);
			if (AopUtils.canApply(this.advisor, targetClass))
			{
				if (bean is Advised)
				{
					((Advised) bean).addAdvisor(0, this.advisor);
					return bean;
				}
				else
				{
					ProxyFactory proxyFactory = new ProxyFactory(bean);
					// Copy our properties (proxyTargetClass etc) inherited from ProxyConfig.
					proxyFactory.copyFrom(this);
					proxyFactory.addAdvisor(this.advisor);
					return proxyFactory.getProxy(this.beanClassLoader);
				}
			}
			else
			{
				// No async proxy needed.
				return bean;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object postProcessBeforeInitialization(Object bean, String beanName) throws org.springframework.beans.BeansException
		public virtual object postProcessBeforeInitialization(object bean, string beanName)
		{
			return bean;
		}
	}

}