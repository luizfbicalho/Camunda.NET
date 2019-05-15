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
namespace org.camunda.bpm.container.impl.ejb.deployment
{
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;

	/// <summary>
	/// <para>MBean Service delegating to the Jca ExecutorService</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class JcaExecutorServiceDelegate : PlatformService<ExecutorService>, JcaExecutorServiceDelegateMBean
	{

	  private ExecutorService executorService;

	  public JcaExecutorServiceDelegate(ExecutorService executorService)
	  {
		this.executorService = executorService;
	  }

	  public virtual void start(PlatformServiceContainer mBeanServiceContainer)
	  {
		// nothing to do
	  }

	  public virtual void stop(PlatformServiceContainer mBeanServiceContainer)
	  {
		// nothing to do
	  }

	  public virtual ExecutorService Value
	  {
		  get
		  {
			return executorService;
		  }
	  }

	}

}