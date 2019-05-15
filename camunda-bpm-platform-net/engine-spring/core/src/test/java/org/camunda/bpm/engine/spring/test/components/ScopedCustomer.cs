using System;
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
namespace org.camunda.bpm.engine.spring.test.components
{
	using ProcessScope = org.camunda.bpm.engine.spring.components.scope.ProcessScope;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using Scope = org.springframework.context.annotation.Scope;
	using Component = org.springframework.stereotype.Component;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Scope(ProcessScope.PROCESS_SCOPE_NAME) public class ScopedCustomer implements java.io.Serializable, org.springframework.beans.factory.InitializingBean
	[Serializable]
	public class ScopedCustomer : InitializingBean
	{
		public ScopedCustomer()
		{
		}
		public ScopedCustomer(string name)
		{
			this.name = name;
		}

		private string name = Thread.CurrentThread.Id + ":" + DateTimeHelper.CurrentUnixTimeMillis();

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void afterPropertiesSet() throws Exception
		public virtual void afterPropertiesSet()
		{
		 Console.WriteLine("starting ..." + this.name);
		}
	}

}