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



	/// <summary>
	/// an instance of a bean discovered to both have an <seealso cref="org.camunda.bpm.engine.ProcessEngineComponent.ActivitiComponent"/>
	/// and one or more <seealso cref="org.camunda.bpm.engine.ProcessEngineComponent.ActivitiComponent"/> annotations present.
	/// <p/>
	/// Describes the metadata extracted from the bean at configuration time
	/// 
	/// @author Josh Long
	/// @since 1.0
	/// </summary>
	public class ActivitiStateHandlerRegistration
	{
		private IDictionary<int, string> processVariablesExpected = new ConcurrentDictionary<int, string>();
		private System.Reflection.MethodInfo handlerMethod;
		private object handler;
		private string stateName;
		private string beanName;
		private int processVariablesIndex = -1;
		private int processIdIndex = -1;
		private string processName;

		public ActivitiStateHandlerRegistration(IDictionary<int, string> processVariablesExpected, System.Reflection.MethodInfo handlerMethod, object handler, string stateName, string beanName, int processVariablesIndex, int processIdIndex, string processName)
		{
			this.processVariablesExpected = processVariablesExpected;
			this.handlerMethod = handlerMethod;
			this.handler = handler;
			this.stateName = stateName;
			this.beanName = beanName;
			this.processVariablesIndex = processVariablesIndex;
			this.processIdIndex = processIdIndex;
			this.processName = processName;
		}

		public virtual int ProcessVariablesIndex
		{
			get
			{
				return processVariablesIndex;
			}
		}

		public virtual int ProcessIdIndex
		{
			get
			{
				return processIdIndex;
			}
		}

		public virtual bool requiresProcessId()
		{
			return this.processIdIndex > -1;
		}

		public virtual bool requiresProcessVariablesMap()
		{
			return processVariablesIndex > -1;
		}

		public virtual string BeanName
		{
			get
			{
				return beanName;
			}
		}

		public virtual IDictionary<int, string> ProcessVariablesExpected
		{
			get
			{
				return processVariablesExpected;
			}
		}

		public virtual System.Reflection.MethodInfo HandlerMethod
		{
			get
			{
				return handlerMethod;
			}
		}

		public virtual object Handler
		{
			get
			{
				return handler;
			}
		}

		public virtual string StateName
		{
			get
			{
				return stateName;
			}
		}

		public virtual string ProcessName
		{
			get
			{
				return processName;
			}
		}

		public override string ToString()
		{
		return base.ToString() + "["
			+ "processVariablesExpected=" + processVariablesExpected + ", "
			+ "handlerMethod=" + handlerMethod + ", "
			+ "handler=" + handler + ", "
			+ "stateName=" + stateName + ", "
			+ "beanName=" + beanName + ", "
			+ "processVariablesIndex=" + processVariablesIndex + ", "
			+ "processIdIndex=" + processIdIndex + ", "
			+ "processName=" + processName + "]";
		}
	}

}