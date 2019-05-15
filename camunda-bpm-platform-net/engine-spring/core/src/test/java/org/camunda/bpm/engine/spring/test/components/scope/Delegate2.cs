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
namespace org.camunda.bpm.engine.spring.test.components.scope
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Assert = org.junit.Assert;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;

	/// <summary>
	/// @author Josh Long
	/// @since 5,3
	/// </summary>

	public class Delegate2 : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger log = Logger.getLogger(this.GetType().FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private StatefulObject statefulObject;
		private StatefulObject statefulObject;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{

			this.statefulObject.increment();

			Assert.assertNotNull("the 'scopedCustomer' reference can't be null", this.statefulObject);
			Assert.assertNotNull("the 'scopedCustomer.name' property should be non-null, since it was set in a previous delegate bound to this very thread", this.statefulObject.Name);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			log.info("the 'uuid' value retrieved from the ScopedCustomer#name property is '" + this.statefulObject.Name + "' in " + this.GetType().FullName);
		}
	}

}