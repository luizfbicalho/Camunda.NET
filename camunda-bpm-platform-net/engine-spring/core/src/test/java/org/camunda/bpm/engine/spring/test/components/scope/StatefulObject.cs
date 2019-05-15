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
namespace org.camunda.bpm.engine.spring.test.components.scope
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using InitializingBean = org.springframework.beans.factory.InitializingBean;
	using Value = org.springframework.beans.factory.annotation.Value;
	using Assert = org.springframework.util.Assert;


	/// <summary>
	/// dumb object to demonstrate holding scoped state for the duration of a business process
	/// 
	/// @author Josh Long
	/// </summary>
	[Serializable]
	public class StatefulObject : InitializingBean
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		[NonSerialized]
		private Logger logger = Logger.getLogger(this.GetType().FullName);

		public const long serialVersionUID = 1L;

		private string name;
		private int visitedCount = 0;

		private long customerId;

		public virtual long CustomerId
		{
			get
			{
				return customerId;
			}
			set
			{
    
				this.customerId = value;
    
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				logger.info("setting this " + typeof(StatefulObject).FullName + " instances 'customerId' to " + this.customerId + ". The current executionId is " + this.executionId);
    
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Value("#{processInstance}") transient org.camunda.bpm.engine.runtime.ProcessInstance processInstance;
		[NonSerialized]
		internal ProcessInstance processInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Value("#{executionId}") String executionId;
		internal string executionId;


		public StatefulObject()
		{
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || this.GetType() != o.GetType())
			{
				return false;
			}

			StatefulObject that = (StatefulObject) o;

			if (visitedCount != that.visitedCount)
			{
				return false;
			}
			if (!string.ReferenceEquals(name, null) ?!name.Equals(that.name) :!string.ReferenceEquals(that.name, null))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = !string.ReferenceEquals(name, null) ? name.GetHashCode() : 0;
			result = 31 * result + visitedCount;
			return result;
		}

		public override string ToString()
		{
			return "StatefulObject{" +
					"name='" + name + '\'' +
					", visitedCount=" + visitedCount +
					'}';
		}

		public virtual void increment()
		{
			this.visitedCount += 1;
		}

		public virtual int VisitedCount
		{
			get
			{
				return this.visitedCount;
			}
		}

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
		 Assert.notNull(this.processInstance, "the processInstance should be equal to the currently active processInstance!");
			logger.info("the 'processInstance' property is non-null: PI ID# " + this.processInstance.Id);
		}
	}

}