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
namespace org.camunda.bpm.application.impl
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>A <seealso cref="ProcessApplicationReference"/> implementation using
	/// <seealso cref="WeakReference"/>.</para>
	/// 
	/// <para>As long as the process application is deployed, the container or the
	/// application will hold a strong reference to the <seealso cref="AbstractProcessApplication"/>
	/// object. This class holds a <seealso cref="WeakReference"/>. When the process
	/// application is undeployed, the container or application releases all strong
	/// references. Since we only pass {@link ProcessApplicationReference
	/// ProcessApplicationReferences} to the process engine, it is guaranteed that
	/// the <seealso cref="AbstractProcessApplication"/> object can be reclaimed by the garbage
	/// collector, even if the undeployment and unregistration should fail for some
	/// improbable reason.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationReferenceImpl : ProcessApplicationReference
	{

	  private static ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  /// <summary>
	  /// the weak reference to the process application </summary>
	  protected internal WeakReference<AbstractProcessApplication> processApplication;

	  protected internal string name;

	  public ProcessApplicationReferenceImpl(AbstractProcessApplication processApplication)
	  {
		this.processApplication = new WeakReference<AbstractProcessApplication>(processApplication);
		this.name = processApplication.Name;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.application.AbstractProcessApplication getProcessApplication() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual AbstractProcessApplication ProcessApplication
	  {
		  get
		  {
			AbstractProcessApplication application = processApplication.get();
			if (application == null)
			{
			  throw LOG.processApplicationUnavailableException(name);
			}
			else
			{
			  return application;
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processEngineStopping(org.camunda.bpm.engine.ProcessEngine processEngine) throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual void processEngineStopping(ProcessEngine processEngine)
	  {
		// do nothing
	  }

	  public virtual void clear()
	  {
		processApplication.clear();
	  }

	}

}