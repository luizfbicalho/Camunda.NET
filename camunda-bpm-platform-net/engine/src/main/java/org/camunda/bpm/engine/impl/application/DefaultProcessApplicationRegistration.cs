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
namespace org.camunda.bpm.engine.impl.application
{

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultProcessApplicationRegistration : ProcessApplicationRegistration
	{

	  protected internal ISet<string> deploymentIds;
	  protected internal string processEngineName;
	  protected internal ProcessApplicationReference reference;

	  /// <param name="reference"> </param>
	  public DefaultProcessApplicationRegistration(ProcessApplicationReference reference, ISet<string> deploymentIds, string processEnginenName)
	  {
		this.reference = reference;
		this.deploymentIds = deploymentIds;
		this.processEngineName = processEnginenName;
	  }

	  public virtual ISet<string> DeploymentIds
	  {
		  get
		  {
			return deploymentIds;
		  }
	  }

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
	  }

	  public virtual ProcessApplicationReference Reference
	  {
		  get
		  {
			return reference;
		  }
	  }

	}

}