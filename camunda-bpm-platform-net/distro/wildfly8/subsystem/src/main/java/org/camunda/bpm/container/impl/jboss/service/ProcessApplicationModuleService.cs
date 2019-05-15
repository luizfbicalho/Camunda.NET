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
namespace org.camunda.bpm.container.impl.jboss.service
{
	using Service = org.jboss.msc.service.Service;
	using ServiceTarget = org.jboss.msc.service.ServiceTarget;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;

	/// <summary>
	/// <para>Service installed for a process application module</para>
	/// 
	/// <para>This service is used as a "root" service for all services installed by a 
	/// process application deployment, be it from a DeploymentProcessor or at Runtime. 
	/// As this service is installed as a child service on the deployment unit, it is 
	/// guaranteed that the undeployment operation removes all services installed by 
	/// the process application.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationModuleService : Service<ServiceTarget>
	{

	  protected internal ServiceTarget childTarget;

	  public ProcessApplicationModuleService()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.jboss.msc.service.ServiceTarget getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual ServiceTarget Value
	  {
		  get
		  {
			return childTarget;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {
		childTarget = context.ChildTarget;
	  }

	  public virtual void stop(StopContext context)
	  {
	  }


	}

}