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
namespace org.camunda.bpm.engine.spring.components.config.xml
{
	using ActivitiStateHandlerRegistry = org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistry;
	using BeansException = org.springframework.beans.BeansException;
	using BeanDefinition = org.springframework.beans.factory.config.BeanDefinition;
	using BeanDefinitionHolder = org.springframework.beans.factory.config.BeanDefinitionHolder;
	using BeanFactoryPostProcessor = org.springframework.beans.factory.config.BeanFactoryPostProcessor;
	using ConfigurableListableBeanFactory = org.springframework.beans.factory.config.ConfigurableListableBeanFactory;
	using BeanDefinitionReaderUtils = org.springframework.beans.factory.support.BeanDefinitionReaderUtils;
	using BeanDefinitionRegistry = org.springframework.beans.factory.support.BeanDefinitionRegistry;
	using RootBeanDefinition = org.springframework.beans.factory.support.RootBeanDefinition;

	/// <summary>
	/// this class is responsible for registering the other <seealso cref="org.springframework.beans.factory.config.BeanFactoryPostProcessor"/>s
	/// and <seealso cref="BeanFactoryPostProcessor"/>s.
	/// <p/>
	/// Particularly, this will register the <seealso cref="ActivitiStateHandlerRegistry"/> which is used to react to states.
	/// 
	/// @author Josh Long
	/// </summary>
	public class StateHandlerAnnotationBeanFactoryPostProcessor : BeanFactoryPostProcessor
	{

		private ProcessEngine processEngine;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger log = Logger.getLogger(this.GetType().FullName);

		public virtual ProcessEngine ProcessEngine
		{
			set
			{
				this.processEngine = value;
			}
		}

		private void configureDefaultActivitiRegistry(string registryBeanName, BeanDefinitionRegistry registry)
		{


			if (!beanAlreadyConfigured(registry, registryBeanName, typeof(ActivitiStateHandlerRegistry)))
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				string registryName = typeof(ActivitiStateHandlerRegistry).FullName;
				log.info("registering a " + registryName + " instance under bean name " + ActivitiContextUtils.ACTIVITI_REGISTRY_BEAN_NAME + ".");

				RootBeanDefinition rootBeanDefinition = new RootBeanDefinition();
				rootBeanDefinition.BeanClassName = registryName;
				rootBeanDefinition.PropertyValues.addPropertyValue("processEngine", this.processEngine);

				BeanDefinitionHolder holder = new BeanDefinitionHolder(rootBeanDefinition, registryBeanName);
				BeanDefinitionReaderUtils.registerBeanDefinition(holder, registry);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void postProcessBeanFactory(org.springframework.beans.factory.config.ConfigurableListableBeanFactory beanFactory) throws org.springframework.beans.BeansException
		public virtual void postProcessBeanFactory(ConfigurableListableBeanFactory beanFactory)
		{
			if (beanFactory is BeanDefinitionRegistry)
			{
				BeanDefinitionRegistry registry = (BeanDefinitionRegistry) beanFactory;
				configureDefaultActivitiRegistry(ActivitiContextUtils.ACTIVITI_REGISTRY_BEAN_NAME, registry);


			}
			else
			{
				log.info("BeanFactory is not a BeanDefinitionRegistry. The default '" + ActivitiContextUtils.ACTIVITI_REGISTRY_BEAN_NAME + "' cannot be configured.");
			}
		}

		private bool beanAlreadyConfigured(BeanDefinitionRegistry registry, string beanName, Type clz)
		{
			if (registry.isBeanNameInUse(beanName))
			{
				BeanDefinition bDef = registry.getBeanDefinition(beanName);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				if (bDef.BeanClassName.Equals(clz.FullName))
				{
					return true; // so the beans already registered, and of the right type. so we assume the user is overriding our configuration
				}
				else
				{
					throw new System.InvalidOperationException("The bean name '" + beanName + "' is reserved.");
				}
			}
			return false;
		}
	}

}