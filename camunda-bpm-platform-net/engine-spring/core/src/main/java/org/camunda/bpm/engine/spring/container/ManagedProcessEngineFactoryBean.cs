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
namespace org.camunda.bpm.engine.spring.container
{
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;

	/// <summary>
	/// <para>Factory bean registering a spring-managed process engine with the <seealso cref="BpmPlatform"/>.</para>
	/// 
	/// <para>Replacement for <seealso cref="ProcessEngineFactoryBean"/>. Use this implementation if you want to 
	/// register a process engine configured in a spring application context with the <seealso cref="BpmPlatform"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ManagedProcessEngineFactoryBean : ProcessEngineFactoryBean
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.ProcessEngine getObject() throws Exception
	  public override ProcessEngine Object
	  {
		  get
		  {
			ProcessEngine processEngine = base.Object;
    
			RuntimeContainerDelegate runtimeContainerDelegate = RuntimeContainerDelegate;
			runtimeContainerDelegate.registerProcessEngine(processEngine);
    
			return processEngine;
		  }
	  }

	  protected internal virtual RuntimeContainerDelegate RuntimeContainerDelegate
	  {
		  get
		  {
			return org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws Exception
	  public override void destroy()
	  {

		if (processEngine != null)
		{
		  RuntimeContainerDelegate.unregisterProcessEngine(processEngine);
		}

		base.destroy();
	  }

	}

}