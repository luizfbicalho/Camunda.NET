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

	using ProcessStartAnnotationBeanPostProcessor = org.camunda.bpm.engine.spring.components.aop.ProcessStartAnnotationBeanPostProcessor;
	using ProcessScope = org.camunda.bpm.engine.spring.components.scope.ProcessScope;
	using BeanDefinition = org.springframework.beans.factory.config.BeanDefinition;
	using BeanDefinitionHolder = org.springframework.beans.factory.config.BeanDefinitionHolder;
	using RuntimeBeanReference = org.springframework.beans.factory.config.RuntimeBeanReference;
	using AbstractBeanDefinition = org.springframework.beans.factory.support.AbstractBeanDefinition;
	using BeanDefinitionBuilder = org.springframework.beans.factory.support.BeanDefinitionBuilder;
	using BeanDefinitionReaderUtils = org.springframework.beans.factory.support.BeanDefinitionReaderUtils;
	using BeanDefinitionParser = org.springframework.beans.factory.xml.BeanDefinitionParser;
	using ParserContext = org.springframework.beans.factory.xml.ParserContext;
	using Conventions = org.springframework.core.Conventions;
	using StringUtils = org.springframework.util.StringUtils;
	using Element = org.w3c.dom.Element;

	/// <summary>
	/// registers support for handling the annotations in the org.camunda.bpm.engine.annotations package.
	/// <p/>
	/// The first major component is the state handlers. For this to work, a BeanFactoryPostProcessor is registered which in turn registers a
	/// <seealso cref="org.camunda.bpm.engine.test.spring.components.registry.ActivitiStateHandlerRegistry"/> if none exists.
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
	public class ActivitiAnnotationDrivenBeanDefinitionParser : BeanDefinitionParser
	{

		private readonly string processEngineAttribute = "process-engine";

		public virtual BeanDefinition parse(Element element, ParserContext parserContext)
		{
			registerProcessScope(element, parserContext);
			registerStateHandlerAnnotationBeanFactoryPostProcessor(element, parserContext);
			registerProcessStartAnnotationBeanPostProcessor(element, parserContext);
			return null;
		}

		private void configureProcessEngine(AbstractBeanDefinition abstractBeanDefinition, Element element)
		{
			string procEngineRef = element.getAttribute(processEngineAttribute);
			if (StringUtils.hasText(procEngineRef))
			{
				abstractBeanDefinition.PropertyValues.add(Conventions.attributeNameToPropertyName(processEngineAttribute), new RuntimeBeanReference(procEngineRef));
			}
		}

		private void registerStateHandlerAnnotationBeanFactoryPostProcessor(Element element, ParserContext context)
		{
			Type clz = typeof(StateHandlerAnnotationBeanFactoryPostProcessor);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			BeanDefinitionBuilder postProcessorBuilder = BeanDefinitionBuilder.genericBeanDefinition(clz.FullName);

			BeanDefinitionHolder postProcessorHolder = new BeanDefinitionHolder(postProcessorBuilder.BeanDefinition, ActivitiContextUtils.ANNOTATION_STATE_HANDLER_BEAN_FACTORY_POST_PROCESSOR_BEAN_NAME);
			configureProcessEngine(postProcessorBuilder.BeanDefinition, element);
			BeanDefinitionReaderUtils.registerBeanDefinition(postProcessorHolder, context.Registry);

		}

		private void registerProcessScope(Element element, ParserContext parserContext)
		{
			Type clz = typeof(ProcessScope);
			BeanDefinitionBuilder processScopeBDBuilder = BeanDefinitionBuilder.genericBeanDefinition(clz);
			AbstractBeanDefinition scopeBeanDefinition = processScopeBDBuilder.BeanDefinition;
			scopeBeanDefinition.Role = BeanDefinition.ROLE_INFRASTRUCTURE;
			configureProcessEngine(scopeBeanDefinition, element);
			string beanName = baseBeanName(clz);
			parserContext.Registry.registerBeanDefinition(beanName, scopeBeanDefinition);
		}

		private void registerProcessStartAnnotationBeanPostProcessor(Element element, ParserContext parserContext)
		{
			Type clz = typeof(ProcessStartAnnotationBeanPostProcessor);

			BeanDefinitionBuilder beanDefinitionBuilder = BeanDefinitionBuilder.genericBeanDefinition(clz);
			AbstractBeanDefinition beanDefinition = beanDefinitionBuilder.BeanDefinition;
			beanDefinition.Role = BeanDefinition.ROLE_INFRASTRUCTURE;
			configureProcessEngine(beanDefinition, element);

			string beanName = baseBeanName(clz);
			parserContext.Registry.registerBeanDefinition(beanName, beanDefinition);
		}

		private string baseBeanName(Type cl)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return cl.FullName.ToLower();
		}
	}


}