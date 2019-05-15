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
	/// <para>A reference to an EJB process application.</para>
	/// 
	/// <para>An EJB process application is an EJB Session Bean that can be looked up in JNDI.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EjbProcessApplicationReference : ProcessApplicationReference
	{

	  private static ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  /// <summary>
	  /// this is an EjbProxy and can be cached </summary>
	  protected internal ProcessApplicationInterface selfReference;

	  /// <summary>
	  /// the name of the process application </summary>
	  protected internal string processApplicationName;

	  public EjbProcessApplicationReference(ProcessApplicationInterface selfReference, string name)
	  {
		this.selfReference = selfReference;
		this.processApplicationName = name;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return processApplicationName;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.application.ProcessApplicationInterface getProcessApplication() throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual ProcessApplicationInterface ProcessApplication
	  {
		  get
		  {
			try
			{
			  // check whether process application is still deployed
			  selfReference.Name;
			}
			catch (EJBException e)
			{
			  throw LOG.processApplicationUnavailableException(processApplicationName, e);
			}
			return selfReference;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processEngineStopping(org.camunda.bpm.engine.ProcessEngine processEngine) throws org.camunda.bpm.application.ProcessApplicationUnavailableException
	  public virtual void processEngineStopping(ProcessEngine processEngine)
	  {
		// do nothing.
	  }

	}

}