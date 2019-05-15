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
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;

	/// <summary>
	/// <para>Start Service for process applications that do not expose an EE Component View
	/// (like <seealso cref="ServletProcessApplication"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class NoViewProcessApplicationStartService : Service<ProcessApplicationInterface>
	{

	  protected internal ProcessApplicationReference reference;

	  public NoViewProcessApplicationStartService(ProcessApplicationReference reference)
	  {
		this.reference = reference;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.application.ProcessApplicationInterface getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual ProcessApplicationInterface Value
	  {
		  get
		  {
			try
			{
			  return reference.ProcessApplication;
    
			}
			catch (ProcessApplicationUnavailableException e)
			{
			  throw new System.InvalidOperationException("Process application '" + reference.Name + "' is not unavailable.", e);
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public virtual void start(StartContext context)
	  {

	  }

	  public virtual void stop(StopContext context)
	  {

	  }

	}

}