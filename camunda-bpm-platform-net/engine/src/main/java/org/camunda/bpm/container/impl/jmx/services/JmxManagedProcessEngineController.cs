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
namespace org.camunda.bpm.container.impl.jmx.services
{
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;

	/// <summary>
	/// <para>Represents a managed process engine that is started / stopped inside the <seealso cref="MBeanServiceContainer"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JmxManagedProcessEngineController : JmxManagedProcessEngine, JmxManagedProcessEngineMBean
	{

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  public JmxManagedProcessEngineController(ProcessEngineConfiguration processEngineConfiguration)
	  {
		this.processEngineConfiguration = processEngineConfiguration;
	  }

	  public override void start(PlatformServiceContainer contanier)
	  {
		processEngine = processEngineConfiguration.buildProcessEngine();
	  }

	  public override void stop(PlatformServiceContainer container)
	  {
		processEngine.close();
	  }

	}

}