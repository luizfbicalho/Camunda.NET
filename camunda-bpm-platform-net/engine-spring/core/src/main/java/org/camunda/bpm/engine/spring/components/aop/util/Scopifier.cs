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
namespace org.camunda.bpm.engine.spring.components.aop.util
{
	using ScopedProxyUtils = org.springframework.aop.scope.ScopedProxyUtils;
	using BeanDefinition = org.springframework.beans.factory.config.BeanDefinition;
	using BeanDefinitionHolder = org.springframework.beans.factory.config.BeanDefinitionHolder;
	using BeanDefinitionVisitor = org.springframework.beans.factory.config.BeanDefinitionVisitor;
	using BeanDefinitionReaderUtils = org.springframework.beans.factory.support.BeanDefinitionReaderUtils;
	using BeanDefinitionRegistry = org.springframework.beans.factory.support.BeanDefinitionRegistry;
	using StringValueResolver = org.springframework.util.StringValueResolver;

	/// <summary>
	/// this class was copied wholesale from Spring 3.1's RefreshScope, which Dave Syer wrote.
	/// 
	/// @author Dave Syer
	/// </summary>
	public class Scopifier : BeanDefinitionVisitor
	{

		private readonly bool proxyTargetClass;

		private readonly BeanDefinitionRegistry registry;

		private readonly string scope;

		private readonly bool scoped;

		public static BeanDefinitionHolder createScopedProxy(string beanName, BeanDefinition definition, BeanDefinitionRegistry registry, bool proxyTargetClass)
		{
			BeanDefinitionHolder proxyHolder = ScopedProxyUtils.createScopedProxy(new BeanDefinitionHolder(definition, beanName), registry, proxyTargetClass);
			registry.registerBeanDefinition(beanName, proxyHolder.BeanDefinition);
			return proxyHolder;
		}

		public Scopifier(BeanDefinitionRegistry registry, string scope, bool proxyTargetClass, bool scoped) : base(new StringValueResolverAnonymousInnerClass())
		{
			this.registry = registry;
			this.proxyTargetClass = proxyTargetClass;
			this.scope = scope;
			this.scoped = scoped;
		}

		private class StringValueResolverAnonymousInnerClass : StringValueResolver
		{
			public string resolveStringValue(string value)
			{
				return value;
			}
		}

		protected internal override object resolveValue(object value)
		{

			BeanDefinition definition = null;
			string beanName = null;
			if (value is BeanDefinition)
			{
				definition = (BeanDefinition) value;
				beanName = BeanDefinitionReaderUtils.generateBeanName(definition, registry);
			}
			else if (value is BeanDefinitionHolder)
			{
				BeanDefinitionHolder holder = (BeanDefinitionHolder) value;
				definition = holder.BeanDefinition;
				beanName = holder.BeanName;
			}

			if (definition != null)
			{
				bool nestedScoped = scope.Equals(definition.Scope);
				bool scopeChangeRequiresProxy = !scoped && nestedScoped;
				if (scopeChangeRequiresProxy)
				{
					// Exit here so that nested inner bean definitions are not
					// analysed
					return createScopedProxy(beanName, definition, registry, proxyTargetClass);
				}
			}

			// Nested inner bean definitions are recursively analysed here
			value = base.resolveValue(value);
			return value;
		}
	}

}