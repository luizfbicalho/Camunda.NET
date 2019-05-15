using System;

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

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;

	/// <summary>
	/// <para>An embedded process application is a ProcessApplication that uses an embedded
	/// process engine. An embedded process engine is loaded by the same classloader as
	/// the process application which usually means that the <code>camunda-engine.jar</code>
	/// is deployed as a web application library (in case of WAR deployments) or as an
	/// application library (in case of EAR deployments).</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EmbeddedProcessApplication : AbstractProcessApplication
	{

	  public const string DEFAULT_NAME = "Process Application";
	  private static ProcessApplicationLogger LOG = ProcessEngineLogger.PROCESS_APPLICATION_LOGGER;

	  protected internal override string autodetectProcessApplicationName()
	  {
		return DEFAULT_NAME;
	  }

	  public override ProcessApplicationReference Reference
	  {
		  get
		  {
			return new EmbeddedProcessApplicationReferenceImpl(this);
		  }
	  }

	  /// <summary>
	  /// Since the process engine is loaded by the same classloader
	  /// as the process application, nothing needs to be done.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T execute(java.util.concurrent.Callable<T> callable) throws org.camunda.bpm.application.ProcessApplicationExecutionException
	  public override T execute<T>(Callable<T> callable)
	  {
		try
		{
		  return callable.call();
		}
		catch (Exception e)
		{
		  throw LOG.processApplicationExecutionException(e);
		}
	  }

	}

}