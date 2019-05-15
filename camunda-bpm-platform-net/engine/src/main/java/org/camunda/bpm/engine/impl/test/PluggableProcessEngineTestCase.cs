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
namespace org.camunda.bpm.engine.impl.test
{



	/// <summary>
	/// Base class for the process engine test cases.
	/// 
	/// The main reason not to use our own test support classes is that we need to
	/// run our test suite with various configurations, e.g. with and without spring,
	/// standalone or on a server etc.  Those requirements create some complications
	/// so we think it's best to use a separate base class.  That way it is much easier
	/// for us to maintain our own codebase and at the same time provide stability
	/// on the test support classes that we offer as part of our api (in org.camunda.bpm.engine.test).
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class PluggableProcessEngineTestCase : AbstractProcessEngineTestCase
	{

	  protected internal static ProcessEngine cachedProcessEngine;

	  protected internal override void initializeProcessEngine()
	  {
		processEngine = OrInitializeCachedProcessEngine;
	  }

	  private static ProcessEngine OrInitializeCachedProcessEngine
	  {
		  get
		  {
			if (cachedProcessEngine == null)
			{
			  try
			  {
				cachedProcessEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml").buildProcessEngine();
			  }
			  catch (Exception ex)
			  {
				if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
				{
				  cachedProcessEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("activiti.cfg.xml").buildProcessEngine();
				}
				else
				{
				  throw ex;
				}
			  }
			}
			return cachedProcessEngine;
		  }
	  }

	  public static ProcessEngine ProcessEngine
	  {
		  get
		  {
			return OrInitializeCachedProcessEngine;
		  }
	  }


	}
}