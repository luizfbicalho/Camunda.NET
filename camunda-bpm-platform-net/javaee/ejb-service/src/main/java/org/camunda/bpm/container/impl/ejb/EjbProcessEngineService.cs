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
namespace org.camunda.bpm.container.impl.ejb
{


	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// <para>Exposes the <seealso cref="ProcessEngineService"/> as EJB inside the container.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stateless(name="ProcessEngineService", mappedName="ProcessEngineService") @Local(ProcessEngineService.class) @TransactionAttribute(TransactionAttributeType.SUPPORTS) public class EjbProcessEngineService implements org.camunda.bpm.ProcessEngineService
	public class EjbProcessEngineService : ProcessEngineService
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @EJB protected EjbBpmPlatformBootstrap ejbBpmPlatform;
		protected internal EjbBpmPlatformBootstrap ejbBpmPlatform;

	  /// <summary>
	  /// the processEngineServiceDelegate </summary>
	  protected internal ProcessEngineService processEngineServiceDelegate;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostConstruct protected void initProcessEngineServiceDelegate()
	  protected internal virtual void initProcessEngineServiceDelegate()
	  {
		processEngineServiceDelegate = ejbBpmPlatform.ProcessEngineService;
	  }

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return processEngineServiceDelegate.DefaultProcessEngine;
		  }
	  }

	  public virtual IList<ProcessEngine> ProcessEngines
	  {
		  get
		  {
			return processEngineServiceDelegate.ProcessEngines;
		  }
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			return processEngineServiceDelegate.ProcessEngineNames;
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		return processEngineServiceDelegate.getProcessEngine(name);
	  }

	}

}