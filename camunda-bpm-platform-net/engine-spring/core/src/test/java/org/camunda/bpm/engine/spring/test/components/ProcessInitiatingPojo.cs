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
namespace org.camunda.bpm.engine.spring.test.components
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BusinessKey = org.camunda.bpm.engine.spring.annotations.BusinessKey;
	using ProcessVariable = org.camunda.bpm.engine.spring.annotations.ProcessVariable;
	using StartProcess = org.camunda.bpm.engine.spring.annotations.StartProcess;

	/// <summary>
	/// simple class that demonstrates the annotations to implicitly handle annotation-driven process managment
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
	public class ProcessInitiatingPojo
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger log = Logger.getLogger(this.GetType().FullName);

		private int methodState = 0;

		public virtual void reset()
		{
			this.methodState = 0;
		}

		public virtual ScopedCustomer Customer
		{
			set
			{
				this.customer = value;
			}
		}

		private ScopedCustomer customer;

		public virtual void logScopedCustomer(ProcessInstance processInstance)
		{
			 Console.WriteLine("ProcessInstance ID:" + processInstance.Id + "; Name: " + this.customer.Name);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @StartProcess(processKey = "b") public void startProcess(@ProcessVariable("customerId") long customerId)
		[StartProcess(processKey : "b")]
		public virtual void startProcess(long customerId)
		{
			log.info("starting 'b' with customerId # " + customerId);
			this.methodState += 1;
			log.info("up'd the method state");
		}

		public virtual int MethodState
		{
			get
			{
				return methodState;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @StartProcess(processKey = "waiter", returnProcessInstanceId = true) public String startProcessA(@ProcessVariable("customerId") long cId)
		[StartProcess(processKey : "waiter", returnProcessInstanceId : true)]
		public virtual string startProcessA(long cId)
		{
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @StartProcess(processKey = "waiter") public org.camunda.bpm.engine.runtime.ProcessInstance enrollCustomer(@BusinessKey String key, @ProcessVariable("customerId") long customerId)
		[StartProcess(processKey : "waiter")]
		public virtual ProcessInstance enrollCustomer(string key, long customerId)
		{
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @StartProcess(processKey = "component-waiter") public void startScopedProcess(@ProcessVariable("customerId") long customerId)
		[StartProcess(processKey : "component-waiter")]
		public virtual void startScopedProcess(long customerId)
		{
			log.info(" start scoped 'component-waiter' process.");
		}


	}

}