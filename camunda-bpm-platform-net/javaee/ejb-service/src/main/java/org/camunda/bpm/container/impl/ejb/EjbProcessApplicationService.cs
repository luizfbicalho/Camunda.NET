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


	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;

	/// <summary>
	/// <para>Exposes the <seealso cref="ProcessApplicationService"/> as EJB inside the container.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Stateless(name="ProcessApplicationService", mappedName="ProcessApplicationService") @Local(ProcessApplicationService.class) @TransactionAttribute(TransactionAttributeType.SUPPORTS) public class EjbProcessApplicationService implements org.camunda.bpm.ProcessApplicationService
	public class EjbProcessApplicationService : ProcessApplicationService
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @EJB protected EjbBpmPlatformBootstrap ejbBpmPlatform;
		protected internal EjbBpmPlatformBootstrap ejbBpmPlatform;

	  /// <summary>
	  /// the processApplicationServiceDelegate </summary>
	  protected internal ProcessApplicationService processApplicationServiceDelegate;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostConstruct protected void initProcessEngineServiceDelegate()
	  protected internal virtual void initProcessEngineServiceDelegate()
	  {
		processApplicationServiceDelegate = ejbBpmPlatform.ProcessApplicationService;
	  }

	  public virtual ISet<string> ProcessApplicationNames
	  {
		  get
		  {
			return processApplicationServiceDelegate.ProcessApplicationNames;
		  }
	  }

	  public virtual ProcessApplicationInfo getProcessApplicationInfo(string processApplicationName)
	  {
		return processApplicationServiceDelegate.getProcessApplicationInfo(processApplicationName);
	  }

	}

}