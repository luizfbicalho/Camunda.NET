using System.Collections.Generic;

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
namespace org.camunda.bpm.application.impl.ejb
{




	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Singleton @Startup @ConcurrencyManagement(ConcurrencyManagementType.BEAN) @TransactionAttribute(TransactionAttributeType.REQUIRED) @ProcessApplication @Local(ProcessApplicationInterface.class) public class DefaultEjbProcessApplication extends org.camunda.bpm.application.impl.EjbProcessApplication
	public class DefaultEjbProcessApplication : EjbProcessApplication
	{

	  protected internal IDictionary<string, string> properties = new Dictionary<string, string>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostConstruct public void start()
	  public virtual void start()
	  {
		deploy();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PreDestroy public void stop()
	  public virtual void stop()
	  {
		undeploy();
	  }

	  public override IDictionary<string, string> Properties
	  {
		  get
		  {
			return properties;
		  }
	  }

	}

}