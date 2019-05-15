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
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using JmxManagedProcessApplication = org.camunda.bpm.container.impl.jmx.services.JmxManagedProcessApplication;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;

	/// <summary>
	/// <para>Represents a Process Application registered with the Msc</para>
	/// 
	/// <para>This is the equivalent of the <seealso cref="JmxManagedProcessApplication"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MscManagedProcessApplication : Service<MscManagedProcessApplication>
	{

	  protected internal ProcessApplicationInfo processApplicationInfo;
	  protected internal ProcessApplicationReference processApplicationReference;

	  public MscManagedProcessApplication(ProcessApplicationInfo processApplicationInfo, ProcessApplicationReference processApplicationReference)
	  {
		this.processApplicationInfo = processApplicationInfo;
		this.processApplicationReference = processApplicationReference;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MscManagedProcessApplication getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual MscManagedProcessApplication Value
	  {
		  get
		  {
			return this;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {
		// call the process application's
	  }

	  public virtual void stop(StopContext context)
	  {
		// Nothing to do
	  }

	  public virtual ProcessApplicationInfo ProcessApplicationInfo
	  {
		  get
		  {
			return processApplicationInfo;
		  }
	  }

	  public virtual ProcessApplicationReference ProcessApplicationReference
	  {
		  get
		  {
			return processApplicationReference;
		  }
	  }

	}

}