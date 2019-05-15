﻿/*
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
namespace org.camunda.bpm.engine.spring.test.transaction
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Transactional = org.springframework.transaction.annotation.Transactional;


	public class ServiceTaskBean : JavaDelegate
	{

	  private BeanWithException beanWithException;

	  public virtual BeanWithException BeanWithException
	  {
		  set
		  {
			this.beanWithException = value;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transactional @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void execute(DelegateExecution execution)
	  {
		beanWithException.doSomething();
	  }

	}

}