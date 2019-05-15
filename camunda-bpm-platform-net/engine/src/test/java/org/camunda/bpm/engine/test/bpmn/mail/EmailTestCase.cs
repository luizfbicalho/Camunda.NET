using System;
using System.Threading;

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
namespace org.camunda.bpm.engine.test.bpmn.mail
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TestLogger = org.camunda.bpm.engine.impl.test.TestLogger;
	using Logger = org.slf4j.Logger;
	using Wiser = org.subethamail.wiser.Wiser;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class EmailTestCase : PluggableProcessEngineTestCase
	{

	  private static readonly Logger LOG = TestLogger.TEST_LOGGER.Logger;

	  protected internal Wiser wiser;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		int port = processEngineConfiguration.MailServerPort;

		bool serverUpAndRunning = false;
		while (!serverUpAndRunning)
		{
		  wiser = new Wiser();
		  wiser.Port = port;

		  try
		  {
			LOG.info("Starting Wiser mail server on port: " + port);
			wiser.start();
			serverUpAndRunning = true;
			LOG.info("Wiser mail server listening on port: " + port);
		  }
		  catch (Exception e)
		  { // Fix for slow port-closing Jenkins
			if (e.Message.ToLower().Contains("BindException"))
			{
			  Thread.Sleep(250L);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		wiser.stop();

		// Fix for slow Jenkins
		Thread.Sleep(250L);

		base.tearDown();
	  }

	}

}